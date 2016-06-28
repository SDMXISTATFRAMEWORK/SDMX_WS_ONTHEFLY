using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Objects.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlyController.Model;
using FlyController;
using FlyController.Model.Error;
using Org.Sdmxsource.Sdmx.Api.Constants;

namespace FlyController.Builder
{
    /// <summary>
    /// AgencySchemaBuilder Create a ImmutableInstance of OrganisationScheme with AgencyScheme
    /// </summary>
    public class AgencySchemeBuilder : ObjectBuilder
    {
        #region IObjectBuilder Property
        /// <summary>
        ///  Identificable Code
        /// </summary>
        public override string Code { get; set; }
        /// <summary>
        ///  Descriptions Names
        /// </summary>
        public override List<SdmxObjectNameDescription> Names { get; set; }
       
        #endregion

        /// <summary>
        /// Inizialize new instance of AgencySchemeBuilder
        /// </summary>
        /// <param name="parsingObject">Parsing Object <see cref="ISdmxParsingObject"/></param>
        /// <param name="versionTypeResp">Sdmx Version</param>
        public AgencySchemeBuilder(ISdmxParsingObject parsingObject, SdmxSchemaEnumType versionTypeResp) :
            base(parsingObject,versionTypeResp)
        { }
       
        /// <summary>
        /// Create a ImmutableInstance of AgencySchema
        /// </summary>
        /// <param name="_AgencyObject">Agency Object</param>
        /// <param name="AgencyOrganisationNames">Agency Organisation Names (Get it from File Config)</param>
        /// <returns>IAgencyScheme</returns>
        public IAgencyScheme BuildAgencyScheme(IAgencyMutableObject _AgencyObject, List<SdmxObjectNameDescription> AgencyOrganisationNames)
        {
            try
            {
                IAgencyScheme defschema = AgencySchemeCore.CreateDefaultScheme();
                IAgencySchemeMutableObject mutabledefschema = defschema.MutableInstance;

                mutabledefschema.AgencyId = FlyConfiguration.MainAgencyId;
                mutabledefschema.Names.Clear();
                foreach (var item in AgencyOrganisationNames)
                    mutabledefschema.AddName(item.Lingua, item.Name);


                mutabledefschema.Items.Clear();
                mutabledefschema.AddItem(_AgencyObject);

                return mutabledefschema.ImmutableInstance;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateImmutable, ex);
            }
        }

    }
}
