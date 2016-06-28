using FlyController.Model;
using FlyController.Model.Error;
using FlyController.Model.Streaming;
using FlyEngine.Manager;
using FlyEngine.Model;
using FlyMapping.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FlyEngine.Engine.Data
{
    /// <summary>
    /// WriterDataBase is abstract class, used for Write Data Message
    /// </summary>
    public abstract class WriterDataBase
    {
        /// <summary>
        /// Message Type, describe which endpoint has been used and what is the kind of answer to give
        /// </summary>
        public MessageTypeEnum MessageType { get; set; }
        /// <summary>
        /// Response Table, contains data formatted in DataTableMessageObject structure
        /// </summary>
        public IDataTableMessageObject _tableResponse { get; set; }
        /// <summary>
        /// RetrievalManager, used for parse query
        /// </summary>
        public RetrievalManager _retrievalManager { get; set; }
        /// <summary>
        /// Dimension At Observation, describe if query is TimeSeries or CrossSectional and contains "AllDimensions" for FLAT result
        /// </summary>
        public string DimensionAtObservation { get; set; }


        /// <summary>
        /// assists research of all Dimension (Escluded timeDimension) in DataTableMessageObject for format the SeriesKey
        /// </summary>
        /// <param name="dim">Dimension test</param>
        /// <returns>Return true if DataMessageObject is Valid</returns>
        internal bool OnlySeriesKey(DataMessageObject dim)
        {
            try
            {


                if (dim == null || dim.Val == null)
                    return false;
                if (dim.ColImpl.ConceptType == ConceptTypeEnum.Dimension)
                {
                    if (string.IsNullOrEmpty(DimensionAtObservation) || new List<string> { "TIME_PERIOD", "ALLDIMENSIONS" }.Contains(DimensionAtObservation.Trim().ToUpper()))
                    {
                        if (((IDimensionConcept)dim.ColImpl).DimensionType != DimensionTypeEnum.Time)
                            return true;
                    }
                    else
                    {
                        if (dim.ColImpl.Id.Trim().ToUpper() != DimensionAtObservation.Trim().ToUpper())
                            return true;
                    }

                }
                return false;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetSeriesKey, ex);
            }
        }

        /// <summary>
        /// assists research of all Attribute with AttachmentLevel= DimensionGroup in DataTableMessageObject for format the SeriesKey
        /// </summary>
        /// <param name="att">Attribute test</param>
        /// <returns>Return true if DataMessageObject is Valid</returns>
        internal bool OnlySeriesAttribute(DataMessageObject att)
        {
            try
            {
                if (att == null || att.Val == null || att.Val.ToString() == String.Empty)
                    return false;
                if (att.ColImpl.ConceptType == ConceptTypeEnum.Attribute)
                {
                    if (((IAttributeConcept)att.ColImpl).AttributeAttachmentLevelType == AttributeAttachmentLevel.DimensionGroup)
                        return true;
                }
                return false;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetSeriesAttribute, ex);
            }
        }
        /// <summary>
        /// assists research of all Attribute with AttachmentLevel= Observation in DataTableMessageObject for format the ObservationKey
        /// </summary>
        /// <param name="att">Attribute test</param>
        /// <returns>Return true if DataMessageObject is Valid</returns>
        internal bool OnlyObservationAttribute(DataMessageObject att)
        {
            try
            {
                if (att == null || att.Val == null)
                    return false;
                if (att.ColImpl.ConceptType == ConceptTypeEnum.Attribute)
                {
                    if (((IAttributeConcept)att.ColImpl).AttributeAttachmentLevelType == AttributeAttachmentLevel.Observation && !((IAttributeConcept)att.ColImpl).IsValueAttribute)
                        return true;
                }
                return false;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetObservationAttribute, ex);
            }
        }
        /// <summary>
        /// assists research of all Attribute with AttachmentLevel= DataSet in DataTableMessageObject for format the DatasetDescription
        /// </summary>
        /// <param name="att">Attribute test</param>
        /// <returns>Return true if DataMessageObject is Valid</returns>
        internal bool OnlyDatasetAttribute(DataMessageObject att)
        {
            try
            {
                if (att == null || att.Val == null)
                    return false;
                if (att.ColImpl.ConceptType == ConceptTypeEnum.Attribute)
                {
                    if (((IAttributeConcept)att.ColImpl).AttributeAttachmentLevelType == AttributeAttachmentLevel.DataSet)
                        return true;
                }
                return false;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetDataSetAttribute, ex);
            }
        }

        /// <summary>
        /// Flag used for ChangeSeries
        /// </summary>
        internal string LastSeriesKey = null;


        /// <summary>
        /// Check if Serie must be Change
        /// </summary>
        /// <param name="dmo"></param>
        /// <returns></returns>
        internal bool CheckChanged(List<DataMessageObject> dmo)
        {
            try
            {
                string ActualSeriesKey = string.Join(",", dmo.FindAll(dim => OnlySeriesKey(dim) || OnlySeriesAttribute(dim)));
                if (string.IsNullOrEmpty(LastSeriesKey) || !LastSeriesKey.Equals(ActualSeriesKey))
                {
                    LastSeriesKey = ActualSeriesKey;
                    return true;
                }
                return false;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CheckSeriesChange, ex);
            }
        }


        /// <summary>
        /// Overridable Function for Write Data
        /// </summary>
        /// <param name="retData">return XmlWriter</param>
        public abstract void WriteDataMessage(IFlyWriter retData);

    }
}
