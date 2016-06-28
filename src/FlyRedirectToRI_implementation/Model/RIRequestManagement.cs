using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Services.Description;
using System.Xml;
using System.Xml.Schema;

namespace FlyRedirectToRI_implementation.Model
{
    /// <summary>
    /// This class implements a NSI Web Service client
    /// </summary>
    public class RIRequestManagement
    {
        #region Constants and Fields

        /// <summary>
        /// SOAP 1.1 namespace
        /// </summary>
        private const string Soap11Ns = "http://schemas.xmlsoap.org/soap/envelope/";

        /// <summary>
        /// SOAP 1.2 namespace
        /// </summary>
        private const string Soap12Ns = "http://www.w3.org/2003/05/soap-envelope";

        /// <summary>
        /// This field holds a template for soap 1.1 request envelope
        /// </summary>
        private const string SoapRequest =
            "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:{0}=\"{1}\"><soap:Header/><soap:Body></soap:Body></soap:Envelope>";

        /// <summary>
        /// The SOAP Action HTTP Header
        /// </summary>
        private const string SoapActionHttpHeader = "SOAPAction";

        /// <summary>
        /// The HTTP HEADER Content type value
        /// </summary>
        private const string ContentTypeValue = "text/xml;charset=\"utf-8\"";

        /// <summary>
        /// This field holds the list of the Web Service available operations
        /// as defined in the web service WSDL
        /// </summary>
        private readonly List<string> _availableOperations;

        /// <summary>
        /// This field holds the a map between a web service operation name
        /// and the parameter wrapper element name. This is used for connecting to .NET WS.
        /// </summary>
        private readonly Dictionary<string, string> _operationParameterName;

        /// <summary>
        /// This field holds the web request
        /// </summary>
        private readonly HttpWebRequest _webRequest;

        /// <summary>
        /// The target namespace from the WSDL
        /// </summary>
        private string _namespace;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RIRequestManagement"/> class. 
        /// Initialize a new instance of the WebServiceClient class
        /// It sets the endpoint, WSDL URL, authentication, proxy and method information
        /// </summary>
        /// <param name="WsUri">WebService Uri</param>
        /// <param name="Operation">Operation (Entrypoint)</param>
        /// <param name="Headers">header settings</param>
        public RIRequestManagement(string WsUri, string Operation, string Headers)
        {

            this._availableOperations = new List<string>();
            this._operationParameterName = new Dictionary<string, string>();

            try
            {
                if (!string.IsNullOrEmpty(Operation))
                {
                    string WSDL = "";
                    if (WsUri.EndsWith("\\") || WsUri.EndsWith("/"))
                        WSDL = WsUri.Substring(0, WsUri.Length - 1) + "?wsdl";
                    else
                        WSDL = WsUri + "?wsdl";

                    this.ParseWSDL(WSDL);
                }
            }
            catch (WebException ex)
            {
                var errorMessage = this.HandleWebException(ex);
                throw new Exception(errorMessage);
            }
            catch (InvalidOperationException ex)
            {
                throw ex;
            }
            catch (UriFormatException ex)
            {
                throw ex;
            }

            try
            {
                this._webRequest = this.CreateWebRequest(WsUri, Operation, Headers);
            }
            catch (WebException ex)
            {
                var errorMessage = this.HandleWebException(ex);
                throw new WebException("Error accessing Web Service. " + errorMessage, ex);
            }
            catch (UriFormatException ex)
            {
                throw new InvalidOperationException("Error parsing Endpoint URL." + ex.Message, ex);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the WebRequest used by this object
        /// </summary>
        public HttpWebRequest WsWebRequest
        {
            get
            {
                return this._webRequest;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Invoke the web method with the given Soap Envelope or SDMX-ML Query
        /// </summary>
        /// <param name="soapRequest">
        /// The soap Request.
        /// </param>
        /// <param name="Operation">Operation (Entrypoint)</param>
        /// <returns>
        /// The Web Service response
        /// </returns>
        public HttpWebRequest InvokeSOAPMethod(XmlDocument soapRequest, string Operation)
        {
            this.WsWebRequest.Timeout = int.MaxValue;
            this.WsWebRequest.ReadWriteTimeout = int.MaxValue;
            if (soapRequest == null)
            {
                throw new ArgumentNullException("soapRequest");
            }

            //XmlDocument doc;
            if (!IsSoapMessage(soapRequest))
            {
                soapRequest = this.CreateSoapRequestEnvelope(soapRequest, Operation);
            }



            using (Stream stream = this.WsWebRequest.GetRequestStream())
            {
                soapRequest.Save(stream);
            }

            return this.WsWebRequest;

        }

        /// <summary>
        /// Invoke the web method in REST (get request)
        /// </summary>
        /// <returns>
        /// The Web Service response
        /// </returns>
        public HttpWebRequest InvokeRESTMethod()
        {
            this.WsWebRequest.Timeout = int.MaxValue;
            this.WsWebRequest.ReadWriteTimeout = int.MaxValue;
            return this.WsWebRequest;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Check if the given XmlDocument is a Soap Envelope
        /// </summary>
        /// <param name="doc">
        /// The XML Document to check
        /// </param>
        /// <returns>
        /// True if the Xml Document is a SOAP Envelope, false otherwise
        /// </returns>
        private static bool IsSoapMessage(XmlDocument doc)
        {
            bool ret = false;

            XmlElement root = doc.DocumentElement;
            if (root != null)
            {
                ret = root.LocalName.Equals("Envelope")
                      && (root.NamespaceURI.Equals(Soap11Ns) || root.NamespaceURI.Equals(Soap12Ns));
            }

            return ret;
        }

        /// <summary>
        /// Create a web request to the endpoint values
        /// </summary>
        /// <param name="WSUri">WebService Uri</param>
        /// <returns>
        /// The HttpWebRequest object for the configured endpoint
        /// </returns>
        private WebRequest CreateRequestWsdl(string WSUri)
        {
            WebRequest webRequest = WebRequest.Create(WSUri);
            webRequest.Method = "GET";
            return webRequest;
        }

        /// <summary>
        /// Encapsulate the give SDMX-ML Query in a SOAP Envelope request
        /// </summary>
        /// <param name="sdmxQuery">
        /// The SDMX-ML Query as a XML Document
        /// </param>
        /// <param name="Operation">Operation (Entrypoint)</param>
        /// <returns>
        /// The Soap Envelope containing the SDMX-ML Query
        /// </returns>

        private XmlDocument CreateSoapRequestEnvelope(XmlDocument sdmxQuery, string Operation)
        {
            if (sdmxQuery == null)
            {
                throw new ArgumentNullException("sdmxQuery");
            }

            var sb = new StringBuilder();
            sb.AppendFormat(SoapRequest, "web", this._namespace);
            var doc = new XmlDocument();
            doc.LoadXml(sb.ToString());
            XmlNodeList nodes = doc.GetElementsByTagName("Body", Soap11Ns);
            XmlElement operation = doc.CreateElement("web", Operation, this._namespace);
            string parameterName;
            XmlElement queryParent = operation;
            if (this._operationParameterName.TryGetValue(Operation, out parameterName)
                && !string.IsNullOrEmpty(parameterName))
            {
                queryParent = doc.CreateElement("web", parameterName, this._namespace);
                operation.AppendChild(queryParent);
            }

            if (sdmxQuery.DocumentElement != null)
            {
                XmlNode sdmxQueryNode = doc.ImportNode(sdmxQuery.DocumentElement, true);
                queryParent.AppendChild(sdmxQueryNode);
            }

            nodes[0].AppendChild(operation);
            return doc;
        }


        private HttpWebRequest CreateWebRequest(String EndPoint, string Operation, string Headers)
        {
            HttpWebRequest webRequest = null;
            if (!string.IsNullOrEmpty(Operation) && string.IsNullOrEmpty(Headers))
            {//SOAP
                webRequest = (HttpWebRequest)WebRequest.Create(EndPoint);
                var soapAction = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}{1}{2}",
                    this._namespace,
                    this._namespace.EndsWith("/", StringComparison.Ordinal) ? string.Empty : "/",
                    Operation);
                webRequest.Headers.Add(SoapActionHttpHeader, soapAction);
                webRequest.ContentType = ContentTypeValue;
                webRequest.Method = "POST";
               
            }
            else
            {//REST
                Uri url = new Uri(EndPoint);
                if (!url.LocalPath.EndsWith("/"))
                    url = new Uri(EndPoint.Replace(url.LocalPath, url.LocalPath + "/"));
                webRequest = (HttpWebRequest)WebRequest.Create(url);

                webRequest.Accept = Headers;
                webRequest.ContentType = ContentTypeValue;
                webRequest.Method = "GET";
            
            }
            return webRequest;
        }

        /// <summary>
        /// Get a more user friendly error message from <paramref name="webException"/> 
        /// </summary>
        /// <param name="webException">
        /// The <see cref="WebException"/>
        /// </param>
        /// <returns>
        /// The a more user friendly error message from <paramref name="webException"/>
        /// </returns>
        private string HandleWebException(WebException webException)
        {
            var errorResponse = webException.Response as HttpWebResponse;
            string errorMessage = webException.Message;
            if (errorResponse != null)
            {
                switch (errorResponse.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        errorMessage = "error 401: enable Authentication";
                        break;
                    default:
                        errorMessage = string.Format(
                            CultureInfo.CurrentCulture,
                            "error_http check connection: {0}",
                            webException.Response.ResponseUri.ToString(),
                            (int)errorResponse.StatusCode,
                            errorResponse.StatusDescription,
                            webException.Message);
                        break;
                }
            }

            return errorMessage;
        }

        private void ParseWSDL(string WsUri)
        {
            WebRequest request = this.CreateRequestWsdl(WsUri);
            using (WebResponse response = request.GetResponse())
            {
                Stream stream = response.GetResponseStream();
                if (stream != null)
                {
                    using (var streamReader = new StreamReader(stream))
                    {
                        ServiceDescription wsdl = ServiceDescription.Read(streamReader);
                        this._namespace = wsdl.TargetNamespace;
                        foreach (PortType pt in wsdl.PortTypes)
                        {
                            foreach (Operation op in pt.Operations)
                            {
                                this._availableOperations.Add(op.Name);
                            }
                        }

                        // Tested with .net and java NSI WS WSDL
                        foreach (XmlSchema schema in wsdl.Types.Schemas)
                        {
                            foreach (XmlSchemaElement element in schema.Elements.Values)
                            {
                                if (element.RefName.IsEmpty)
                                {
                                    var complexType = element.SchemaType as XmlSchemaComplexType;
                                    if (complexType != null)
                                    {
                                        var seq = complexType.Particle as XmlSchemaSequence;
                                        if (seq != null && seq.Items.Count == 1)
                                        {
                                            var body = seq.Items[0] as XmlSchemaElement;
                                            if (body != null && body.RefName.IsEmpty)
                                            {
                                                this._operationParameterName.Add(element.Name, body.Name);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
