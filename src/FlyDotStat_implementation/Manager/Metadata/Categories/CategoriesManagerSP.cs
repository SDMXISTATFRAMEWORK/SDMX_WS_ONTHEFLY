using FlyController;
using FlyController.Builder;
using FlyController.Model;
using FlyController.Model.DbSetting;
using FlyController.Model.Error;
using FlyMapping.Build;
using FlyMapping.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.CategoryScheme;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Xml;

namespace FlyDotStat_implementation.Manager.Metadata
{
    /// <summary>
    /// CategoriesManager retrieves the data for build  CategoryScheme and CategorisationScheme
    /// </summary>
    public class CategoriesManagerSP : BaseCategoriesManager
    {
         /// <summary>
        /// Inizialize for BaseManager Class
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        public CategoriesManagerSP(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
            :base(_parsingObject, _versionTypeResp){}

        /// <summary>
        ///Build a list of category hierarchical from internal structure categories
        /// </summary>
        /// <param name="categoria">Internal retreived categories</param>
        /// <returns>list of categories (ICategoryMutableObject)</returns>
        public override List<ICategoryMutableObject> RecursiveCreateCategoryHierarchy(List<InternalCategoryObject> categoria)
        {
            try
            {
                if (categoria == null)
                    return null;
                List<ICategoryMutableObject> SubCategories = new List<ICategoryMutableObject>();
                foreach (InternalCategoryObject _cal in categoria)
                    SubCategories.Add(BuildCategoryObject(_cal.Id, _cal.Nomi, RecursiveCreateCategoryHierarchy(_cal.SubCategory)));

                return SubCategories;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.RecursiveCreateCategoryHierarchy, ex);
            }

        }
        /// <summary>
        /// Build Category Object (ICategoryMutableObject)
        /// </summary>
        /// <param name="CategoryObjectId">Category Code</param>
        /// <param name="names">Description Names</param>
        /// <param name="SubCategory">list of SubCategory </param>
        /// <returns></returns>
        private ICategoryMutableObject BuildCategoryObject(string CategoryObjectId, List<SdmxObjectNameDescription> names, List<ICategoryMutableObject> SubCategory)
        {
            try
            {
                ICategoryMutableObject co = new CategoryMutableCore();
                co.Id = CategoryObjectId;
                foreach (SdmxObjectNameDescription item in names)
                    co.AddName(item.Lingua, item.Name);

                if (SubCategory != null)
                    foreach (ICategoryMutableObject subCo in SubCategory)
                        co.AddItem(subCo);

                return co;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateICategoryMutableObject, ex);
            }
        }

        /// <summary>
        /// Create list of categories objects
        /// </summary>
        /// <param name="_datasetsLinear">return list of dataset associated with the categories</param>
        /// <returns>hierarchical categories</returns>
        public override List<InternalCategoryObject> CreateCategoryObjects(out List<InternalDatasetObject> _datasetsLinear)
        {
            List<InternalCategoryObject> _categoriesLinear = new List<InternalCategoryObject>();
            _datasetsLinear = new List<InternalDatasetObject>();
            try
            {
                List<InternalCategoryObject> categorie = new List<InternalCategoryObject>();
                List<XmlNode> TemeList = DbAccess.Execute(DBOperationEnum.GetCategory, new List<IParameterValue>());
                RecursivePopulateTreeCategorySP("-1", TemeList, ref categorie, ref _categoriesLinear, ref _datasetsLinear);

                _datasetsLinear = GetCategorisationObjectWhitCategory(_datasetsLinear, _categoriesLinear);
                return categorie;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DBCreateCategoryObjects, ex);
            }
        }


        #region Portion of code that creates a list of InternalCategoryObject from a table in another DB
        private List<InternalDatasetObject> GetCategorisationObjectWhitCategory(List<InternalDatasetObject> _datasetsLinear, List<InternalCategoryObject> _categoriesLinear)
        {
            try
            {
                if (GetDataFlowDelegate == null)
                    GetDataFlowDelegate = new InternalGetDataFlowDelegate(InternalGetDataFlow);
                List<IDataflowObject> dfls = GetDataFlowDelegate();

                //Qui manca il match con i dataflow estratti datta tabella in BuildCategorySchemeObject
                List<InternalDatasetObject> dflscatMapped = _datasetsLinear.FindAll(df => dfls.Exists(dfs => dfs.Id.Trim() == df.Code.Trim()));

                foreach (var dfl in dflscatMapped)
                {
                    dfl.Agency = dfls.First(df => df.Id.Trim() == dfl.Code.Trim()).AgencyId;
                    dfl.Version = dfls.First(df => df.Id.Trim() == dfl.Code.Trim()).Version;

                    InternalCategoryObject _foundcat = _categoriesLinear.Find(cat => cat.Id == dfl.ThemeId);
                    dfl.CategoryParent = new List<string>() { dfl.ThemeId };
                    while (_foundcat.PadreId != "-1" && _categoriesLinear.Exists(cat => cat.Id == _foundcat.PadreId))
                    {
                        _foundcat = _categoriesLinear.Find(cat => cat.Id == _foundcat.PadreId);
                        dfl.CategoryParent.Insert(0, _foundcat.Id);
                    }
                }

                return dflscatMapped;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.InternalError, ex);
            }
        }
   
        private void RecursivePopulateTreeCategorySP(string IdCategoryParent, List<XmlNode> figli, ref List<InternalCategoryObject> categorie, ref List<InternalCategoryObject> _categoriesLinear, ref List<InternalDatasetObject> _datasetsLinear)
        {
            try
            {
                foreach (XmlNode ContentObject in figli)
                {
                    if (ContentObject.Name == "ContentObject" && ContentObject.ChildNodes.Count > 0)
                    {
                        InternalCategoryObject categoria = new InternalCategoryObject();
                        categoria.PadreId = IdCategoryParent;

                        if (ContentObject.Attributes["Code"] != null)
                        {
                            categoria.Id = ContentObject.Attributes["Code"].Value.ToString();
                        }
                        categoria.Nomi = SdmxObjectNameDescription.GetNameDescriptions(ContentObject);
                        List<InternalCategoryObject> SubCategory = new List<InternalCategoryObject>();
                        foreach (XmlNode CategoryProperty in ContentObject.ChildNodes)
                        {
                            switch (CategoryProperty.Name)
                            {
                                case "DatasetList":
                                    PopolaDataSets_inCategorySP(categoria.Id, CategoryProperty.ChildNodes, ref _datasetsLinear);
                                    break;
                                case "ContentObject":
                                    RecursivePopulateTreeCategorySP(categoria.Id, new List<XmlNode>() { CategoryProperty }, ref SubCategory, ref _categoriesLinear, ref _datasetsLinear);
                                    break;
                            }
                        }
                        categoria.SubCategory = SubCategory;

                        categorie.Add(categoria);
                        _categoriesLinear.Add(categoria);
                    }
                }
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.InternalError, ex);
            }
        }


        private void PopolaDataSets_inCategorySP(string CategoryName, XmlNodeList Datasets, ref List<InternalDatasetObject> _datasetsLinear)
        {
            try
            {
                if (Datasets == null) return;
                foreach (XmlNode ContentObject in Datasets)
                {
                    if (ContentObject.Name == "Dataset" && ContentObject.Attributes["Code"] != null)
                    {
                        InternalDatasetObject dataset = new InternalDatasetObject();
                        dataset.ThemeId = CategoryName;
                        dataset.Code = ContentObject.Attributes["Code"].Value.ToString();
                        dataset.Agency = FlyConfiguration.MainAgencyId;
                        dataset.Version = FlyConfiguration.Version;
                        _datasetsLinear.Add(dataset);
                    }
                }
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.InternalError, ex);
            }
        }
        #endregion
    }

   
}
