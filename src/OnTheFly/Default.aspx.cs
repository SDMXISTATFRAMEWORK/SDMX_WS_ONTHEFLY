using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OnTheFly
{
    /// <summary>
    /// Default Page show a link of entryPoint of all Application
    /// </summary>
    public partial class Default : System.Web.UI.Page
    {
        /// <summary>
        /// Url Path
        /// </summary>
        public string UrlPath { get; set; }
        /// <summary>
        /// EndPoint for SOAP Sdmx v 2.0
        /// </summary>
        public string EndPointSdmx20 { get; set; }
        /// <summary>
        /// EndPoint for SOAP Sdmx v 2.0
        /// </summary>
        public string EndPointSdmx21 { get; set; }

        /// <summary>
        /// EndPoint for Rest Metadata Help
        /// </summary>
        public string EndPointRestMetadata { get; set; }
        /// <summary>
        /// EndPoint for Rest Metadata
        /// </summary>
        public string EndPointRestMetadataPath { get; set; }
        /// <summary>
        /// EndPoint for Rest Data Help
        /// </summary>
        public string EndPointRestData { get; set; }
        /// <summary>
        /// EndPoint for Rest Data
        /// </summary>
        public string EndPointRestDataPath { get; set; }

        /// <summary>
        /// Page_Load of Page populate a property of page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            UrlPath = HttpContext.Current.Request.Url.AbsoluteUri;
            if (UrlPath.Trim().ToLower().EndsWith("default.aspx"))
                UrlPath = UrlPath.Substring(0, UrlPath.Length - 12);
            if (!UrlPath.EndsWith("/"))
                UrlPath += "/";
            EndPointSdmx20 = string.Format("{0}SoapSdmx20", UrlPath);
            EndPointSdmx21 = string.Format("{0}SoapSdmx21", UrlPath);

            EndPointRestMetadata = string.Format("{0}rest/help", UrlPath);
            EndPointRestMetadataPath = string.Format("{0}rest/", UrlPath);
            EndPointRestData = string.Format("{0}rest/data/help", UrlPath);
            EndPointRestDataPath = string.Format("{0}rest/data/", UrlPath);
        }
    }
}