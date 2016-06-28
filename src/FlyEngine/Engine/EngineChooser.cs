using FlyController.Model;
using FlyEngine.Engine.Metadata;
using FlyEngine.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyEngine.Engine
{
    /// <summary>
    /// Choose which implementations use classes to create the metadata 
    /// </summary>
    public class EngineChooser
    {
        /// <summary>
        /// Processed request
        /// </summary>
        private ISdmxParsingObject _parsingObject;
        /// <summary>
        /// Sdmx Version
        /// </summary>
        private SdmxSchemaEnumType _versionTypeResp;

        /// <summary>
        /// Instance of EngineChooser <see cref="EngineChooser"/>
        /// </summary>
        /// <param name="_parsingObject">Processed request</param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        public EngineChooser(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
        {
            this._parsingObject = _parsingObject;
            this._versionTypeResp = _versionTypeResp;
        }

      
        internal IGetDataflows GetDataflows()
        {
            return new GetDataflows(this._parsingObject, this._versionTypeResp);
        }

        internal IGetConcepts GetConcepts()
        {
            return new GetConcepts(this._parsingObject, this._versionTypeResp);
        }

        internal IGetCodelists GetCodelists()
        {
            return new GetCodelists(this._parsingObject, this._versionTypeResp);
        }

        internal IGetAgencyScheme GetAgencyScheme()
        {
            return new GetAgencyScheme(this._parsingObject, this._versionTypeResp);
        }

        internal IGetCategoryScheme GetCategoryScheme()
        {
            return new GetCategoryScheme(this._parsingObject, this._versionTypeResp);
        }

        internal IGetStructure GetStructure()
        {
            return new GetStructure(this._parsingObject, this._versionTypeResp);
        }
    }
}
