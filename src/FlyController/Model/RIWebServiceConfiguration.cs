using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FlyController.Model
{
    /// <summary>
    /// Part of the configuration on the redirect the query to RI web services 
    /// </summary>
    public class RIWebServiceConfiguration
    {
        /// <summary>
        /// SOAP WebService Uri for Sdmx v2.0
        /// </summary>
        public string Soap20Uri { get; set; }
        /// <summary>
        /// SOAP WebService Uri for Sdmx v2.1
        /// </summary>
        public string Soap21Uri { get; set; }
        /// <summary>
        /// SOAP WebService Uri for Sdmx Rest
        /// </summary>
        public string RestUri { get; set; }
        /// <summary>
        /// Create the object from the node of configuration file
        /// </summary>
        /// <param name="RIWebServiceNode">node of configuration file</param>
        public void Configure(XmlNode RIWebServiceNode)
        {
            foreach (XmlNode item in RIWebServiceNode.ChildNodes)
            {
                if (item.Name != "WebServicesUrl" || item.Attributes == null || item.Attributes["EndPoint"] == null || item.Attributes["EndPointType"] == null)
                    continue;
                switch (item.Attributes["EndPointType"].Value.Trim().ToUpper())
                {
                    case "V20":
                        Soap20Uri = item.Attributes["EndPoint"].Value;
                        break;
                    case "V21":
                        Soap21Uri = item.Attributes["EndPoint"].Value;
                        break;
                    case "REST":
                        RestUri = item.Attributes["EndPoint"].Value;
                        break;
                }
            }
        }
    }
}
