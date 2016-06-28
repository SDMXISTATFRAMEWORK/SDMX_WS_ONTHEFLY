using FlyController;
using FlyController.Model;
using FlyController.Model.Error;
using FlyController.Model.Streaming;
using FlyEngine.Manager;
using FlyMapping.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Engine;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.DataStructure;
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
    /// Write20DataMessage implementing WriteDataBase for Write a data in SDMX 2.0 Version
    /// </summary>
    public class Write20DataMessage : WriterDataBase
    {
        /// <summary>
        /// Write a DataMessage in XmlWriter
        /// </summary>
        /// <param name="writer">Stream Destination</param>
        public override void WriteDataMessage(IFlyWriter writer)
        {
            try
            {


                IDataWriterEngine dataWriterEngine = null;
                switch (MessageType)
                {
                    case MessageTypeEnum.Compact_20:
                        dataWriterEngine = new CompactDataWriterEngine(writer.__SdmxXml, SdmxSchema.GetFromEnum(SdmxSchemaEnumType.VersionTwo));
                        break;
                    case MessageTypeEnum.Generic_20:
                        dataWriterEngine = new GenericDataWriterEngine(writer.__SdmxXml, SdmxSchema.GetFromEnum(SdmxSchemaEnumType.VersionTwo));
                        break;
                    case MessageTypeEnum.CrossSectional_20:
                        break;
                }

                if (dataWriterEngine == null)
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.InternalError, new Exception("No Type Message recognized"));


                // write header
                dataWriterEngine.WriteHeader(FlyConfiguration.HeaderSettings.GetHeader());

                // start dataset
                DataStructureObjectImpl dsd = _retrievalManager._dsd;
                dataWriterEngine.StartDataset(null, dsd.Immutated, null);

                // Aggiungo dataset Title
                if (FlyConfiguration.DatasetTitle && !string.IsNullOrEmpty(this._retrievalManager.DataFlowTitle))
                    dataWriterEngine.WriteAttributeValue("Title", this._retrievalManager.DataFlowTitle);



                this.LastSeriesKey = null;
                List<DataMessageObject> dmo = _tableResponse.GetNext();

                if (dmo != null)
                {

                    //Aggiungo gli Attributi di Dataset
                    foreach (DataMessageObject seriesAttribute in dmo.FindAll(att => OnlyDatasetAttribute(att)))
                        dataWriterEngine.WriteAttributeValue(seriesAttribute.ColImpl.ConceptObjectCode, seriesAttribute.Val.ToString());


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

                    do
                    {
                        if (dmo == null)
                            break;

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
                        DataMessageObject time = dmo.Find(dimT => dimT.ColImpl.ConceptType == ConceptTypeEnum.Dimension && ((IDimensionConcept)dimT.ColImpl).DimensionType == DimensionTypeEnum.Time);
                        dataWriterEngine.WriteObservation(time.Val.ToString(), val.Val.ToString());

                        foreach (DataMessageObject seriesAttribute in dmo.FindAll(att => OnlyObservationAttribute(att)))
                            if (seriesAttribute.Val.ToString() != String.Empty)
                                dataWriterEngine.WriteAttributeValue(seriesAttribute.ColImpl.ConceptObjectCode, seriesAttribute.Val.ToString());

                        dmo = _tableResponse.GetNext();
                    } while (dmo != null);

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
    }
}
