using FlyController;
using FlyController.Builder;
using FlyController.Model;
using FlyController.Model.DbSetting;
using FlyController.Model.Error;
using FlyEngine.Model;
using FlyMapping.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyDotStatExtra_implementation.Manager.Metadata
{
    /// <summary>
    /// ConceptSchemeManager retrieves the data for build  ConceptScheme
    /// </summary>
    public class ConceptSchemeManager : BaseManager, IConceptSchemeManager
    {
        /// <summary>
        /// Inizialize for BaseManager Class
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        public ConceptSchemeManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
            : base(_parsingObject, _versionTypeResp) { }


        /// <summary>
        /// ConceptScheme Names Description
        /// </summary>
        public List<SdmxObjectNameDescription> ConceptSchemeNames { get; set; }

        /// <summary>
        /// Referenced objects 
        /// </summary>
        public override IReferencesObject ReferencesObject { get; set; }

        /// <summary>
        /// Build a ConceptSchemes
        /// </summary>
        /// <returns>list of IConceptSchemeObject for SdmxObject</returns>
        public List<IConceptSchemeObject> GetConceptSchemes()
        {
            try
            {
                if (ReferencesObject == null)
                    ReferencesObject = new IReferencesObject();

                this.ReferencesObject.ConceptSchemes = new List<IConceptSchemeObject>();
                this.ReferencesObject.Concepts = new Dictionary<string, List<IConceptObjectImpl>>();

               
                //Cerco tutti i DataFlow Per Capire se esiste
                IDataflowsManager gdf = new MetadataFactory().InstanceDataflowsManager((ISdmxParsingObject)this.parsingObject.Clone(), this.versionTypeResp);
                gdf.parsingObject.MaintainableId = null;

                ReferencesObject.FoundedDataflows = gdf.GetDataFlows();
                if (!string.IsNullOrEmpty(this.parsingObject.MaintainableId))
                {
                    FlyNameArtefactSettings fnas = new FlyNameArtefactSettings(this.parsingObject);
                    string DataflowCode = fnas.GetDataFlowCodeFromConceptSchema();
                    if (ReferencesObject.FoundedDataflows.Exists(d => d.Id.Trim().ToUpper() == DataflowCode.Trim().ToUpper()))
                        ReferencesObject.FoundedDataflows = new List<IDataflowObject>() { ReferencesObject.FoundedDataflows.Find(d => d.Id.Trim().ToUpper() == DataflowCode.Trim().ToUpper()) };
                }

                foreach (var referenceDF in ReferencesObject.FoundedDataflows)
                    BuildConcepts(referenceDF.Id);

                return ReferencesObject.ConceptSchemes;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetConceptsScheme, ex);
            }
        }


        /// <summary>
        /// Populate a Conceptscheme property of SDMXObjectBuilder for insert this in DataStructure response
        /// whitout calling all dataflows
        /// </summary>
        public void BuildConcepts(string DataflowRealCode)
        {
            List<IConceptObjectImpl> ListConcepts = GetConceptList(DataflowRealCode);
            BuildConcepts(DataflowRealCode, ListConcepts);
        }

        /// <summary>
        /// Populate a Conceptscheme property of SDMXObjectBuilder for insert this in DataStructure response
        /// whitout calling all dataflows and whitout calling a list of concept
        /// </summary>
        public void BuildConcepts(string DataflowRealCode, List<IConceptObjectImpl> ListConcepts)
        {
            ConceptBuilder _conceptBuilder = new ConceptBuilder(this.ConceptSchemeNames, this.parsingObject, this.versionTypeResp);
            _conceptBuilder.Code = string.Format(FlyConfiguration.ConceptSchemeFormat, DataflowRealCode);
            _conceptBuilder.VersionTypeResp = this.versionTypeResp;

            if (!this.parsingObject.ReturnStub)
                _conceptBuilder.Concepts = ListConcepts;

            IConceptSchemeObject conceptScheme=_conceptBuilder.BuildConceptScheme();
            this.ReferencesObject.ConceptSchemes.Add(conceptScheme);
            this.ReferencesObject.Concepts.Add(conceptScheme.Id, ListConcepts);
        }

        /// <summary>
        /// Create a list of Concept object in Dataflow
        /// </summary>
        /// <param name="_dataflowCode">Dataflow Code</param>
        /// <returns>list of ConceptObjectImpl</returns>
        public List<IConceptObjectImpl> GetConceptList(string _dataflowCode)
        {
            try
            {
                List<IConceptObjectImpl> _concepts = new List<IConceptObjectImpl>();
                IDimensionManager _DimensionManager = new DimensionManager(this.parsingObject, this.versionTypeResp);

                List<SdmxObjectNameDescription> _names;
                List<IDimensionConcept> _dimensions = _DimensionManager.GetDimensionConceptObjects(_dataflowCode, out _names);
                this.ConceptSchemeNames = _names;

                if (this.versionTypeResp == SdmxSchemaEnumType.VersionTwoPointOne)
                {//Modifico il nome del Concept Time
                    IDimensionConcept timedim = _dimensions.Find(x => x.DimensionType == DimensionTypeEnum.Time);
                    timedim.RealNameTime = timedim.ConceptObjectCode;
                    timedim.ConceptObjectCode = "TIME_PERIOD";
                    timedim.Id = "TIME_PERIOD";
                }

                if (_dimensions != null && _dimensions.Count > 0)
                    _concepts.AddRange(_dimensions);

                IAttributeManager _AttributeManager = new AttributeManagerSP(this.parsingObject, this.versionTypeResp);
                IFLAGManager _FlagManager = new FlyDotStat_implementation.Manager.Metadata.FLAGManager(this.parsingObject, this.versionTypeResp);

                IConceptObjectImpl _flag = _FlagManager.GetFlag();
                if (_flag != null)
                    _concepts.Add(_flag);

                List<IAttributeConcept> _attributes = _AttributeManager.GetAttribute(_dataflowCode);
                if (_attributes != null && _attributes.Count > 0)
                    _concepts.AddRange(_attributes);

                _concepts.Add(_AttributeManager.GetObsValue());

                return _concepts;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetConceptsScheme, ex);
            }
        }


        /// <summary>
        /// Build a ConceptSchemes
        /// </summary>
        /// <param name="refObj">Referenced Objects</param>
        /// <returns>list of IConceptSchemeObject for SdmxObject</returns>
        public List<IConceptSchemeObject> GetConceptSchemesReferences(IReferencesObject refObj)
        {
            try
            {
               
                this.ReferencesObject.ConceptSchemes = new List<IConceptSchemeObject>();
                this.ReferencesObject.Concepts = new Dictionary<string, List<IConceptObjectImpl>>();
                 if (refObj.FoundedDataflows == null)
                return this.ReferencesObject.ConceptSchemes;

                 if (refObj.Concepts == null)
                 {
                     foreach (var referenceDF in refObj.FoundedDataflows)
                         BuildConcepts(referenceDF.Id);
                 }
                 else
                 {
                     foreach (var referenceDF in refObj.FoundedDataflows)
                     {
                         string cs = string.Format(FlyConfiguration.ConceptSchemeFormat, referenceDF.Id);
                         if (refObj.Concepts.ContainsKey(cs))
                             BuildConcepts(referenceDF.Id, refObj.Concepts[cs]);
                         else
                             BuildConcepts(referenceDF.Id);
                     }
                 }
               
                return ReferencesObject.ConceptSchemes;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetConceptsScheme, ex);
            }
        }


    }
}
