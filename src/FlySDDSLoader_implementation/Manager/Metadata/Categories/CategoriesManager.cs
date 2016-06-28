using FlyController.Builder;
using FlyController.Model;
using FlyController.Model.DbSetting;
using FlyController.Model.Error;
using FlyEngine.Model;
using FlyMapping.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.CategoryScheme;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;

namespace FlySDDSLoader_implementation.Manager.Metadata
{
    /// <summary>
    ///  retrieves the data for build  CategoryScheme and CategorisationScheme
    /// </summary>
    public class CategoriesManager : BaseManager, ICategoriesManager
    {

        private CategorySchemeBuilder csb = null;

        /// <summary>
        /// Referenced objects 
        /// </summary>
        public override IReferencesObject ReferencesObject { get; set; }

        /// <summary>
        /// Inizialize for BaseManager Class
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        public CategoriesManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
            : base(_parsingObject, _versionTypeResp) { }


        /// <summary>
        /// Build a CategoryScheme
        /// </summary>
        /// <returns>ICategorySchemeObject for SdmxObject</returns>
        public List<ICategorySchemeObject> GetCategoryScheme()
        {
            if (ReferencesObject == null)
                ReferencesObject = new IReferencesObject();

            if (ReferencesObject.CategoryScheme != null)
                return ReferencesObject.CategoryScheme;

            if (this.parsingObject.ReturnStub)
            {
                GetStubCategoryInformation();
            }
            else
            {
                GetAllCategoryInformation();
            }

            return ReferencesObject.CategoryScheme;
        }

        private void GetStubCategoryInformation()
        {
            DataTable dt;
            ICategorySchemeMutableObject tmpCategoryscheme = null;

            dt = DbAccess.ExecutetoTable(DBOperationEnum.GetCategorySchemes, new List<IParameterValue>());

            if (dt.Rows.Count > 0)
            {
                this.ReferencesObject.CategoryScheme = new List<ICategorySchemeObject>();

                string csOldUri = string.Empty,csCurrentUri = string.Empty;

                foreach (DataRow row in dt.Rows)
                {
                    // ART_ID,ID,AGENCY,VERSION,LANGUAGE,TEXT

                    csCurrentUri = row["ID"].ToString() + row["AGENCY"].ToString() + row["VERSION"].ToString();

                    if (csOldUri != csCurrentUri)
                    {
                        if (tmpCategoryscheme != null)
                            this.ReferencesObject.CategoryScheme.Add(tmpCategoryscheme.ImmutableInstance);

                        tmpCategoryscheme = new CategorySchemeMutableCore();

                        tmpCategoryscheme.Id = row["ID"].ToString();
                        tmpCategoryscheme.AgencyId = row["AGENCY"].ToString();
                        tmpCategoryscheme.Version = row["VERSION"].ToString();
                        tmpCategoryscheme.FinalStructure = TertiaryBool.ParseBoolean(row["IS_FINAL"].ToString() == "1");

                        csOldUri = csCurrentUri;
                    }

                    tmpCategoryscheme.AddName(row["LANGUAGE"].ToString(), row["TEXT"].ToString());
                }

                if (tmpCategoryscheme != null)
                    this.ReferencesObject.CategoryScheme.Add(tmpCategoryscheme.ImmutableInstance);
            }

        }

        /// <summary>
        /// Build a Categorisation
        /// </summary>
        /// <returns>list of ICategorisationObject for SdmxObject</returns>
        public List<ICategorisationObject> GetCategorisation()
        {
            if (ReferencesObject == null)
                ReferencesObject = new IReferencesObject();

            if (ReferencesObject.Categorisation != null && ReferencesObject.Categorisation.Count > 0)
                return ReferencesObject.Categorisation;


            GetAllCategoryInformation();

            return ReferencesObject.Categorisation;
        }

        private void GetAllCategoryInformation()
        {
            try
            {
                //Devo popolare tutte e due le referenze (Category Categorisation)
                DataflowsManager dfMan = new DataflowsManager(this.parsingObject.CloneForReferences(), this.versionTypeResp);
                List<MSDataflow> DFs = dfMan.GetMSDataflows(new BuilderParameter() { });


                csb = new CategorySchemeBuilder(this.parsingObject, this.versionTypeResp);
                List<XmlNode> CategorySchemeList = DbAccess.Execute(DBOperationEnum.MSGetCategoryAndCategorisation, new List<IParameterValue>());

               this.ReferencesObject.CategoryScheme = new List<ICategorySchemeObject>();
                this.ReferencesObject.Categorisation = new List<ICategorisationObject>();
                foreach (XmlNode CategorySchemeNode in CategorySchemeList)
                {

                    csb.Code = CategorySchemeNode.Attributes["Code"].Value.ToString();
                    csb.CategorySchemeAgencyId = CategorySchemeNode.Attributes["AgencyId"].Value;
                    csb.CategorySchemeVersion = CategorySchemeNode.Attributes["Version"].Value;
                    csb.Names = SdmxObjectNameDescription.GetNameDescriptions(CategorySchemeNode);
                    List<ICategoryMutableObject> CategoryObjects = RecursivePopulateTreeCategorySP(CategorySchemeNode.ChildNodes, DFs, new List<string>());
                    this.ReferencesObject.CategoryScheme.Add(csb.BuildCategorySchemeObject(CategoryObjects));
                }
                
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.InternalError, ex);
            }
        }

        private List<ICategoryMutableObject> RecursivePopulateTreeCategorySP(XmlNodeList Categories, List<MSDataflow> DFs, List<string> IdCategoryParents)
        {
            try
            {
                if (Categories == null)
                    return new List<ICategoryMutableObject>();
                List<ICategoryMutableObject> SubCategory = new List<ICategoryMutableObject>();
                foreach (XmlNode ContentObject in Categories)
                {
                    if (ContentObject.Name == "ContentObject" && ContentObject.ChildNodes.Count > 0)
                    {

                        ICategoryMutableObject ActualCategory = BuildCategoryObject(
                             ContentObject.Attributes["Code"].Value.ToString(),
                            SdmxObjectNameDescription.GetNameDescriptions(ContentObject));
                        List<string> MyParents = IdCategoryParents.ToList();
                        MyParents.Add(ActualCategory.Id);
                        bool MustAdded = false;
                        foreach (XmlNode CategoryProperty in ContentObject.ChildNodes)
                        {
                            switch (CategoryProperty.Name)
                            {
                                case "DataflowList":
                                    PopolaDataflows_inCategory(ActualCategory.Id, CategoryProperty.ChildNodes, DFs, MyParents);
                                    break;
                                case "ContentObject":
                                    if (!MustAdded)
                                    {
                                        List<ICategoryMutableObject> SubCategoryChildren = RecursivePopulateTreeCategorySP(ContentObject.ChildNodes, DFs, MyParents);
                                        if (SubCategoryChildren != null)
                                            foreach (ICategoryMutableObject subCo in SubCategoryChildren)
                                                ActualCategory.AddItem(subCo);
                                        MustAdded = true;
                                    }
                                    break;
                            }
                        }
                        SubCategory.Add(ActualCategory);
                    }
                }
                return SubCategory;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.InternalError, ex);
            }
        }

        private void PopolaDataflows_inCategory(string CategoryId, XmlNodeList DataflowNodes, List<MSDataflow> DFs, List<string> IdCategoryParents)
        {
            if (DataflowNodes == null)
                return;
            foreach (XmlNode DataflowNode in DataflowNodes)
            {
                string DfId = DataflowNode.Attributes["Code"].Value;
                MSDataflow DfFounded = DFs.Find(df => df.IdDf.ToString() == DfId.Trim());
                if (DfFounded != null)
                {
                    //Aggiungo il Categorisation
                    if (this.ReferencesObject.Categorisation == null)
                        this.ReferencesObject.Categorisation = new List<ICategorisationObject>();

                    this.ReferencesObject.Categorisation.Add(csb.BuildCategorisation(DfFounded.DFCode, DfFounded.DFAgency, DfFounded.DFVersion, IdCategoryParents));
                }
            }

        }

        /// <summary>
        /// Build Category Object (ICategoryMutableObject)
        /// </summary>
        /// <param name="CategoryObjectId">Category Code</param>
        /// <param name="names">Description Names</param>
        /// <returns></returns>
        private ICategoryMutableObject BuildCategoryObject(string CategoryObjectId, List<SdmxObjectNameDescription> names)
        {
            try
            {
                ICategoryMutableObject co = new CategoryMutableCore();
                co.Id = CategoryObjectId;
                foreach (SdmxObjectNameDescription item in names)
                    co.AddName(item.Lingua, item.Name);


                return co;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateICategoryMutableObject, ex);
            }
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
    }
}
