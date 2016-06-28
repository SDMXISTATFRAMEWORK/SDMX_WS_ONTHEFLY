using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RESTSdmx.Utils
{
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
        /// The RDF data/Structure.
        /// </summary>
        public const string RdfXml = "application/rdf+xml";

        /// <summary>
        /// The DSPL data/Structure.
        /// </summary>
        public const string DSPL = "application/DSPL";

        /// <summary>
        /// The Json data
        /// </summary>
        public const string JsonData = "application/json";

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
