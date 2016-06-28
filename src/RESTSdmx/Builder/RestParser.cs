using FlyController;
using FlyController.Model;
using FlyController.Model.Error;
using FlyController.Model.Streaming;
using FlyEngine.Manager;
using FlyEngine.Model;
using FlyRedirectToRI_implementation;
using OnTheFlyLog;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Data.Query;
using Org.Sdmxsource.Sdmx.Api.Model.Query;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Data.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RESTSdmx.Builder
{
    /// <summary>
    /// Parsing Query REST divided on Metadata and Data query
    /// </summary>
    public class RestParser
    {
        #region Metadata
        /// <summary>
        /// Action Fired when client read a body
        /// </summary>
        /// <param name="query">Contains all object and methods for writing a response <see cref="IRestStructureQuery"/></param>
        /// <param name="schemaVersion">Sdmx Version</param>
        /// <param name="ConstrainParameter">Parameters for Codelist Contrained</param>
        /// <returns>Action FlyWriter, Queue Action populated streaming trasport object FlyWriter</returns>
        public Action<IFlyWriter, Queue<Action>> GenerateResponseFunctionMetadata(IRestStructureQuery query, SdmxSchema schemaVersion, string ConstrainParameter)
        {
            IFlyWriterBody WriterBody = ParseMetadataQuery(query, schemaVersion, ConstrainParameter);
            return (writer, actions) => WriterBody.WriterBody(writer);
        }

        /// <summary>
        /// Parse Query Metadata
        /// </summary>
        /// <param name="query">Contains all object and methods for writing a response <see cref="IRestStructureQuery"/></param>
        /// <param name="schemaVersion">Sdmx Version</param>
        /// <param name="ConstrainParameter">Parameters for Codelist Contrained</param>
        /// <returns>Action FlyWriter, Queue Action populated streaming trasport object FlyWriter</returns>
        private IFlyWriterBody ParseMetadataQuery(IRestStructureQuery query, SdmxSchema schemaVersion, string ConstrainParameter)
        {
            FlyLog.WriteLog(this, FlyLog.LogTypeEnum.All, "Parsing Rest Query...");

            DataStructureEngineObject meo = new DataStructureEngineObject();

            ISdmxParsingObject parsingObject = SdmxParsingObject.Parse(query, ConstrainParameter);
            if (schemaVersion.EnumType == SdmxSchemaEnumType.VersionTwo && parsingObject.QueryDetail != StructureQueryDetailEnumType.Full && parsingObject.QueryDetail != StructureQueryDetailEnumType.Null)
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.RestDetail20);

            if (parsingObject.SdmxStructureType == SdmxStructureEnumType.CodeList && !string.IsNullOrEmpty(parsingObject.ConstrainDataFlow))
            {
                //Check Redirect request to RI Web Service
                RedirectToRI redirectToRI = new RedirectToRI(parsingObject.ConstrainDataFlow, parsingObject.ConstrainDataFlowAgency, parsingObject.ConstrainDataFlowVersion);
                if (redirectToRI.CheckSOAPRedirectToRI(SdmxSchemaEnumType.VersionTwo))
                    return redirectToRI.GetRESTCodelistConstrain(parsingObject);
            }

            meo.Engine(parsingObject, schemaVersion);
            if (meo.HaveError)
                throw meo.ErrorMessage;

            return meo.GetResult();
        }

        #endregion

        #region Data
        /// <summary>
        /// Action Fired when client read a body
        /// </summary>
        /// <param name="query">Contains all object and methods for writing a response <see cref="IRestDataQuery"/></param>
        /// <param name="schemaVersion">Sdmx Version</param>
        /// <param name="format">request format of response</param>
        /// <param name="requestAccept">entire request string</param>
        /// <param name="originalQuery">Original String Query required</param>
        /// <returns>Action FlyWriter, Queue Action populated streaming trasport object FlyWriter</returns>
        public Action<IFlyWriter, Queue<Action>> GenerateResponseFunctionData(IRestDataQuery query, SdmxSchema schemaVersion, RESTSdmx.Utils.DataMediaType.FLYBaseDataFormatEnumType format, string requestAccept, string originalQuery)
        {
            IFlyWriterBody WriterBody = ParseDataQuery(query, schemaVersion, format, requestAccept, originalQuery);
            return (writer, actions) => WriterBody.WriterBody(writer);
        }
        /// <summary>
        /// Parse Query Data
        /// </summary>
        /// <param name="query">Contains all object and methods for writing a response <see cref="IRestStructureQuery"/></param>
        /// <param name="schemaVersion">Sdmx Version</param>
        /// <param name="format">Type of request</param>
        /// <param name="requestAccept">entire request string</param>
        /// <param name="originalQuery">Original String Query required</param>
        /// <returns>Action FlyWriter, Queue Action populated streaming trasport object FlyWriter</returns>
        private IFlyWriterBody ParseDataQuery(IRestDataQuery query, SdmxSchema schemaVersion, RESTSdmx.Utils.DataMediaType.FLYBaseDataFormatEnumType format, string requestAccept, string originalQuery)
        {
            try
            {
                MessageTypeEnum MessageType = MessageTypeEnum.None;
                switch (format)
                {
                    case RESTSdmx.Utils.DataMediaType.FLYBaseDataFormatEnumType.Compact:
                        if (schemaVersion.EnumType == SdmxSchemaEnumType.VersionTwo)
                            MessageType = MessageTypeEnum.Compact_20;
                        else
                            MessageType = MessageTypeEnum.StructureSpecific_21;
                        break;
                    case RESTSdmx.Utils.DataMediaType.FLYBaseDataFormatEnumType.Generic:
                        if (schemaVersion.EnumType == SdmxSchemaEnumType.VersionTwo)
                            MessageType = MessageTypeEnum.Generic_20;
                        else
                            MessageType = MessageTypeEnum.GenericData_21;
                        break;

                    case Utils.DataMediaType.FLYBaseDataFormatEnumType.Rdf:
                        MessageType = MessageTypeEnum.Rdf;
                        break;
                    case Utils.DataMediaType.FLYBaseDataFormatEnumType.Dspl:
                        MessageType = MessageTypeEnum.Dspl;
                        break;
                    case Utils.DataMediaType.FLYBaseDataFormatEnumType.Json:
                        MessageType = MessageTypeEnum.Json;
                        break;
                }


                if (MessageType == MessageTypeEnum.None)
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.InternalError, new Exception("No Message Type recognized"));

                DataMessageEngineObject _messageEngine = new DataMessageEngineObject()
                {
                    DataFlowElementId = query.FlowRef.MaintainableReference.MaintainableId,
                    MessageType = MessageType,
                    VersionTypeResp = schemaVersion
                };

                //Check Redirect request to RI Web Service
                RedirectToRI redirectToRI = new RedirectToRI(query.FlowRef.MaintainableReference.MaintainableId, query.FlowRef.MaintainableReference.AgencyId, query.FlowRef.MaintainableReference.Version);
                if (redirectToRI.CheckRESTRedirectToRI())
                {
                    //return redirectToRI.GetRESTResultFromRI(originalQuery, requestAccept);
                    RetrievalManager SdmxRetrievalManager = new RetrievalManager(query.FlowRef.MaintainableId, SdmxSchemaEnumType.VersionTwoPointOne);
                    IDataQuery dataquery = new DataQueryImpl(query, SdmxRetrievalManager);
                    if (query == null)
                        throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.RetrivalParsingError);
                    return redirectToRI.GetRESTResultFromRIToSoap(dataquery, MessageType);
                }


                _messageEngine.ParseQueryMessageREST(query);
                if (_messageEngine.HaveError)
                    throw _messageEngine.ErrorMessage;
                FlyLog.WriteLog(this, FlyLog.LogTypeEnum.All, "Parsed Rest query. Start get Result");

                return _messageEngine.GetResult();
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetData, ex);
            }
        }

        #endregion
    }
}
