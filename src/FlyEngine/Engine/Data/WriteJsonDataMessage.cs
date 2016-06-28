using FlyController;
using FlyController.Model;
using FlyController.Model.Error;
using FlyController.Model.Streaming;
using FlyEngine.Manager;
using FlyEngine.Engine.Metadata;
using FlyMapping.Model;
using IstatExtension.SdmxJson.DataWriter.Engine;
using Newtonsoft.Json;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Engine;
using Org.Sdmxsource.Sdmx.Api.Model.Header;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Header;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace FlyEngine.Engine.Data
{
    /// <summary>
    /// Write20DataMessage implementing WriteDataBase for Write a data in SDMX 2.0 Version
    /// </summary>
    public class WriteJsonDataMessage : WriterDataBase
    {

        /// <summary>
        /// Write a DataMessage in XmlWriter
        /// </summary>
        /// <param name="writer">Stream Destination</param>
        public override void WriteDataMessage(IFlyWriter writer)
        {
            try
            {

                JsonWriter jsonwriter = new JsonTextWriter(writer.__DsplJSONTextWriter);
                SdmxJsonBaseDataWriter dataWriterEngine = null;
                switch (MessageType)
                {
                    case MessageTypeEnum.Json:
                        dataWriterEngine = new SdmxJsonBaseDataWriter(jsonwriter);
                        break;
                }

                if (dataWriterEngine == null)
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.InternalError, new Exception("No Type Message recognized"));

                dataWriterEngine.WriteHeaderForDownloadJson();

                IHeader header = FlyConfiguration.HeaderSettings.GetHeader();
                dataWriterEngine.WriteHeader(header);
                DataStructureObjectImpl dsd = _retrievalManager._dsd;

                dataWriterEngine._dsd = dsd.Immutated;

                // start dataset
                dataWriterEngine.StartDataset(_retrievalManager.GetDataflowFromSdmxObject(), dsd.Immutated, null, null);


                this.LastSeriesKey = null;
                WriteTimeSeriesData(dataWriterEngine);

                dataWriterEngine.WriteSDMXJsonStructure(_retrievalManager.GetDataflowFromSdmxObject(), new SdmxJsonBaseDataWriter.RetreiveCodelistDelegate(GetCodelist));

                writer.__DsplJSONTextWriter.Flush();


            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.ErrorWriteDataMessage, ex);
            }
        }
        /// <summary>
        /// Write a TimeSeries Data
        /// </summary>
        /// <param name="dataWriterEngine">input structure contains data</param>
        private void WriteTimeSeriesData(SdmxJsonBaseDataWriter dataWriterEngine)
        {
            try
            {
                bool AddedDatasetAttribute = false;
                List<DataMessageObject> dmo = _tableResponse.GetNext();
                do
                {
                    if (dmo == null)
                        break;

                    SettingValues(dmo, dataWriterEngine);

                    if (!AddedDatasetAttribute)
                    {
                        dataWriterEngine.StartElement("attributes", false);
                        foreach (DataMessageObject seriesAttribute in dmo.FindAll(att => OnlyDatasetAttribute(att)))
                            dataWriterEngine.WriteAttributes(seriesAttribute);

                        AddedDatasetAttribute = true;
                        dataWriterEngine.CloseArray();
                        dataWriterEngine.StartSeries();
                    }
                    //SERIE
                    if (CheckChanged(dmo))
                    {
                        if (dataWriterEngine.StartedObservations)
                        {
                            dataWriterEngine.CloseObject();
                            dataWriterEngine.CloseObject();
                            dataWriterEngine._startedObservations = false;
                        }

                        dataWriterEngine.WriteSeriesKey(dmo);

                        // write series attributes
                        dataWriterEngine.StartElement("attributes", false);
                        foreach (DataMessageObject seriesAttribute in dmo.FindAll(att => OnlySeriesAttribute(att)))
                            dataWriterEngine.WriteAttributes(seriesAttribute);
                        dataWriterEngine.CloseArray();


                        if (!dataWriterEngine._startedObservations)
                        {
                            dataWriterEngine.StartElement("observations", true);
                            dataWriterEngine._startedObservations = true;
                        }
                    }
                    ////OBSERVATION
                    DataMessageObject val = dmo.Find(dimV => dimV.ColImpl.ConceptType == ConceptTypeEnum.Attribute && ((IAttributeConcept)dimV.ColImpl).IsValueAttribute);

                    DataMessageObject ObsVal = null;

                    ObsVal = dmo.Find(dimT => dimT.ColImpl.Id.Trim().ToUpper() == DimensionAtObservation.Trim().ToUpper());
                    dataWriterEngine.WriteObservation(dataWriterEngine.GetObsValPosition(ObsVal.Val.ToString()), val.Val.ToString());
                    //Write Observation Attributes
                    foreach (DataMessageObject seriesAttribute in dmo.FindAll(att => OnlyObservationAttribute(att)))
                        dataWriterEngine.WriteAttributes(seriesAttribute);

                    dataWriterEngine.CloseArray();


                    dmo = _tableResponse.GetNext();
                } while (dmo != null);
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.ErrorWriteTimeSeriesData, ex);
            }
        }


        internal void SettingValues(List<DataMessageObject> dmo, SdmxJsonBaseDataWriter dataWriterEngine)
        {
            try
            {
                bool AddedDatasetAttribute = false;

                foreach (DataMessageObject seriesKey in dmo.FindAll(dim => OnlySeriesKey(dim)))
                    dataWriterEngine.AddSerie(seriesKey.ColImpl.ConceptObjectCode, seriesKey.Val.ToString(), dataWriterEngine.GetKeyPosition(dataWriterEngine._dsd, seriesKey.ColImpl.ConceptObjectCode), true);

                foreach (DataMessageObject seriesAttribute in dmo.FindAll(att => OnlySeriesAttribute(att)))
                    dataWriterEngine.AddAttribute(seriesAttribute.ColImpl.ConceptObjectCode, seriesAttribute.Val.ToString(), "Serie");


                DataMessageObject val = dmo.Find(dimV => dimV.ColImpl.ConceptType == ConceptTypeEnum.Attribute && ((IAttributeConcept)dimV.ColImpl).IsValueAttribute);

                DataMessageObject ObsVal = null;

                if (string.IsNullOrEmpty(DimensionAtObservation))
                    DimensionAtObservation = "TIME_PERIOD";

                ObsVal = dmo.Find(dimT => dimT.ColImpl.Id.Trim().ToUpper() == DimensionAtObservation.Trim().ToUpper());
                dataWriterEngine.AddObsValue(ObsVal.Val.ToString());

                if (dataWriterEngine.JsonStruct.Observation.ID == null)
                {
                    dataWriterEngine.JsonStruct.Observation.Role = "time";
                    dataWriterEngine.JsonStruct.Observation.ID = ObsVal.ColImpl.Id;
                }

                if (!AddedDatasetAttribute)
                {
                    foreach (DataMessageObject seriesAttribute in dmo.FindAll(att => OnlyDatasetAttribute(att)))
                        dataWriterEngine.AddAttribute(seriesAttribute.ColImpl.ConceptObjectCode, seriesAttribute.Val.ToString(), "DataSet");
                    AddedDatasetAttribute = true;
                }

                foreach (DataMessageObject seriesAttribute in dmo.FindAll(att => OnlyObservationAttribute(att)))
                    dataWriterEngine.AddAttribute(seriesAttribute.ColImpl.ConceptObjectCode, seriesAttribute.Val.ToString(), "Observation");

            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.ErrorWriteTimeSeriesData, ex);
            }

        }


        private List<ICodelistObject> GetCodelist(string agency, string codelistId, string version, IDataflowObject Dataflow, string ConceptId)
        {
            GetCodelists gc = new GetCodelists(new SdmxParsingObject(SdmxStructureEnumType.CodeList)
            {
                AgencyId = agency,
                MaintainableId = codelistId,
                ConstrainDataFlow = Dataflow.Id,
                ConstrainDataFlowAgency = Dataflow.AgencyId,
                ConstrainDataFlowVersion = Dataflow.Version,
                ConstrainConcept = ConceptId,
                _version = version

            }, SdmxSchemaEnumType.VersionTwo);
            List<ICodelistMutableObject> codelistMutable = gc.GetCodelist();
            List<ICodelistObject> codelists = new List<ICodelistObject>();
            codelistMutable.ForEach(c => codelists.Add(c.ImmutableInstance));
            return codelists;
        }

    }
}
