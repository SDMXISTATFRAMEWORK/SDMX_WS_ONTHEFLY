using FlyController;
using FlyController.Builder;
using FlyController.Model;
using FlyController.Model.Error;
using FlyEngine.Model;
using FlyMapping.Build;
using FlyMapping.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyDotStat_implementation.Manager.Metadata
{
    /// <summary>
    /// AgencyManager retrieves the data for build  AgencySchema and OrganisationScheme
    /// </summary>
    public class AgencyManager : BaseManager, IAgencyManager
    {
        /// <summary>
        /// Inizialize for BaseManager Class
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        public AgencyManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
            :base(_parsingObject, _versionTypeResp){}

        /// <summary>
        /// Referenced objects 
        /// </summary>
        public override IReferencesObject ReferencesObject { get; set; }

        /// <summary>
        /// Build a AgencyScheme
        /// </summary>
        /// <returns>IAgencyScheme for SdmxObject</returns>
        public IAgencyScheme GetAgencyScheme()
        {
            AgencySchemeBuilder _AgencySchemaBuilder = new AgencySchemeBuilder(this.parsingObject, this.versionTypeResp);
            return _AgencySchemaBuilder.BuildAgencyScheme(this.BuildAgencyObject(), this.GetOrganisationNames());
        }
      
        /// <summary>
        /// Build a Mutalble AgencyObject 
        /// </summary>
        /// <returns>IAgencyMutableObject</returns>
        public IAgencyMutableObject BuildAgencyObject()
        {
            try
            {
                IAgencyMutableObject _agency = new AgencyMutableCore();
                _agency.Id = FlyConfiguration.MainAgencyId;
                foreach (SdmxObjectNameDescription nome in FlyConfiguration.MainAgencyDescription)
                    _agency.AddName(nome.Lingua, nome.Name);
                return _agency;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateIAgencyMutableObject, ex);
            }

        }

        /// <summary>
        /// Get Organisation Names Description
        /// </summary>
        /// <returns></returns>
        public List<SdmxObjectNameDescription> GetOrganisationNames()
        {
            return FlyConfiguration.AgencyOrganisationDescription;
        }




       
    }
}
