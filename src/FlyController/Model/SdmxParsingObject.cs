using FlyController;
using FlyController.Builder.ConstrainParse;
using FlyController.Model.Error;
using Org.Sdmx.Resources.SdmxMl.Schemas.V20.common;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference.Complex;
using Org.Sdmxsource.Sdmx.Api.Model.Query;
using Org.Sdmxsource.Sdmx.Structureparser.Workspace;
using Org.Sdmxsource.Sdmx.Util.Objects.Reference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace FlyController.Model
{
    /// <summary>
    /// SdmxParsingObject is a Object Model used for parse a query request
    /// </summary>
    public class SdmxParsingObject : ISdmxParsingObject
    {
        /// <summary>
        /// Create a SdmxParsingObject instance
        /// </summary>
        /// <param name="_sdmxStructureType">Sdmx Version</param>
        public SdmxParsingObject(SdmxStructureEnumType _sdmxStructureType)
        {
            this.OtherRegistry = new List<ISdmxParsingObject>();
            this.SdmxStructureType = _sdmxStructureType;
            this.ResolveReferenceSdmx20 = false;
        }
        #region Property
        /// <summary>
        /// Sdmx Version
        /// </summary>
        public SdmxStructureEnumType SdmxStructureType { get; set; }

        private string _maintainableId = null;
        /// <summary>
        /// Structure Maintainable Code (properties that change the type of value according to the type required)
        /// </summary>
        public string MaintainableId
        {
            get
            {
                if (!string.IsNullOrEmpty(_maintainableId) && new List<string>() { "*", "ALL" }.Contains(_maintainableId.Trim().ToUpper()))
                    return null;
                return _maintainableId;
            }
            set { _maintainableId = value; }
        }
        /// <summary>
        /// Structure Agency Code
        /// </summary>
        public string AgencyId { get; set; }
        /// <summary>
        /// Structure Version
        /// </summary>
        public string _version { get; set; }

        /// <summary>
        /// Not load a full Object (For Sdmx 2.1 or REST)
        /// </summary>
        public StructureQueryDetailEnumType QueryDetail { get; set; }


        /// <summary>
        /// Determinate what structure references return
        /// </summary>
        public StructureReferenceDetailEnumType References { get; set; }

        /// <summary>
        /// Determinate what is a ResolveReference property of a Query in Sdmx 2.0
        /// </summary>
        public bool ResolveReferenceSdmx20 { get; set; }

        /// <summary>
        /// Determinate if this class is using for build a reference
        /// </summary>
        public bool isReferenceOf { get; set; }

        /// <summary>
        /// List of Structure required for references
        /// </summary>
        public List<SdmxStructureType> SpecificReference { get; set; }

        /// <summary>
        /// If the Query contains other Registry this field contains a type of other regitry requested
        /// </summary>
        public List<ISdmxParsingObject> OtherRegistry { get; set; }

        /// <summary>
        /// Dataflow Code for Codelist Constrained
        /// </summary>
        public string ConstrainDataFlow { get; set; }
        /// <summary>
        /// Dataflow Agency for Codelist Constrained
        /// </summary>
        public string ConstrainDataFlowAgency { get; set; }
        /// <summary>
        /// Dataflow Version for Codelist Constrained
        /// </summary>
        public string ConstrainDataFlowVersion { get; set; }

        /// <summary>
        /// Concept Code for Codelist Constrained
        /// </summary>
        public string ConstrainConcept { get; set; }
        /// <summary>
        /// List of Member for Codelist Constrained (Other concept value in constrain, call a Special Concept CL_CONSTRAIN)
        /// </summary>
        public Dictionary<string, IList<MemberValueType>> ContrainConceptREF { get; set; }

        /// <summary>
        /// LastUpdate parameter request only observation from this date onwards
        /// </summary>
        public string TimeStamp { get; set; }

        /// <summary>
        /// Determines whether the result is in STUB format or not: if detail is AllStubs or if detail is ReferencedStubs and is writing a reference
        /// </summary>
        public bool ReturnStub
        {
            get
            {
                return this.QueryDetail == StructureQueryDetailEnumType.AllStubs
                    || (this.QueryDetail == StructureQueryDetailEnumType.ReferencedStubs && this.isReferenceOf);
            }
        }
        #endregion

        #region Parsing
        /// <summary>
        /// build SdmxParsingObject from IQueryWorkspace
        /// </summary>
        /// <param name="workspace">return object of CommonApi.parse request</param>
        /// <returns>SdmxParsingObject</returns>
        public static SdmxParsingObject Parse(IQueryWorkspace workspace)
        {
            try
            {
                if (workspace == null)
                    throw new SdmxException(typeof(SdmxParsingObject), FlyExceptionObject.FlyExceptionTypeEnum.ParsingQueryError);

                SdmxParsingObject parsedObject = null;

                if (workspace.ComplexStructureQuery != null)
                {
                    IComplexStructureReferenceObject workspaceRef = workspace.ComplexStructureQuery.StructureReference as IComplexStructureReferenceObject;

                    parsedObject = new SdmxParsingObject(workspaceRef.ReferencedStructureType)
                    {
                        MaintainableId = workspaceRef.Id == null ? null : workspaceRef.Id.SearchParameter,
                        AgencyId = workspaceRef.AgencyId == null ? null : workspaceRef.AgencyId.SearchParameter,
                        _version = workspaceRef.VersionReference == null ? null : workspaceRef.VersionReference.Version,
                        QueryDetail = (StructureQueryDetailEnumType)workspace.ComplexStructureQuery.StructureQueryMetadata.StructureQueryDetail.EnumType,
                        References = workspace.ComplexStructureQuery.StructureQueryMetadata.StructureReferenceDetail.EnumType,
                        SpecificReference = workspace.ComplexStructureQuery.StructureQueryMetadata.ReferenceSpecificStructures != null ? workspace.ComplexStructureQuery.StructureQueryMetadata.ReferenceSpecificStructures.ToList() : null,

                    };
                    if (workspaceRef.ReferencedStructureType.EnumType == SdmxStructureEnumType.CodeList)
                    {
                        //Constrin 2.1
                        /*
                        <query:CodeWhere>
                        <query:Annotation>
                        <query:Title>DataflowConstrain</query:Title>
                        <query:Text>ISTAT+DF+1.0+Concept</query:Text>
                        </query:Annotation>
                        </query:CodeWhere>
                         * */
                        try
                        {
                            if (workspaceRef.ChildReference != null && workspaceRef.ChildReference.AnnotationReference != null)
                            {
                                string Title = workspaceRef.ChildReference.AnnotationReference.TitleReference.SearchParameter;
                                string Text = workspaceRef.ChildReference.AnnotationReference.TextReference.SearchParameter;
                                if (!string.IsNullOrEmpty(Title) && XDocument.Parse(Title).Root.Value.Trim() == "DataflowConstrain" && !string.IsNullOrEmpty(Text))
                                {
                                    string[] DataflowConstrin = XDocument.Parse(Text).Root.Value.Trim().Split('+');
                                    if (DataflowConstrin.Length==4)
                                    {
                                        parsedObject.ConstrainDataFlowAgency = DataflowConstrin[0];
                                        parsedObject.ConstrainDataFlow = DataflowConstrin[1];
                                        parsedObject.ConstrainDataFlowVersion = DataflowConstrin[2];
                                        parsedObject.ConstrainConcept = DataflowConstrin[3];
                                    }
                                }
                            }

                        }
                        catch (Exception)
                        {//Non metto la Costrain
                        }

                    }
                }
                else if (workspace.SimpleStructureQueries != null && workspace.SimpleStructureQueries.Count > 0)
                {
                    IStructureReference workspaceRef = workspace.SimpleStructureQueries[0] as IStructureReference;
                    parsedObject = new SdmxParsingObject(workspaceRef.MaintainableStructureEnumType.EnumType)
                    {
                        MaintainableId = workspaceRef.MaintainableId,
                        AgencyId = workspaceRef.AgencyId,
                        _version = workspaceRef.Version,
                        ResolveReferenceSdmx20 = workspace.ResolveReferences,
                        QueryDetail = StructureQueryDetailEnumType.Full
                    };

                    if (workspace.SimpleStructureQueries.Count == 2 &&
                        workspaceRef.MaintainableStructureEnumType.EnumType == SdmxStructureEnumType.CodeList &&
                        workspace.SimpleStructureQueries[1] is ConstrainableStructureReference &&
                        ((ConstrainableStructureReference)workspace.SimpleStructureQueries[1]).MaintainableStructureEnumType.EnumType == SdmxStructureEnumType.Dataflow &&
                         ((ConstrainableStructureReference)workspace.SimpleStructureQueries[1]).ConstraintObject != null)
                    {
                        ConstrainableStructureReference workspaceDF = workspace.SimpleStructureQueries[1] as ConstrainableStructureReference;
                        Org.Sdmx.Resources.SdmxMl.Schemas.V20.common.ConstraintType ct = ((ConstrainableStructureReference)workspace.SimpleStructureQueries[1]).ConstraintObject;
                        parsedObject.ConstrainDataFlow = workspaceDF.MaintainableId;
                        parsedObject.ConstrainDataFlowAgency = workspaceDF.AgencyId;
                        parsedObject.ConstrainDataFlowVersion = workspaceDF.Version;
                        if (ct.CubeRegion != null && ct.CubeRegion.Count > 0 && ct.CubeRegion[0].Member != null && ct.CubeRegion[0].Member.Count > 0 && !string.IsNullOrEmpty(workspaceRef.MaintainableId))
                        {
                            parsedObject.ConstrainConcept = ct.CubeRegion[0].Member[0].ComponentRef;
                            if (ct.CubeRegion[0].Member.Count > 1)
                            {
                                parsedObject.ContrainConceptREF = new Dictionary<string, IList<MemberValueType>>();
                                for (int i = 1; i < ct.CubeRegion[0].Member.Count; i++)
                                    parsedObject.ContrainConceptREF[ct.CubeRegion[0].Member[i].ComponentRef] = ct.CubeRegion[0].Member[i].MemberValue;
                            }

                        }
                        parsedObject.OtherRegistry = new List<ISdmxParsingObject>(){
                            new SdmxParsingObject(workspaceDF.MaintainableStructureEnumType.EnumType)
                            {
                                MaintainableId = workspaceDF.MaintainableId,
                                AgencyId = workspaceDF.AgencyId,
                                _version = workspaceDF.Version,
                                ResolveReferenceSdmx20 = workspace.ResolveReferences,
                                QueryDetail = StructureQueryDetailEnumType.Full
                            }};
                    }
                    else if (workspace.SimpleStructureQueries.Count > 1)
                    {
                        parsedObject.OtherRegistry = new List<ISdmxParsingObject>();

                        for (int i = 1; i < workspace.SimpleStructureQueries.Count; i++)
                        {
                            workspaceRef = workspace.SimpleStructureQueries[i] as IStructureReference;
                            parsedObject.OtherRegistry.Add(new SdmxParsingObject(workspaceRef.MaintainableStructureEnumType.EnumType)
                            {
                                MaintainableId = workspaceRef.MaintainableId,
                                AgencyId = workspaceRef.AgencyId,
                                _version = workspaceRef.Version,
                                ResolveReferenceSdmx20 = workspace.ResolveReferences,
                                QueryDetail = StructureQueryDetailEnumType.Full
                            });
                        }
                    }
                }

                if (parsedObject == null)
                    throw new SdmxException(typeof(SdmxParsingObject), FlyExceptionObject.FlyExceptionTypeEnum.StructureNotFound);

                return parsedObject;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(typeof(SdmxParsingObject), FlyExceptionObject.FlyExceptionTypeEnum.ParsingQueryError, ex);
            }
        }

        /// <summary>
        /// build SdmxParsingObject from IRestStructureQuery
        /// </summary>
        /// <param name="query">return object of RESTStructureQueryCore request</param>
        /// <param name="ConstrainParameter">Parameters for Codelist Contrained</param>
        /// <returns>SdmxParsingObject</returns>
        public static SdmxParsingObject Parse(IRestStructureQuery query, string ConstrainParameter)
        {
            try
            {
                if (query == null)
                    throw new SdmxException(typeof(SdmxParsingObject), FlyExceptionObject.FlyExceptionTypeEnum.ParsingQueryError);

                SdmxParsingObject po = new SdmxParsingObject(query.StructureReference.MaintainableStructureEnumType.EnumType)
                {
                    MaintainableId = query.StructureReference.MaintainableReference.MaintainableId,
                    AgencyId = query.StructureReference.MaintainableReference.AgencyId,
                    _version = query.StructureReference.MaintainableReference.Version,
                    QueryDetail = query.StructureQueryMetadata.StructureQueryDetail.EnumType,
                    References = query.StructureQueryMetadata.StructureReferenceDetail.EnumType,
                    SpecificReference = query.StructureQueryMetadata.SpecificStructureReference != null ? new List<SdmxStructureType>() { query.StructureQueryMetadata.SpecificStructureReference } : null,
                };


                if (!string.IsNullOrEmpty(ConstrainParameter))
                {
                    string dataflowCode = null;
                    string dataflowAgency = null;
                    string dataflowVersion = null;
                    string ConceptCode = null;
                    if (RedirectForCodelistConstrain.ParseContrainReferences(ConstrainParameter, ref dataflowCode, ref dataflowAgency, ref dataflowVersion, ref ConceptCode))
                    {
                        po.ConstrainDataFlow = dataflowCode;
                        po.ConstrainDataFlowAgency = dataflowAgency;
                        po.ConstrainDataFlowVersion = dataflowVersion;
                        po.ConstrainConcept = ConceptCode;
                    }
                }

                return po;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(typeof(SdmxParsingObject), FlyExceptionObject.FlyExceptionTypeEnum.ParsingQueryError, ex);
            }
        }

        /// <summary>
        /// Check if AgencyId of request is equals to AgencyId configured in File Config
        /// </summary>
        public void PreliminarCheck()
        {
            if (!string.IsNullOrEmpty(this.ConstrainDataFlow) && this.AgencyId.ToUpper() == "MA")
            {
                //Tutto OK casi Speciali
                return;
            }

            if (FlyConfiguration.OnTheFlyVersion == OnTheFlyVersionEnum.OnTheFly2)
                return;

            //Controlli Preliminari
            //Controllo se il QueryAgencyId è diverso da quello in configurazione non ritorno nessun dato
            if (!string.IsNullOrEmpty(this._version) && !new List<string>() { "*", "ALL", "LATEST" }.Contains(this._version) && !this._version.Equals(FlyConfiguration.Version))
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DifferentVersion);

            //Controlli Preliminari
            //Controllo se il QueryAgencyId è diverso da quello in configurazione non ritorno nessun dato
            if (!string.IsNullOrEmpty(this.AgencyId) && this.AgencyId != "*" && !this.AgencyId.Trim().ToUpper().Equals(FlyConfiguration.MainAgencyId.Trim().ToUpper()))
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DifferentAgency);

        }

        #endregion



        /// <summary>
        /// Create a Clone of <see cref="ISdmxParsingObject"/> but with isReferenceOf to True for declare that the structure is a reference
        /// </summary>
        /// <returns></returns>
        public ISdmxParsingObject CloneForReferences()
        {
            SdmxParsingObject newParsing = (SdmxParsingObject)this.Clone();
            newParsing.MaintainableId = null;
            newParsing.isReferenceOf = true;
            return newParsing;
        }

        /// <summary>
        /// a clone of <see cref="SdmxParsingObject"/>
        /// </summary>
        /// <returns>the <see cref="SdmxParsingObject"/> cloned</returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }


        /// <summary>
        /// Check if agency have a consistent Value
        /// </summary>
        /// <returns>return false if empty or "*", "MA"</returns>
        public bool isValidAgency()
        {
            return !string.IsNullOrEmpty(this.AgencyId) && this.AgencyId != "*" && this.AgencyId != "MA";
        }

        /// <summary>
        /// Check if version have a consistent Value
        /// </summary>
        /// <returns>return false if empty or "*", "ALL", "LATEST"</returns>
        public bool isValidVersion()
        {
            return !string.IsNullOrEmpty(this._version) && !new List<string>() { "*", "ALL", "LATEST" }.Contains(this._version);
        }
    }
}
