using FlyEngine.Model;
using FlyController.Builder;
using FlyController.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Manager.Retrieval;
using Org.Sdmxsource.Sdmx.Api.Model.Data.Query;
using Org.Sdmxsource.Sdmx.Api.Util;
using Org.Sdmxsource.Sdmx.Structureparser.Manager.Parsing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using FlyController.Model.Error;
using FlyMapping.Model;
using FlyController;
using Org.Sdmxsource.Sdmx.Api.Model.Data.Query.Complex;
using System.Xml;
using System.IO;
using System.Data.SqlClient;
using FlyMapping.Build;
using Org.Sdmxsource.Sdmx.Api.Model.Query;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Data.Query;
using FlyController.Model.Delegate;
using FlyController.Model.WhereParsing;

namespace FlyEngine.Manager
{
    /// <summary>
    /// EntryPoint Class for request Data Message
    /// </summary>
    public class DataMessageEngineObject : DataEngineObject
    {

        /// <summary>
        /// Xml ID where to get Dataflow information
        /// </summary>
        public String DataFlowElementId { get; set; }
        /// <summary>
        /// Where Statement used for request data from database
        /// </summary>
        public IDataWhereStatment WhereStatement { get; set; }
        /// <summary>
        /// Type of request, both SDMX 2.0 and Sdmx 2.1
        /// </summary>
        public MessageTypeEnum MessageType { get; set; }
        /// <summary>
        /// Retrieval Manager contains DataStructure information
        /// </summary>
        public RetrievalManager RetrievalManagerObject { get; set; }
        /// <summary>
        /// Data builder object Contains data to return and function for write return streaming data
        /// </summary>
        private DataMessageObjectBuilder Builder { get; set; }

        /// <summary>
        /// LastUpdate parameter request only observation from this date onwards
        /// </summary>
        public string TimeStamp { get; set; }


      

        /// <summary>
        /// First Parse Message arrived from external process for SDMX 2.0
        /// </summary>
        /// <param name="manager">data parsed from DataQueryParseManager (Sdmx CommonAPI)</param>
        /// <param name="location">Arrived Message converted to IReadableDataLocation</param>
        public void ParseQueryMessage20(IDataQueryParseManager manager, IReadableDataLocation location)
        {
            HaveError = false;
            ErrorMessage = null;
            try
            {
                RetrievalManagerObject = new RetrievalManager(DataFlowElementId, SdmxSchemaEnumType.VersionTwo);
                //RetrievalManagerObject.
                IList<IDataQuery> WhereStatement20 = manager.BuildDataQuery(location, (ISdmxObjectRetrievalManager)RetrievalManagerObject);
                WhereStatement = new DataWhereStatment(WhereStatement20);
            }
            catch (SdmxException sdmxerr)
            {
                HaveError = true;
                ErrorMessage = sdmxerr;
            }
            catch (Exception err)
            {
                HaveError = true;
                ErrorMessage = new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.ParsingQueryError, err);
            }
        }
        /// <summary>
        /// First Parse Message arrived from external process for SDMX 2.1
        /// </summary>
        /// <param name="manager">data parsed from DataQueryParseManager (Sdmx CommonAPI)</param>
        /// <param name="location">Arrived Message converted to IReadableDataLocation</param>
        public void ParseQueryMessage21(IDataQueryParseManager manager, IReadableDataLocation location)
        {
            HaveError = false;
            ErrorMessage = null;
            try
            {
                RetrievalManagerObject = new RetrievalManager(DataFlowElementId, SdmxSchemaEnumType.VersionTwoPointOne);

                IList<IComplexDataQuery> WhereStatement21 = manager.BuildComplexDataQuery(location, (ISdmxObjectRetrievalManager)RetrievalManagerObject);
                WhereStatement = new DataWhereStatment(WhereStatement21);
            }
            catch (SdmxException sdmxerr)
            {
                HaveError = true;
                ErrorMessage = sdmxerr;
            }
            catch (Exception err)
            {
                HaveError = true;
                ErrorMessage = new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.ParsingQueryError, err);
            }
        }
        /// <summary>
        /// First Parse Message arrived from external process for REST SDMX
        /// </summary>
        /// <param name="query">data parsed from Sdmx CommonAPI</param>
        public void ParseQueryMessageREST(IRestDataQuery query)
        {
            HaveError = false;
            ErrorMessage = null;
            try
            {
                ISdmxParsingObject pc = new SdmxParsingObject(SdmxStructureEnumType.Any)
                {
                    AgencyId = query.FlowRef.AgencyId,
                    _version = query.FlowRef.Version,
                };
                pc.PreliminarCheck();
                RetrievalManagerObject = new RetrievalManager(DataFlowElementId, this.VersionTypeResp);

                //RetrievalManagerObject.
                WhereStatement = new DataWhereStatment(query);
            }
            catch (SdmxException sdmxerr)
            {
                HaveError = true;
                ErrorMessage = sdmxerr;
            }
            catch (Exception err)
            {
                HaveError = true;
                ErrorMessage = new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.ParsingQueryError, err);
            }
        }

        /// <summary>
        /// Get parsed result of request
        /// </summary>
        /// <returns>Object for Write response in streaming <see cref="IFlyWriterBody"/></returns>
        public override IFlyWriterBody GetResult()
        {
            try
            {
                IDataMessageManager dmb =MappingConfiguration.DataFactory(this.VersionTypeResp);
                dmb.Concepts = this.RetrievalManagerObject.GetAllConceptsImpl();


                Builder = new DataMessageObjectBuilder()
                {
                    MessageType = this.MessageType,
                    _versionTypeResp = this.VersionTypeResp,
                    _retrievalManager = this.RetrievalManagerObject,

                };
                if (VersionTypeResp == SdmxSchemaEnumType.VersionTwoPointOne && this.WhereStatement != null)
                {
                    Builder.DimensionAtObservation = this.WhereStatement.DimensionAtObservation;
                    if (!string.IsNullOrEmpty(Builder.DimensionAtObservation))
                    {
                        IConceptObjectImpl DimAtObs = dmb.Concepts.Find(c => c.Id.Trim().ToUpper() == Builder.DimensionAtObservation);
                        if (DimAtObs != null && DimAtObs is IDimensionConcept &&
                           ((IDimensionConcept)DimAtObs).DimensionType != DimensionTypeEnum.Time && DimAtObs.Id != "AllDimensions")
                       {
                           throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.NotImplemented,new Exception("DimensionAtObservation can be only TIME_PERIOD or AllDimensions"));
                       }
                    }
                }
                if (this.WhereStatement.IsRest)
                    this.WhereStatement.BuildRestQuery(this.RetrievalManagerObject);


                WriteResponseDelegate callback = new WriteResponseDelegate(Builder.WriteDataMessage);
                return dmb.GetTableMessage(DataFlowElementId, this.WhereStatement, callback, this.TimeStamp);

            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DataMessageEngineGetResults, ex);
            }
            finally
            {
                DestroyObjects();
            }
        }


        /// <summary>
        /// Dispose all Object used for retreial data
        /// </summary>
        public override void DestroyObjects()
        {
            try
            {
                this.RetrievalManagerObject = null;

            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DestroyObjects, ex);
            }
        }

    }
}
