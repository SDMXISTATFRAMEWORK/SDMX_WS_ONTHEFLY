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
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Header;
using RDFProvider.Writer.Engine;
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
    public class WriteRDFDataMessage : WriterDataBase
    {
        /// <summary>
        /// Write a DataMessage in XmlWriter
        /// </summary>
        /// <param name="writer">Stream Destination</param>
        public override void WriteDataMessage(IFlyWriter writer)
        {
            try
            {


                IRDFDataWriterEngine dataWriterEngine = null;
                switch (MessageType)
                {
                    case MessageTypeEnum.Rdf:
                        dataWriterEngine = new RDFDataWriterEngine(writer.__SdmxXml);
                        break;
                }


                if (dataWriterEngine == null)
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.InternalError, new Exception("No Type Message recognized"));

                DataStructureObjectImpl dsd = _retrievalManager._dsd;

                // start dataset
                dataWriterEngine.StartDataset(dsd.Immutated.TimeDimension.Id, _retrievalManager.DataFlowID, _retrievalManager._dsd.Id);



                //// Aggiungo dataset Title
                //if (FlyConfiguration.DatasetTitle && !string.IsNullOrEmpty(this._retrievalManager.DataFlowTitle))
                //    dataWriterEngine.WriteAttributeValue("Title", this._retrievalManager.DataFlowTitle);


                //Aggiungo gli Attributi di Dataset
                //bool AddedDatasetAttribute = false;
                this.LastSeriesKey = null;
                List<DataMessageObject> dmo = _tableResponse.GetNext();

                do
                {
                    if (dmo == null)
                        break;
                    //if (!AddedDatasetAttribute)
                    //{
                    //    foreach (DataMessageObject seriesAttribute in dmo.FindAll(att => OnlyDatasetAttribute(att)))
                    //        dataWriterEngine.WriteAttributeValue(seriesAttribute.ColImpl.ConceptObjectCode, seriesAttribute.Val.ToString());
                    //    AddedDatasetAttribute = true;
                    //}



                    //SERIE
                    //if (CheckChanged(dmo))
                    //{
                    List<string> Vals = new List<string>();
                    Vals.Add(RDFProvider.Constants.RDFConstants.RdfDataset + _retrievalManager.DataFlowID);
                    foreach (DataMessageObject seriesKey in dmo.FindAll(dim => OnlySeriesKey(dim)))
                        Vals.Add(seriesKey.Val.ToString());

                    dataWriterEngine.StartSeries(string.Join("/", Vals), _retrievalManager.DataFlowID);


                    foreach (DataMessageObject seriesKey in dmo.FindAll(dim => OnlySeriesKey(dim)))
                    {
                        if (seriesKey.ColImpl.ConceptDSDInfo != null)
                            dataWriterEngine.WriteSeriesKeyValue(seriesKey.ColImpl.ConceptObjectCode, seriesKey.Val.ToString(), seriesKey.ColImpl.ConceptDSDInfo.CodelistVersion, seriesKey.ColImpl.ConceptDSDInfo.CodelistId);
                        else
                            dataWriterEngine.WriteSeriesKeyValue(seriesKey.ColImpl.ConceptObjectCode, seriesKey.Val.ToString(), FlyConfiguration.Version, string.Format("CL_{0}", seriesKey.ColImpl.ConceptObjectCode));
                    }

                    //foreach (DataMessageObject seriesAttribute in dmo.FindAll(att => OnlySeriesAttribute(att)))
                    //    dataWriterEngine.WriteAttributeValue(seriesAttribute.ColImpl.ConceptObjectCode, seriesAttribute.Val.ToString());
                    //}

                    //OBSERVATION
                    DataMessageObject val = dmo.Find(dimV => dimV.ColImpl.ConceptType == ConceptTypeEnum.Attribute && ((IAttributeConcept)dimV.ColImpl).IsValueAttribute);
                    DataMessageObject time = dmo.Find(dimT => dimT.ColImpl.ConceptType == ConceptTypeEnum.Dimension && ((IDimensionConcept)dimT.ColImpl).DimensionType == DimensionTypeEnum.Time);
                    dataWriterEngine.RDFWriteObservation(time.ColImpl.ConceptObjectCode, time.Val.ToString(), val.Val.ToString());

                    //foreach (DataMessageObject seriesAttribute in dmo.FindAll(att => OnlyObservationAttribute(att)))
                    //    dataWriterEngine.WriteAttributeValue(seriesAttribute.ColImpl.ConceptObjectCode, seriesAttribute.Val.ToString());

                    dmo = _tableResponse.GetNext();
                } while (dmo != null);


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
