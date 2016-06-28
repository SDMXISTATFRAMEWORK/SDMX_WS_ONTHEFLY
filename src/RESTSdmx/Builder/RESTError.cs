using FlyController.Model.Error;
using OnTheFlyLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using System.Xml.Linq;

namespace RESTSdmx.Builder
{
    /// <summary>
    /// An Object to build the Error output of Rest Call
    /// </summary>
    public class RESTError
    {
        /// <summary>
        /// Builds an object of type  <see cref="WebFaultException{String}"/>
        /// </summary>
        /// <param name="buildFrom">
        /// An Object to build the output object from
        /// </param>
        /// <returns>
        /// Object of type <see cref="WebFaultException{String}"/>
        /// </returns>
        public static WebFaultException<XElement> BuildException(SdmxException buildFrom)
        {
            FlyLog.WriteLog(typeof(RESTError), FlyLog.LogTypeEnum.Error, "Build Rest Error: {0}", buildFrom.MessageText);

            XElement el = new XElement("OnTheFly_RestError");
            el.Add(new XElement("Code", buildFrom.SDMXFaultCode.ToString()));
            el.Add(new XElement("Text", buildFrom.SDMXFaultMessage));
            XElement detail = new XElement("Detail");
            detail.Add(new XElement("Message", buildFrom.MessageText));
            detail.Add(new XElement("MessageDetails", buildFrom.MessageError.ToElement()));
            detail.Add(new XElement("Source", buildFrom.ProcSource));
            el.Add(detail);

            return new WebFaultException<XElement>(el,buildFrom.GetRESTStatusCode() );
        }

        
    }
}
