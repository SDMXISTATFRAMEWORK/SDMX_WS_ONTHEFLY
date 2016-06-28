using FlyController;
using FlyController.Model;
using FlyController.Model.Error;
using FlyController.Model.Streaming;
using FlyEngine.Manager;
using FlyMapping.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Engine;
using Org.Sdmxsource.Sdmx.Api.Model.Header;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.DataParser.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FlyEngine.Engine.Data
{
    /// <summary>
    /// Write21DataMessage implementing WriteDataBase for Write a data in SDMX 2.1 Version
    /// </summary>
    public class Write21DataMessage : WriterDataBase
    {
        /// <summary>
        /// Flag representing writing data in FLAT format
        /// </summary>
        private bool isFlat = false;

        /// <summary>
        /// Write a DataMessage in XmlWriter calling WriteTimeSeriesData or WriteFlatData or WriteCrossSectional
        /// </summary>
        /// <param name="writer">Stream Destination</param>
        public override void WriteDataMessage(IFlyWriter writer)
        {
            try
            {

                IDataWriterEngine dataWriterEngine = null;
                switch (MessageType)
                {

                    case MessageTypeEnum.GenericData_21:
                        dataWriterEngine = new GenericDataWriterEngine(writer.__SdmxXml, SdmxSchema.GetFromEnum(SdmxSchemaEnumType.VersionTwoPointOne));
                        break;
                    case MessageTypeEnum.GenericTimeSeries_21:
                        dataWriterEngine = new GenericDataWriterEngine(writer.__SdmxXml, SdmxSchema.GetFromEnum(SdmxSchemaEnumType.VersionTwoPointOne));
                        break;
                    case MessageTypeEnum.StructureSpecific_21:
                        dataWriterEngine = new CompactDataWriterEngine(writer.__SdmxXml, SdmxSchema.GetFromEnum(SdmxSchemaEnumType.VersionTwoPointOne));
                        break;
                    case MessageTypeEnum.StructureSpecificTimeSeries_21:
                        dataWriterEngine = new CompactDataWriterEngine(writer.__SdmxXml, SdmxSchema.GetFromEnum(SdmxSchemaEnumType.VersionTwoPointOne));
                        break;

                }

                isFlat = false;
                if (!string.IsNullOrEmpty(this.DimensionAtObservation) && this.DimensionAtObservation == "AllDimensions")
                    isFlat = true;


                //Da capire!!!! perche in teoria le crossSectional dovrebbero essere GenericData_21 e StructureSpecific_21 mentre le atre 2 sono TimeSeries
                //Ma le CommonApi non supportano le TimeSeries e quelle che in teoria sono Cross in realtà sono TimeSeries

                //WriteCrossSectional(ref writer);
                //return;

                if (dataWriterEngine == null)
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.InternalError, new Exception("No Type Message recognized"));


                // write header
                IHeader header = FlyConfiguration.HeaderSettings.GetHeader();
                dataWriterEngine.WriteHeader(header);

                // start dataset
                DataStructureObjectImpl dsd = _retrievalManager._dsd;
                dataWriterEngine.StartDataset(null, dsd.Immutated, null);

                // Aggiungo dataset Title
                if (FlyConfiguration.DatasetTitle && !string.IsNullOrEmpty(this._retrievalManager.DataFlowTitle))
                    dataWriterEngine.WriteAttributeValue("Title", this._retrievalManager.DataFlowTitle);

                //GROUPS
                if (_retrievalManager.Groups != null && _retrievalManager.Groups.Count > 0)
                {
                    foreach (DataGroupObject group in _retrievalManager.Groups)
                    {
                        dataWriterEngine.StartGroup(group.GroupId);
                        foreach (GroupReferenceObject GroupKey in group.DimensionReferences)
                            dataWriterEngine.WriteGroupKeyValue(GroupKey.ConceptCode, GroupKey.ConceptValue.ToString());

                        foreach (GroupReferenceObject GroupAttribute in group.AttributeReferences)
                            dataWriterEngine.WriteAttributeValue(GroupAttribute.ConceptCode, GroupAttribute.ConceptValue.ToString());
                    }
                }

                this.LastSeriesKey = null;
                if (isFlat)
                {

                    WriteFlatData(dataWriterEngine);
                }
                else
                {
                    WriteTimeSeriesData(dataWriterEngine);
                }
                dataWriterEngine.Close();
                writer.__SdmxXml.Flush();
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
        private void WriteTimeSeriesData(IDataWriterEngine dataWriterEngine)
        {
            //TODO: MAX - FORSE E' QUESTA LA PARTE DA MODIFICARE!!

            try
            {
                bool AddedDatasetAttribute = false;
                List<DataMessageObject> dmo = _tableResponse.GetNext();
                do
                {
                    if (dmo == null)
                        break;

                    //Aggiungo gli Attributi di Dataset
                    if (!AddedDatasetAttribute)
                    {
                        foreach (DataMessageObject seriesAttribute in dmo.FindAll(att => OnlyDatasetAttribute(att)))
                            dataWriterEngine.WriteAttributeValue(seriesAttribute.ColImpl.ConceptObjectCode, seriesAttribute.Val.ToString());
                        AddedDatasetAttribute = true;
                    }


                    //SERIE
                    if (CheckChanged(dmo))
                    {
                        dataWriterEngine.StartSeries();

                        foreach (DataMessageObject seriesKey in dmo.FindAll(dim => OnlySeriesKey(dim)))
                            dataWriterEngine.WriteSeriesKeyValue(seriesKey.ColImpl.ConceptObjectCode, seriesKey.Val.ToString());

                        foreach (DataMessageObject seriesAttribute in dmo.FindAll(att => OnlySeriesAttribute(att)))
                            dataWriterEngine.WriteAttributeValue(seriesAttribute.ColImpl.ConceptObjectCode, seriesAttribute.Val.ToString());
                    }

                    //OBSERVATION
                    DataMessageObject val = dmo.Find(dimV => dimV.ColImpl.ConceptType == ConceptTypeEnum.Attribute && ((IAttributeConcept)dimV.ColImpl).IsValueAttribute);

                    DataMessageObject ObsVal = null;
                    if (string.IsNullOrEmpty(DimensionAtObservation) || new List<string> { "TIME_PERIOD", "ALLDIMENSIONS" }.Contains(DimensionAtObservation.Trim().ToUpper()))
                    {
                        ObsVal = dmo.Find(dimT => dimT.ColImpl.ConceptType == ConceptTypeEnum.Dimension && ((IDimensionConcept)dimT.ColImpl).DimensionType == DimensionTypeEnum.Time);
                        dataWriterEngine.WriteObservation(ObsVal.Val.ToString(), val.Val.ToString());
                    }
                    else
                    {
                        ObsVal = dmo.Find(dimT => dimT.ColImpl.Id.Trim().ToUpper() == DimensionAtObservation.Trim().ToUpper());
                        dataWriterEngine.WriteObservation(ObsVal.ColImpl.Id, ObsVal.Val.ToString(), val.Val.ToString());

                    }

                    foreach (DataMessageObject seriesAttribute in dmo.FindAll(att => OnlyObservationAttribute(att)))
                        if (seriesAttribute.Val.ToString() != String.Empty)
                            dataWriterEngine.WriteAttributeValue(seriesAttribute.ColImpl.ConceptObjectCode, seriesAttribute.Val.ToString());

                    dmo = _tableResponse.GetNext();
                } while (dmo != null);
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.ErrorWriteTimeSeriesData, ex);
            }
        }

        /// <summary>
        /// Write a TimeSeries Data in FLAT format
        /// </summary>
        /// <param name="dataWriterEngine">input structure contains data</param>
        private void WriteFlatData(IDataWriterEngine dataWriterEngine)
        {
            try
            {
                dataWriterEngine.StartSeries();
                bool AddedDatasetAttribute = false;
                List<DataMessageObject> dmo = _tableResponse.GetNext();
                do
                {
                    if (dmo == null)
                        break;

                    //Aggiungo gli Attributi di Dataset
                    if (!AddedDatasetAttribute)
                    {
                        foreach (DataMessageObject seriesAttribute in dmo.FindAll(att => OnlyDatasetAttribute(att)))
                            dataWriterEngine.WriteAttributeValue(seriesAttribute.ColImpl.ConceptObjectCode, seriesAttribute.Val.ToString());
                        AddedDatasetAttribute = true;
                    }


                    //OBSERVATION
                    DataMessageObject val = dmo.Find(dimV => dimV.ColImpl.ConceptType == ConceptTypeEnum.Attribute && ((IAttributeConcept)dimV.ColImpl).IsValueAttribute);
                    DataMessageObject time = dmo.Find(dimT => dimT.ColImpl.ConceptType == ConceptTypeEnum.Dimension && ((IDimensionConcept)dimT.ColImpl).DimensionType == DimensionTypeEnum.Time);
                    dataWriterEngine.WriteObservation(time.Val.ToString(), val.Val.ToString());


                    foreach (DataMessageObject seriesKey in dmo.FindAll(dim => OnlySeriesKey(dim)))
                        dataWriterEngine.WriteSeriesKeyValue(seriesKey.ColImpl.ConceptObjectCode, seriesKey.Val.ToString());

                    foreach (DataMessageObject seriesAttribute in dmo.FindAll(att => OnlySeriesAttribute(att)))
                        dataWriterEngine.WriteAttributeValue(seriesAttribute.ColImpl.ConceptObjectCode, seriesAttribute.Val.ToString());
                    foreach (DataMessageObject seriesAttribute in dmo.FindAll(att => OnlyObservationAttribute(att)))
                        dataWriterEngine.WriteAttributeValue(seriesAttribute.ColImpl.ConceptObjectCode, seriesAttribute.Val.ToString());

                    dmo = _tableResponse.GetNext();
                } while (dmo != null);
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.ErrorWriteFlatData, ex);
            }
        }

        //private void WriteCrossSectional(ref XmlWriter writer)
        //{
        //    ICrossSectionalWriterEngine dataWriterEngine = new CrossSectionalWriterEngine(writer, SdmxSchema.GetFromEnum(SdmxSchemaEnumType.VersionTwoPointOne));

        //    if (dataWriterEngine == null)
        //        throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.InternalError, new Exception("No Type Message recognized"));


        //    // write header
        //    IHeader head = FlyConfiguration.HeaderSettings.GetHeader();
        //    dataWriterEngine.WriteHeader(head);

        //    // start dataset
        //    IDataStructureObject dsd = _retrievalManager.GetDataStructure();
        //    dataWriterEngine.StartDataset(null, dsd, null);

        //    // Aggiungo dataset Title
        //    if (FlyConfiguration.DatasetTitle && !string.IsNullOrEmpty(this._retrievalManager.DataFlowTitle))
        //        dataWriterEngine.WriteAttributeValue("Title", this._retrievalManager.DataFlowTitle);

        //    //Aggiungo gli Attributi di Dataset
        //    if (_tableResponse.RisCount > 0)
        //        foreach (DataMessageObject seriesAttribute in _tableResponse.Get(0).FindAll(att => OnlyDatasetAttribute(att)))
        //            dataWriterEngine.WriteAttributeValue(seriesAttribute.ColImpl.ConceptObjectCode, seriesAttribute.Val.ToString());

        //    this.LastSeriesKey = null;
        //    for (int i = 0; i < _tableResponse.RisCount; i++)
        //    {
        //        List<DataMessageObject> dmo = _tableResponse.Get(i);

        //        //SERIE
        //        if (CheckChanged(dmo))
        //        {
        //            dataWriterEngine.StartSeries();

        //            foreach (DataMessageObject seriesKey in dmo.FindAll(dim => OnlySeriesKey(dim)))
        //                dataWriterEngine.WriteSeriesKeyValue(seriesKey.ColImpl.ConceptObjectCode, seriesKey.Val.ToString());

        //            foreach (DataMessageObject seriesAttribute in dmo.FindAll(att => OnlySeriesAttribute(att)))
        //                dataWriterEngine.WriteAttributeValue(seriesAttribute.ColImpl.ConceptObjectCode, seriesAttribute.Val.ToString());
        //        }

        //        //OBSERVATION
        //        DataMessageObject val = dmo.Find(dimV => dimV.ColImpl.ConceptType == ConceptObjectImpl.ConceptTypeEnum.Attribute && ((AttributeConcept)dimV.ColImpl).IsValueAttribute);
        //        DataMessageObject time = dmo.Find(dimT => dimT.ColImpl.ConceptType == ConceptObjectImpl.ConceptTypeEnum.Dimension && ((DimensionConcept)dimT.ColImpl).DimensionType == DimensionConcept.DimensionTypeEnum.Time);
        //        dataWriterEngine.WriteObservation(time.Val.ToString(), val.Val.ToString());

        //        foreach (DataMessageObject seriesAttribute in dmo.FindAll(att => OnlyObservationAttribute(att)))
        //            dataWriterEngine.WriteAttributeValue(seriesAttribute.ColImpl.ConceptObjectCode, seriesAttribute.Val.ToString());

        //    }


        //    dataWriterEngine.Close();
        //    writer.Flush();
        //    writer.Close();
        //}

    }
}
