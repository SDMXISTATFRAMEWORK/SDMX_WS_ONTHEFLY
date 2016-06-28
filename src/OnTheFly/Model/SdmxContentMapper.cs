using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;

namespace OnTheFly.Model
{
    /// <summary>
    /// returns the message format used for a specified content type.
    /// </summary>
    public class SdmxContentMapper : WebContentTypeMapper
    {
        /// <summary>
        /// When overridden in a derived class, returns the message format used for a specified content type.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.ServiceModel.Channels.WebContentFormat"/> that specifies the format to which the message content type is mapped. 
        /// </returns>
        /// <param name="contentType">The content type that indicates the MIME type of data to be interpreted.</param>
        public override WebContentFormat GetMessageFormatForContentType(string contentType)
        {
            if (contentType.Contains("application/vnd.sdmx") || contentType.Contains("text/*") || contentType.Contains("application/*"))
            {
                return WebContentFormat.Raw;
            }
            if (contentType.Contains(SdmxMedia.ApplicationXml) || contentType.Contains(SdmxMedia.TextXml))
            {
                return WebContentFormat.Xml;
            }

            return WebContentFormat.Default;
        }
    }
    /// <summary>
    ///     MIME types
    /// </summary>
    public static class SdmxMedia
    {
        #region Static Fields

        /// <summary>
        /// The application xml.
        /// </summary>
        public const string ApplicationXml = "application/xml";

        /// <summary>
        /// The Text xml.
        /// </summary>
        public const string TextXml = "text/xml";

        /// <summary>
        /// The compact data.
        /// </summary>
        public const string CompactData = "application/vnd.sdmx.compactdata+xml";

        /// <summary>
        /// The cross sectional data.
        /// </summary>
        public const string CrossSectionalData = "application/vnd.sdmx.crosssectionaldata+xml";

        /// <summary>
        /// The CSV data.
        /// </summary>
        public const string CsvData = "text/csv";

        /// <summary>
        /// The EDI data.
        /// </summary>
        public const string EdiData = "application/vnd.sdmx.edidata";

        /// <summary>
        /// The EDI structure.
        /// </summary>
        public const string EdiStructure = "application/vnd.sdmx.edistructure";

        /// <summary>
        /// The generic data.
        /// </summary>
        public const string GenericData = "application/vnd.sdmx.genericdata+xml";

        /// <summary>
        /// The structure.
        /// </summary>
        public const string Structure = "application/vnd.sdmx.structure+xml";

        /// <summary>
        /// The structure specific data.
        /// </summary>
        public const string StructureSpecificData = "application/vnd.sdmx.structurespecificdata+xml";

        #endregion
    }
}
