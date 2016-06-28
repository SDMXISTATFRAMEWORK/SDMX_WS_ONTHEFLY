using FlyController;
using FlyController.Builder;
using FlyController.Model;
using FlyController.Model.Error;
using FlyEngine.Model;
using FlyMapping.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.DataStructure;
using Org.Sdmxsource.Sdmx.Util.Objects.Reference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyDotStat_implementation.Manager.Metadata
{
    /// <summary>
    /// DsdManager retrieves the data for build  KeyFamilies (sdmx v 2.0) or DataStructures (sdmx v 2.1)
    /// </summary>
    public class DsdManager : BaseManager, IDsdManager
    {
        /// <summary>
        /// Inizialize for BaseManager Class
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        public DsdManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
            : base(_parsingObject, _versionTypeResp) { }


        /// <summary>
        /// Referenced objects 
        /// </summary>
        public override IReferencesObject ReferencesObject { get; set; }

        /// <summary>
        /// retrieves the DSD from database
        /// </summary>
        /// <returns>list of DataStructure for SDMXObject</returns>
        public List<DataStructureObjectImpl> GetDSDs()
        {
            try
            {
                if (ReferencesObject == null)
                    ReferencesObject = new IReferencesObject();

                if (ReferencesObject.DSDs != null)
                    return ReferencesObject.DSDs;

                if (ReferencesObject.FoundedDataflows == null || ReferencesObject.FoundedDataflows.Count == 0)
                {
                    IDataflowsManager gdf = new MetadataFactory().InstanceDataflowsManager((ISdmxParsingObject)this.parsingObject.Clone(), this.versionTypeResp);
                    gdf.parsingObject.MaintainableId = null;
                    ReferencesObject.FoundedDataflows = gdf.GetDataFlows();
                }

                if (ReferencesObject.FoundedDataflows == null || ReferencesObject.FoundedDataflows.Count == 0)
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DataflowNotFound, new Exception(this.parsingObject.ConstrainDataFlow));



                if (!string.IsNullOrEmpty(this.parsingObject.MaintainableId))
                {
                    FlyNameArtefactSettings fnas = new FlyNameArtefactSettings(this.parsingObject);
                    string DataFlowCode = fnas.GetDataFlowCodeFromKeyFamily();
                    //Controllo se esiste il Dataflow
                    ReferencesObject.FoundedDataflows = new List<IDataflowObject>() { ReferencesObject.FoundedDataflows.Find(df => df.Id.Trim().ToUpper() == DataFlowCode.Trim().ToUpper()) };
                }

                if (ReferencesObject.FoundedDataflows == null)
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DataflowNotFound);

                ReferencesObject.DSDs = new List<DataStructureObjectImpl>();
                if (ReferencesObject.Codelists == null)
                    ReferencesObject.Codelists = new List<ICodelistMutableObject>();
                if (ReferencesObject.Concepts == null)
                    ReferencesObject.Concepts = new Dictionary<string, List<IConceptObjectImpl>>();
                foreach (var df in ReferencesObject.FoundedDataflows)
                    ReferencesObject.DSDs.Add(BuildDSD(df));
                return ReferencesObject.DSDs;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.BuildDSD, ex);
            }
        }



        /// <summary>
        /// Populate DataStructure (KeyFamyly for SDMX 2.0, Structure for SDMX 2.1)
        /// property of SDMXObjectBuilder for insert this elements in DataStructure response
        /// </summary>
        public DataStructureObjectImpl BuildDSD(IDataflowObject _foundedDataflow)
        {
            try
            {
                if (!this.parsingObject.ReturnStub)
                {
                    if (!ReferencesObject.Concepts.ContainsKey(string.Format(FlyConfiguration.ConceptSchemeFormat, _foundedDataflow.Id)))
                    {
                        ConceptSchemeManager gdf = new ConceptSchemeManager((ISdmxParsingObject)this.parsingObject.Clone(), this.versionTypeResp);
                        ReferencesObject.Concepts.Add(string.Format(FlyConfiguration.ConceptSchemeFormat, _foundedDataflow.Id), gdf.GetConceptList(_foundedDataflow.Id));
                    }

                    //CodelistManager cm = new CodelistManager((ISdmxParsingObject)this.parsingObject.Clone(), this.versionTypeResp);
                    //cm.parsingObject.QueryDetail = StructureQueryDetailEnumType.AllStubs;
                    //cm.BuildCodelist(_foundedDataflow.Id);
                    //if (ReferencesObject.Codelists == null) ReferencesObject.Codelists = new List<ICodelistMutableObject>();
                    //ReferencesObject.Codelists.AddRange(cm.ReferencesObject.Codelists);
                }


                return BuildDataStructure(_foundedDataflow.Id);
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.BuildDSD, ex);
            }
        }




        private DataStructureObjectImpl BuildDataStructure(string _foundedDataflowId)
        {
            try
            {
                string ConceptSchemeId = string.Format(FlyConfiguration.ConceptSchemeFormat, _foundedDataflowId);
                List<IComponentMutableObject> components = new List<IComponentMutableObject>();

                if (!this.ReferencesObject.Concepts.ContainsKey(ConceptSchemeId))
                {
                    ConceptSchemeManager gdf = new ConceptSchemeManager((ISdmxParsingObject)this.parsingObject.Clone(), this.versionTypeResp);
                    ReferencesObject.Concepts.Add(string.Format(FlyConfiguration.ConceptSchemeFormat, _foundedDataflowId), gdf.GetConceptList(_foundedDataflowId));
                }
                if (!this.ReferencesObject.Concepts.ContainsKey(ConceptSchemeId))
                    throw new Exception("ConceptScheme not found");


                List<IConceptObjectImpl> conceptsObject = this.ReferencesObject.Concepts[ConceptSchemeId];
                if (!this.parsingObject.ReturnStub)
                {

                    foreach (IConceptObjectImpl _concept in conceptsObject)
                    {
                        //Cerco la giusta Codelist
                        ICodelistMutableObject _CodelistAssociata = null;
                        if (this.ReferencesObject.Codelists!=null)
                           _CodelistAssociata=  this.ReferencesObject.Codelists.Find(cl => cl.Id.ToUpper() == string.Format(FlyConfiguration.CodelistFormat, _concept.Id));
                        // CodelistBuilder _CodelistAssociata = this._Codelists.Find(cl => cl.Concept.ConceptObjectCode == _concept.ConceptObjectCode);
                        //Capire se è un attribute/Dimension
                        if (_CodelistAssociata == null)
                        {
                            CodelistManager cm = new CodelistManager((ISdmxParsingObject)this.parsingObject.Clone(), this.versionTypeResp);
                            cm.parsingObject.MaintainableId = string.Format(FlyConfiguration.CodelistFormat, _concept.Id);
                            cm.parsingObject.QueryDetail = StructureQueryDetailEnumType.AllStubs;
                            cm.BuildCodelist(_foundedDataflowId, _concept);
                            if (cm.ReferencesObject.Codelists != null && cm.ReferencesObject.Codelists.Count > 0)
                                _CodelistAssociata = cm.ReferencesObject.Codelists[0];
                           
                        }
                        if (_CodelistAssociata != null)
                        {
                            _concept.ConceptDSDInfo = new ConceptDSDInfoObject()
                            {
                                CodelistId = _CodelistAssociata.Id,
                                CodelistAgency = _CodelistAssociata.AgencyId,
                                CodelistVersion = _CodelistAssociata.Version
                            };
                        }

                       

                        switch (_concept.ConceptType)
                        {
                            case ConceptTypeEnum.Dimension:
                                //Se è una Dimension Capire che tipologia di Dimension è Frequency/Time
                                switch (((IDimensionConcept)_concept).DimensionType)
                                {
                                    case DimensionTypeEnum.Dimension:
                                        DimensionMutableCore dim = new DimensionMutableCore();
                                        dim.ConceptRef = ReferenceBuilder.CreateConceptReference(ConceptSchemeId, _concept.Id);
                                        dim.Id = _concept.Id;
                                        if (_CodelistAssociata != null)
                                        {
                                            dim.Representation = new RepresentationMutableCore()
                                            {
                                                Representation = ReferenceBuilder.CreateCodelistReference(_CodelistAssociata.Id),
                                            };
                                        }
                                        components.Add(dim);
                                        break;
                                    case DimensionTypeEnum.Time:
                                        DimensionMutableCore TimeDim = new DimensionMutableCore();
                                        TimeDim.ConceptRef = ReferenceBuilder.CreateConceptReference(ConceptSchemeId, _concept.ConceptObjectCode);
                                        TimeDim.Id = _concept.ConceptObjectCode;
                                        TimeDim.TimeDimension = true;
                                        components.Add(TimeDim);

                                        break;
                                    case DimensionTypeEnum.Frequency:
                                        DimensionMutableCore FreqDim = new DimensionMutableCore();
                                        FreqDim.ConceptRef = ReferenceBuilder.CreateConceptReference(ConceptSchemeId, _concept.ConceptObjectCode);
                                        if (_CodelistAssociata != null)
                                        {
                                            FreqDim.Representation = new RepresentationMutableCore()
                                            {  ////Si da per scontato che la frequency la codelist ce l'abbia
                                                Representation = ReferenceBuilder.CreateCodelistReference(_CodelistAssociata.Id),
                                            };
                                        }
                                        FreqDim.Id = _concept.ConceptObjectCode;
                                        FreqDim.FrequencyDimension = true;

                                        if (FreqDim.ConceptRole != null)
                                            FreqDim.ConceptRole.Add(new StructureReferenceImpl("ESTAT", "ESTAT_CONCEPT_ROLES_SCHEME", "0.1", SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Concept), new List<string> { "FREQUENCY" }));

                                        components.Add(FreqDim);
                                        break;
                                    default:
                                        break;
                                }

                                break;
                            case ConceptTypeEnum.Attribute:

                                AttributeMutableCore attr = new AttributeMutableCore();
                                attr.ConceptRef = ReferenceBuilder.CreateConceptReference(ConceptSchemeId, _concept.ConceptObjectCode);
                                if (_CodelistAssociata != null)
                                {
                                    IRepresentationMutableObject representation = new RepresentationMutableCore();
                                    representation.Representation = ReferenceBuilder.CreateCodelistReference(_CodelistAssociata.Id);
                                    attr.Representation = representation;
                                }
                                //Aggiungo attributi all'attribute AssignmentStatus e AttachmentLevel
                                attr.AssignmentStatus = ((IAttributeConcept)_concept).AssignmentStatusType.ToString();
                                attr.AttachmentLevel = ((IAttributeConcept)_concept).AttributeAttachmentLevelType;
                                if (attr.AttachmentLevel == AttributeAttachmentLevel.DimensionGroup)
                                {
                                    foreach (var dimref in ((IAttributeConcept)_concept).GetDimensionsReference(ReferencesObject.Concepts[ConceptSchemeId]))
                                        attr.DimensionReferences.Add(dimref);
                                }
                                if (attr.AttachmentLevel == AttributeAttachmentLevel.Group)
                                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.AttributeErrorAttachmentGroup);

                                //if (_CodelistAssociata.CodesObjects == null || _CodelistAssociata.CodesObjects.Count == 0)
                                //{//levo dall'attribute la referenza alla codelist
                                //    attr.Representation = null;
                                //}

                                if (((IAttributeConcept)_concept).IsValueAttribute)
                                {
                                    PrimaryMeasureMutableCore PrimaryMeasure = new PrimaryMeasureMutableCore();//SdmxStructureType.GetFromEnum(SdmxStructureEnumType.PrimaryMeasure));
                                    PrimaryMeasure.ConceptRef = ReferenceBuilder.CreateConceptReference(ConceptSchemeId, _concept.ConceptObjectCode);
                                    components.Add(PrimaryMeasure);
                                }
                                else
                                    components.Add(attr);
                                break;
                        }
                    }

                }
                this.ReferencesObject.Codelists = null;

                DataStructureBuilder _DataStructureBuilder = new DataStructureBuilder(conceptsObject, this.parsingObject, this.versionTypeResp);
                _DataStructureBuilder.Code = string.Format(FlyConfiguration.DsdFormat, _foundedDataflowId);
                _DataStructureBuilder.Names = new List<SdmxObjectNameDescription>();
                foreach (var nome in ReferencesObject.FoundedDataflows.Find(df => df.Id == _foundedDataflowId).Names)
                    _DataStructureBuilder.Names.Add(new SdmxObjectNameDescription() { Lingua = nome.Locale, Name = nome.Value });

                return _DataStructureBuilder.BuildDataStructure(components, null, FlyConfiguration.MainAgencyId, FlyConfiguration.Version);
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateImmutable, ex);
            }
        }


        /// <summary>
        /// retrieves the DSD from database
        /// </summary>
        /// <param name="refObj">Referenced Objects</param>
        /// <returns>list of DataStructure for SDMXObject</returns>
        public List<DataStructureObjectImpl> GetDSDsReferences(IReferencesObject refObj)
        {
            try
            {
                if (refObj.FoundedDataflows == null)
                    return GetDSDs();

                ReferencesObject.DSDs = new List<DataStructureObjectImpl>();
                if (ReferencesObject.Codelists == null)
                    ReferencesObject.Codelists = new List<ICodelistMutableObject>();
                if (ReferencesObject.Concepts == null)
                    ReferencesObject.Concepts = new Dictionary<string, List<IConceptObjectImpl>>();
                foreach (var df in refObj.FoundedDataflows)
                    ReferencesObject.DSDs.Add(BuildDataStructure(df.Id));
                return ReferencesObject.DSDs;

            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.BuildDSD, ex);
            }
        }
    }
}
