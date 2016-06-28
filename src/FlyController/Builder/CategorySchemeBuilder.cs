using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.BaseObjects.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.CategoryScheme;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Objects.CategoryScheme;
using Org.Sdmxsource.Sdmx.Util.Objects.Reference;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Xml;
using FlyController;
using FlyController.Model;
using FlyController.Builder;
using FlyController.Model.Error;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Base;

namespace FlyController.Builder
{
    /// <summary>
    /// CategorySchemeBuilder Create a ImmutableInstance of CategoryScheme
    /// </summary>
    public class CategorySchemeBuilder : ObjectBuilder
    {
        #region IObjectBuilder Property
        /// <summary>
        ///  Identificable Code
        /// </summary>
        public override string Code
        {
            get
            {
                return string.Format(FlyConfiguration.CategorySchemeFormat, FlyConfiguration.MainAgencyId);
            }
            set { }
        }

        private List<SdmxObjectNameDescription> names = FlyConfiguration.MainAgencyDescription;
        /// <summary>
        ///  Descriptions Names
        /// </summary>
        public override List<SdmxObjectNameDescription> Names { get { return names; } set { this.names = value; } }

        private string categorySchemeAgencyId = FlyConfiguration.MainAgencyId;
        /// <summary>
        ///  CategoryScheme AgencyId
        /// </summary>
        public string CategorySchemeAgencyId
        {
            get { return categorySchemeAgencyId; }
            set { categorySchemeAgencyId = value; }
        }
        private string categorySchemeVersion = FlyConfiguration.Version;
        /// <summary>
        ///  CategoryScheme Version
        /// </summary>
        public string CategorySchemeVersion
        {
            get { return categorySchemeVersion; }
            set { categorySchemeVersion = value; }
        }
        
        #endregion

        /// <summary>
        /// Inizialize new instance of CategorySchemeBuilder
        /// </summary>
        /// <param name="parsingObject">Parsing Object <see cref="ISdmxParsingObject"/></param>
        /// <param name="versionTypeResp">Sdmx Version</param>
        public CategorySchemeBuilder(ISdmxParsingObject parsingObject, SdmxSchemaEnumType versionTypeResp) :
            base(parsingObject, versionTypeResp)
        { }

        /// <summary>
        /// Create a ImmutableInstance of CategoryScheme
        /// </summary>
        /// <param name="CategoryObjects">List of CategoryObject</param>
        /// <returns>ICategorySchemeObject</returns>
        public ICategorySchemeObject BuildCategorySchemeObject(List<ICategoryMutableObject> CategoryObjects)
        {
            try
            {

                ICategorySchemeMutableObject ca = new CategorySchemeMutableCore();
                ca.AgencyId = this.CategorySchemeAgencyId;
                ca.Id = this.Code;
                ca.Version = this.categorySchemeVersion;
                foreach (SdmxObjectNameDescription item in this.Names)
                    ca.AddName(item.Lingua, item.Name);

                //IAnnotationMutableObject ann = new AnnotationMutableCore();
                //ann.Type = "CategoryScheme_node_order";
                //ann.Text.Add(new TextTypeWrapperMutableCore("en", "0"));
                //ca.AddAnnotation(ann);

                if (!this.ParsingObject.ReturnStub)
                {
                    foreach (ICategoryMutableObject item in CategoryObjects)
                        ca.AddItem(item);

                    ca.FinalStructure = TertiaryBool.ParseBoolean(true);
                }

                if (this.ParsingObject.isReferenceOf || this.ParsingObject.ReturnStub)
                {
                    ca.ExternalReference = TertiaryBool.ParseBoolean(true);
                    ca.StructureURL = RetreivalStructureUrl.Get(this, ca.Id, ca.AgencyId, ca.Version);
                }

                return ca.ImmutableInstance;

            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateImmutable, ex);
            }
        }

        /// <summary>
        ///  Create a ImmutableInstance of Categorisation
        /// </summary>
        /// <param name="DataflowCode">Dataflow Code</param>
        /// <param name="DataflowAgency">Dataflow Agency Id</param>
        /// <param name="DataflowVersion">Dataflow Version</param>
        /// <param name="CategoryParent">list of category parent, from hightLevel parent to low level child category</param>
        /// <returns>ICategorisationObject</returns>
        public ICategorisationObject BuildCategorisation(string DataflowCode,string DataflowAgency, string DataflowVersion, List<string> CategoryParent)
        {
            try
            {
                ICategorisationMutableObject ca = new CategorisationMutableCore();
                ca.AgencyId = this.CategorySchemeAgencyId;
                ca.Version = this.CategorySchemeVersion;
                string IDName=string.Format("{0}@{1}@{2}@{3}@{4}@{5}",
                    DataflowCode, DataflowAgency, DataflowVersion.Replace(".",""),
                this.Code,  this.CategorySchemeVersion.Replace(".",""), CategoryParent[CategoryParent.Count - 1]);
                ca.Id = IDName;
                ca.AddName("en", IDName);
                
                //IAnnotationMutableObject ann = new AnnotationMutableCore();
                //ann.Type="CategoryScheme_node_order";
                //ann.Text.Add(new TextTypeWrapperMutableCore("en", (CategoryParent.Count).ToString()));
                //ca.AddAnnotation(ann);
               
                //ca.Id = string.Format(FlyConfiguration.CategorisationFormat, DataflowCode);
                //foreach (SdmxObjectNameDescription item in this.Names)
                //    ca.AddName(item.Lingua, item.Name);

                if (!this.ParsingObject.ReturnStub)
                {
                    if (this.VersionTypeResp == SdmxSchemaEnumType.VersionTwo)
                        ca.CategoryReference = ReferenceBuilder.CreateCategoryReference(this.Code, this.CategorySchemeAgencyId, this.CategorySchemeVersion, CategoryParent.ToArray());
                    else
                        ca.CategoryReference = ReferenceBuilder.CreateCategoryReference(this.Code, this.CategorySchemeAgencyId, this.CategorySchemeVersion, CategoryParent != null && CategoryParent.Count > 0 ? new string[1] { CategoryParent[CategoryParent.Count - 1] } : CategoryParent.ToArray());
                    
                    ca.StructureReference = ReferenceBuilder.CreateDataflowReference(DataflowCode, DataflowAgency, DataflowVersion);
                }

                ca.FinalStructure = TertiaryBool.ParseBoolean(true);

                //if (this.ParsingObject.isReferenceOf || this.ParsingObject.ReturnStub)
                //{
                //    ca.ExternalReference = TertiaryBool.ParseBoolean(true);
                //    ca.StructureURL = RetreivalStructureUrl.Get(this, ca.Id, ca.AgencyId, ca.Version);
                //}

                return ca.ImmutableInstance;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateImmutable, ex);
            }
        }

    }
}
