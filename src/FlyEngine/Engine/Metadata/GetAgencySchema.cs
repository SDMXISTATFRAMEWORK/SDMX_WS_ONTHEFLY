using FlyEngine.Model;
using FlyController;
using FlyController.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Constants;
using FlyController.Model.Error;
using FlyController.Model;
using OnTheFlyLog;
using FlyMapping.Model;

namespace FlyEngine.Engine.Metadata
{
    /// <summary>
    /// GetAgencySchema implementation of SDMXObjectBuilder for creation of OrganisationScheme and AgencyScheme
    /// </summary>
    class GetAgencyScheme : SDMXObjectBuilder, IGetAgencyScheme
    {
        /// <summary>
        /// create a GetAgencyScheme instance
        /// </summary>
        /// <param name="_parsingObject">Sdmx Parsing Object</param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        public GetAgencyScheme(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
            : base(_parsingObject, _versionTypeResp)
        { }

        /// <summary>
        /// Populate a AgencyScheme property of SDMXObjectBuilder for insert this in DataStructure response
        /// </summary>
        public override void Build()
        {
            try
            {
                this._AgencyScheme = _GetAgencyScheme();
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.BuildAgencyScheme, ex);
            }
        }

        /// <summary>
        /// Create a immutable instance of IAgencyScheme
        /// </summary>
        /// <returns>IAgencyScheme</returns>
        public IAgencyScheme _GetAgencyScheme()
        {
            try
            {
                IAgencyManager AgMan = MappingConfiguration.MetadataFactory.InstanceAgencyManager(this.ParsingObject, this.VersionTypeResp);
                return AgMan.GetAgencyScheme();
               
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetAgencyScheme, ex);
            }
        }

        #region References
        /// <summary>
        /// Add External reference into SdmxObject
        /// </summary>
        public override void AddReferences()
        {
            try
            {
                FlyLog.WriteLog(this, FlyLog.LogTypeEnum.All, "No References for AgencyScheme");

            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.AddReferences, ex);
            }
        }

        
        #endregion
    }
}
