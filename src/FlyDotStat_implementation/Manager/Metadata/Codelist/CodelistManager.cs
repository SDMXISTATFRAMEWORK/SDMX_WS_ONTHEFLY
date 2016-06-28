using FlyController;
using FlyController.Builder;
using FlyController.Model;
using FlyController.Model.Error;
using FlyEngine.Model;
using FlyMapping.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
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
    public class CodelistManager : BaseManager, ICodelistManager
    {
        /// <summary>
        /// Inizialize for BaseManager Class
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        public CodelistManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
            : base(_parsingObject, _versionTypeResp) { }


        /// <summary>
        /// Referenced objects 
        /// </summary>
        public override IReferencesObject ReferencesObject { get; set; }

        /// <summary>
        /// retrieves the codelist Contrain  from database
        /// </summary>
        /// <returns>list of Mutable Code Object</returns>
        public List<ICodelistMutableObject> GetCodelistConstrain()
        {
            if (ReferencesObject == null)
                ReferencesObject = new IReferencesObject();

            if (ReferencesObject.FoundedDataflows == null || ReferencesObject.FoundedDataflows.Count == 0)
            {
                IDataflowsManager gdf = new MetadataFactory().InstanceDataflowsManager((ISdmxParsingObject)this.parsingObject.Clone(), this.versionTypeResp);
                gdf.parsingObject.MaintainableId = null;
                ReferencesObject.FoundedDataflows = gdf.GetDataFlows();
            }

            if (ReferencesObject.FoundedDataflows == null || ReferencesObject.FoundedDataflows.Count == 0)
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DataflowNotFound, new Exception(this.parsingObject.ConstrainDataFlow));

            if (ReferencesObject.Concepts == null)
                ReferencesObject.Concepts = new Dictionary<string, List<IConceptObjectImpl>>();

            IConceptObjectImpl Concept = null;
            SpecialTypeEnum specType;
            if (!string.IsNullOrEmpty(this.parsingObject.MaintainableId) &&
                Enum.TryParse<SpecialTypeEnum>(this.parsingObject.MaintainableId.Trim().ToUpper(), out specType) &&
                this.parsingObject.AgencyId == "MA")
            {
                if (!string.IsNullOrEmpty(this.parsingObject.ConstrainConcept))
                    Concept = new SpecialConcept(this.parsingObject.ConstrainConcept, specType);
                else
                    Concept = new SpecialConcept(specType.ToString(), specType);

            }
            else
            {
                List<IConceptObjectImpl> concepts = new ConceptSchemeManager(this.parsingObject, this.versionTypeResp).GetConceptList(this.parsingObject.ConstrainDataFlow);
                ReferencesObject.Concepts.Add(string.Format(FlyConfiguration.ConceptSchemeFormat, this.parsingObject.ConstrainDataFlow), concepts);
                Concept = concepts.Find(c => c.ConceptObjectCode.Trim().ToLower() == this.parsingObject.ConstrainConcept.Trim().ToLower());
                if (Concept == null)
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CodelistInvalid, new Exception(string.Format("Concept {0} Not Found in Dataflow {1}", this.parsingObject.ConstrainConcept, this.parsingObject.ConstrainDataFlow)));

                if (this.parsingObject.ContrainConceptREF != null && this.parsingObject.ContrainConceptREF.Keys.Count > 0)
                {
                    ISpecialConcept sc = new SpecialConcept(Concept.Id, SpecialTypeEnum.CL_CONTRAINED);
                    sc.SetNames(Concept.ConceptObjectNames);
                    sc.TimeDimensionRef = concepts.Find(c => c.ConceptType == ConceptTypeEnum.Dimension && ((IDimensionConcept)c).DimensionType == DimensionTypeEnum.Time);
                    sc.ContrainConceptREF = this.parsingObject.ContrainConceptREF;
                    Concept = sc;
                }
            }

            BuildCodelist(this.parsingObject.ConstrainDataFlow, Concept);
            return ReferencesObject.Codelists;
        }


        /// <summary>
        /// retrieves the codelist Contrain from database
        /// </summary>
        /// <returns>list of Mutable Code Object</returns>
        public List<ICodelistMutableObject> GetCodelistNoConstrain()
        {
            if (ReferencesObject == null)
                ReferencesObject = new IReferencesObject();


            if (ReferencesObject.FoundedDataflows == null || ReferencesObject.FoundedDataflows.Count == 0)
            {
                IDataflowsManager gdf = new MetadataFactory().InstanceDataflowsManager((ISdmxParsingObject)this.parsingObject.Clone(), this.versionTypeResp);
                gdf.parsingObject.MaintainableId = null;
                ReferencesObject.FoundedDataflows = gdf.GetDataFlows();
            }

            if (ReferencesObject.FoundedDataflows == null || ReferencesObject.FoundedDataflows.Count == 0)
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DataflowNotFound, new Exception(this.parsingObject.ConstrainDataFlow));

            if(ReferencesObject.Concepts==null)
                ReferencesObject.Concepts = new Dictionary<string, List<IConceptObjectImpl>>();
            foreach (var df in ReferencesObject.FoundedDataflows)
            {

                List<IConceptObjectImpl> concepts = new ConceptSchemeManager(this.parsingObject, this.versionTypeResp).GetConceptList(df.Id);
                ReferencesObject.Concepts.Add(string.Format(FlyConfiguration.ConceptSchemeFormat, df.Id), concepts);
                if (!string.IsNullOrEmpty(this.parsingObject.MaintainableId))
                {
                    FlyNameArtefactSettings fnas = new FlyNameArtefactSettings(this.parsingObject);
                    string ConceptCode = fnas.GetConceptCodeFromCodelist();
                    IConceptObjectImpl Concept = concepts.Find(c => c.ConceptObjectCode.Trim().ToLower() == ConceptCode.Trim().ToLower());
                    if (Concept == null)
                        continue;

                    BuildCodelist(null, Concept);
                    break;
                }
                else
                {
                    foreach (var Concept in concepts)
                        BuildCodelist(null, Concept);
                }
            }
            return ReferencesObject.Codelists;

        }


        /// <summary>
        /// Populate a list of Codelist property of SDMXObjectBuilder for insert this in DataStructure response
        /// Whitout Call all Dataflows
        /// </summary>
        public void BuildCodelist(string DataFlowCode)
        {

            List<IConceptObjectImpl> concepts = new ConceptSchemeManager(this.parsingObject, this.versionTypeResp).GetConceptList(DataFlowCode);
            foreach (IConceptObjectImpl concept in concepts)
                BuildCodelist(DataFlowCode, concept);
        }

        /// <summary>
        /// Populate a list of Codelist property of SDMXObjectBuilder for insert this in DataStructure response
        /// Whitout Call all Dataflows and Concepts
        /// </summary>
        public void BuildCodelist(string DataFlowCode, IConceptObjectImpl Concept)
        {

            if (ReferencesObject == null)
                ReferencesObject = new IReferencesObject();

          
            if (Concept.ConceptType == ConceptTypeEnum.Dimension && ((IDimensionConcept)Concept).DimensionType == DimensionTypeEnum.Time)
                return;
            if (Concept.ConceptType == ConceptTypeEnum.Attribute && ((IAttributeConcept)Concept).IsValueAttribute)
                return;
            CodelistBuilder _CodelistBuilder = new CodelistBuilder(this.parsingObject, this.versionTypeResp);
            _CodelistBuilder.Code = string.Format(FlyConfiguration.CodelistFormat, Concept.ConceptObjectCode);
            _CodelistBuilder.Names = Concept.ConceptObjectNames;

            if (!this.parsingObject.ReturnStub)
                _CodelistBuilder.CodesObjects = GetCodelist(DataFlowCode, Concept);
            if (this.ReferencesObject.Codelists == null) this.ReferencesObject.Codelists = new List<ICodelistMutableObject>();

            if (this.ReferencesObject.Codelists.Exists(cl => cl.Id == _CodelistBuilder.Code))
                _CodelistBuilder.AddItemto(this.ReferencesObject.Codelists.Find(cl => cl.Id == _CodelistBuilder.Code));
            else
            {
                if (Concept==null || Concept.ConceptType!=ConceptTypeEnum.Special)
                    this.ReferencesObject.Codelists.Add(_CodelistBuilder.BuildCodelist(FlyConfiguration.MainAgencyId, FlyConfiguration.Version));
                else
                    this.ReferencesObject.Codelists.Add(_CodelistBuilder.BuildCodelist(this.parsingObject.AgencyId, this.parsingObject._version));
            }
        }

        /// <summary>
        /// Create a list of Code that compose a Codelist
        /// </summary>
        /// <param name="_dataFlowCode">Dataflow Code</param>
        /// <param name="_concept">Concept Object</param>
        /// <returns>List of ICodeMutableObject</returns>
        public List<ICodeMutableObject> GetCodelist(string _dataFlowCode, IConceptObjectImpl _concept)
        {
            try
            {
                DimensionCodelistsManager cm = new DimensionCodelistsManager(this.parsingObject, this.versionTypeResp);
                AttributeCodelistsManager at = new AttributeCodelistsManager(this.parsingObject, this.versionTypeResp);
                FLAGCodelistManager fl = new FLAGCodelistManager(this.parsingObject, this.versionTypeResp);
                SpecialCodelistsManager sp = new SpecialCodelistsManager(this.parsingObject, this.versionTypeResp);

                switch (_concept.ConceptType)
                {
                    case ConceptTypeEnum.Dimension:
                        if (string.IsNullOrEmpty(_dataFlowCode))
                            return cm.GetDimensionCodelistNoContrain(_concept as IDimensionConcept);
                        else
                            return cm.GetDimensionCodelistContrain(_dataFlowCode, _concept as IDimensionConcept);
                    case ConceptTypeEnum.Attribute:
                        if (((IAttributeConcept)_concept).IsFlagAttribute)
                            return fl.GetFlagCodelist(_dataFlowCode, _concept as IAttributeConcept);
                        else if (!((IAttributeConcept)_concept).IsValueAttribute)
                        {
                            if (string.IsNullOrEmpty(_dataFlowCode))
                                return at.GetAttributeCodelistNoConstrain(_concept as IAttributeConcept);
                            else
                                return at.GetAttributeCodelistConstrain(_dataFlowCode, _concept as IAttributeConcept);
                        }
                        break;
                    case ConceptTypeEnum.Special:
                        return sp.GetSpecialCodelist(_dataFlowCode, _concept as ISpecialConcept);
                }
                return null;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetCodelist, ex);
            }
        }

        /// <summary>
        /// retrieves the codelist Contrain from database
        /// </summary>
        /// <param name="refObj">Referenced Objects</param>
        /// <returns>list of Mutable Code Object</returns>
        public List<ICodelistMutableObject> GetCodelistReferences(IReferencesObject refObj)
        {

            if (refObj.FoundedDataflows == null)
                return new List<ICodelistMutableObject>();

            if (refObj.Concepts != null)
            {
                foreach (var df in refObj.FoundedDataflows)
                {
                    List<IConceptObjectImpl> concepts = new ConceptSchemeManager(this.parsingObject, this.versionTypeResp).GetConceptList(df.Id);
                    foreach (var Concept in concepts)
                        BuildCodelist(df.Id, Concept);
                }
            }
            else
            {
                foreach (var df in refObj.FoundedDataflows)
                {
                    string cs = string.Format(FlyConfiguration.ConceptSchemeFormat, df.Id);
                    if (refObj.Concepts.ContainsKey(cs))
                        foreach (var Concept in refObj.Concepts[cs])
                            BuildCodelist(df.Id, Concept);
                    else
                        BuildCodelist(df.Id);
                    foreach (List<IConceptObjectImpl> Concepts in refObj.Concepts.Values)
                        foreach (var Concept in Concepts)
                            BuildCodelist(df.Id, Concept);
                }
            }

            return ReferencesObject.Codelists;
        }

    }
}
