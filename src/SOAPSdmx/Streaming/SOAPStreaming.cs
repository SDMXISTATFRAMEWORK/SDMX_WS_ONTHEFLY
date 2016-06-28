using FlyController.Model;
using FlyController.Model.Error;
using FlyController.Utils;
using Org.Sdmxsource.Sdmx.Api.Util;
using Org.Sdmxsource.Util.Io;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using SOAPSdmx.Builder;
using SOAPSdmx.Model;
using FlyController.Streaming;
using FlyController.Model.Streaming;

namespace SOAPSdmx.Streaming
{
    /// <summary>
    /// Management response SOAP streaming
    /// </summary>
    public class SOAPStreaming
    {
        /// <summary>
        /// Create a message with empty body contains header and type of response.
        ///This fired a callback for retrieving data stream
        /// </summary>
        /// <param name="WriterBody">Contains all object and methods for writing a response</param>
        /// <param name="ActionResp">Action response message</param>
        /// <returns>message with empty body contains header and type of response.</returns>
        public Message CreateStream(IFlyWriterBody WriterBody, string ActionResp)
        {
            try
            {
                IStreamController<IFlyWriter> streamController = new StreamController<IFlyWriter>(GenerateResponseFunction(WriterBody));
                WebOperationContext ctx = WebOperationContext.Current;
                if (ctx == null)
                {
                    throw new WebFaultException(HttpStatusCode.InternalServerError);
                }

                Message message = new SdmxMessageSoap(
                    streamController,
                     exception => BuildException(exception, ActionResp),
                    new XmlQualifiedName(ActionResp, Constant.ServiceNamespace),
                    ActionResp);

                return message;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateMessageResponseError, ex);
            }
        }



        /// <summary>
        /// Action Fired when client read a body
        /// </summary>
        /// <param name="WriterBody">Contains all object and methods for writing a response</param>
        /// <returns>Action FlyWriter, Queue Action populated streaming trasport object FlyWriter</returns>
        public Action<IFlyWriter, Queue<Action>> GenerateResponseFunction(IFlyWriterBody WriterBody)
        {
            return (writer, actions) => GetDataCallBack(WriterBody, writer);
        }

        /// <summary>
        /// Call a Delegate for populate a streaming trasport object
        /// </summary>
        /// <param name="WriterBody">Contains all object and methods for writing a response</param>
        /// <param name="responseStream">OUT streaming trasport object</param>
        public void GetDataCallBack(IFlyWriterBody WriterBody, IFlyWriter responseStream)
        {
            WriterBody.WriterBody(responseStream);
        }


        /// <summary>
        /// Builds an object of type  <see cref="FaultException{String}"/>
        /// </summary>
        /// <param name="buildFrom">
        /// An Object to build the output object from
        /// </param>
        /// <param name="messageAction">
        /// A Message Action
        /// </param>
        /// <returns>
        /// Object of type <see cref="FaultException{String}"/>
        /// </returns>
        public FaultException<SdmxFaultError> BuildException(Exception buildFrom, string messageAction)
        {
            var faultException = buildFrom as FaultException<SdmxFaultError>;
            if (faultException != null)
            {
                return faultException;
            }

            SdmxException ex = null;
            if (buildFrom is SdmxException)
                ex = buildFrom as SdmxException;
            else
                ex = new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.SoapParseError, new Exception(buildFrom.Message));
            return SdmxFaultError.BuildException(ex);
        }
    }
}