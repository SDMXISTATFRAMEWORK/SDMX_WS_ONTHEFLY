using FlyController.Builder;
using FlyController.Model;
using FlyController.Model.DbSetting;
using FlyController.Model.Error;
using FlyDotStat_implementation.Manager.Metadata;
using FlyEngine.Model;
using FlyMapping.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.CategoryScheme;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using Org.Sdmxsource.Sdmx.Util.Objects.Reference;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;

namespace FlyDotStatExtra_implementation.Manager.Metadata
{
    /// <summary>
    ///  retrieves the data for build  CategoryScheme and CategorisationScheme
    /// </summary>
    public class CategoriesManager : BaseManager, ICategoriesManager
    {
        private DataTable _dtCSFull;
        private DataTable _dtCategorisationDataflow;

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

            GetAllCategoryInformation();

            return ReferencesObject.CategoryScheme;
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

        private CategorySchemeBuilder csb = null;

        private void GetAllCategoryInformation()
        {
            try
            {
                //Devo popolare tutte e due le referenze (Category Categorisation)
                //DataflowsManager dfMan = new DataflowsManager(this.parsingObject.CloneForReferences(), this.versionTypeResp);
                //List<MSDataflow> DFs = dfMan.GetMSDataflows(new BuilderParameter() { });

                //sdmxObjContainer = new SdmxObjectsImpl();

                _dtCSFull = DbAccess.ExecutetoTable(DBOperationEnum.GetCategorySchemes, null);

                if (_dtCSFull.Rows.Count <= 0)
                    return;

                ReferencesObject.CategoryScheme = new List<ICategorySchemeObject>();

                List<ICategorySchemeObject> lCS = new List<ICategorySchemeObject>();
                DataView viewCS = new DataView(_dtCSFull);
                DataTable dtCS = viewCS.ToTable(true, "IDCS", "CSID", "CSAgency", "CSVersion");

                ICategorySchemeMutableObject catSchema = null;

                string filter;

                foreach (DataRow cs in dtCS.Rows)
                {
                    catSchema = new CategorySchemeMutableCore();
                    catSchema.Id = cs["CSID"].ToString();
                    catSchema.AgencyId = cs["CSAgency"].ToString();
                    catSchema.Version = cs["CSVersion"].ToString();

                    // Get Names
                    DataTable dtCSNames = viewCS.ToTable(true, "IDCS", "CSLangName", "CSValueName");
                    filter = String.Format("IDCS = {0}", cs["IDCS"].ToString());
                    SetNames(dtCSNames.Select(filter), catSchema);

                    AddChilds(catSchema, filter);

                    ReferencesObject.CategoryScheme.Add(catSchema.ImmutableInstance);
                }

                GetCategorisationDataflow();

                //ICategoryMutableObject catO;
                //List<ICategoryMutableObject> lCatO = null;

                //catO = new CategoryMutableCore();

                //catO.AddName("nome", "en");
                //catO.Id = row["CatCode"].ToString();


                //    lCatO.Add(catO);


                //                    lCatO = new List<ICategoryMutableObject>();

                //            csb.Code = row["ID"].ToString();
                //            csb.CategorySchemeAgencyId = row["Agency"].ToString();
                //            csb.CategorySchemeVersion = row["Version"].ToString();

                //            csb.Names.Add(new SdmxObjectNameDescription() { Name = "nome", Lingua = "en" });

                //            idCS = currIDCS;

                //ReferencesObject.CategoryScheme.Add(csb.BuildCategorySchemeObject(lCatO));

                //csb.Names = SdmxObjectNameDescription.GetNameDescriptions(CategorySchemeNode);

                //ReferencesObject.CategoryScheme.Add(csb.BuildCategorySchemeObject(CategoryObjects));


                //this.ReferencesObject.CategoryScheme = new List<ICategorySchemeObject>();
                //this.ReferencesObject.Categorisation = new List<ICategorisationObject>();
                //foreach (XmlNode CategorySchemeNode in CategorySchemeList)
                //{
                //    csb.Code = CategorySchemeNode.Attributes["Code"].Value.ToString();
                //    csb.CategorySchemeAgencyId = CategorySchemeNode.Attributes["AgencyId"].Value;
                //    csb.CategorySchemeVersion = CategorySchemeNode.Attributes["Version"].Value;
                //    csb.Names = SdmxObjectNameDescription.GetNameDescriptions(CategorySchemeNode);
                //    List<ICategoryMutableObject> CategoryObjects = RecursivePopulateTreeCategorySP(CategorySchemeNode.ChildNodes, DFs, new List<string>());
                //    this.ReferencesObject.CategoryScheme.Add(csb.BuildCategorySchemeObject(CategoryObjects));
                //}

            }
            catch (SdmxException sEx)
            {
                throw sEx;
            }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.InternalError, ex);
            }
        }

        private void GetCategorisationDataflow()
        {
            _dtCategorisationDataflow = DbAccess.ExecutetoTable(DBOperationEnum.GetCategorisationDataflow, null);

            if (_dtCategorisationDataflow.Rows.Count <= 0)
                return;

            ReferencesObject.Categorisation = new List<ICategorisationObject>();

            string DFID, DFAgency, DFVersion, CSID, CSAgency, CSVersion, CatCode, IDCAT, IDDataFlow;

            DataView viewCSDF = new DataView(_dtCategorisationDataflow);
            DataTable dtCSDF = viewCSDF.ToTable(true, "IDDataFlow","DFID", "DFAgency", "DFVersion", "CSID",
                                                        "CSAgency", "CSVersion", "CatCode", "IDCAT");
            DataTable dtCatNames = viewCSDF.ToTable(true, "IDCAT", "CubeLang", "CubeName", "IDDataFlow");

            ICategorisationMutableObject categorisation;

            foreach (DataRow row in dtCSDF.Rows)
            {
                IDDataFlow = row["IDDataFlow"].ToString();
                DFID = row["DFID"].ToString();
                DFAgency = row["DFAgency"].ToString();
                DFVersion = row["DFVersion"].ToString();
                CSID = row["CSID"].ToString();
                CSAgency = row["CSAgency"].ToString();
                CSVersion = row["CSVersion"].ToString();
                CatCode = row["CatCode"].ToString();
                IDCAT = row["IDCAT"].ToString();

                categorisation = new CategorisationMutableCore()
                {
                    Id = DFID,
                    AgencyId = DFAgency,
                    Version = DFVersion,
                    FinalStructure = TertiaryBool.ParseBoolean(true),

                };

                IStructureReference structureRef = new StructureReferenceImpl(DFAgency,DFID,DFVersion,
                                                                            SdmxStructureEnumType.Dataflow);
                IStructureReference categoryRef = new StructureReferenceImpl(CSAgency, CSID, CSVersion,
                                                                            SdmxStructureEnumType.Category, CatCode);

                categorisation.StructureReference = structureRef;
                categorisation.CategoryReference = categoryRef;

                SetNames(dtCatNames.Select("IDCat = " + IDCAT + " AND IDDataFlow = " + IDDataFlow), categorisation);

                ReferencesObject.Categorisation.Add(categorisation.ImmutableInstance);
            }
        }

        private void AddChilds(ICategorySchemeMutableObject catSchema, string filter)
        {
            DataView viewCS = new DataView(_dtCSFull);
            DataTable dtCS = viewCS.ToTable(true, "IDCS", "IDCat", "CatCode", "IDParent");

            DataTable dtCatNames = viewCS.ToTable(true, "IDCat", "CatLangName", "CatValueName");

            filter += " AND IDParent = 0";

            DataRow[] dRows = dtCS.Select(filter);

            ICategoryMutableObject cat;

            foreach (DataRow row in dRows)
            {
                cat = new CategoryMutableCore();
                cat.Id = row["CatCode"].ToString();

                SetNames(dtCatNames.Select("IDCat = " + row["IDCat"].ToString()), cat);

                AddRecursiveChilds(cat, dtCatNames, dtCS, row["IDCat"].ToString());

                catSchema.Items.Add(cat);
            }

        }

        private void AddRecursiveChilds(ICategoryMutableObject cat, DataTable dtCatNames, DataTable dtCS, string idCat)
        {
            string filter = " IDParent = " + idCat;
            DataRow[] dRows = dtCS.Select(filter);

            ICategoryMutableObject catChild;

            foreach (DataRow row in dRows)
            {
                catChild = new CategoryMutableCore();
                catChild.Id = row["CatCode"].ToString();

                SetNames(dtCatNames.Select("IDCat = " + row["IDCat"].ToString()), catChild);

                AddRecursiveChilds(catChild, dtCatNames, dtCS, row["IDCat"].ToString());

                cat.Items.Add(catChild);
            }
        }

        private void SetNames(DataRow[] dataRow, INameableMutableObject mutableObj)
        {
            foreach (DataRow name in dataRow)
            {
                mutableObj.AddName(name[1].ToString(), name[2].ToString());
            }
        }

        //private List<ICategoryMutableObject> RecursivePopulateTreeCategorySP(XmlNodeList Categories, List<MSDataflow> DFs, List<string> IdCategoryParents)
        //{
        //    try
        //    {
        //        if (Categories == null)
        //            return new List<ICategoryMutableObject>();
        //        List<ICategoryMutableObject> SubCategory = new List<ICategoryMutableObject>();
        //        foreach (XmlNode ContentObject in Categories)
        //        {
        //            if (ContentObject.Name == "ContentObject" && ContentObject.ChildNodes.Count > 0)
        //            {

        //                ICategoryMutableObject ActualCategory = BuildCategoryObject(
        //                     ContentObject.Attributes["Code"].Value.ToString(),
        //                    SdmxObjectNameDescription.GetNameDescriptions(ContentObject));
        //                List<string> MyParents = IdCategoryParents.ToList();
        //                MyParents.Add(ActualCategory.Id);
        //                bool MustAdded = false;
        //                foreach (XmlNode CategoryProperty in ContentObject.ChildNodes)
        //                {
        //                    switch (CategoryProperty.Name)
        //                    {
        //                        case "DataflowList":
        //                            PopolaDataflows_inCategory(ActualCategory.Id, CategoryProperty.ChildNodes, DFs, MyParents);
        //                            break;
        //                        case "ContentObject":
        //                            if (!MustAdded)
        //                            {
        //                                List<ICategoryMutableObject> SubCategoryChildren = RecursivePopulateTreeCategorySP(ContentObject.ChildNodes, DFs, MyParents);
        //                                if (SubCategoryChildren != null)
        //                                    foreach (ICategoryMutableObject subCo in SubCategoryChildren)
        //                                        ActualCategory.AddItem(subCo);
        //                                MustAdded = true;
        //                            }
        //                            break;
        //                    }
        //                }
        //                SubCategory.Add(ActualCategory);
        //            }
        //        }
        //        return SubCategory;
        //    }
        //    catch (SdmxException) { throw; }
        //    catch (Exception ex)
        //    {
        //        throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.InternalError, ex);
        //    }
        //}

        //private void PopolaDataflows_inCategory(string CategoryId, XmlNodeList DataflowNodes, List<MSDataflow> DFs, List<string> IdCategoryParents)
        //{
        //    if (DataflowNodes == null)
        //        return;
        //    foreach (XmlNode DataflowNode in DataflowNodes)
        //    {
        //        string DfId = DataflowNode.Attributes["Code"].Value;
        //        MSDataflow DfFounded = DFs.Find(df => df.IdDf.ToString() == DfId.Trim());
        //        if (DfFounded != null)
        //        {
        //            //Aggiungo il Categorisation
        //            if (this.ReferencesObject.Categorisation == null)
        //                this.ReferencesObject.Categorisation = new List<ICategorisationObject>();

        //            this.ReferencesObject.Categorisation.Add(csb.BuildCategorisation(DfFounded.DFCode, DfFounded.DFAgency, DfFounded.DFVersion, IdCategoryParents));
        //        }
        //    }

        //}

        /// <summary>
        /// Build Category Object (ICategoryMutableObject)
        /// </summary>
        /// <param name="CategoryObjectId">Category Code</param>
        /// <param name="names">Description Names</param>
        /// <returns></returns>
        //private ICategoryMutableObject BuildCategoryObject(string CategoryObjectId, List<SdmxObjectNameDescription> names)
        //{
        //    try
        //    {
        //        ICategoryMutableObject co = new CategoryMutableCore();
        //        co.Id = CategoryObjectId;
        //        foreach (SdmxObjectNameDescription item in names)
        //            co.AddName(item.Lingua, item.Name);


        //        return co;
        //    }
        //    catch (SdmxException) { throw; }
        //    catch (Exception ex)
        //    {
        //        throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateICategoryMutableObject, ex);
        //    }
        //}


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
