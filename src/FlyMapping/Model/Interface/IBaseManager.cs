using FlyController.Model;
using FlyEngine.Model;
using FlyMapping.Build;
using Org.Sdmxsource.Sdmx.Api.Constants;
using System;
namespace FlyMapping.Model
{
    /// <summary>
    /// Interface for connetion to Database
    /// </summary>
    public interface IBaseManager
    {
        /// <summary>
        /// for retrival data from database
        /// </summary>
        IDBAccess DbAccess { get; set; }

        /// <summary>
        ///  Parsing Object <see cref="ISdmxParsingObject"/>
        /// </summary>
        ISdmxParsingObject parsingObject { get; set; }

        /// <summary>
        /// Sdmx Version
        /// </summary>
        SdmxSchemaEnumType versionTypeResp { get; set; }

        /// <summary>
        /// Referenced objects 
        /// </summary>
        IReferencesObject ReferencesObject { get; set; }

    }
    
}
