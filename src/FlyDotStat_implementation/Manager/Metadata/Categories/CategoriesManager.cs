using FlyController;
using FlyController.Builder;
using FlyController.Model;
using FlyController.Model.Error;
using FlyDotStat_implementation.Build;
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
    public class CategoriesManager : BaseCategoriesManager
    {
        /// <summary>
        /// Inizialize for BaseManager Class
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        public CategoriesManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
            : base(_parsingObject, _versionTypeResp) { }

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

                XmlNodeList TemeList = ((DWHAccess)DbAccess).TreeExtract();
                RecursivePopulateTreeCategory("-1", TemeList, ref categorie, ref _categoriesLinear, ref _datasetsLinear);

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


        private void RecursivePopulateTreeCategory(string IdCategoryParent, XmlNodeList figli, ref List<InternalCategoryObject> categorie, ref List<InternalCategoryObject> _categoriesLinear, ref List<InternalDatasetObject> _datasetsLinear)
        {
            try
            {
                foreach (XmlNode ContentObject in figli)
                {
                    if (ContentObject.Name == "ContentObject" && ContentObject.ChildNodes.Count > 0)
                    {
                        InternalCategoryObject categoria = new InternalCategoryObject();
                        categoria.PadreId = IdCategoryParent;
                        foreach (XmlNode CategoryProperty in ContentObject.ChildNodes)
                        {
                            switch (CategoryProperty.Name)
                            {
                                case "Id":
                                    categoria.Id = CategoryProperty.InnerText;
                                    break;
                                case "Name":
                                    categoria.Nomi = PopolaNomiTreeCategory(CategoryProperty.ChildNodes);
                                    break;
                                case "DatasetList":
                                    PopolaDataSets_inCategory(CategoryProperty.ChildNodes, ref _datasetsLinear);
                                    break;
                                case "ThemeList":
                                    List<InternalCategoryObject> SubCategory = new List<InternalCategoryObject>();
                                    RecursivePopulateTreeCategory(categoria.Id, CategoryProperty.ChildNodes, ref SubCategory, ref _categoriesLinear, ref _datasetsLinear);
                                    categoria.SubCategory = SubCategory;
                                    break;
                            }
                        }
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

        private List<SdmxObjectNameDescription> PopolaNomiTreeCategory(XmlNodeList xmlNodeList)
        {
            try
            {
                List<SdmxObjectNameDescription> nomi = new List<SdmxObjectNameDescription>();
                if (xmlNodeList != null)
                {
                    foreach (XmlNode item in xmlNodeList)
                    {
                        if (item.Name.Contains(":KeyValueOfLanguageEnumstringi31JDBa4"))
                        {
                            SdmxObjectNameDescription nome = new SdmxObjectNameDescription();
                            foreach (XmlNode name in item)
                            {
                                if (name.Name.Contains(":Key"))
                                    nome.Lingua = CovertLanguageName(name.InnerText);
                                else if (name.Name.Contains(":Value"))
                                    nome.Name = name.InnerText;
                            }
                            nomi.Add(nome);
                        }
                    }
                }
                return nomi;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.InternalError, ex);
            }
        }

        private string CovertLanguageName(string Language)
        {
            try
            {
                switch (Language.Trim().ToUpper())
                {
                    case "ENGLISH":
                        return "en";
                    case "FRENCH":
                        return "fr";
                    default:
                        return Language;
                }
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.ParsingLanguagesIsoCode, ex);
            }
        }

        private void PopolaDataSets_inCategory(XmlNodeList Datasets, ref List<InternalDatasetObject> _datasetsLinear)
        {
            try
            {
                foreach (XmlNode ContentObject in Datasets)
                {
                    if (ContentObject.Name == "ContentObject" && ContentObject.ChildNodes.Count > 0)
                    {
                        InternalDatasetObject dataset = new InternalDatasetObject();
                        foreach (XmlNode datasetsProperty in ContentObject.ChildNodes)
                        {
                            switch (datasetsProperty.Name)
                            {
                                case "Id":
                                    dataset.Id = datasetsProperty.InnerText;
                                    break;
                                case "Name":
                                    dataset.Nomi = PopolaNomiTreeCategory(datasetsProperty.ChildNodes);
                                    break;
                                case "Code":
                                    dataset.Code = datasetsProperty.InnerText;
                                    dataset.Agency=FlyConfiguration.MainAgencyId;
                                    dataset.Version = FlyConfiguration.Version;
                                    break;
                                case "ThemeId":
                                    dataset.ThemeId = datasetsProperty.InnerText;
                                    break;
                            }
                        }
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
