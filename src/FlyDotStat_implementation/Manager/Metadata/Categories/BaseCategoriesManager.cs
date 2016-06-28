using FlyController.Builder;
using FlyController.Model;
using FlyEngine.Model;
using FlyMapping.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyDotStat_implementation.Manager.Metadata
{
    /// <summary>
    /// BaseCategoriesManager retrieves the data for build  CategoryScheme and CategorisationScheme
    /// </summary>
    public abstract class BaseCategoriesManager : BaseManager, ICategoriesManager
    {

        private List<InternalDatasetObject> _categorisationMapping;
        private List<InternalCategoryObject> _categoryschemeMapping;

        /// <summary>
        /// Delegate for Get Dataflow
        /// </summary>
        /// <returns>List of Dataflow</returns>
        public delegate List<IDataflowObject> InternalGetDataFlowDelegate();
        
        /// <summary>
        /// Implementation of delegate for Get Dataflow
        /// </summary>
        public InternalGetDataFlowDelegate GetDataFlowDelegate = null;

        /// <summary>
        /// Referenced objects 
        /// </summary>
        public override IReferencesObject ReferencesObject { get; set; }

        /// <summary>
        /// Inizialize for BaseManager Class
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        public BaseCategoriesManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
            : base(_parsingObject, _versionTypeResp) { }

        /// <summary>
        /// Build a CategoryScheme
        /// </summary>
        /// <returns>ICategorySchemeObject for SdmxObject</returns>
        public List<ICategorySchemeObject> GetCategoryScheme()
        {
            if (ReferencesObject == null)
                ReferencesObject = new IReferencesObject();

            CategorySchemeBuilder _CategorySchemeBuilder = new CategorySchemeBuilder(this.parsingObject, this.versionTypeResp);

            if (_categoryschemeMapping == null)
            {
                _categorisationMapping = new List<InternalDatasetObject>();
                _categoryschemeMapping = CreateCategoryObjects(out _categorisationMapping);
            }

            if (!parsingObject.ReturnStub)
            {
                List<ICategoryMutableObject> CategoriesHierarchy = RecursiveCreateCategoryHierarchy(_categoryschemeMapping);
                this.ReferencesObject.CategoryScheme =new List<ICategorySchemeObject>(){ _CategorySchemeBuilder.BuildCategorySchemeObject(CategoriesHierarchy)};
            }
            else
            {
                this.ReferencesObject.CategoryScheme = new List<ICategorySchemeObject>(){ _CategorySchemeBuilder.BuildCategorySchemeObject(null)};
            }
            return this.ReferencesObject.CategoryScheme;
        }
        /// <summary>
        /// Build a Categorisation
        /// </summary>
        /// <returns>list of ICategorisationObject for SdmxObject</returns>
        public List<ICategorisationObject> GetCategorisation()
        {
            if (ReferencesObject == null)
                ReferencesObject = new IReferencesObject();

            CategorySchemeBuilder _CategorySchemeBuilder = new CategorySchemeBuilder(this.parsingObject, this.versionTypeResp);

            if (_categoryschemeMapping == null)
            {
                _categorisationMapping = new List<InternalDatasetObject>();
                _categoryschemeMapping = CreateCategoryObjects(out _categorisationMapping);
            }

            this.ReferencesObject.Categorisation = new List<ICategorisationObject>();
            foreach (InternalDatasetObject dfl in _categorisationMapping)
                this.ReferencesObject.Categorisation.Add(_CategorySchemeBuilder.BuildCategorisation(dfl.Code, dfl.Agency, dfl.Version, dfl.CategoryParent));
            return this.ReferencesObject.Categorisation;
        }

        /// <summary>
        /// Build a CategoryScheme
        /// </summary>
        /// <param name="refObj">Referenced Objects</param>
        /// <returns>ICategorySchemeObject for SdmxObject</returns>
        public List<ICategorySchemeObject> GetCategorySchemeReferences(IReferencesObject refObj)
        {
            if (refObj.CategoryScheme != null)
                return refObj.CategoryScheme;
            return GetCategoryScheme();
            
        }
        /// <summary>
        /// Build a Categorisation
        /// </summary>
        /// <param name="refObj">Referenced Objects</param>
        /// <returns>list of ICategorisationObject for SdmxObject</returns>
        public List<ICategorisationObject> GetCategorisationReferences(IReferencesObject refObj)
        {
            if (refObj.Categorisation != null)
                return refObj.Categorisation;
            return GetCategorisation();
        }

        internal List<IDataflowObject> InternalGetDataFlow()
        {
            List<IDataflowObject> dfls = null;
            if (this.ReferencesObject == null || this.ReferencesObject.FoundedDataflows == null || this.ReferencesObject.FoundedDataflows.Count == 0)
            {
                IDataflowsManager DataflowManager = new MetadataFactory().InstanceDataflowsManager(this.parsingObject, this.versionTypeResp);
                dfls = DataflowManager.GetDataFlows();
            }
            else
                dfls = this.ReferencesObject.FoundedDataflows;
            return dfls;
        }

        /// <summary>
        ///Build a list of category hierarchical from internal structure categories
        /// </summary>
        /// <param name="categoria">Internal retreived categories</param>
        /// <returns>list of categories (ICategoryMutableObject)</returns>
        public abstract List<ICategoryMutableObject> RecursiveCreateCategoryHierarchy(List<InternalCategoryObject> categoria);

        /// <summary>
        /// Create list of categories objects
        /// </summary>
        /// <param name="_datasetsLinear">return list of dataset associated with the categories</param>
        /// <returns>hierarchical categories</returns>
        public abstract List<InternalCategoryObject> CreateCategoryObjects(out List<InternalDatasetObject> _datasetsLinear);
    }
}
