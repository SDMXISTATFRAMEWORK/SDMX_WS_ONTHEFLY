using FlyController;
using FlyController.Builder;
using FlyController.Model;
using FlyController.Model.DbSetting;
using FlyController.Model.Error;
using FlyEngine.Model;
using FlyMapping.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.DataStructure;
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
using System.Xml;

namespace FlySDDSLoader_implementation.Manager.Metadata
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


                BuilderParameter bp = new BuilderParameter()
                {
                    IsStub = this.parsingObject.ReturnStub,
                    TimeStamp = this.parsingObject.TimeStamp,
                };
                if (!string.IsNullOrEmpty(this.parsingObject.MaintainableId))
                    bp.DSDCode = this.parsingObject.MaintainableId;
                if (this.parsingObject.isValidAgency())
                    bp.DSDAgencyId = this.parsingObject.AgencyId;
                if (this.parsingObject.isValidVersion())
                    bp.DSDVersion = this.parsingObject._version;


                ReferencesObject.DSDs = BuildDSD(bp);


                return ReferencesObject.DSDs;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.BuildDSD, ex);
            }
        }



        internal List<DataStructureObjectImpl> BuildDSD(BuilderParameter bp)
        {
            try
            {
                List<XmlNode> risposta = this.DbAccess.Execute(DBOperationEnum.MSGetDSD, bp.GetParameter());
                List<DataStructureObjectImpl> DSDs = new List<DataStructureObjectImpl>();
                foreach (var xmlDSD in risposta)
                    DSDs.Add(BuildDataStructure(xmlDSD));
                return DSDs;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.BuildDSD, ex);
            }
        }

        private DataStructureObjectImpl BuildDataStructure(XmlNode xmlDSD)
        {
            try
            {
                List<IConceptObjectImpl> concepts = new List<IConceptObjectImpl>();
                List<IGroupMutableObject> Groups = new List<IGroupMutableObject>();
                List<IComponentMutableObject> components = new List<IComponentMutableObject>();
                string DSDCode = xmlDSD.Attributes["id"].Value;
                string DSDAgency = xmlDSD.Attributes["agencyID"].Value;
                string DSDVersion = xmlDSD.Attributes["version"].Value;
                List<SdmxObjectNameDescription> DsdName = SdmxObjectNameDescription.GetNameDescriptions(xmlDSD);

                XmlNode xmlComponents = null;
                for (int i = 0; i < xmlDSD.ChildNodes.Count; i++)
                {
                    if (xmlDSD.ChildNodes[i].Name == "Components")
                    {
                        xmlComponents = xmlDSD.ChildNodes[i];
                        break;
                    }
                }
                if (xmlComponents != null)
                    foreach (XmlNode xmlcomponent in xmlComponents)
                    {
                        string conceptRef = "";
                        string codelist = "";
                        string codelistAgency = "";
                        string codelistVersion = "";
                        string conceptSchemeRef = "";
                        string conceptSchemeAgency = "";
                        string conceptVersion = "";
                        if (xmlcomponent.Attributes["conceptRef"] != null)
                            conceptRef = xmlcomponent.Attributes["conceptRef"].Value;

                        if (xmlcomponent.Attributes["codelist"] != null)
                            codelist = xmlcomponent.Attributes["codelist"].Value;
                        if (xmlcomponent.Attributes["codelistAgency"] != null)
                            codelistAgency = xmlcomponent.Attributes["codelistAgency"].Value;
                        if (xmlcomponent.Attributes["codelistVersion"] != null)
                            codelistVersion = xmlcomponent.Attributes["codelistVersion"].Value;

                        if (xmlcomponent.Attributes["conceptSchemeRef"] != null)
                            conceptSchemeRef = xmlcomponent.Attributes["conceptSchemeRef"].Value;
                        if (xmlcomponent.Attributes["conceptSchemeAgency"] != null)
                            conceptSchemeAgency = xmlcomponent.Attributes["conceptSchemeAgency"].Value;
                        if (xmlcomponent.Attributes["conceptVersion"] != null)
                            conceptVersion = xmlcomponent.Attributes["conceptVersion"].Value;

                        ConceptDSDInfoObject conceptDSDInfoObject = null;
                        if (!string.IsNullOrEmpty(codelist))
                        {
                            conceptDSDInfoObject = new ConceptDSDInfoObject()
                            {
                                CodelistId = codelist,
                                CodelistAgency = codelistAgency,
                                CodelistVersion = codelistVersion
                            };
                        }
                        switch (xmlcomponent.Name)
                        {
                            case "Dimension":
                                DimensionMutableCore dim = new DimensionMutableCore();
                                dim.Id = conceptRef;
                                dim.ConceptRef = new StructureReferenceImpl(conceptSchemeAgency, conceptSchemeRef, conceptVersion, SdmxStructureEnumType.Concept, conceptRef);
                                if (!string.IsNullOrEmpty(codelist))
                                {
                                    dim.Representation = new RepresentationMutableCore()
                                    {
                                        Representation = new Org.Sdmxsource.Sdmx.Util.Objects.Reference.StructureReferenceImpl(codelistAgency, codelist, codelistVersion, SdmxStructureEnumType.CodeList),
                                    };
                                }
                                bool isFrequencyDimension = (xmlcomponent.Attributes["isFrequencyDimension"] != null && xmlcomponent.Attributes["isFrequencyDimension"].Value.Trim().ToLower() == "true");

                                if (isFrequencyDimension)
                                {
                                    dim.FrequencyDimension = true;
                                    if (dim.ConceptRole != null)
                                        dim.ConceptRole.Add(new StructureReferenceImpl("ESTAT", "ESTAT_CONCEPT_ROLES_SCHEME", "0.1", SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Concept), new List<string> { "FREQUENCY" }));
                                    concepts.Add(new DimensionConcept(conceptRef, new List<SdmxObjectNameDescription>() { new SdmxObjectNameDescription() { Lingua = "en", Name = conceptRef } }) { DimensionType = DimensionTypeEnum.Frequency, ConceptDSDInfo=conceptDSDInfoObject });

                                }
                                else
                                {
                                    concepts.Add(new DimensionConcept(conceptRef, new List<SdmxObjectNameDescription>() { new SdmxObjectNameDescription() { Lingua = "en", Name = conceptRef } }) { DimensionType = DimensionTypeEnum.Dimension, ConceptDSDInfo = conceptDSDInfoObject });
                                }
                                components.Add(dim);

                                break;
                            case "TimeDimension":
                                DimensionMutableCore TimeDim = new DimensionMutableCore();
                                TimeDim.ConceptRef = new StructureReferenceImpl(conceptSchemeAgency, conceptSchemeRef, conceptVersion, SdmxStructureEnumType.Concept, conceptRef);
                                TimeDim.Id = conceptRef;
                                TimeDim.TimeDimension = true;
                                components.Add(TimeDim);
                                concepts.Add(new DimensionConcept(conceptRef, new List<SdmxObjectNameDescription>() { new SdmxObjectNameDescription() { Lingua = "en", Name = conceptRef } }) { DimensionType = DimensionTypeEnum.Time, ConceptDSDInfo = conceptDSDInfoObject });

                                break;
                            case "Attribute":
                                AttributeMutableCore attr = new AttributeMutableCore();
                                attr.Id = conceptRef;
                                attr.ConceptRef = new StructureReferenceImpl(conceptSchemeAgency, conceptSchemeRef, conceptVersion, SdmxStructureEnumType.Concept, conceptRef);
                                if (!string.IsNullOrEmpty(codelist))
                                {
                                    attr.Representation = new RepresentationMutableCore()
                                    {
                                        Representation = new Org.Sdmxsource.Sdmx.Util.Objects.Reference.StructureReferenceImpl(codelistAgency, codelist, codelistVersion, SdmxStructureEnumType.CodeList),
                                    };
                                }
                                //Aggiungo attributi all'attribute AssignmentStatus e AttachmentLevel
                                string AttachmentLevel = "";
                                string AssignmentStatus = "";
                                if (xmlcomponent.Attributes["assignmentStatus"] != null)
                                    AssignmentStatus = xmlcomponent.Attributes["assignmentStatus"].Value;
                                if (xmlcomponent.Attributes["attachmentLevel"] != null)
                                    AttachmentLevel = xmlcomponent.Attributes["attachmentLevel"].Value;

                                attr.AttachmentLevel = (AttributeAttachmentLevel)Enum.Parse(typeof(AttributeAttachmentLevel), AttachmentLevel, true);
                                attr.AssignmentStatus = ((AssignmentStatusTypeEnum)Enum.Parse(typeof(AssignmentStatusTypeEnum), AssignmentStatus, true)).ToString();


                                AttributeConcept _concept = new AttributeConcept(conceptRef, new List<SdmxObjectNameDescription>() { new SdmxObjectNameDescription() { Lingua = "en", Name = conceptRef } });
                                _concept.ConceptDSDInfo = conceptDSDInfoObject;

                                if (attr.AttachmentLevel == AttributeAttachmentLevel.DimensionGroup)
                                {
                                    //foreach (var dimref in ((IAttributeConcept)_concept).GetDimensionsReference(concepts))
                                    //    attr.DimensionReferences.Add(dimref);

                                    foreach(XmlNode dimRef in xmlcomponent.ChildNodes)
                                        attr.DimensionReferences.Add(dimRef.Attributes["id"].Value);
                                }

                                if (attr.AttachmentLevel == AttributeAttachmentLevel.Group)
                                {
                                    if (xmlcomponent.Attributes["attachmentGroup"] != null)
                                        _concept.GroupName = xmlcomponent.Attributes["attachmentGroup"].Value;
                                    attr.AttachmentGroup = _concept.GroupName;
                                }

                                components.Add(attr);
                                concepts.Add(_concept);
                                break;
                            case "PrimaryMeasure":
                                PrimaryMeasureMutableCore PrimaryMeasure = new PrimaryMeasureMutableCore();//SdmxStructureType.GetFromEnum(SdmxStructureEnumType.PrimaryMeasure));
                                PrimaryMeasure.Id = conceptRef;
                                PrimaryMeasure.ConceptRef = new StructureReferenceImpl(conceptSchemeAgency, conceptSchemeRef, conceptVersion, SdmxStructureEnumType.Concept, conceptRef);
                                components.Add(PrimaryMeasure);
                                concepts.Add(new AttributeConcept(conceptRef, new List<SdmxObjectNameDescription>() { new SdmxObjectNameDescription() { Lingua = "en", Name = conceptRef } }) { IsValueAttribute = true, ConceptDSDInfo = conceptDSDInfoObject });
                                break;
                            case "Group":
                                IGroupMutableObject group = new GroupMutableCore();
                                group.Id = xmlcomponent.Attributes["id"].Value;
                                foreach (XmlNode dimref in xmlcomponent.ChildNodes)
                                {
                                    if (dimref.Name == "GroupDimension")
                                        group.DimensionRef.Add(dimref.Attributes["id"].Value);
                                }
                                Groups.Add(group);
                                break;
                        }
                    }


                DataStructureBuilder _DataStructureBuilder = new DataStructureBuilder(concepts, this.parsingObject, this.versionTypeResp);
                _DataStructureBuilder.Code = DSDCode;
                _DataStructureBuilder.Names = DsdName;
                return _DataStructureBuilder.BuildDataStructure(components, Groups, DSDAgency, DSDVersion);

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
                List<DataStructureObjectImpl> DSDs = new List<DataStructureObjectImpl>();
                if (refObj == null)
                    return DSDs;
                if (refObj.FoundedDataflows != null && refObj.FoundedDataflows.Count > 0)
                {
                    DataflowsManager dfman = new DataflowsManager(this.parsingObject.CloneForReferences(), this.versionTypeResp);
                    List<MSDataflow> MSDfs = dfman.GetDataflows();
                    foreach (MSDataflow MSDf in MSDfs.FindAll(df => refObj.FoundedDataflows.Exists(founded => founded.Id == df.DFCode)))
                    {
                        BuilderParameter bp = new BuilderParameter()
                        {
                            IsStub = this.parsingObject.ReturnStub,
                            TimeStamp = this.parsingObject.TimeStamp,
                            DFId = MSDf.IdDf
                        };
                        DSDs.AddRange(BuildDSD(bp));

                    }

                }
                if (refObj.ConceptSchemes != null && refObj.ConceptSchemes.Count > 0)
                {
                    foreach (IConceptSchemeObject cs in refObj.ConceptSchemes)
                    {
                        BuilderParameter bp = new BuilderParameter()
                       {
                           IsStub = this.parsingObject.ReturnStub,
                           TimeStamp = this.parsingObject.TimeStamp,
                           ConceptSchemeCode = cs.Id,
                           ConceptSchemeAgencyId = cs.AgencyId,
                           ConceptSchemeVersion = cs.Version
                       };
                        DSDs.AddRange(BuildDSD(bp));
                    }

                }

                if (refObj.Codelists != null && refObj.Codelists.Count > 0)
                {
                    foreach (ICodelistMutableObject cl in refObj.Codelists)
                    {
                        BuilderParameter bp = new BuilderParameter()
                        {
                            IsStub = this.parsingObject.ReturnStub,
                            TimeStamp = this.parsingObject.TimeStamp,
                            CodelistCode = cl.Id,
                            CodelistAgencyId = cl.AgencyId,
                            CodelistVersion = cl.Version
                        };
                        DSDs.AddRange(BuildDSD(bp));
                    }
                }

                //Elimino i Duplicati
                ReferencesObject.DSDs = new List<DataStructureObjectImpl>();
                foreach (var DSD in DSDs)
                {
                    if (!ReferencesObject.DSDs.Exists(rdsd => rdsd.Id == DSD.Id))
                        ReferencesObject.DSDs.Add(DSD);
                }
                return ReferencesObject.DSDs;

            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.BuildDSD, ex);
            }
        }


        //internal DataStructureObjectImpl GetDSD_DataflowRef(int IdDf)
        //{
        //    try
        //    {

        //        BuilderParameter bp = new BuilderParameter()
        //               {
        //                   IsStub = true,
        //                   DFId = IdDf
        //               };
        //        List<DataStructureObjectImpl> DSDs = BuildDSD(bp);
        //        if (DSDs != null && DSDs.Count > 0)
        //            return DSDs[0];
        //        return null;
        //    }
        //    catch (SdmxException) { throw; }
        //    catch (Exception ex)
        //    {
        //        throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.BuildDSD, ex);
        //    }
        //}
    }
}
