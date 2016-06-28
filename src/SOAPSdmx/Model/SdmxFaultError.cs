using FlyController.Model;
using FlyController.Model.Error;
using FlyController.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using SOAPSdmx.Model;

namespace SOAPSdmx.Model
{
    /// <summary>
    /// Create a SOAP error Response
    /// </summary>
    [DataContract(Name = "Error")]
    public class SdmxFaultError
    {
        /// <summary>
        /// this Error Response don't have actor information it is used only for Exception in MessageFault creation exception 
        /// </summary>
        /// <param name="ex">The <see cref="SdmxException"/></param> 
        /// <returns>Soap FaultException whit SdmxFaultError details</returns>
        public static FaultException<SdmxFaultError> BuildException(SdmxException ex)
        {
            SdmxFaultError Details = new SdmxFaultError();
            Details.Message = ex.MessageText;
            Details.MessageDetails = ex.MessageError.ToElement();
            Details.Source = ex.ProcSource;

            FaultCode fc = new FaultCode(ex.SDMXFaultCode.ToString());
            FaultReason fr = new FaultReason(ex.SDMXFaultMessage);


            FaultException<SdmxFaultError> _faultException = new FaultException<SdmxFaultError>(Details, fr, fc);
            return _faultException;
        }

        /// <summary>
        /// Build a MessageFault response whit a Sdmx Standard Error 
        /// </summary>
        /// <param name="ex">The <see cref="SdmxException"/></param>
        /// <param name="Actor">EntryPonit name</param>
        /// <returns>Sdmx Standard Error in MessageFault</returns>
        public static MessageFault CreateMessageFault(SdmxException ex, string Actor)
        {
            FaultCode fc = new FaultCode(ex.SDMXFaultCode.ToString());
            FaultReason fr = new FaultReason(ex.SDMXFaultMessage);

            XmlObjectSerializer ndcs = new DataContractSerializer(typeof(XElement));
            MessageFault mf = MessageFault.CreateFault(fc, fr, ex.MessageError.ToDetail(ex), ndcs, Actor);
            return mf;
        }

        /// <summary>
        /// Source of Error
        /// </summary>
        [DataMember]
        public string Source { get; set; }
        /// <summary>
        /// Error Message
        /// </summary>
        [DataMember]
        public string Message { get; set; }
        /// <summary>
        /// Element with Error Detail
        /// </summary>
        [DataMember]
        public XElement MessageDetails { get; set; }
    }


}