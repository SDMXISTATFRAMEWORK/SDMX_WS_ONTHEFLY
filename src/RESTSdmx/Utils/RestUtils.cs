using FlyController.Model;
using FlyController.Model.Error;
using FlyController.Model.Streaming;
using FlyController.Streaming;
using Org.Sdmxsource.Sdmx.Api.Constants;
using RESTSdmx.Builder;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;

namespace RESTSdmx.Utils
{
    /// <summary>
    /// REST related utils
    /// </summary>
    public class RestUtils
    {
       
        /// <summary>
        /// Initializes static members of the <see cref="RestUtils"/> class.
        /// </summary>
        static RestUtils()
        {
        }

        /// <summary>
        /// Gets the char set encoding.
        /// </summary>
        /// <param name="acceptValue">The accept value.</param>
        /// <returns>The response encoding</returns>
        public static Encoding GetCharSetEncoding(ContentType acceptValue)
        {
            if (!string.IsNullOrWhiteSpace(acceptValue.CharSet))
            {
                try
                {
                    var encoding = Encoding.GetEncoding(acceptValue.CharSet);
                    if (encoding.Equals(Encoding.UTF8))
                    {
                        return new UTF8Encoding(false);
                    }

                    return encoding;
                }
                catch (Exception)
                {
                }
            }

            return new UTF8Encoding(false);
        }

        /// <summary>
        /// Gets the type of the content.
        /// </summary>
        /// <param name="acceptValue">The accept value.</param>
        /// <param name="resolvedContentType">Type of the resolved content.</param>
        /// <returns>The response content type</returns>
        public static string GetContentType(ContentType acceptValue, ContentType resolvedContentType)
        {
            switch (acceptValue.MediaType)
            {
                case SdmxMedia.ApplicationXml:
                case SdmxMedia.TextXml:
                    return acceptValue.ToString();
                case "text/*":
                    return SdmxMedia.TextXml;
                default:
                    return resolvedContentType.ToString();
            }
        }

       
        /// <summary>
        /// Gets the type of the content.
        /// </summary>
        /// <param name="ctx">
        /// The current <see cref="WebOperationContext"/>.
        /// </param>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <param name="defaultMediaType">
        /// Default type of the media.
        /// </param>
        /// <exception cref="WebFaultException{String}">
        /// Cannot serve content type
        /// </exception>
        /// <returns>
        /// The <see cref="ContentType"/>.
        /// </returns>
        public static ContentType GetContentType(WebOperationContext ctx, Func<ContentType, bool> predicate, ContentType defaultMediaType)
        {
            IList<ContentType> acceptHeaderElements = ctx.IncomingRequest.GetAcceptHeaderElements();
            var contentType = acceptHeaderElements.FirstOrDefault(predicate);

            if (contentType == null)
            {
                if (acceptHeaderElements.Count == 0)
                {
                    contentType = defaultMediaType;
                }
                else
                {
                    string accept = ctx.IncomingRequest.Accept;
                    throw new WebFaultException<string>("Cannot serve content type: " + accept, HttpStatusCode.NotAcceptable);
                }
            }

            return contentType;
        }

        /// <summary>
        /// The get version from media type.
        /// </summary>
        /// <param name="mediaType">
        /// The media type.
        /// </param>
        /// <returns>
        /// The <see cref="SdmxSchema"/>.
        /// </returns>
        public static SdmxSchema GetVersionFromMediaType(ContentType mediaType)
        {
            if (mediaType.Parameters == null)
            {
                return SdmxSchema.GetFromEnum(SdmxSchemaEnumType.VersionTwoPointOne);
            }

            if (!mediaType.Parameters.ContainsKey("version"))
            {
                return SdmxSchema.GetFromEnum(SdmxSchemaEnumType.VersionTwoPointOne);
            }

            if (mediaType.Parameters["version"].Equals("2.1"))
            {
                return SdmxSchema.GetFromEnum(SdmxSchemaEnumType.VersionTwoPointOne);
            }

            if (mediaType.Parameters["version"].Equals("2.0"))
            {
                return SdmxSchema.GetFromEnum(SdmxSchemaEnumType.VersionTwo);
            }

            return null;
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
        public static string GetVersionFromAccept(IEnumerable<ContentType> acceptHeaderList, string mediatype)
        {
            string version = "2.1";

            var accept = acceptHeaderList.FirstOrDefault(h => h.MediaType.Contains(mediatype));
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
        /// The get query string as dict.
        /// </summary>
        /// <param name="queryParameters">
        /// The query parameters.
        /// </param>
        /// <returns>
        /// The parameter in Dictionary
        /// </returns>
        /// <exception cref="WebFaultException">
        /// </exception>
        public static IDictionary<string, string> GetQueryStringAsDict(NameValueCollection queryParameters)
        {
            IDictionary<string, string> paramsDict = new Dictionary<string, string>();
            var enumQ = queryParameters.GetEnumerator();
            while (enumQ.MoveNext())
            {
                var queryName = enumQ.Current.ToString();
                var queryValue = queryParameters[queryName];
                if (paramsDict.ContainsKey(queryName))
                {
                    throw new WebFaultException(HttpStatusCode.BadRequest);
                }

                paramsDict.Add(queryName, queryValue);
            }

            return paramsDict;
        }


        /// <summary>
        /// Streams the structural metadata.
        /// </summary>
        /// <param name="mediaType">Specific a mediatype response (sdmx, rdf, dspl, json)</param>
        /// <param name="schemaVersion">The schema version.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="streamController">The stream controller.</param>
        /// <param name="encoding">The response encoding.</param>
        public static void StreamXml(FlyMediaEnum mediaType, SdmxSchema schemaVersion, Stream stream, IStreamController<IFlyWriter> streamController, Encoding encoding)
        {
            try
            {
                if (mediaType == FlyMediaEnum.Sdmx || mediaType == FlyMediaEnum.Rdf)
                {
                    if (schemaVersion.IsXmlFormat())
                    {
                        using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true, Encoding = encoding }))
                        {
                            IFlyWriter fw = new FlyWriter(mediaType, writer);
                            streamController.StreamTo(fw, new Queue<Action>());
                            writer.Flush();
                        }
                    }
                }
                else
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        IFlyWriter fw = new FlyWriter(mediaType, writer);
                        streamController.StreamTo(fw, new Queue<Action>());
                        writer.Flush();
                    }
                }
                
            }
            catch (Exception e)
            {
                SdmxException ex = null;
                if (e is SdmxException)
                    ex = e as SdmxException;
                else
                    ex = new SdmxException(typeof(RESTSdmx.Utils.RestUtils), FlyExceptionObject.FlyExceptionTypeEnum.SoapParseError, new Exception(e.Message));

                throw RESTError.BuildException(ex);
            }
        }
    }
}
