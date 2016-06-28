using FlyController.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyController.Model
{
    /// <summary>
    /// Interface for all FlyController builder
    /// </summary>
    public interface IObjectBuilder
    {
       
        /// <summary>
        ///  Identificable Code
        /// </summary>
        string Code { get; set; }
        /// <summary>
        ///  Descriptions Names
        /// </summary>
        List<SdmxObjectNameDescription> Names { get; set; }
        /// <summary>
        ///  Sdmx Version
        /// </summary>
        SdmxSchemaEnumType VersionTypeResp { get; set; }

        /// <summary>
        ///  Parsing Object <see cref="ISdmxParsingObject"/>
        /// </summary>
        ISdmxParsingObject ParsingObject { get; set; }
        

    }
}
