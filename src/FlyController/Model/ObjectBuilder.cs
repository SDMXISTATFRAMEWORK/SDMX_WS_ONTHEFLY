using Org.Sdmxsource.Sdmx.Api.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyController.Model
{
    /// <summary>
    /// Base Class for all FlyController builder
    /// </summary>
    public abstract class ObjectBuilder : IObjectBuilder
    {
        /// <summary>
        /// Inizialize new instance of ObjectBuilder
        /// </summary>
        /// <param name="parsingObject">Parsing Object <see cref="ISdmxParsingObject"/></param>
        /// <param name="versionTypeResp">Sdmx Version</param>
        public ObjectBuilder(ISdmxParsingObject parsingObject, SdmxSchemaEnumType versionTypeResp)
        {
            this.ParsingObject = parsingObject;
            this.VersionTypeResp = versionTypeResp;
        }
        /// <summary>
        ///  Identificable Code
        /// </summary>
        public abstract string Code { get; set; }
       
        /// <summary>
        ///  Descriptions Names
        /// </summary>
        public abstract List<SdmxObjectNameDescription> Names { get; set; }
       
        /// <summary>
        ///  Sdmx Version
        /// </summary>
        public SdmxSchemaEnumType VersionTypeResp { get; set; }

        /// <summary>
        ///  Parsing Object <see cref="ISdmxParsingObject"/>
        /// </summary>
        public ISdmxParsingObject ParsingObject { get; set; }
    }
}
