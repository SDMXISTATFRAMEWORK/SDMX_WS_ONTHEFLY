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
using System.Text.RegularExpressions;

namespace RESTSdmx
{
    /// <summary>
    ///  The SDMX-ML Structural meta-data resource implementation 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(Namespace = Constant.ServiceNamespace, InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class StructureResource : IStructureResource
    {


        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="StructureResource"/> class.
        /// </summary>
        static StructureResource()
        {
            FlyConfiguration.InitConfig(AppDomain.CurrentDomain.RelativeSearchPath);
        }


        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get structure.
        /// </summary>
        /// <param name="structure">
        /// The structure.
        /// </param>
        /// <param name="agencyId">
        /// The agency id.
        /// </param>
        /// <param name="resourceId">
        /// The resource id.
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <returns>
        /// The <see cref="Message"/>.
        /// </returns>
        /// <exception cref="WebFaultException{T}">
        /// Bad Request.
        /// </exception>
        public Message GetStructure(string structure, string agencyId, string resourceId, string version)
        {
            WebOperationContext ctx = WebOperationContext.Current;

            if (!IsStructureValid(structure))
            {
                throw new WebFaultException<string>("Invalid structure: " + structure, HttpStatusCode.BadRequest);
            }

            try
            {
                return this.ProcessRequest(structure, agencyId, resourceId, version, ctx);
            }
            catch (SdmxException e)
            {
                throw RESTError.BuildException(e);
            }
        }

        /// <summary>
        /// The get structure all.
        /// </summary>
        /// <param name="structures">
        /// The structures.
        /// </param>
        /// <returns>
        /// The <see cref="Message"/>.
        /// </returns>
        public Message GetStructureAll(string structures)
        {
            return this.GetStructure(structures, "ALL", "ALL", "latest");
        }

        /// <summary>
        /// The get structure all ids latest.
        /// </summary>
        /// <param name="structure">
        /// The structure.
        /// </param>
        /// <param name="agencyIds">
        /// The agency ids.
        /// </param>
        /// <returns>
        /// The <see cref="Message"/>.
        /// </returns>
        public Message GetStructureAllIdsLatest(string structure, string agencyIds)
        {
            return this.GetStructure(structure, agencyIds, "ALL", "latest");
        }

        /// <summary>
        /// The get structure latest.
        /// </summary>
        /// <param name="structure">
        /// The structure.
        /// </param>
        /// <param name="agencyId">
        /// The agency id.
        /// </param>
        /// <param name="resourceId">
        /// The resource id.
        /// </param>
        /// <returns>
        /// The <see cref="Message"/>.
        /// </returns>
        public Message GetStructureLatest(string structure, string agencyId, string resourceId)
        {
            return this.GetStructure(structure, agencyId, resourceId, "latest");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Builds the rest query bean.
        /// </summary>
        /// <param name="structure">The structure.</param>
        /// <param name="agencyId">The agency id.</param>
        /// <param name="resourceId">The resource id.</param>
        /// <param name="version">The version.</param>
        /// <param name="queryParameters">The query parameters.</param>
        /// <returns>
        /// The <see cref="IRestStructureQuery" />.
        /// </returns>
        /// <exception cref="WebFaultException{String}">An exception is thrown</exception>
        private static IRestStructureQuery BuildRestQueryBean(string structure, string agencyId, string resourceId, string version, NameValueCollection queryParameters)
        {
            var queryString = new string[4];
            queryString[0] = structure;
            queryString[1] = agencyId;
            queryString[2] = resourceId;
            queryString[3] = version;

            var paramsDict = RestUtils.GetQueryStringAsDict(queryParameters);

            IRestStructureQuery query;

            try
            {
                query = new RESTStructureQueryCore(queryString, paramsDict);
            }
            catch (SdmxException e)
            {
                throw RESTError.BuildException(e);
            }
            catch (Exception e)
            {
                throw new WebFaultException<string>(e.Message, HttpStatusCode.BadRequest);
            }

            return query;
        }

        /// <summary>
        /// The is structure valid.
        /// </summary>
        /// <param name="structure">
        /// The structure.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool IsStructureValid(string structure)
        {
            Match match = Regex.Match(
                structure,
                @"^(datastructure|metadatastructure|categoryscheme|conceptscheme|codelist|hierarchicalcodelist|organisationscheme|agencyscheme|dataproviderscheme|dataconsumerscheme|organisationunitscheme|dataflow|metadataflow|reportingtaxonomy|provisionagreement|structureset|process|categorisation|contentconstraint|attachmentconstraint|structure)$",
                RegexOptions.IgnoreCase);

            return match.Success;
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="structure">The structure.</param>
        /// <param name="agencyId">The agency id.</param>
        /// <param name="resourceId">The resource id.</param>
        /// <param name="version">The version.</param>
        /// <param name="ctx">The current <see cref="WebOperationContext"/>.</param>
        /// <returns>
        /// The <see cref="Message" />.
        /// </returns>
        /// <exception cref="WebFaultException{XMLElement}">
        /// Cannot serve content type
        /// </exception>
        /// <exception cref="WebFaultException{String}">Cannot serve content type</exception>
        private Message ProcessRequest(string structure, string agencyId, string resourceId, string version, WebOperationContext ctx)
        {
            try
            {



                Match match = Regex.Match(resourceId, @"[A-Za-z0-9\-]+$", RegexOptions.IgnoreCase);
                if (!match.Success)
                {
                    throw new WebFaultException(HttpStatusCode.BadRequest);
                }

                var defaultMediaType = StructureMediaType.GetFromEnum(StructureMediaEnumType.Structure).MediaTypeName;
                var requestAccept = ctx.IncomingRequest.Accept;

                IList<ContentType> acceptHeaderElements = ctx.IncomingRequest.GetAcceptHeaderElements();
                Func<ContentType, bool> predicate = type => StructureMediaType.GetTypeFromName(type.MediaType) != null;
                var contentType = RestUtils.GetContentType(ctx, predicate, defaultMediaType);

                string requestedVersion = RestUtils.GetVersionFromAccept(acceptHeaderElements, contentType.MediaType);

                var selectedStructureMediaType = StructureMediaType.GetTypeFromName(contentType.MediaType);
                var selectedMediaTypeWithVersion = selectedStructureMediaType.GetMediaTypeVersion(requestedVersion);

                if (selectedMediaTypeWithVersion == null)
                {
                    throw new WebFaultException<string>("Cannot serve content type: " + requestAccept, HttpStatusCode.NotAcceptable);
                }

                FlyMediaEnum FlyMedia=FlyMediaEnum.Sdmx;
                if (selectedStructureMediaType.EnumType==StructureMediaEnumType.RdfXml)
	                FlyMedia=FlyMediaEnum.Rdf;
               

                SdmxSchema schemaVersion = RestUtils.GetVersionFromMediaType(selectedMediaTypeWithVersion);
                FlyLog.WriteLog(this, FlyLog.LogTypeEnum.Warning, @"
_______________________________________________________________________________
ARRIVED REST METADATA QUERY
structure {0}, agencyId {1}, resourceId {2}, version {3},
selectedMediaTypeWithVersion {4} ", structure, agencyId, resourceId, version, selectedMediaTypeWithVersion);

               
                //Controllo se ho aggiunto le referenze per le Codelist Constrainate
                RedirectForCodelistConstrain restCodelistCostrain = new RedirectForCodelistConstrain()
                {
                    _structure = structure,
                    _agencyId = agencyId,
                    _resourceId = resourceId,
                    _version = version,
                    _queryParameters = ctx.IncomingRequest.UriTemplateMatch.QueryParameters,
                };

                string ContrainParameter = null;
                System.Collections.Specialized.NameValueCollection QueryParameters = restCodelistCostrain.CheckExistReference(out ContrainParameter);

                
                IRestStructureQuery query = BuildRestQueryBean(structure, agencyId, resourceId, version, QueryParameters);
                RestParser RestParser = new Builder.RestParser();
                IStreamController<IFlyWriter> streamController = new StreamController<IFlyWriter>(RestParser.GenerateResponseFunctionMetadata(query, schemaVersion, ContrainParameter));

                var charSetEncoding = RestUtils.GetCharSetEncoding(contentType);

                var responseContentType = RestUtils.GetContentType(contentType, selectedMediaTypeWithVersion);
                selectedMediaTypeWithVersion.CharSet = charSetEncoding.WebName;
                return ctx.CreateStreamResponse(
                    stream => RestUtils.StreamXml(FlyMedia, schemaVersion, stream, streamController, charSetEncoding), responseContentType);
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
