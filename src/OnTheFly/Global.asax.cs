using OnTheFly.Model;
using OnTheFlyLog;
using RESTSdmx;
using SOAPSdmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace OnTheFly
{
    /// <summary>
    /// Application Global inizialize the WebServices
    /// </summary>
    public class Global : System.Web.HttpApplication
    {

        /// <summary>
        /// The application_ begin request.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            string path = Request.PhysicalPath;
            var rawUrl = Request.RawUrl;
            var url = Request.Url.AbsolutePath;

            // rewrite URL for WCF WSDL requests.
            if (rawUrl.EndsWith("?wsdl", StringComparison.OrdinalIgnoreCase))
            {
                string originalPath = HttpContext.Current.Request.Path.ToLowerInvariant();
                var index = url.LastIndexOf('/');
                if (index > -1)
                {
                    var lastPart = url.Substring(index + 1);
                    var typeName = WsdlRegistry.Instance.GetWsdlInfo(lastPart);

                    if (typeName != null)
                    {
                        var wsdlUri = originalPath.Replace(lastPart.ToLowerInvariant(), "wsdl/" + typeName.Name);
                        Context.RewritePath(wsdlUri, null, null);
                    }
                }
            }
        }
        /// <summary>
        /// inizialize the WebServices on start of Application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Start(object sender, EventArgs e)
        {
            try
            {
                FlyLog.WriteLog(this, FlyLog.LogTypeEnum.All, @"
__________________________________________________________________
                Start On The Fly Application 
__________________________________________________________________");
                var wsdlRegistry = WsdlRegistry.Instance;
                Org.Sdmxsource.Util.Url.UriUtils.FixSystemUriDotBug();

                wsdlRegistry.Add(
                    new WsdlInfo { Name = "SoapSdmx20", OriginalPath = "OTFSdmx20.wsdl" },
                    new WsdlInfo { Name = "SoapSdmx21", OriginalPath = "OTFSdmx21.wsdl" });

                RouteTable.Routes.Add(new ServiceRoute("wsdl", new SdmxRestServiceHostFactory(typeof(IStaticWsdlService)), typeof(StaticWsdlService)));


                RouteTable.Routes.Add(new ServiceRoute("SoapSdmx20", new SoapServiceHostFactory(typeof(IOnTheFly_SOAP_SDMX_v20), "OTFSdmx20.wsdl"), typeof(OnTheFly_SOAP_SDMX_v20)));
                RouteTable.Routes.Add(new ServiceRoute("SoapSdmx21", new SoapServiceHostFactory(typeof(IOnTheFly_SOAP_SDMX_v21), "OTFSdmx21.wsdl"), typeof(OnTheFly_SOAP_SDMX_v21)));
                RouteTable.Routes.Add(new ServiceRoute("rest/data", new SdmxRestServiceHostFactory(typeof(IDataResource)), typeof(DataResource)));
                RouteTable.Routes.Add(new ServiceRoute("rest", new SdmxRestServiceHostFactory(typeof(IStructureResource)), typeof(StructureResource)));

                FlyLog.WriteLog(this, FlyLog.LogTypeEnum.All, "On The Fly Application Routing complete. The application is ready for use");
            }
            catch (Exception ex)
            {
                FlyLog.WriteLog(this, FlyLog.LogTypeEnum.Error, "Error On The Fly Application Routing a Web Services: {0}", ex.Message);
            }

        }

       
    }
}