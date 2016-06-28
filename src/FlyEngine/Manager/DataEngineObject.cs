using FlyController.Model;
using FlyController.Model.Error;
using Org.Sdmxsource.Sdmx.Api.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyEngine.Manager
{
    /// <summary>
    /// Abstract Class for get parsed result of request Data and Metadata
    /// </summary>
    public abstract class DataEngineObject : IDataEngineObject
    {
        /// <summary>
        /// Flag representing Error in application
        /// </summary>
        public bool HaveError { get; set; }
        /// <summary>
        /// Error Message in case of error
        /// </summary>
        public SdmxException ErrorMessage { get; set; }
        /// <summary>
        /// Sdmx Version
        /// </summary>
        public SdmxSchemaEnumType VersionTypeResp { get; set; }


        /// <summary>
        /// Calling at the end of GetResult Implemented function. this call a DestroyObject function
        /// </summary>
        /// <returns>Object for Write response in streaming <see cref="IFlyWriterBody"/></returns>
        public abstract IFlyWriterBody GetResult();
       

        /// <summary>
        /// Dispose all Object used for retreial data
        /// </summary>
        public abstract void DestroyObjects();
    }
}
