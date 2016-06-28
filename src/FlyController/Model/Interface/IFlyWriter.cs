using System;
namespace FlyController.Model.Streaming
{
    /// <summary>
    /// Contains the object of transport used for transmitting data in streaming
    /// </summary>
    public interface IFlyWriter
    {
        /// <summary>
        /// XmlWriter transport object
        /// </summary>
        System.Xml.XmlWriter __SdmxXml { get; set; }

        /// <summary>
        /// DsplJsonTextWriter transport object
        /// </summary>
        System.IO.StreamWriter __DsplJSONTextWriter { get; set; }

        /// <summary>
        /// Specific a mediatype response (sdmx, rdf, Dspl, json)
        /// </summary>
        FlyController.Model.FlyMediaEnum FlyMediaType { get; set; }

    }
}
