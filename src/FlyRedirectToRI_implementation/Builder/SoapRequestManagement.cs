using FlyController;
using FlyController.Model;
using FlyRedirectToRI_implementation.Interfaces;
using FlyRedirectToRI_implementation.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace FlyRedirectToRI_implementation.Builder
{
    /// <summary>
    /// Build <see cref="IFlyWriterBody"/> response Message for SOAP request and contains method for parse response
    /// </summary>
    public class SoapRequestManagement : IRequestManagement
    {

        /// <summary>
        /// Element query whitout Envelop header
        /// </summary>
        public XElement XmlQuery { get; set; }
        /// <summary>
        /// EntryPoint Name: Type of request both SDMX 2.0 and Sdmx 2.1
        /// </summary>
        public String MessageType { get; set; }
        /// <summary>
        /// Sdmx Version
        /// </summary>
        public SdmxSchemaEnumType VersionTypeResp { get; set; }

        /// <summary>
        /// Build <see cref="IFlyWriterBody"/> response Message for SOAP request
        /// </summary>
        /// <returns>the <see cref="IFlyWriterBody"/></returns>
        public IFlyWriterBody CreateResponseMessage()
        {
            string EndPoint = null;
            if (VersionTypeResp == SdmxSchemaEnumType.VersionTwo)
                EndPoint = FlyConfiguration.RIWebServices.Soap20Uri;
            else
                EndPoint = FlyConfiguration.RIWebServices.Soap21Uri;

            XmlDocument doc = new XmlDocument();
            doc.Load(XmlQuery.CreateReader());

            RIRequestManagement request = new RIRequestManagement(EndPoint, MessageType, null);
            HttpWebRequest SoapReq = request.InvokeSOAPMethod(doc, MessageType);
            return new FlyRIWriterBody(SoapReq);
        }
    }
}
