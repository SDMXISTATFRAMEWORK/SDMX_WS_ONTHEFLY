using FlyController;
using FlyController.Model;
using FlyController.Model.DbSetting;
using FlyController.Model.Error;
using FlyEngine.Model;
using FlyMapping.Build;
using FlyMapping.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;

namespace FlyDotStat_implementation.Manager.Metadata
{
    /// <summary>
    /// FLAGManager retrieves the data for Concept FLAG
    /// </summary>
    public class FLAGManager : BaseManager, IFLAGManager
    {
         /// <summary>
        /// Inizialize for BaseManager Class
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        public FLAGManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
            :base(_parsingObject, _versionTypeResp){}

        /// <summary>
        /// Referenced objects 
        /// </summary>
        public override IReferencesObject ReferencesObject { get; set; }

        /// <summary>
        /// Get Flag Attribute (OBS_STATUS)
        /// </summary>
        /// <returns>Flag Attribute</returns>
        public IAttributeConcept GetFlag()
        {
            if (FlyConfiguration.ConceptObservationAttribute != null && this.DbAccess.CheckExistStoreProcedure(DBOperationEnum.GetFlags))
            {//Aggiungo l'attributo Flag
                return FlyConfiguration.ConceptObservationAttribute;
            }
            return null;
        }

       
    }
}
