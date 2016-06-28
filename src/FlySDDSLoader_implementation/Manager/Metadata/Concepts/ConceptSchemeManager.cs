using FlyController;
using FlyController.Builder;
using FlyController.Model;
using FlyController.Model.DbSetting;
using FlyController.Model.Error;
using FlyEngine.Model;
using FlyMapping.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FlySDDSLoader_implementation.Manager.Metadata
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

                BuilderParameter bp = new BuilderParameter()
                {
                    IsStub = this.parsingObject.ReturnStub,
                    TimeStamp = this.parsingObject.TimeStamp,
                };
                if (!string.IsNullOrEmpty(this.parsingObject.MaintainableId))
                    bp.ConceptSchemeCode = this.parsingObject.MaintainableId;
                if (this.parsingObject.isValidAgency())
                    bp.ConceptSchemeAgencyId = this.parsingObject.AgencyId;
                if (this.parsingObject.isValidVersion())
                    bp.ConceptSchemeVersion = this.parsingObject._version;

                ReferencesObject.ConceptSchemes = BuildConcepts(bp);

                return ReferencesObject.ConceptSchemes;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetConceptsScheme, ex);
            }
        }


        private List<IConceptSchemeObject> BuildConcepts(BuilderParameter bp)
        {
            List<XmlNode> risposta = this.DbAccess.Execute(DBOperationEnum.MSGetConceptScheme, bp.GetParameter());
            List<IConceptSchemeObject> ConceptSchemes = new List<IConceptSchemeObject>();
            foreach (var xmlDSD in risposta)
                ConceptSchemes.Add(BuildConcept(xmlDSD));
            return ConceptSchemes;

        }

        private IConceptSchemeObject BuildConcept(XmlNode xmlConceptScheme)
        {
            string CSCode = xmlConceptScheme.Attributes["Code"].Value; ;
            string CSAgencyId = xmlConceptScheme.Attributes["Agency"].Value; ;
            string CSVersion = xmlConceptScheme.Attributes["Version"].Value; ;
            List<SdmxObjectNameDescription> CsName = SdmxObjectNameDescription.GetNameDescriptions(xmlConceptScheme);

            ConceptBuilder _conceptBuilder = new ConceptBuilder(CsName, this.parsingObject, this.versionTypeResp);
            _conceptBuilder.Code = CSCode;
            _conceptBuilder.VersionTypeResp = this.versionTypeResp;

            List<IConceptObjectImpl> ListConcepts = new List<IConceptObjectImpl>();
            foreach (XmlNode xmlconcept in xmlConceptScheme.ChildNodes)
            {
                if (xmlconcept.Name != "Concept")
                    continue;

                string ConceptCode = "";
                if (xmlconcept.Attributes["Code"] != null)
                    ConceptCode = xmlconcept.Attributes["Code"].Value;
                List<SdmxObjectNameDescription> ConceptNames = SdmxObjectNameDescription.GetNameDescriptions(xmlconcept);
                
                ListConcepts.Add(new DimensionConcept(ConceptCode, ConceptNames));
                
                //string ConceptType = "";
                //if (xmlconcept.Attributes["Type"] != null)
                //    ConceptType = xmlconcept.Attributes["Type"].Value;


                /*switch (ConceptType)
                {
                    case "Dimension":
                        ListConcepts.Add(new DimensionConcept(ConceptCode, ConceptNames));
                        break;
                    case "TimeDimension":
                        ListConcepts.Add(new DimensionConcept(ConceptCode, ConceptNames) { DimensionType = DimensionTypeEnum.Time });
                        break;
                    case "Attribute":
                        AttributeConcept attr = new AttributeConcept(ConceptCode, ConceptNames);
                        string AttachmentLevel = "";
                        string AssignmentStatus = "";
                        if (xmlconcept.Attributes["assignmentStatus"] != null)
                            AssignmentStatus = xmlconcept.Attributes["assignmentStatus"].Value;
                        if (xmlconcept.Attributes["attachmentLevel"] != null)
                            AttachmentLevel = xmlconcept.Attributes["attachmentLevel"].Value;

                        attr.AttributeAttachmentLevelType = (AttributeAttachmentLevel)Enum.Parse(typeof(AttributeAttachmentLevel), AttachmentLevel, true);
                        attr.AssignmentStatusType = (AssignmentStatusTypeEnum)Enum.Parse(typeof(AssignmentStatusTypeEnum), AssignmentStatus, true);
                        ListConcepts.Add(attr);
                        break;
                    case "PrimaryMeasure":
                        ListConcepts.Add(new AttributeConcept(ConceptCode, ConceptNames) { IsValueAttribute = true });
                        break;
                }*/
            }

            if (!this.parsingObject.ReturnStub)
                _conceptBuilder.Concepts = ListConcepts;

            IConceptSchemeObject conceptScheme = _conceptBuilder.BuildConceptScheme(CSAgencyId, CSVersion);

            
            return conceptScheme;
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
                List<IConceptSchemeObject> _ConceptSchemes = new List<IConceptSchemeObject>();
                if (refObj == null)
                    return _ConceptSchemes;

                if (refObj.DSDs != null && refObj.DSDs.Count > 0)
                {
                    foreach (DataStructureObjectImpl dsd in refObj.DSDs)
                    {
                        BuilderParameter bp = new BuilderParameter()
                        {
                            IsStub = this.parsingObject.ReturnStub,
                            TimeStamp = this.parsingObject.TimeStamp,
                            DSDCode = dsd.Id,
                            DSDAgencyId = dsd.AgencyId,
                            DSDVersion = dsd.Version
                        };
                        _ConceptSchemes.AddRange(BuildConcepts(bp));
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
                        _ConceptSchemes.AddRange(BuildConcepts(bp));
                    }
                }

                //Elimino i Duplicati
                ReferencesObject.ConceptSchemes = new List<IConceptSchemeObject>();
                foreach (var cs in _ConceptSchemes)
                {
                    if (!ReferencesObject.ConceptSchemes.Exists(rdsd => rdsd.Id == cs.Id))
                        ReferencesObject.ConceptSchemes.Add(cs);
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
