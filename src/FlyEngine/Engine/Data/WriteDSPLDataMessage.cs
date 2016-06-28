using DsplDataFormat.Engine;
using FlyController;
using FlyController.Model;
using FlyController.Model.Error;
using FlyController.Model.Streaming;
using FlyEngine.Engine.Metadata;
using FlyEngine.Manager;
using FlyMapping.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Engine;
using Org.Sdmxsource.Sdmx.Api.Model.Header;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Header;
using RDFProvider.Writer.Engine;
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
    public class WriteDSPLDataMessage : WriterDataBase
    {
        /// <summary>
        /// Write a DataMessage in XmlWriter
        /// </summary>
        /// <param name="writer">Stream Destination</param>
        public override void WriteDataMessage(IFlyWriter writer)
        {
            try
            {
                using (TextWriter tw = new StreamWriter(new MemoryStream()))
                {
                    DsplTextWriter dsplTextWriter = new DsplTextWriter(tw);
                    IDsplDataWriterEngine dataWriterEngine = null;
                    switch (MessageType)
                    {
                        case MessageTypeEnum.Dspl:
                            dataWriterEngine = new DsplBaseDataWriter(dsplTextWriter);
                            break;
                    }

                    try
                    {
                        if (dataWriterEngine == null)
                            throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.InternalError, new Exception("No Type Message recognized"));

                        DataStructureObjectImpl dsd = _retrievalManager._dsd;
                        IHeader header = FlyConfiguration.HeaderSettings.GetHeader();
                        dataWriterEngine.WriteHeader(header);

                        // start dataset
                        dataWriterEngine.SetDsdOrder(dsd.Immutated);
                        dataWriterEngine.StartDataset(_retrievalManager.GetDataflowFromSdmxObject(), dsd.Immutated, new DsplBaseDataWriter.RetreiveCodelistDelegate(GetCodelist));


                        dataWriterEngine.StartSeries(null);
                        List<DataMessageObject> dmo = _tableResponse.GetNext();
                        do
                        {
                            if (dmo == null)
                                break;

                            foreach (DataMessageObject seriesKey in dmo.FindAll(dim => OnlySeriesKey(dim)))
                                dataWriterEngine.WriteSeriesKeyValue(seriesKey.ColImpl.ConceptObjectCode, seriesKey.Val.ToString());

                            //OBSERVATION
                            DataMessageObject val = dmo.Find(dimV => dimV.ColImpl.ConceptType == ConceptTypeEnum.Attribute && ((IAttributeConcept)dimV.ColImpl).IsValueAttribute);
                            DataMessageObject time = dmo.Find(dimT => dimT.ColImpl.ConceptType == ConceptTypeEnum.Dimension && ((IDimensionConcept)dimT.ColImpl).DimensionType == DimensionTypeEnum.Time);
                            dataWriterEngine.WriteObservation(time.Val.ToString(), val.Val.ToString());

                            dmo = _tableResponse.GetNext();
                        } while (dmo != null);


                        dataWriterEngine.CloseAndZip(writer.__DsplJSONTextWriter);
                        writer.__DsplJSONTextWriter.Flush();

                    }
                    catch (Exception ex)
                    {
                        throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.ErrorWriteFlatData, ex);
                    }
                    finally
                    {
                        dataWriterEngine.ClearTempFolder();
                    }
                }
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.ErrorWriteDataMessage, ex);
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
