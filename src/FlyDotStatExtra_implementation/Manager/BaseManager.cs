using FlyController;
using FlyController.Model;
using FlyEngine.Model;
using FlyMapping.Build;
using FlyMapping.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyDotStatExtra_implementation.Manager
{
    /// <summary>
    /// Model Class for connetion to Database
    /// </summary>
    public abstract class BaseManager : IBaseManager
    {
        /// <summary>
        /// Inizialize for BaseManager Class
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        public BaseManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
        {
            this.parsingObject = _parsingObject;
            this.versionTypeResp = _versionTypeResp;
        }

        private IDBAccess _dbAccess = null;
        /// <summary>
        /// for retrival data from database
        /// </summary>
        public IDBAccess DbAccess
        {
            get
            {
                if (this._dbAccess == null)
                    this._dbAccess = new FlyDotStat_implementation.Build.DWHAccess(FlyConfiguration.ConnectionString);
                return _dbAccess;
            }
            set { _dbAccess = value; }
        }
       
       
        /// <summary>
        ///  Parsing Object <see cref="ISdmxParsingObject"/>
        /// </summary>
        public ISdmxParsingObject parsingObject { get; set; }

        /// <summary>
        /// Sdmx Version
        /// </summary>
        public SdmxSchemaEnumType versionTypeResp { get; set; }

        /// <summary>
        /// Referenced objects 
        /// </summary>
        public abstract IReferencesObject ReferencesObject { get; set; }

    }
}
