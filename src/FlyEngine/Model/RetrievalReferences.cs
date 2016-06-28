using FlyController;
using FlyController.Builder;
using FlyController.Model;
using FlyController.Model.Error;
using FlyEngine.Engine.Metadata;
using FlyEngine.Model;
using FlyMapping.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyEngine.Model
{
    /// <summary>
    /// Determines which and how many are the references which we must return
    /// </summary>
    public class RetrievalReferences : IRetrievalReferences
    {
        /// <summary>
        /// <see cref="ISDMXObjectBuilder"/> where to add a reference Metadata
        /// </summary>
        public ISDMXObjectBuilder _SDMXObjectBuilder { get; set; }
        private MetadataReferences.ReferenceTreeEnum structType { get; set; }
        private ISdmxParsingObject ParsingObject { get; set; }

        /// <summary>
        /// Initialize a new instance of <see cref="RetrievalReferences"/>
        /// </summary>
        /// <param name="_sdmxObjectBuilder">the <see cref="ISDMXObjectBuilder"/></param>
        public RetrievalReferences(ISDMXObjectBuilder _sdmxObjectBuilder)
        {
            this._SDMXObjectBuilder = _sdmxObjectBuilder;
            this.ParsingObject = this._SDMXObjectBuilder.ParsingObject.CloneForReferences();
            this.ReferencesObject = new IReferencesObject();
        }

        /// <summary>
        /// adds a reference from a structure
        /// </summary>
        /// <param name="_structType">starting structure</param>
        public void AddReferences(MetadataReferences.ReferenceTreeEnum _structType)
        {
            try
            {
                structType = _structType;
                List<MetadataReferences.ReferenceTreeEnum> references = new List<MetadataReferences.ReferenceTreeEnum>();

                if (this.ParsingObject.ResolveReferenceSdmx20)
                    references = MetadataReferences.ResolveReferences(structType);
                else if (this.ParsingObject.References != StructureReferenceDetailEnumType.None && this.ParsingObject.References != StructureReferenceDetailEnumType.Null)
                    references = MetadataReferences.GetReferences(structType, this.ParsingObject.References, this.ParsingObject.SpecificReference);

                if (this._SDMXObjectBuilder.VersionTypeResp == Org.Sdmxsource.Sdmx.Api.Constants.SdmxSchemaEnumType.VersionTwo)
                {
                    if (structType == MetadataReferences.ReferenceTreeEnum.CategoryScheme || structType == MetadataReferences.ReferenceTreeEnum.Dataflow)
                    {//Nella versione 2.0 non ha senso category senza categorisation
                        if (!references.Contains(MetadataReferences.ReferenceTreeEnum.Categorisation))
                            references.Add(MetadataReferences.ReferenceTreeEnum.Categorisation);
                    }
                }

                foreach (MetadataReferences.ReferenceTreeEnum _ref in references)
                {
                    switch (_ref)
                    {
                        case MetadataReferences.ReferenceTreeEnum.Categorisation:
                            AddCategorisation();
                            break;
                        case MetadataReferences.ReferenceTreeEnum.CategoryScheme:
                            AddCategoryScheme();
                            break;
                        case MetadataReferences.ReferenceTreeEnum.Dataflow:
                            AddDataflow();
                            break;
                        case MetadataReferences.ReferenceTreeEnum.Dsd:
                            AddDataStructure();
                            break;
                        case MetadataReferences.ReferenceTreeEnum.Concept:
                            AddConceptScheme();
                            break;
                        case MetadataReferences.ReferenceTreeEnum.Codelist:
                            AddCodelist();
                            break;
                    }
                }
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.AddReferences, ex);
            }
        }


        /// <summary>
        /// Referenced objects 
        /// </summary>
        public IReferencesObject ReferencesObject { get; set; }

        //FIX ME

        /// <summary>
        /// Add Categorisation References
        /// </summary>
        public void AddCategorisation()
        {
            if ((this._SDMXObjectBuilder._CategorisationObject == null || this._SDMXObjectBuilder._CategorisationObject.Count == 0) && structType != MetadataReferences.ReferenceTreeEnum.Categorisation)
            {

                if (ReferencesObject.Categorisation != null)
                    this._SDMXObjectBuilder._CategorisationObject = ReferencesObject.Categorisation;
                else
                {
                    var CatManager = MappingConfiguration.MetadataFactory.InstanceCategoriesManager(this.ParsingObject.CloneForReferences(), this._SDMXObjectBuilder.VersionTypeResp);
                    CatManager.ReferencesObject = this.ReferencesObject;
                    this._SDMXObjectBuilder._CategorisationObject = CatManager.GetCategorisationReferences(this.ReferencesObject);
                }
            }
        }

        /// <summary>
        /// Add CategoryScheme References
        /// </summary>
        public void AddCategoryScheme()
        {
            if (this._SDMXObjectBuilder._CategorySchemeObject == null && structType != MetadataReferences.ReferenceTreeEnum.CategoryScheme)
            {

                if (ReferencesObject.CategoryScheme != null)
                    this._SDMXObjectBuilder._CategorySchemeObject = ReferencesObject.CategoryScheme;
                else
                {
                    var CatManager = MappingConfiguration.MetadataFactory.InstanceCategoriesManager(this.ParsingObject.CloneForReferences(), this._SDMXObjectBuilder.VersionTypeResp);
                    CatManager.ReferencesObject = this.ReferencesObject;
                    this._SDMXObjectBuilder._CategorySchemeObject = CatManager.GetCategorySchemeReferences(this.ReferencesObject);
                }
            }
        }

        /// <summary>
        /// Add Dataflows References
        /// </summary>
        public void AddDataflow()
        {
            if (this._SDMXObjectBuilder._Dataflows == null || this._SDMXObjectBuilder._Dataflows.Count == 0)
            {
                if (ReferencesObject.FoundedDataflows != null)
                    this._SDMXObjectBuilder._Dataflows = ReferencesObject.FoundedDataflows;
                else
                {
                    var dataflowManager = MappingConfiguration.MetadataFactory.InstanceDataflowsManager(this.ParsingObject.CloneForReferences(), this._SDMXObjectBuilder.VersionTypeResp);
                    dataflowManager.ReferencesObject = this.ReferencesObject;
                    this._SDMXObjectBuilder._Dataflows = dataflowManager.GetDataFlowsReferences(this.ReferencesObject);
                }
            }
        }

        /// <summary>
        /// Add DataStructure References
        /// </summary>
        public void AddDataStructure()
        {
            if (this._SDMXObjectBuilder._KeyFamily == null || this._SDMXObjectBuilder._KeyFamily.Count == 0)
            {

                if (ReferencesObject.DSDs != null)
                    this._SDMXObjectBuilder._KeyFamily = ReferencesObject.DSDs;
                else
                {
                    var dsdManager = MappingConfiguration.MetadataFactory.InstanceDsdManager(this.ParsingObject.CloneForReferences(), this._SDMXObjectBuilder.VersionTypeResp);
                    dsdManager.ReferencesObject = this.ReferencesObject;
                    this._SDMXObjectBuilder._KeyFamily = dsdManager.GetDSDsReferences(this.ReferencesObject);
                }
            }
        }

        /// <summary>
        /// Add ConceptScheme References
        /// </summary>
        public void AddConceptScheme()
        {
            if (this._SDMXObjectBuilder._Conceptscheme == null || this._SDMXObjectBuilder._Conceptscheme.Count == 0)
            {

                if (ReferencesObject.ConceptSchemes != null)
                    this._SDMXObjectBuilder._Conceptscheme = ReferencesObject.ConceptSchemes;
                else
                {
                    var conceptSchemeManager = MappingConfiguration.MetadataFactory.InstanceConceptSchemeManager(this.ParsingObject.CloneForReferences(), this._SDMXObjectBuilder.VersionTypeResp);
                    conceptSchemeManager.ReferencesObject = this.ReferencesObject;
                    this._SDMXObjectBuilder._Conceptscheme = conceptSchemeManager.GetConceptSchemesReferences(this.ReferencesObject);
                }
            }
        }

        /// <summary>
        /// Add Codelist References
        /// </summary>
        public void AddCodelist()
        {
            if (this._SDMXObjectBuilder._Codelists == null || this._SDMXObjectBuilder._Codelists.Count == 0)
            {

                if (ReferencesObject.Codelists != null)
                    this._SDMXObjectBuilder._Codelists = ReferencesObject.Codelists;
                else
                {
                    var codelistManager = MappingConfiguration.MetadataFactory.InstanceCodelistsManager(this.ParsingObject.CloneForReferences(), this._SDMXObjectBuilder.VersionTypeResp);
                    codelistManager.ReferencesObject = this.ReferencesObject;
                    this._SDMXObjectBuilder._Codelists = codelistManager.GetCodelistReferences(this.ReferencesObject);
                }
            }
        }

        #region Private Function
        private List<ICodelistMutableObject> MergeCodelist(List<ICodelistMutableObject> cl1, List<ICodelistMutableObject> cl2)
        {
            if (cl2 == null || cl2.Count == 0)
                return cl1;
            List<ICodelistMutableObject> basecl = cl1;
            foreach (var cod in cl2)
            {
                ICodelistMutableObject presenteCodelist = basecl.Find(c => cod.Id == c.Id);
                if (presenteCodelist == null)
                {
                    basecl.Add(cod);
                    continue;
                }
                foreach (ICodeMutableObject obj in cod.Items)
                {
                    if (presenteCodelist.Items.Count(o => o.Id == obj.Id) == 0)
                        presenteCodelist.Items.Add(obj);
                }
            }
            return basecl;
        }

        /// <summary>
        /// Get DataFlow Code From KeyFamily (Using DsdFormat specified in File Config)
        /// </summary>
        /// <returns>DataFlow Code</returns>
        internal string GetDataFlowCodeFromKeyFamily(string DataflowRealCode)
        {
            try
            {
                string DataflowId = DataflowRealCode;
                string dsdForm = FlyConfiguration.DsdFormat.Replace("{0}", "").ToUpper();
                if (DataflowRealCode.EndsWith(dsdForm) || DataflowRealCode.StartsWith(dsdForm))
                {

                    if (DataflowRealCode.EndsWith(dsdForm))
                        DataflowId = DataflowRealCode.Trim().Substring(0, DataflowRealCode.Trim().Length - dsdForm.Length);
                    else
                        DataflowId = DataflowRealCode.Trim().Substring(dsdForm.Length);

                    if (string.IsNullOrEmpty(DataflowId))
                        throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.KeyFamilyInvalid, new Exception(DataflowRealCode));
                }
                return DataflowId;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetDataFlowCodeFromKeyFamily, ex);
            }
        }
        #endregion

    }
}
