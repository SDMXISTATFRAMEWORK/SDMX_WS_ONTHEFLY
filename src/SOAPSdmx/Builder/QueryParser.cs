using FlyController.Model;
using FlyController.Model.Error;
using FlyEngine.Model;
using FlyController.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using SOAPSdmx.Model;
using SOAPSdmx.Streaming;
using OnTheFlyLog;

namespace SOAPSdmx.Builder
{
    /// <summary>
    /// Object base for parse data Message and create a SOAP response
    /// </summary>
    public abstract class QueryParser
    {

        private static object lockobject = new object();
        /// <summary>
        /// representing a QueryOperation
        /// </summary>
        public QueryOperation QueryType { get; set; }

        /// <summary>
        /// create a instance of QueryParser
        /// </summary>
        /// <param name="_QueryType">indicates from which the request was made entrypoint </param>
        /// <param name="Version">Sdmx Version</param>
        public QueryParser(QueryOperation.MessageTypeEnum _QueryType, QueryOperation.MessageVersionEnum Version)
        {
            QueryType = new QueryOperation(_QueryType, Version);
        }

        /// <summary>
        /// Read a Query Message, call a correct builder, create a SOAP message result
        /// </summary>
        /// <param name="query">query input</param>
        /// <returns>Soap Message response</returns>
        public Message Parse(Message query)
        {
            try
            {
                lock (lockobject)
                {

                    FlyLog.WriteLog(this, FlyLog.LogTypeEnum.Warning, @"
_______________________________________________________________________________
ARRIVED SOAP QUERY
MessageType {0}, MessageVersion {1}", QueryType.MessageType, QueryType.MessageVersion);

                    MessageBuffer msgbuf = query.CreateBufferedCopy(int.MaxValue);
                    XPathNavigator nav = msgbuf.CreateNavigator();

                    //load the old message into xmldocument
                    MemoryStream ms = new MemoryStream();
                    XmlWriter xw = XmlWriter.Create(ms);
                    nav.WriteSubtree(xw);
                    xw.Flush();
                    xw.Close();

                    ms.Position = 0;


                    XDocument xdoc = XDocument.Load(XmlReader.Create(ms));
                    XElement body = FindSoapBobyElement(xdoc);

                    string ActionResp = ChangeSoapActionElement(xdoc);

                    IFlyWriterBody WriterBody = ParseElementQuery(body);
                    SOAPStreaming StreamingCallBack = new SOAPStreaming();
                    return StreamingCallBack.CreateStream(WriterBody, ActionResp);
                }
            }

            catch (Exception sdmxerr)
            {
                SdmxException ex = null;
                if (sdmxerr is SdmxException)
                    ex = sdmxerr as SdmxException;
                else
                    ex = new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.SoapParseError, new Exception(sdmxerr.Message));

                return CreateMessageErrorResponse(ex, query.Version);
            }

        }



        /// <summary>
        ///  Abstract method for Parsing element into query and return a SdmxMessage (XElement return message or Error)
        /// </summary>
        /// <param name="SdmxQuery"></param>
        /// <returns>Object for Write response in streaming <see cref="IFlyWriterBody"/></returns>
        public abstract IFlyWriterBody ParseElementQuery(XElement SdmxQuery);

        /// <summary>
        /// Find Boby message in SOAP structure
        /// </summary>
        /// <param name="xdoc">SOAP structure</param>
        /// <returns>Body Element</returns>
        private XElement FindSoapBobyElement(XDocument xdoc)
        {
            try
            {
                XElement body = xdoc.Elements().FirstOrDefault().Elements().First(x => x.Name.LocalName == "Body").Elements().FirstOrDefault();//.Elements().FirstOrDefault();
                // if (body.Name.LocalName.Trim().ToUpper()=="QUERYSTRUCTURE")
                body = body.Elements().FirstOrDefault();
                return body;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.FindSoapBobyElement, ex);
            }
        }

        /// <summary>
        /// Change Action Element from Soap Message for build a Message response
        /// </summary>
        /// <param name="xdoc">SOAP structure</param>
        private string ChangeSoapActionElement(XDocument xdoc)
        {
            try
            {
                IEnumerable<XElement> action = (from el in xdoc.Descendants().Elements()
                                                where el.Name.LocalName == "Action"
                                                select el);
                if (action != null && action.Count() > 0)
                {
                    //string ActionResp = string.Format("{2}/{0}/{1}", GetConstNameSpace(), this.QueryType.ResponseType.ToString(), Constant.ServiceNamespace);
                    string ActionResp = string.Format("{1}", GetConstNameSpace(), this.QueryType.ResponseType.ToString(), Constant.ServiceNamespace);
                    action.FirstOrDefault().Value = ActionResp;
                    return ActionResp;
                }
                else throw new Exception("No Action response recognized");
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.ChangeSoapActionElement, ex);
            }
        }


        /// <summary>
        /// Create a Soap message response (with a SDMX Error)
        /// </summary>
        /// <param name="ex">SDMX Exception</param>
        /// <param name="messageVersion">SOAP Version</param>
        /// <returns>Soap message response</returns>
        private Message CreateMessageErrorResponse(SdmxException ex, MessageVersion messageVersion)
        {
            try
            {

                MessageFault mess = SdmxFaultError.CreateMessageFault(ex, QueryType.MessageType.ToString());
                Message me = Message.CreateMessage(messageVersion, mess, "Action");
                return me;
            }
            catch (SdmxException) { throw; }
            catch (Exception err)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateMessageResponseError, err);
            }

        }

        /// <summary>
        /// Reads variables constant namespace for the construction of the response message
        /// </summary>
        /// <returns></returns>
        private string GetConstNameSpace()
        {
            switch (this.QueryType.MessageVersion)
            {
                case QueryOperation.MessageVersionEnum.Version2_0:
                    return Constant.ContractNamespaceSdmx20;
                case QueryOperation.MessageVersionEnum.Version2_1:
                    return Constant.ContractNamespaceSdmx21;
                default:
                    return null;
            }
        }

    }



}
