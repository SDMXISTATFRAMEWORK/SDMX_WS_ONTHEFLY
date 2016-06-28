using FlyEngine.Model;
using FlyController.Builder;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.Sdmxsource.Sdmx.Api.Constants;
using FlyController.Model.Error;
using FlyController.Model;
using OnTheFlyLog;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using FlyController;
using FlyMapping.Model;

namespace FlyEngine.Engine.Metadata
{
    /// <summary>
    /// GetCategoryScheme implementation of SDMXObjectBuilder for creation of CategorisationScheme and CategoryScheme
    /// </summary>
    class GetCategoryScheme : SDMXObjectBuilder, IGetCategoryScheme
    {
        /// <summary>
        /// create a GetCategoryScheme instance
        /// </summary>
        /// <param name="_parsingObject">Sdmx Parsing Object</param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        public GetCategoryScheme(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
            : base(_parsingObject, _versionTypeResp)
        { }

        /// <summary>
        /// Populate a CategoryScheme and list of Categorisation property of SDMXObjectBuilder for insert this in DataStructure response
        /// </summary>
        public override void Build()
        {
            try
            {
             
                if (ParsingObject.SdmxStructureType == SdmxStructureEnumType.CategoryScheme)
                {
                    this._CategorySchemeObject = _GetCategory();
                }

                if (ParsingObject.SdmxStructureType == SdmxStructureEnumType.Categorisation)
                {
                    this._CategorisationObject = _GetCategorisation();
                }
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.BuildCategoryScheme, ex);
            }

        }

       

        /// <summary>
        /// Get and Create CategoryScheme
        /// </summary>
        /// <returns><see cref="ICategorySchemeObject"/></returns>
        public List<ICategorySchemeObject> _GetCategory()
        {
            try
            {
                if(CatManager==null)
                    CatManager = MappingConfiguration.MetadataFactory.InstanceCategoriesManager(this.ParsingObject, this.VersionTypeResp);
                return CatManager.GetCategoryScheme();
               
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.BuildCategoryScheme, ex);
            }
        }

        /// <summary>
        /// Get and Create All Categorisation
        /// </summary>
        /// <returns>List of <see cref="ICategorisationObject"/></returns>
        public List<ICategorisationObject> _GetCategorisation()
        {
            try
            {

                if (CatManager == null)
                    CatManager = MappingConfiguration.MetadataFactory.InstanceCategoriesManager(this.ParsingObject, this.VersionTypeResp);
                return CatManager.GetCategorisation();

                
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.BuildCategoryScheme, ex);
            }
        }

        #region References
        internal ICategoriesManager CatManager { get; set; } 
      
        /// <summary>
        /// Add External reference into SdmxObject
        /// </summary>
        public override void AddReferences()
        {
            try
            {
                MetadataReferences.ReferenceTreeEnum structType = MetadataReferences.ReferenceTreeEnum.CategoryScheme;
                if (ParsingObject.SdmxStructureType == SdmxStructureEnumType.Categorisation)
                    structType = MetadataReferences.ReferenceTreeEnum.Categorisation;

                RetrievalReferences Mr = new RetrievalReferences(this);
                Mr.ReferencesObject=this.CatManager.ReferencesObject;
               

                Mr.AddReferences(structType);
                //Destroy Obj
                Mr = null;
                CatManager = null;
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
