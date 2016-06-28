using FlyController;
using FlyController.Model;
using FlyController.Model.Error;
using FlyEngine.Manager;
using FlyEngine.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Util;
using Org.Sdmxsource.Sdmx.Structureparser.Manager.Parsing;
using Org.Sdmxsource.Sdmx.Structureparser.Workspace;
using Org.Sdmxsource.Sdmx.Util.Objects.Reference;
using Org.Sdmxsource.Util.Io;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using SOAPSdmx.Model;
using OnTheFlyLog;
using FlyRedirectToRI_implementation;

namespace SOAPSdmx.Builder
{
    /// <summary>
    /// Implementation of QueryParser, read a query Sdmx 2.1 version and call a corret method of FlyEngine
    /// </summary>
    public class QueryParser21 : QueryParser
    {

        /// <summary>
        /// Create a instance of QueryParser20
        /// </summary>
        /// <param name="_QueryType">indicates from which the request was made entrypoint</param>
        public QueryParser21(QueryOperation.MessageTypeEnum _QueryType) :
            base(_QueryType, QueryOperation.MessageVersionEnum.Version2_1)
        {
        }


        /// <summary>
        /// identifies what the response structure, analyzing the query input and return a SdmxMessage (XElement return message or Error)
        /// </summary>
        /// <param name="SdmxQuery">Element query whitout Envelop header</param>
        /// <returns>Object for Write response in streaming <see cref="IFlyWriterBody"/></returns>
        public override IFlyWriterBody ParseElementQuery(XElement SdmxQuery)
        {
            XmlElement _originalQuery = new XmlDocument().ReadNode(SdmxQuery.CreateReader()) as XmlElement;
            try
            {
                switch (this.QueryType.MessageType)
                {
                    case QueryOperation.MessageTypeEnum.GetDataflow:
                    case QueryOperation.MessageTypeEnum.GetCategorisation:
                    case QueryOperation.MessageTypeEnum.GetCategoryScheme:
                    case QueryOperation.MessageTypeEnum.GetConceptScheme:
                    case QueryOperation.MessageTypeEnum.GetCodelist:
                    case QueryOperation.MessageTypeEnum.GetHierarchicalCodelist:
                    case QueryOperation.MessageTypeEnum.GetStructures:
                    case QueryOperation.MessageTypeEnum.GetOrganisationScheme:
                        FlyLog.WriteLog(this, FlyLog.LogTypeEnum.All, "Get Sdmx 2.1 Metadata: {0}", this.QueryType.MessageType.ToString());
                        return GetMetadata(_originalQuery);
                    case QueryOperation.MessageTypeEnum.GetGenericTimeSeriesData:
                    case QueryOperation.MessageTypeEnum.GetGenericData:
                    case QueryOperation.MessageTypeEnum.GetStructureSpecificData:
                    case QueryOperation.MessageTypeEnum.GetStructureSpecificTimeSeriesData:
                        FlyLog.WriteLog(this, FlyLog.LogTypeEnum.All, "Get Sdmx 2.1 Data: {0}", this.QueryType.MessageType.ToString());
                        return GetData(SdmxQuery);
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
        /// Parsing a query and Requires metadata to flyEngine
        /// </summary>
        /// <param name="_originalQuery">Element query whitout Envelop header</param>
        /// <returns>Object for Write response in streaming <see cref="IFlyWriterBody"/></returns>
        private IFlyWriterBody GetMetadata(XmlElement _originalQuery)
        {
            try
            {
                DataStructureEngineObject meo = new DataStructureEngineObject();

                IQueryParsingManager manager = new QueryParsingManager(SdmxSchemaEnumType.VersionTwoPointOne);
                IReadableDataLocation location = new XmlDocReadableDataLocation(_originalQuery);
                IQueryWorkspace workspace = manager.ParseQueries(location);
                location.Close();
                ISdmxParsingObject parsed = SdmxParsingObject.Parse(workspace);
                parsed.TimeStamp = CommonFunction.GetHeaderElement(_originalQuery, "ReportingBegin");
                meo.Engine(parsed, SdmxSchemaEnumType.VersionTwoPointOne);
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
        ///Parsing a query and Requires Data to flyEngine and populate a streaming response
        /// </summary>
        /// <param name="_originalQuery">Element query whitout Envelop header</param>
        /// <returns>Object for Write response in streaming <see cref="IFlyWriterBody"/></returns>
        private IFlyWriterBody GetData(XElement _originalQuery)
        {
            try
            {
                XmlElement XmlQuery = new XmlDocument().ReadNode(_originalQuery.CreateReader()) as XmlElement;

                IDataQueryParseManager manager = new DataQueryParseManager(SdmxSchemaEnumType.VersionTwoPointOne);

                IReadableDataLocation location = new XmlDocReadableDataLocation(XmlQuery);

                #region getdataflow
                string DataflowCode = "";
                string DataflowAgency = "*";
                string DataflowVersion = "ALL";
                string DataSetCode = "";

                try
                {
                    XElement _dataFlowElement = null;
                    if (_originalQuery.Descendants().Count(e => e.Name.LocalName.Trim().ToLower() == "dataflow") > 0)
                    {
                        _dataFlowElement = _originalQuery.Descendants().First(e => e.Name.LocalName.Trim().ToLower() == "dataflow");
                        if (_dataFlowElement == null || string.IsNullOrEmpty(CommonFunction.FindRefIDNodo21(_dataFlowElement)))
                        {
                            _dataFlowElement = _originalQuery.Descendants().First(e => e.Name.LocalName.Trim().ToLower() == "datasetid");
                            if (_dataFlowElement == null || string.IsNullOrEmpty(_dataFlowElement.Value))
                                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DataflowNotFound);
                            else
                                DataSetCode = _dataFlowElement.Value;
                        }
                        else
                        {
                            DataflowCode = CommonFunction.FindRefIDNodo21(_dataFlowElement);
                            XElement Dataflowref = _dataFlowElement.Elements().FirstOrDefault(el => el.Name.LocalName == "Ref");
                            if (Dataflowref!=null)
                            {
                                if (Dataflowref.Attribute("agencyID") != null && !string.IsNullOrEmpty(Dataflowref.Attribute("agencyID").Value))
                                    DataflowAgency = Dataflowref.Attribute("agencyID").Value;
                                if (Dataflowref.Attribute("version") != null && !string.IsNullOrEmpty(Dataflowref.Attribute("version").Value))
                                    DataflowVersion = Dataflowref.Attribute("version").Value;
                            }
                        }
                    }
                    else if (_originalQuery.Descendants().Count(e => e.Name.LocalName.Trim().ToLower() == "datasetid") > 0)
                    {
                        _dataFlowElement = _originalQuery.Descendants().First(e => e.Name.LocalName.Trim().ToLower() == "datasetid");
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

                    case QueryOperation.MessageTypeEnum.GetStructureSpecificData:
                        MessageType = MessageTypeEnum.StructureSpecific_21;
                        break;
                    case QueryOperation.MessageTypeEnum.GetStructureSpecificTimeSeriesData:
                        MessageType = MessageTypeEnum.StructureSpecificTimeSeries_21;
                        break;
                    case QueryOperation.MessageTypeEnum.GetGenericData:
                        MessageType = MessageTypeEnum.GenericData_21;
                        break;
                    case QueryOperation.MessageTypeEnum.GetGenericTimeSeriesData:
                        MessageType = MessageTypeEnum.GenericTimeSeries_21;
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

                    if (redirectToRI.CheckSOAPRedirectToRI(SdmxSchemaEnumType.VersionTwoPointOne))
                        return redirectToRI.GetSOAPResultFromRI(_originalQuery, this.QueryType.MessageType.ToString(), SdmxSchemaEnumType.VersionTwoPointOne);


                    if (string.IsNullOrEmpty(DataflowCode))
                        DataflowCode = DataSetCode;

                    DataMessageEngineObject _messageEngine = new DataMessageEngineObject()
                    {
                        DataFlowElementId = DataflowCode,
                        MessageType = MessageType,
                        VersionTypeResp = SdmxSchemaEnumType.VersionTwoPointOne,
                        TimeStamp = CommonFunction.GetHeaderElement(XmlQuery, "ReportingBegin"),
                    };
                    _messageEngine.ParseQueryMessage21(manager, location);
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