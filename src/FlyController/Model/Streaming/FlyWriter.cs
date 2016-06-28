using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FlyController.Model.Streaming;
using System.Xml;
using System.IO;


namespace FlyController.Model
{
    /// <summary>
    /// Contains the object of transport used for transmitting data in streaming
    /// </summary>
    public class FlyWriter : IFlyWriter
    {
        /// <summary>
        /// Create a new instance of FlyWriter with XmlWriter transport object
        /// </summary>
        /// <param name="mediaType">Specific a mediatype response (sdmx, rdf, dspl,  json)</param>
        /// <param name="writer"></param>
        public FlyWriter(FlyMediaEnum mediaType, XmlWriter writer)
        {
            this.__SdmxXml = writer;
            this.FlyMediaType = mediaType;
        }

        /// <summary>
        /// Create a new instance of FlyWriter with XmlWriter transport object
        /// </summary>
        /// <param name="mediaType">Specific a mediatype response (sdmx, rdf, dspl,  json)</param>
        /// <param name="writer"></param>
        public FlyWriter(FlyMediaEnum mediaType, StreamWriter writer)
        {
            this.__DsplJSONTextWriter = writer;
            this.FlyMediaType = mediaType;
        }

        /// <summary>
        /// Specific a mediatype response (sdmx, rdf, dspl, json)
        /// </summary>
        public FlyMediaEnum FlyMediaType { get; set; }

        /// <summary>
        /// XmlWriter transport object
        /// </summary>
        public XmlWriter __SdmxXml { get; set; }

        /// <summary>
        /// DsplJsonTextWriter transport object
        /// </summary>
        public System.IO.StreamWriter __DsplJSONTextWriter { get; set; }
        
       
       

    }

    
}