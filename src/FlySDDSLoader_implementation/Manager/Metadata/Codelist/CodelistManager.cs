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
            try
            {
                string CodelistAgency = this.parsingObject.AgencyId;
                string CodelistVersion = this.parsingObject._version;

                DataflowsManager dfMan = new DataflowsManager(this.parsingObject.CloneForReferences(), this.versionTypeResp);
                dfMan.parsingObject.MaintainableId = this.parsingObject.ConstrainDataFlow;
                if (this.ReferencesObject == null)
                    this.ReferencesObject = new IReferencesObject();
                this.ReferencesObject.FoundedDataflows = dfMan.GetDataFlows();

                if (dfMan.MSDataflows != null && dfMan.MSDataflows.Count == 1)
                {
                    BuilderParameter bp = new BuilderParameter()
                    {
                        DFId = dfMan.MSDataflows[0].IdDf
                    };
                    DsdManager dsdMan = new DsdManager(this.parsingObject.CloneForReferences(), this.versionTypeResp);

                    List<DataStructureObjectImpl> dsds = dsdMan.BuildDSD(bp);
                    if (dsds != null && dsds.Count == 1)
                    {
                        CodelistBuilder _CodelistBuilder = new CodelistBuilder(this.parsingObject, this.versionTypeResp);

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
                            _CodelistBuilder.Code = this.parsingObject.MaintainableId;
                            _CodelistBuilder.Names = new List<SdmxObjectNameDescription>();
                            foreach (var codename in Concept.Names)
                                _CodelistBuilder.Names.Add(new SdmxObjectNameDescription() { Lingua = codename.Locale, Name = codename.Value });
                        }
                        else
                        {
                            foreach (IConceptObjectImpl concept in dsds[0]._Concepts)
                            {
                                if (concept.ConceptObjectCode.Trim().ToLower() == this.parsingObject.ConstrainConcept.Trim().ToLower())
                                {
                                    Concept = concept;

                                    if (this.parsingObject.ContrainConceptREF != null && this.parsingObject.ContrainConceptREF.Keys.Count > 0)
                                    {
                                        ISpecialConcept sc = new SpecialConcept(Concept.Id, SpecialTypeEnum.CL_CONTRAINED);
                                        sc.SetNames(Concept.ConceptObjectNames);
                                        sc.TimeDimensionRef = dsds[0]._Concepts.Find(c => c.ConceptType == ConceptTypeEnum.Dimension && ((IDimensionConcept)c).DimensionType == DimensionTypeEnum.Time);
                                        sc.ContrainConceptREF = this.parsingObject.ContrainConceptREF;
                                        Concept = sc;
                                        _CodelistBuilder.Code = this.parsingObject.MaintainableId;
                                        _CodelistBuilder.Names = new List<SdmxObjectNameDescription>();
                                        foreach (var codename in Concept.Names)
                                            _CodelistBuilder.Names.Add(new SdmxObjectNameDescription() { Lingua = codename.Locale, Name = codename.Value });

                                    }
                                    else
                                    {
                                        bp = new BuilderParameter()
                                        {
                                            CodelistCode = this.parsingObject.MaintainableId,
                                            IsStub = true
                                        };
                                        if (this.parsingObject.isValidAgency())
                                            bp.CodelistAgencyId = this.parsingObject.AgencyId;
                                        if (this.parsingObject.isValidVersion())
                                            bp.CodelistVersion = this.parsingObject._version;

                                        List<ICodelistMutableObject> listCL = BuildCodelists(bp);
                                        if (listCL == null || listCL.Count == 0)
                                            throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetCodelist, new Exception("Codelist not found"));


                                        _CodelistBuilder.Code = listCL[0].Id;
                                        _CodelistBuilder.Names = new List<SdmxObjectNameDescription>();
                                        foreach (var clname in listCL[0].Names)
                                            _CodelistBuilder.Names.Add(new SdmxObjectNameDescription() { Lingua = clname.Locale, Name = clname.Value });
                                        CodelistAgency = listCL[0].AgencyId;
                                        CodelistVersion = listCL[0].Version;
                                    }
                                    break;
                                }
                            }
                        }

                        if (Concept == null)
                            throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetCodelist, new Exception("Codelist not found"));

                        if (!this.parsingObject.ReturnStub)
                        {
                            FlyDotStatExtra_implementation.Manager.Metadata.CodelistManager cm = new FlyDotStatExtra_implementation.Manager.Metadata.CodelistManager(this.parsingObject, this.versionTypeResp);
                            _CodelistBuilder.CodesObjects = cm.GetCodelist(dfMan.MSDataflows[0].DFCode, Concept);
                        }

                        this.ReferencesObject.Codelists = new List<ICodelistMutableObject>();
                        this.ReferencesObject.Codelists.Add(_CodelistBuilder.BuildCodelist(CodelistAgency, CodelistVersion));
                    }

                }

                return this.ReferencesObject.Codelists;
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
        /// <returns>list of Mutable Code Object</returns>
        public List<ICodelistMutableObject> GetCodelistNoConstrain()
        {
            try
            {
                if (ReferencesObject == null)
                    ReferencesObject = new IReferencesObject();


                this.ReferencesObject.Codelists = new List<ICodelistMutableObject>();

                BuilderParameter bp = new BuilderParameter()
                {
                    IsStub = this.parsingObject.ReturnStub,
                    TimeStamp = this.parsingObject.TimeStamp,
                };
                if (!string.IsNullOrEmpty(this.parsingObject.MaintainableId))
                    bp.CodelistCode = this.parsingObject.MaintainableId;
                if (this.parsingObject.isValidAgency())
                    bp.CodelistAgencyId = this.parsingObject.AgencyId;
                if (this.parsingObject.isValidVersion())
                    bp.CodelistVersion = this.parsingObject._version;

                ReferencesObject.Codelists = BuildCodelists(bp);
                return ReferencesObject.Codelists;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetCodelist, ex);
            }
        }


        private List<ICodelistMutableObject> BuildCodelists(BuilderParameter bp)
        {

            List<XmlNode> risposta = this.DbAccess.Execute(DBOperationEnum.MSGetCodelist, bp.GetParameter());
            List<ICodelistMutableObject> Codelists = new List<ICodelistMutableObject>();
            foreach (var xmlcodelist in risposta)
                Codelists.Add(BuildCodelist(xmlcodelist));
            return Codelists;
        }
        private ICodelistMutableObject BuildCodelist(XmlNode xmlCodelist)
        {
            string CLCode = xmlCodelist.Attributes["Code"].Value;
            string CLAgency = xmlCodelist.Attributes["AgencyID"].Value;
            string CLVersion = xmlCodelist.Attributes["Version"].Value;

            List<SdmxObjectNameDescription> CLName = SdmxObjectNameDescription.GetNameDescriptions(xmlCodelist);


            List<ICodeMutableObject> CodelistItem = new List<ICodeMutableObject>();
            RecursivePopulateCodelist(xmlCodelist, CodelistItem, null);

            CodelistBuilder _CodelistBuilder = new CodelistBuilder(this.parsingObject, this.versionTypeResp);
            _CodelistBuilder.Code = CLCode;
            _CodelistBuilder.Names = CLName;

            if (!this.parsingObject.ReturnStub)
                _CodelistBuilder.CodesObjects = CodelistItem;

            return _CodelistBuilder.BuildCodelist(CLAgency, CLVersion);
        }

        private void RecursivePopulateCodelist(XmlNode xmlCodelist, List<ICodeMutableObject> CodelistItems, string ParentCode)
        {
            foreach (XmlNode item in xmlCodelist.ChildNodes)
            {
                if (item.Name == "Code")
                {
                    string CLItemCode = item.Attributes["value"].Value;
                    List<SdmxObjectNameDescription> CLItemName = SdmxObjectNameDescription.GetNameDescriptions(item);
                    CodelistItems.Add(CodelistItemBuilder.BuildCodeObjects(CLItemCode, CLItemName, ParentCode));
                    RecursivePopulateCodelist(item, CodelistItems, CLItemCode);
                }
            }
        }


        /// <summary>
        /// retrieves the codelist Contrain from database
        /// </summary>
        /// <param name="refObj">Referenced Objects</param>
        /// <returns>list of Mutable Code Object</returns>
        public List<ICodelistMutableObject> GetCodelistReferences(IReferencesObject refObj)
        {

            try
            {
                List<ICodelistMutableObject> _Codelist = new List<ICodelistMutableObject>();
                if (refObj == null)
                    return _Codelist;


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
                        _Codelist.AddRange(BuildCodelists(bp));
                    }

                }
                else if (refObj.ConceptSchemes != null && refObj.ConceptSchemes.Count > 0)
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
                        _Codelist.AddRange(BuildCodelists(bp));
                    }
                }

                //Elimino i Duplicati
                ReferencesObject.Codelists = new List<ICodelistMutableObject>();
                foreach (var cl in _Codelist)
                {
                    if (!ReferencesObject.Codelists.Exists(rcl => rcl.Id == cl.Id))
                        ReferencesObject.Codelists.Add(cl);
                }
                return ReferencesObject.Codelists;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetCodelist, ex);
            }
        }

    }
}
