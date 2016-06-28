using FlyController.Model;
using FlyController.Model.Error;
using FlyEngine.Engine;
using FlyEngine.Manager;
using FlyEngine.Model;
using FlyController.Utils;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Manager.Retrieval;
using Org.Sdmxsource.Sdmx.Api.Model.Data.Query;
using Org.Sdmxsource.Sdmx.Api.Util;
using Org.Sdmxsource.Sdmx.Structureparser.Manager.Parsing;
using Org.Sdmxsource.Sdmx.Structureparser.Workspace;
using Org.Sdmxsource.Sdmx.Util.Objects.Reference;
using Org.Sdmxsource.Util.Io;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using SOAPSdmx.Model;
using OnTheFlyLog;
using Org.Sdmxsource.Sdmx.Structureparser.Builder.Query;
using FlyController.Builder.ConstrainParse;
using FlyController;
using FlyRedirectToRI_implementation;

namespace SOAPSdmx.Builder
{
    /// <summary>
    /// Implementation of QueryParser, read a query Sdmx 2.0 version and call a corret method of FlyEngine
    /// </summary>
    public class QueryParser20 : QueryParser
    {
        /// <summary>
        /// Create a instance of QueryParser20
        /// </summary>
        /// <param name="_QueryType">indicates from which the request was made entrypoint</param>
        public QueryParser20(QueryOperation.MessageTypeEnum _QueryType) :
            base(_QueryType, QueryOperation.MessageVersionEnum.Version2_0)
        {
        }


        /// <summary>
        /// identifies what the response structure, analyzing the query input and return a SdmxMessage (XElement return message or Error)
        /// </summary>
        /// <param name="SdmxQuery">Element query whitout Envelop header</param>
        /// <returns>Object for Write response in streaming <see cref="IFlyWriterBody"/></returns>
        public override IFlyWriterBody ParseElementQuery(XElement SdmxQuery)
        {
            try
            {
                switch (this.QueryType.MessageType)
                {
                    case QueryOperation.MessageTypeEnum.GetCompactData:
                    case QueryOperation.MessageTypeEnum.GetCrossSectionalData:
                    case QueryOperation.MessageTypeEnum.GetGenericData:
                        FlyLog.WriteLog(this, FlyLog.LogTypeEnum.All, "Get Sdmx 2.0 Data: {0}", this.QueryType.MessageType.ToString());
                        return GetData(SdmxQuery);
                    case QueryOperation.MessageTypeEnum.QueryStructure:
                        FlyLog.WriteLog(this, FlyLog.LogTypeEnum.All, "Get Sdmx 2.0 Metadata: {0}", this.QueryType.MessageType.ToString());
                        return GetMetadata(SdmxQuery);
                    default:
                        throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.InternalError, new Exception("QueryType not recognized"));

                }
            }
            catch (SdmxException) { throw; }
            catch (Exception sdmxerr)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.ParsingQueryError, sdmxerr);
            }
        }

        /// <summary>
        /// Parsing a query and Requires metadata to flyEngine and populate a streaming response
        /// </summary>
        /// <param name="SdmxQuery">Element query whitout Envelop header</param>
        /// <returns>Object for Write response in streaming <see cref="IFlyWriterBody"/></returns>
        private IFlyWriterBody GetMetadata(XElement SdmxQuery)
        {
            try
            {
                XmlElement _originalQuery = new XmlDocument().ReadNode(SdmxQuery.CreateReader()) as XmlElement;

                DataStructureEngineObject meo = new DataStructureEngineObject();

                IQueryParsingManager manager = new QueryParsingManager(SdmxSchemaEnumType.VersionTwo, new QueryBuilder(null, new ConstrainQueryBuilderV2(), null));
                IReadableDataLocation location = new XmlDocReadableDataLocation(_originalQuery);
                IQueryWorkspace workspace = manager.ParseQueries(location);
                location.Close();
                ISdmxParsingObject parsed = SdmxParsingObject.Parse(workspace);
                parsed.TimeStamp = CommonFunction.GetHeaderElement(_originalQuery, "ReportingBegin");


                if (parsed.SdmxStructureType==SdmxStructureEnumType.CodeList && !string.IsNullOrEmpty(parsed.ConstrainDataFlow))
                {
                    //Check Redirect request to RI Web Service
                    RedirectToRI redirectToRI = new RedirectToRI(parsed.ConstrainDataFlow, parsed.ConstrainDataFlowAgency, parsed.ConstrainDataFlowVersion);
                    if (redirectToRI.CheckSOAPRedirectToRI(SdmxSchemaEnumType.VersionTwo))
                        return redirectToRI.GetSOAPResultFromRI(SdmxQuery, this.QueryType.MessageType.ToString(), SdmxSchemaEnumType.VersionTwo);
                }


                meo.Engine(parsed, SdmxSchemaEnumType.VersionTwo);
                if (meo.HaveError)
                    throw meo.ErrorMessage;

                return meo.GetResult();
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetMetadata, ex);
            }
        }

        /// <summary>
        /// Parsing a query and Requires Data to flyEngine and populate a streaming response
        /// </summary>
        /// <param name="_originalQuery">Element query whitout Envelop header</param>
        /// <returns>Object for Write response in streaming <see cref="IFlyWriterBody"/></returns>
        private IFlyWriterBody GetData(XElement _originalQuery)
        {
            try
            {
                XmlElement XmlQuery = new XmlDocument().ReadNode(_originalQuery.CreateReader()) as XmlElement;

                IDataQueryParseManager manager = new DataQueryParseManager(SdmxSchemaEnumType.VersionTwo);

                IReadableDataLocation location = new XmlDocReadableDataLocation(XmlQuery);

                #region getdataflow
                XElement _dataFlowElement = null;
                string DataflowCode = "";
                string DataflowAgency = "*";
                string DataflowVersion = "ALL";
                string DataSetCode = "";
                try
                {
                    if (_originalQuery.Descendants().Count(e => e.Name.LocalName.Trim().ToLower() == "dataflow") > 0)
                    {
                        _dataFlowElement = _originalQuery.Descendants().First(e => e.Name.LocalName.Trim().ToLower() == "dataflow");
                        if (_dataFlowElement == null || string.IsNullOrEmpty(_dataFlowElement.Value))
                            throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DataflowNotFound);
                        else
                            DataflowCode = _dataFlowElement.Value;
                    }
                    if (_originalQuery.Descendants().Count(e => e.Name.LocalName.Trim().ToLower() == "dataset") > 0)
                    {
                        _dataFlowElement = _originalQuery.Descendants().First(e => e.Name.LocalName.Trim().ToLower() == "dataset");
                        if (_dataFlowElement == null || string.IsNullOrEmpty(_dataFlowElement.Value))
                            throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DataflowNotFound);
                        else
                            DataSetCode = _dataFlowElement.Value;
                    }
                    if (string.IsNullOrEmpty(DataflowCode) && string.IsNullOrEmpty(DataSetCode))
                        throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DataflowNotFound);

                   
                }
                catch (SdmxException) { throw; }
                catch (Exception sdmxerr)
                {
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DataflowNotFound, sdmxerr);
                }
                #endregion

                MessageTypeEnum MessageType = MessageTypeEnum.None;
                switch (this.QueryType.MessageType)
                {
                    case QueryOperation.MessageTypeEnum.GetCompactData:
                        MessageType = MessageTypeEnum.Compact_20;
                        break;
                    case QueryOperation.MessageTypeEnum.GetCrossSectionalData:
                        MessageType = MessageTypeEnum.CrossSectional_20;
                        break;
                    case QueryOperation.MessageTypeEnum.GetGenericData:
                        MessageType = MessageTypeEnum.Generic_20;
                        break;
                }
                if (MessageType == MessageTypeEnum.None)
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.InternalError, new Exception("No Message Type recognized"));


                try
                {

                    //Check Redirect request to RI Web Service
                    RedirectToRI redirectToRI = null;
                    if (!string.IsNullOrEmpty(DataflowCode))
                        redirectToRI = new RedirectToRI(DataflowCode, DataflowAgency, DataflowVersion);
                    else
                        redirectToRI = new RedirectToRI(DataSetCode);

                    if (redirectToRI.CheckSOAPRedirectToRI(SdmxSchemaEnumType.VersionTwo))
                        return redirectToRI.GetSOAPResultFromRI(_originalQuery, this.QueryType.MessageType.ToString(), SdmxSchemaEnumType.VersionTwo);

                    if (string.IsNullOrEmpty(DataflowCode))
                        DataflowCode = DataSetCode;

                    DataMessageEngineObject _messageEngine = new DataMessageEngineObject()
                    {
                        DataFlowElementId = DataflowCode,
                        MessageType = MessageType,
                        VersionTypeResp = SdmxSchemaEnumType.VersionTwo,
                        TimeStamp = CommonFunction.GetHeaderElement(XmlQuery, "ReportingBegin"),
                    };

                    _messageEngine.ParseQueryMessage20(manager, location);
                    if (_messageEngine.HaveError)
                        throw _messageEngine.ErrorMessage;

                    IFlyWriterBody WriterBody = _messageEngine.GetResult();

                    return WriterBody;
                }
                catch (Exception) { throw; }
                finally
                {
                    location.Close();
                }
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetData, ex);
            }

        }

    }
}