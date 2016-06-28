using FlyController.Model;
using FlyController.Model.Error;
using Org.Sdmxsource.Sdmx.Api.Constants;
using System;
namespace FlyEngine.Manager
{
    /// <summary>
    /// Interface for Get parsed result of request Data and Metadata
    /// </summary>
    interface IDataEngineObject
    {
        /// <summary>
        /// Get parsed result of request
        /// </summary>
        /// <returns>Object for Write response in streaming <see cref="IFlyWriterBody"/></returns>
        IFlyWriterBody GetResult();
        /// <summary>
        /// Flag representing Error in application
        /// </summary>
        bool HaveError { get; set; }
        /// <summary>
        /// Error Message in case of error
        /// </summary>
        SdmxException ErrorMessage { get; set; }
        /// <summary>
        /// Sdmx Version
        /// </summary>
        SdmxSchemaEnumType VersionTypeResp { get; set; }
        /// <summary>
        /// Dispose all Object used for retreial data
        /// </summary>
        void DestroyObjects();
    }
}
