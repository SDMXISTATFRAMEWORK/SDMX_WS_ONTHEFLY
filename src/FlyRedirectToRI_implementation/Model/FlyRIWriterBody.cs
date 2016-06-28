using FlyController.Model;
using FlyController.Model.Streaming;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace FlyRedirectToRI_implementation.Model
{
    /// <summary>
    /// Get a response from RIWebServices SOAP data Message and redirect into OnTheFly response
    /// </summary>
    public class FlyRIWriterBody : FlyWriterBody
    {
        private HttpWebRequest _request { get; set; }

        /// <summary>
        /// Initialize instance of <see cref="FlyRIWriterBody"/>
        /// </summary>
        /// <param name="request"></param>
        public FlyRIWriterBody(HttpWebRequest request)
        {
            this._request = request;
        }

        /// <summary>
        /// Write a data body content into FlyWriter
        /// </summary>
        /// <param name="writer">object of transport used for transmitting data in streaming <see cref="IFlyWriter"/></param>
        public override void WriterBody(IFlyWriter writer)
        {
            try
            {
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)_request.GetResponse();
                // Gets the stream associated with the response.
                using (Stream responseStream = myHttpWebResponse.GetResponseStream())
                {
                    using (XmlTextReader reader = new XmlTextReader(responseStream))
                    {
                        if (reader.Read())
                        {
                            while (reader.NodeType != XmlNodeType.Element)
                                reader.Read();

                            if (reader.LocalName.Trim().ToLower() == "envelope")
                            {
                                reader.Read();
                                if (reader.LocalName.Trim().ToLower() == "body")
                                {
                                    reader.Read();
                                    if (reader.Prefix == "web")
                                        reader.Read();
                                }
                            }

                            writer.__SdmxXml.WriteNode(reader, true);
                            writer.__SdmxXml.Flush();
                        }
                    }
                }
            }
            catch (WebException e)
            {
                if (e.Response != null)
                {
                    using (Stream responseStream = e.Response.GetResponseStream())
                    {
                        using (XmlTextReader reader = new XmlTextReader(responseStream))
                        {
                            if (reader.Read())
                            {
                                while (reader.NodeType != XmlNodeType.Element)
                                    reader.Read();

                                if (reader.LocalName.Trim().ToLower() == "envelope")
                                {
                                    reader.Read();
                                    if (reader.LocalName.Trim().ToLower() == "body")
                                    {
                                        reader.Read();
                                        if (reader.Prefix == "web")
                                            reader.Read();
                                    }
                                }
                                writer.__SdmxXml.WriteNode(reader, true);
                                writer.__SdmxXml.Flush();
                            }
                        }
                    }
                }
                else
                    throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
