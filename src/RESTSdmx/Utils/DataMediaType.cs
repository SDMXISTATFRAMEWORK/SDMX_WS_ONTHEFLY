using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RESTSdmx.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Mime;

    using Org.Sdmxsource.Sdmx.Api.Constants;

    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    public enum DataMediaEnumType
    {
        /// <summary>
        /// The generic data.
        /// </summary>
        GenericData,

        /// <summary>
        /// The structure specific data.
        /// </summary>
        StructureSpecificData,

        /// <summary>
        /// The application xml.
        /// </summary>
        ApplicationXml,

        /// <summary>
        /// The text xml mime type
        /// </summary>
        TextXml,

        /// <summary>
        /// The compact data.
        /// </summary>
        CompactData,

        /// <summary>
        /// The cross sectional data.
        /// </summary>
        CrossSectionalData,

        /// <summary>
        /// The edi data.
        /// </summary>
        EdiData,

        /// <summary>
        /// The csv data.
        /// </summary>
        CsvData,

        /// <summary>
        /// The rdf data.
        /// </summary>
        RdfData,

        /// <summary>
        /// The dspl data.
        /// </summary>
        DSPLData,

        /// <summary>
        /// The Json data.
        /// </summary>
        JsonData,
    }

    /// <summary>
    /// The data media type.
    /// </summary>
    public sealed class DataMediaType : BaseConstantType<DataMediaEnumType>
    {
        #region Static Fields

        /// <summary>
        /// The instances.
        /// </summary>
        private static readonly IDictionary<DataMediaEnumType, DataMediaType> Instances = new Dictionary<DataMediaEnumType, DataMediaType>
        {
            {
                DataMediaEnumType.GenericData, 
                new DataMediaType(
                DataMediaEnumType.GenericData, 
                SdmxMedia.GenericData, 
                FLYBaseDataFormatEnumType.Generic)
            }, 
            {
                DataMediaEnumType.StructureSpecificData, 
                new DataMediaType(
                DataMediaEnumType.StructureSpecificData, 
                SdmxMedia.StructureSpecificData, 
                FLYBaseDataFormatEnumType.Compact)
            }, 
            {
                DataMediaEnumType.ApplicationXml, 
                new DataMediaType(
                DataMediaEnumType.ApplicationXml, 
                SdmxMedia.ApplicationXml, 
                FLYBaseDataFormatEnumType.Generic)
            }, 
            {
                DataMediaEnumType.TextXml, 
                new DataMediaType(
                DataMediaEnumType.TextXml, 
                SdmxMedia.TextXml, 
                FLYBaseDataFormatEnumType.Generic)
            }, 
            {
                DataMediaEnumType.CompactData, 
                new DataMediaType(
                DataMediaEnumType.CompactData, 
                SdmxMedia.CompactData, 
                FLYBaseDataFormatEnumType.Compact)
            }, 
            {
                DataMediaEnumType.CrossSectionalData, 
                new DataMediaType(
                DataMediaEnumType.CrossSectionalData, 
                SdmxMedia.CrossSectionalData, 
                FLYBaseDataFormatEnumType.CrossSectional)
            }, 
            {
                DataMediaEnumType.EdiData, 
                new DataMediaType(
                DataMediaEnumType.EdiData, 
                SdmxMedia.EdiData, 
                FLYBaseDataFormatEnumType.Edi)
            }, 
            {
                DataMediaEnumType.RdfData, 
                new DataMediaType(
                DataMediaEnumType.RdfData, 
                SdmxMedia.RdfXml, 
                FLYBaseDataFormatEnumType.Rdf)
            }, 
            {
                DataMediaEnumType.DSPLData, 
                new DataMediaType(
                DataMediaEnumType.DSPLData, 
                SdmxMedia.DSPL, 
                FLYBaseDataFormatEnumType.Dspl)
            }, 
            {
                DataMediaEnumType.JsonData, 
                new DataMediaType(
                DataMediaEnumType.JsonData, 
                SdmxMedia.JsonData, 
                FLYBaseDataFormatEnumType.Json)
            }, 
            {
                DataMediaEnumType.CsvData, 
                new DataMediaType(
                DataMediaEnumType.CsvData, 
                SdmxMedia.CsvData, 
                FLYBaseDataFormatEnumType.Csv)
            }
        };

        #endregion

        #region Fields

        /// <summary>
        /// The _format.
        /// </summary>
        private readonly FLYBaseDataFormatEnumType _format;

        /// <summary>
        /// The _media type name.
        /// </summary>
        private readonly string _mediaTypeName;

        #endregion

        #region Constructors and Destructors
        /// <summary>
        /// Base Data FormatEnum 
        /// </summary>
        public enum FLYBaseDataFormatEnumType
        {
            /// <summary>
            /// Nothing Format
            /// </summary>
            Null = 0,
            /// <summary>
            /// Generic Format
            /// </summary>
            Generic = 1,
            /// <summary>
            /// Compact Format
            /// </summary>
            Compact = 2,
            /// <summary>
            /// Utility Format
            /// </summary>
            Utility = 3,
            /// <summary>
            /// Edi Format
            /// </summary>
            Edi = 4,
            /// <summary>
            /// CrossSectional Format
            /// </summary>
            CrossSectional = 5,
            /// <summary>
            /// MessageGroup Format
            /// </summary>
            MessageGroup = 6,
            /// <summary>
            /// Csv Format
            /// </summary>
            Csv = 7,
            /// <summary>
            /// Json Format
            /// </summary>
            Json = 8,
            /// <summary>
            /// Rdf Format
            /// </summary>
            Rdf = 9,
             /// <summary>
            /// Rdf Format
            /// </summary>
            Dspl = 10
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataMediaType"/> class.
        /// </summary>
        /// <param name="enumType">
        /// The enum type.
        /// </param>
        /// <param name="mediaTypeName">
        /// The media type name.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        private DataMediaType(DataMediaEnumType enumType, string mediaTypeName, FLYBaseDataFormatEnumType format)
            : base(enumType)
        {
            this._mediaTypeName = mediaTypeName;
            this._format = format;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the values.
        /// </summary>
        public static IEnumerable<DataMediaType> Values
        {
            get
            {
                return Instances.Values;
            }
        }

        /// <summary>
        /// Gets the format.
        /// </summary>
        public FLYBaseDataFormatEnumType Format
        {
            get
            {
                return this._format;
            }
        }

        /// <summary>
        /// Gets the media type.
        /// </summary>
        public ContentType MediaType
        {
            get
            {
                var contentType = new ContentType();
                contentType.MediaType = this._mediaTypeName;
                return contentType;
            }
        }

        /// <summary>
        /// Gets the media type name.
        /// </summary>
        public string MediaTypeName
        {
            get
            {
                return this._mediaTypeName;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get from enum.
        /// </summary>
        /// <param name="enumType">
        /// The enum type.
        /// </param>
        /// <returns>
        /// The <see cref="DataMediaType"/>.
        /// </returns>
        public static DataMediaType GetFromEnum(DataMediaEnumType enumType)
        {
            DataMediaType output;
            if (Instances.TryGetValue(enumType, out output))
            {
                return output;
            }

            return null;
        }

        /// <summary>
        /// The get type from name.
        /// </summary>
        /// <param name="mediaTypeName">
        /// The media type name.
        /// </param>
        /// <returns>
        /// The <see cref="DataMediaType"/>.
        /// </returns>
        public static DataMediaType GetTypeFromName(string mediaTypeName)
        {
            if (string.IsNullOrEmpty(mediaTypeName) || new ContentType(mediaTypeName).MediaType.EndsWith("/*", StringComparison.Ordinal))
            {
                mediaTypeName = GetFromEnum(DataMediaEnumType.GenericData).MediaTypeName;
            }

            return Values.FirstOrDefault(m => m.MediaTypeName.Equals(mediaTypeName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// The get media type version.
        /// </summary>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <returns>
        /// The <see cref="ContentType"/>.
        /// </returns>
        public ContentType GetMediaTypeVersion(string version)
        {
            string outVersion = version;
            if (string.IsNullOrWhiteSpace(version))
            {
                version = "2.1";
            }

            switch (EnumType)
            {
                case DataMediaEnumType.GenericData:
                    outVersion = string.IsNullOrWhiteSpace(version) ? "2.1" : version;
                    break;
                case DataMediaEnumType.StructureSpecificData:
                    outVersion = string.IsNullOrWhiteSpace(version) ? "2.1" : version;
                    if (!"2.1".Equals(outVersion))
                    {
                        return null;
                    }

                    break;
                case DataMediaEnumType.ApplicationXml:
                case DataMediaEnumType.TextXml:
                    return GetFromEnum(DataMediaEnumType.GenericData).GetMediaTypeVersion("2.1");
                case DataMediaEnumType.CompactData:
                case DataMediaEnumType.CrossSectionalData:
                    outVersion = string.IsNullOrWhiteSpace(version) ? "2.0" : version;
                    if (!"2.0".Equals(outVersion))
                    {
                        return null;
                    }

                    break;
                case DataMediaEnumType.EdiData:
                case DataMediaEnumType.RdfData:
                case DataMediaEnumType.DSPLData:
                case DataMediaEnumType.JsonData:
                case DataMediaEnumType.CsvData:
                    return this.MediaType;
            }

            var contentType = new ContentType(string.Format(CultureInfo.InvariantCulture, "{0};version={1}", this._mediaTypeName, outVersion));

            return contentType;
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return this._mediaTypeName;
        }

        #endregion
    }
}
