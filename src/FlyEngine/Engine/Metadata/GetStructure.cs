using FlyMapping.Model;
using FlyEngine.Model;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlyController;
using FlyController.Builder;
using FlyController.Model;
using FlyController.Model.Error;
using Org.Sdmxsource.Sdmx.Api.Constants;
using OnTheFlyLog;

namespace FlyEngine.Engine.Metadata
{
    /// <summary>
    /// GetStructure implementation of SDMXObjectBuilder for creation of DataStructure
    /// </summary>
    public class GetStructure : SDMXObjectBuilder, IGetStructure
    {
        /// <summary>
        /// create a GetStructure instance
        /// </summary>
        /// <param name="_parsingObject">Sdmx Parsing Object</param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        public GetStructure(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
            : base(_parsingObject, _versionTypeResp)
        { }


        /// <summary>
        /// Populate DataStructure (KeyFamyly for SDMX 2.0, Structure for SDMX 2.1)
        /// property of SDMXObjectBuilder for insert this elements in DataStructure response
        /// </summary>
        public override void Build()
        {
            try
            {
                this._KeyFamily = GetDSDs();
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.BuildDSD, ex);
            }
        }


        /// <summary>
        /// retrieves the DSD from database
        /// </summary>
        /// <returns>list of DataStructure for SDMXObject</returns>
        public List<DataStructureObjectImpl> GetDSDs()
        {
            try
            {
                if (DsdManager == null)
                    DsdManager = MappingConfiguration.MetadataFactory.InstanceDsdManager(this.ParsingObject, this.VersionTypeResp);
                return DsdManager.GetDSDs();
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.BuildDSD, ex);
            }
        }



        #region References
        internal IDsdManager DsdManager { get; set; }

     
        /// <summary>
        /// Add External reference into SdmxObject
        /// </summary>
        public override void AddReferences()
        {
            try
            {
                RetrievalReferences Mr = new RetrievalReferences(this);
                Mr.ReferencesObject=this.DsdManager.ReferencesObject;
             
                Mr.AddReferences(MetadataReferences.ReferenceTreeEnum.Dsd);
                //Destroy Obj
                Mr = null;
                DsdManager = null;

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
