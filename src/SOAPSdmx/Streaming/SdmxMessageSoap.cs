using FlyController;
using FlyController.Model;
using FlyController.Model.Error;
using System;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using System.Xml;
using SOAPSdmx.Model;
using FlyController.Streaming;
using FlyController.Model.Streaming;



namespace SOAPSdmx.Streaming
{


    /// <summary>
    /// The SDMX message
    /// </summary>
    public class SdmxMessageSoap : SdmxMessageBase
    {
        #region Constants and Fields

        /// <summary>
        /// The _XML qualified name
        /// </summary>
        private readonly XmlQualifiedName _xmlQualifiedName;

        /// <summary>
        /// A value indicating whether the message is a fault.
        /// </summary>
        private bool _isFaulted;

        /// <summary>
        /// Initializes a new instance of the <see cref="SdmxMessageSoap"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="exceptionHandler">The exception handler.</param>
        /// <param name="xmlQualifiedName">Name of the XML qualified.</param>
        /// <param name="Action">Action</param>
        public SdmxMessageSoap(IStreamController<IFlyWriter> controller, Func<Exception, FaultException> exceptionHandler, XmlQualifiedName xmlQualifiedName, string Action)
            : base(controller, exceptionHandler, OperationContext.Current.IncomingMessageVersion, Action)
        {
            if (xmlQualifiedName == null)
            {
                throw new ArgumentNullException("xmlQualifiedName");
            }

            this._xmlQualifiedName = xmlQualifiedName;
        }

        #endregion

        /// <summary>
        /// Gets a value that indicates whether this message generates any SOAP faults.
        /// </summary>
        /// <returns>
        /// true if this message generates any SOAP faults; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.ObjectDisposedException">The message has been disposed of.</exception>
        public override bool IsFault
        {
            get
            {
                return this._isFaulted;
            }
        }

        /// <summary>
        /// Called when the message body is written to an XML file.
        /// </summary>
        /// <param name="writer">A <see cref="T:System.Xml.XmlDictionaryWriter"/> that is used to write this message body to an XML file.</param>
        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {

            try
            {


                var prefix = writer.LookupPrefix(this._xmlQualifiedName.Namespace) ?? "web";

                //this.ActionQueue.Enqueue(() => writer.WriteStartElement(prefix, this._xmlQualifiedName.Name, this._xmlQualifiedName.Namespace));
                writer.WriteStartElement(prefix, this._xmlQualifiedName.Name, this._xmlQualifiedName.Namespace);
                base.OnWriteBodyContents(writer);
                writer.WriteEndElement();


            }
            catch (FaultException sdmxerr)
            {
               
                var messageFault = sdmxerr.CreateMessageFault();
                messageFault.WriteTo(writer, this.Version.Envelope);
                if (WebOperationContext.Current != null)
                {
                    WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                }

                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Response.StatusCode = 500;
                }
                this._isFaulted = true;

            }
            finally
            {
                writer.Flush();
            }
        }
    }
}
