using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace FlyController.Model.Error
{
    /// <summary>
    /// FlyMessageError Create Exception Detail Object
    /// </summary>
    public class FlyMessageError
    {
        /// <summary>
        /// Flag that representing a Enty Detail
        /// </summary>
        public bool IsEmptyDetail { get; set; }

        /// <summary>
        /// Create instance of FlyMessageError
        /// </summary>
        /// <param name="ex">Additional Exception, this can be Sdmx.Api.Exception (Caused from CommonAPI) or System Exception (Caused from application) </param>
        /// <param name="_isEmptyDetail">Flag that representing a Enty Detail</param>
        public FlyMessageError(Exception ex, bool _isEmptyDetail)
        {
            IsEmptyDetail = _isEmptyDetail;
            if (ex is Org.Sdmxsource.Sdmx.Api.Exception.SdmxException)
                TipoMessaggio = typeMessageEnum.SDMXError;
            else
                TipoMessaggio = typeMessageEnum.InternalError;
            this.ThisException = ex;
        }

        /// <summary>
        /// Internal Message Type 
        /// SDMXError: Sdmx.Api.Exception (Caused from CommonAPI) 
        /// InternalError: System Exception (Caused from application)
        /// </summary>
        public enum typeMessageEnum
        {
            /// <summary>
            /// SDMXError: Sdmx.Api.Exception (Caused from CommonAPI) 
            /// </summary>
            SDMXError,
            /// <summary>
            /// InternalError: System Exception (Caused from application)
            /// </summary>
            InternalError
        }

        /// <summary>
        /// Internal Message Type 
        /// </summary>
        public typeMessageEnum TipoMessaggio { get; set; }
        /// <summary>
        /// Additional Exception, this can be Sdmx.Api.Exception (Caused from CommonAPI) or System Exception (Caused from application)
        /// </summary>
        public Exception ThisException { get; set; }

        /// <summary>
        /// Create a XElement MessageDetail for SOAP FaultException (SdmxFaultError)
        /// </summary>
        /// <returns>XElement with exception description </returns>
        public XElement ToElement()
        {
            XElement el = new XElement("MessageDetail");
            if (this.TipoMessaggio == typeMessageEnum.InternalError)
            {
                el.Add(new XElement("ErrorType", TipoMessaggio.ToString()));
                el.Add(new XElement("Message", string.Join(": ", this.ThisException.Message.Split(new string[2] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries))));
            }
            else
            {
                Org.Sdmxsource.Sdmx.Api.Exception.SdmxException ex = ((Org.Sdmxsource.Sdmx.Api.Exception.SdmxException)this.ThisException);
                el.Add(new XElement("ErrorType", ex.ErrorType));
                //el.Add(new XElement("Code", ex.Code.Code));
                el.Add(new XElement("Message", string.Join(": ", ex.Message.Split(new string[2] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries))));
                if (ex != null && ex.InnerException != null)
                    el.Add(new XElement("InternalMessage", string.Join(": ", ((Org.Sdmxsource.Sdmx.Api.Exception.SdmxException)(ex.InnerException)).FullMessage.Split(new string[2] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries))));
            }
            //ex.MessageError.FullMessage.Substring(0,ex.MessageError.FullMessage.IndexOf("\n"))
            return el;
        }


        /// <summary>
        /// Create a XElement MessageDetail for SOAP MessageFault (In case of Exception during create FaultException of SdmxFaultError)
        /// </summary>
        /// <param name="Fromex">SdmxException</param>
        /// <returns>XElement with exception description</returns>
        public object ToDetail(SdmxException Fromex)
        {
            XElement el = new XElement("Error");
            el.SetAttributeValue("Type", TipoMessaggio.ToString());
            el.Add(new XElement("Message", Fromex.MessageText));

            if (this.TipoMessaggio == typeMessageEnum.InternalError)
            {
                if (!IsEmptyDetail)
                    el.Add(new XElement("MessageDetail", string.Join(": ", this.ThisException.Message.Split(new string[2] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries))));
            }
            else
            {
                Org.Sdmxsource.Sdmx.Api.Exception.SdmxException ex = ((Org.Sdmxsource.Sdmx.Api.Exception.SdmxException)this.ThisException);
                el.Add(new XElement("ErrorType", ex.ErrorType));
                if (ex.Code != null)
                    el.Add(new XElement("Code", ex.Code.Code));
                el.Add(new XElement("MessageDetail", string.Join(": ", ex.Message.Split(new string[2] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries))));
                if (ex.InnerException is Org.Sdmxsource.Sdmx.Api.Exception.SdmxException)
                    el.Add(new XElement("InternalMessage", string.Join(": ", ((Org.Sdmxsource.Sdmx.Api.Exception.SdmxException)(ex.InnerException)).FullMessage.Split(new string[2] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries))));
                else if (ex.InnerException != null)
                    el.Add(new XElement("InternalMessage", string.Join(": ", ex.InnerException.ToString().Split(new string[2] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries))));

            }
            el.Add(new XElement("Source", Fromex.ProcSource));

            //ex.MessageError.FullMessage.Substring(0,ex.MessageError.FullMessage.IndexOf("\n"))
            return el;
        }
    }
}
