using FlyController;
using FlyController.Model;
using FlyController.Model.Error;
using FlyController.Model.Streaming;
using FlyController.Streaming;
using FlyController.Utils;
using OnTheFlyLog;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Query;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Objects.Reference;
using RESTSdmx.Builder;
using RESTSdmx.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;

namespace RESTSdmx
{
    /// <summary>
    /// The data resource.
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(Namespace = Constant.ServiceNamespace, InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class DataResource : IDataResource
    {

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DataResource" /> class.
        /// </summary>
        public DataResource()
        {
            FlyConfiguration.InitConfig(AppDomain.CurrentDomain.RelativeSearchPath);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get generic data.
        /// </summary>
        /// <param name="flowRef">
        /// The flow ref.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="providerRef">
        /// The provider ref.
        /// </param>
        /// <returns>
        /// The <see cref="Message"/>.
        /// </returns>
        /// <exception cref="WebFaultException">
        /// </exception>
        /// <exception cref="WebFaultException{T}">
        /// </exception>
        public Message GetGenericData(string flowRef, string key, string providerRef)
        {
            try
            {

                WebOperationContext ctx = WebOperationContext.Current;

                if (string.IsNullOrEmpty(flowRef))
                {
                    FlyLog.WriteLog(this, FlyLog.LogTypeEnum.Error, "Arrived Rest Query with flowRef Null Or Empty (NotImplemented)");
                    throw new WebFaultException(HttpStatusCode.NotImplemented); // $$$ Strange 501 ?
                }

                return this.ProcessRequest(flowRef, key, providerRef, ctx);
            }
            catch (SdmxException e)
            {
                throw RESTError.BuildException(e);
            }
        }

        /// <summary>
        /// The get generic data all keys.
        /// </summary>
        /// <param name="flowRef">
        /// The flow ref.
        /// </param>
        /// <returns>
        /// The <see cref="Message"/>.
        /// </returns>
        public Message GetGenericDataAllKeys(string flowRef)
        {
            return this.GetGenericData(flowRef, "ALL", "ALL");
        }

        /// <summary>
        /// The get generic data all providers.
        /// </summary>
        /// <param name="flowRef">
        /// The flow ref.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="Message"/>.
        /// </returns>
        public Message GetGenericDataAllProviders(string flowRef, string key)
        {
            return this.GetGenericData(flowRef, key, "ALL");
        }

        #endregion

        #region Methods

        /// <summary>
        /// The build query bean.
        /// </summary>
        /// <param name="flowRef">
        /// The flow ref.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="providerRef">
        /// The provider ref.
        /// </param>
        /// <param name="queryParameters">
        /// The query parameters.
        /// </param>
        /// <returns>
        /// The <see cref="IRestDataQuery"/>.
        /// </returns>
        /// <exception cref="WebFaultException{T}">
        /// </exception>
        private IRestDataQuery BuildQueryBean(string flowRef, string key, string providerRef, NameValueCollection queryParameters)
        {
            var queryString = new string[4];
            queryString[0] = "data";
            queryString[1] = flowRef;
            queryString[2] = key;
            queryString[3] = providerRef;

            IDictionary<string, string> paramsDict = RestUtils.GetQueryStringAsDict(queryParameters);
            IRestDataQuery restQuery;

            try
            {
                restQuery = new RESTDataQueryCore(queryString, paramsDict);
            }
            catch (SdmxException e)
            {
                throw RESTError.BuildException(e);
            }
            catch (Exception e)
            {
                throw new WebFaultException<string>(e.Message, HttpStatusCode.BadRequest);
            }

            return restQuery;
        }

        /// <summary>
        /// The get version from accept.
        /// </summary>
        /// <param name="acceptHeaderList">
        /// The accept header list.
        /// </param>
        /// <param name="mediatype">
        /// The mediatype.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetVersionFromAccept(IEnumerable<ContentType> acceptHeaderList, string mediatype)
        {
            string version = null;

            var accept = acceptHeaderList.Where(h => h.MediaType.Contains(mediatype)).FirstOrDefault();
            if (accept != null)
            {
                if (accept.Parameters != null && accept.Parameters.ContainsKey("version"))
                {
                    version = accept.Parameters["version"];
                }
            }

            return version;
        }

        /// <summary>
        /// The get version from media type.
        /// </summary>
        /// <param name="selectedMediaTypeWithVersion">
        /// The selected media type with version.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <returns>
        /// The <see cref="SdmxSchema"/>.
        /// </returns>
        private static SdmxSchema GetVersionFromMediaType(ContentType selectedMediaTypeWithVersion, RESTSdmx.Utils.DataMediaType.FLYBaseDataFormatEnumType format)
        {
            SdmxSchema version = null;

            switch (format)
            {
                case RESTSdmx.Utils.DataMediaType.FLYBaseDataFormatEnumType.Edi:
                    version = SdmxSchema.GetFromEnum(SdmxSchemaEnumType.Edi);
                    break;
                case RESTSdmx.Utils.DataMediaType.FLYBaseDataFormatEnumType.Csv:
                    version = SdmxSchema.GetFromEnum(SdmxSchemaEnumType.Csv);
                    break;
                default:
                    version = RestUtils.GetVersionFromMediaType(selectedMediaTypeWithVersion);
                    break;
            }

            return version;
        }

        /// <summary>
        /// The process request.
        /// </summary>
        /// <param name="flowRef">
        /// The flow ref.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="providerRef">
        /// The provider ref.
        /// </param>
        /// <param name="ctx">
        /// The ctx.
        /// </param>
        /// <returns>
        /// The <see cref="Message"/>.
        /// </returns>
        /// <exception cref="WebFaultException{T}">
        /// </exception>
        private Message ProcessRequest(string flowRef, string key, string providerRef, WebOperationContext ctx)
        {
            try
            {
                string requestAccept = ctx.IncomingRequest.Accept;

                IList<ContentType> acceptHeaderElements = ctx.IncomingRequest.GetAcceptHeaderElements();

                var defaultMediaType = DataMediaType.GetFromEnum(DataMediaEnumType.GenericData).MediaType;
                var contentType = RestUtils.GetContentType(ctx, h => DataMediaType.GetTypeFromName(h.MediaType) != null, defaultMediaType);

                string requestedVersion = RestUtils.GetVersionFromAccept(acceptHeaderElements, contentType.MediaType);

                var datamediaType = DataMediaType.GetTypeFromName(contentType.MediaType);
                var selectedMediaTypeWithVersion = datamediaType.GetMediaTypeVersion(requestedVersion);

                FlyMediaEnum FlyMedia = FlyMediaEnum.Sdmx;
                if (datamediaType.EnumType == DataMediaEnumType.RdfData)
                    FlyMedia = FlyMediaEnum.Rdf;
                if (datamediaType.EnumType == DataMediaEnumType.DSPLData)
                    FlyMedia = FlyMediaEnum.Dspl;
                else if (datamediaType.EnumType == DataMediaEnumType.JsonData)
                    FlyMedia = FlyMediaEnum.Json;

                if (selectedMediaTypeWithVersion == null)
                {
                    FlyLog.WriteLog(this, FlyLog.LogTypeEnum.Error, "Arrived Rest Query Cannot serve content type: {0} (NotAcceptable)", requestAccept);
                    throw new WebFaultException<string>("Cannot serve content type: " + requestAccept, HttpStatusCode.NotAcceptable);
                }

                RESTSdmx.Utils.DataMediaType.FLYBaseDataFormatEnumType format = DataMediaType.GetTypeFromName(selectedMediaTypeWithVersion.MediaType).Format;
                SdmxSchema version = GetVersionFromMediaType(selectedMediaTypeWithVersion, format);

                FlyLog.WriteLog(this, FlyLog.LogTypeEnum.Warning, @"
_______________________________________________________________________________
ARRIVED REST DATA QUERY
flowRef {0}, key {1}, providerRef {2}, 
selectedMediaTypeWithVersion {3} ", flowRef, key, providerRef, selectedMediaTypeWithVersion);


                IRestDataQuery query = this.BuildQueryBean(flowRef, key, providerRef, ctx.IncomingRequest.UriTemplateMatch.QueryParameters);

                RestParser RestParser = new Builder.RestParser();
                string OriginalQuery = ctx.IncomingRequest.UriTemplateMatch.RequestUri.ToString().Replace(ctx.IncomingRequest.UriTemplateMatch.BaseUri.ToString(), "data");
                IStreamController<IFlyWriter> streamController = new StreamController<IFlyWriter>(RestParser.GenerateResponseFunctionData(query, version, format, requestAccept, OriginalQuery));


                var charSetEncoding = RestUtils.GetCharSetEncoding(contentType);
                var responseContentType = RestUtils.GetContentType(contentType, selectedMediaTypeWithVersion);
                selectedMediaTypeWithVersion.CharSet = charSetEncoding.WebName;
                return ctx.CreateStreamResponse(stream => RestUtils.StreamXml(FlyMedia, version, stream, streamController, charSetEncoding), responseContentType);
            }
            catch (Exception sdmxerr)
            {
                SdmxException ex = null;
                if (sdmxerr is SdmxException)
                    ex = sdmxerr as SdmxException;
                else
                    ex = new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.SoapParseError, new Exception(sdmxerr.Message));

                throw ex;
            }
        }

        #endregion
    }
}
