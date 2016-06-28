using FlyEngine.Manager;
using FlyController;
using FlyController.Builder;
using FlyController.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;

using Org.Sdmxsource.Sdmx.Api.Manager.Parse;
using Org.Sdmxsource.Sdmx.Api.Manager.Retrieval;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Mapping;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.MetadataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Process;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Registry;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.Structureparser.Manager.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FlyController.Model.Error;
using Org.Sdmx.Resources.SdmxMl.Schemas.V21.Common;
using Org.Sdmxsource.Sdmx.Util.Objects.Reference;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Objects.DataStructure;
using FlyMapping.Model;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.DataStructure;

namespace FlyEngine.Model
{
    /// <summary>
    /// Implementation of interface ISdmxObjectRetrievalManager
    /// Retrieval Manager contains DataStructure information
    /// </summary>
    public class RetrievalManager : ISdmxObjectRetrievalManager
    {
        #region Fields

        /// <summary>
        ///     The parsing manager.
        /// </summary>
        private readonly IStructureParsingManager _parsingManager = new StructureParsingManager(SdmxSchemaEnumType.VersionTwo);

        /// <summary>
        ///     The sdmx objects.
        /// </summary>
        private readonly ISdmxObjects _sdmxObjects;

        #endregion

        /// <summary>
        /// Dataflow code
        /// </summary>
        public string DataFlowID { get; set; }
        /// <summary>
        /// Dataset Title
        /// </summary>
        public string DataFlowTitle { get; set; }
        /// <summary>
        /// DataStructure 
        /// </summary>
        public DataStructureObjectImpl _dsd { get; set; }
        /// <summary>
        /// Sdmx Version
        /// </summary>
        public SdmxSchemaEnumType VersionType { get; set; }

        /// <summary>
        /// LastUpdate parameter request only data from this date onwards
        /// </summary>
        public string _TimeStamp { get; set; }
        #region Constructors and Destructors



        /// <summary>
        /// Initialie a new instance of the <see cref="RetrievalManager"/> class.
        /// </summary>
        /// <param name="_dataFlowID">
        /// Dataflow ID
        /// </param>     
        /// <param name="_versionType">Sdmx Version</param>     
        public RetrievalManager(string _dataFlowID, SdmxSchemaEnumType _versionType)
        {
            try
            {
                this.DataFlowID = _dataFlowID;
                this.VersionType = _versionType;
                DataStructureEngineObject ds = new DataStructureEngineObject();
                ISDMXObjectBuilder Builder = ds.CreateBuilder(new SdmxParsingObject(SdmxStructureEnumType.Dataflow)
                {
                    MaintainableId = DataFlowID,
                    References = StructureReferenceDetailEnumType.Specific,
                    SpecificReference = new List<SdmxStructureType>()
                    {
                        SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Dsd),
                        SdmxStructureType.GetFromEnum(SdmxStructureEnumType.ConceptScheme),
                    },
                    QueryDetail = StructureQueryDetailEnumType.Full,
                }, _versionType);

                Builder.Build();
                Builder.AddReferences();

                if (Builder._KeyFamily == null || Builder._KeyFamily.Count == 0)
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.InternalError, new Exception(string.Format("Dsd Not found for Dataflow code: {0}", DataFlowID)));

                //1DF = 1DSD
                this._dsd = Builder._KeyFamily[0];


                if (Builder._Dataflows.Count > 0)
                    this.DataFlowTitle = Builder._Dataflows[0].Name;
                else
                    throw new Exception("No Dataflow Found");
                this._sdmxObjects = Builder.CreateDSD();


                this.DataFlowID = Builder._Dataflows[0].Id;
                GetGroups();
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.InternalError, ex);
                //non sono riuscito a cambiare il nome che è arrivato dalla query con quello effettivo della dsd
            }
        }

        #endregion

        #region GroupsManagement

        /// <summary>
        /// List of <see cref="DataGroupObject"/>
        /// </summary>
        public List<DataGroupObject> Groups { get; set; }


        private void GetGroups()
        {
            Groups = new List<DataGroupObject>();
            if (_dsd.Groups == null || _dsd.Groups.Count == 0)
                return;
            IGroupsManager gpm = MappingConfiguration.GroupFactory(this._TimeStamp, this.VersionType);

            foreach (IGroupMutableObject group in _dsd.Groups)
            {
                List<string> DimensionRef = (List<string>)group.DimensionRef;
                List<string> AttributeRef = new List<string>();

                foreach (var Attr in _dsd.Attributes)
                {
                    if (Attr.AttachmentLevel == AttributeAttachmentLevel.Group && Attr.AttachmentGroup == group.Id)
                        AttributeRef.Add(Attr.Id);
                }
                Groups.AddRange(gpm.GetGroups(this.DataFlowID, group.Id, DimensionRef, AttributeRef));
            }

        }
        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Gets the agency with the given Id
        /// </summary>
        /// <param name="id">
        /// The id parameter.
        /// </param>
        /// <returns>
        /// The <see cref="IAgency"/> .
        /// </returns>
        public IAgency GetAgency(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns AgencySchemeObjects that match the parameters in the ref object.  If the ref object is null or
        /// has no attributes set, then this will be interpreted as a search for all AgencySchemeObjects
        /// </summary>
        /// <param name="xref">
        /// The reference object defining the search parameters, can be empty or null
        /// </param>
        /// <param name="returnStub">
        /// If true then a stub object will be returned
        /// </param>
        /// <returns>
        /// Set of sdmxObjects that match the search criteria
        /// </returns>
        public virtual ISet<IAgencyScheme> GetAgencySchemeObjects(IMaintainableRefObject xref, bool returnStub)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Returns the categorisation.
        /// </summary>
        /// <param name="xref">
        /// The maintainable object reference.
        /// </param>
        /// <returns>
        /// The <see cref="ICategorisationObject"/>.
        /// </returns>
        public ICategorisationObject GetCategorisation(IMaintainableRefObject xref)
        {
            ISet<ICategorisationObject> objects = this._sdmxObjects.GetCategorisations(xref);
            return objects.FirstOrDefault();
        }


        /// <summary>
        /// Returns the categorisation beans.
        /// </summary>
        /// <param name="xref">
        /// The maintainable object reference.
        /// </param>
        /// <param name="returnStub">
        /// If true then a stub object will be returned
        /// </param>
        /// <returns>
        /// The set of requested objects.
        /// </returns>
        public ISet<ICategorisationObject> GetCategorisationObjects(IMaintainableRefObject xref, bool returnStub)
        {
            return this._sdmxObjects.GetCategorisations(xref);
        }

        /// <summary>
        /// Returns the category scheme.
        /// </summary>
        /// <param name="xref">
        /// The maintainable object reference.
        /// </param>
        /// <returns>
        /// The <see cref="ICategorySchemeObject"/>.
        /// </returns>
        public ICategorySchemeObject GetCategoryScheme(IMaintainableRefObject xref)
        {
            ISet<ICategorySchemeObject> objects = this._sdmxObjects.GetCategorySchemes(xref);
            return objects.FirstOrDefault();
        }



        /// <summary>
        /// Returns the category scheme beans.
        /// </summary>
        /// <param name="xref">
        /// The maintainable object reference.
        /// </param>
        /// <param name="returnLatest">
        /// If true then the latest version is returned, regardless of whether version information is supplied
        /// </param>
        /// <param name="returnStub">
        /// If true then a stub object will be returned
        /// </param>
        /// <returns>
        /// The set of requested objects.
        /// </returns>
        public ISet<ICategorySchemeObject> GetCategorySchemeObjects(IMaintainableRefObject xref, bool returnLatest, bool returnStub)
        {
            return this._sdmxObjects.GetCategorySchemes(xref);
        }


        /// <summary>
        /// Returns the codelist.
        /// </summary>
        /// <param name="xref">
        /// The maintainable object reference.
        /// </param>
        /// <returns>
        /// The <see cref="ICodelistObject"/>.
        /// </returns>
        public ICodelistObject GetCodelist(IMaintainableRefObject xref)
        {
            ISet<ICodelistObject> objects = this._sdmxObjects.GetCodelists(xref);
            return objects.FirstOrDefault();
        }



        /// <summary>
        /// Returns the codelist beans.
        /// </summary>
        /// <param name="xref">
        /// The maintainable object reference.
        /// </param>
        /// <param name="returnLatest">
        /// If true then the latest version is returned, regardless of whether version information is supplied
        /// </param>
        /// <param name="returnStub">
        /// If true then a stub object will be returned
        /// </param>
        /// <returns>
        /// The set of requested objects.
        /// </returns>
        public ISet<ICodelistObject> GetCodelistObjects(IMaintainableRefObject xref, bool returnLatest, bool returnStub)
        {
            return this._sdmxObjects.GetCodelists(xref);
        }



        /// <summary>
        /// Get all concepts contains in dsd
        /// </summary>
        /// <returns>List of ConceptCore </returns>
        internal List<IConceptObjectImpl> GetAllConceptsImpl()
        {
            if (_dsd == null)
                return null;

            foreach (IConceptObjectImpl con in _dsd._Concepts)
            {
                IAttributeObject ao = _dsd.Immutated.Attributes.Where(att => att.Id == con.ConceptObjectCode).FirstOrDefault();
                if (ao != null)
                    ((AttributeConcept)con).AttributeAttachmentLevelType = ao.AttachmentLevel;
            }


            return this._dsd._Concepts;

            //List<IConceptObjectImpl> concepts = new List<IConceptObjectImpl>();
            //foreach (IComponent comp in _dsd.Components)
            //{
            //    switch (comp.StructureType.EnumType)
            //    {
            //        case SdmxStructureEnumType.Dimension:

            //            if (((DimensionCore)comp).FrequencyDimension ||
            //                (((DimensionCore)comp).ConceptRole != null
            //                && ((DimensionCore)comp).ConceptRole.Count(cr => cr.IdentifiableIds != null && cr.IdentifiableIds.Contains("FREQUENCY")) > 0))
            //                concepts.Add(new DimensionConcept(comp.Id, new List<SdmxObjectNameDescription>() { new SdmxObjectNameDescription() { Lingua = "en", Name = comp.Id } }) { DimensionType = DimensionTypeEnum.Frequency });
            //            else
            //                concepts.Add(new DimensionConcept(comp.Id, new List<SdmxObjectNameDescription>() { new SdmxObjectNameDescription() { Lingua = "en", Name = comp.Id } }) { DimensionType = DimensionTypeEnum.Dimension });
            //            break;
            //        case SdmxStructureEnumType.TimeDimension:
            //            concepts.Add(new DimensionConcept(comp.Id, new List<SdmxObjectNameDescription>() { new SdmxObjectNameDescription() { Lingua = "en", Name = comp.Id } }) { DimensionType = DimensionTypeEnum.Time });
            //            break;
            //        case SdmxStructureEnumType.DataAttribute:
            //            AttributeConcept _concept = new AttributeConcept(comp.Id, new List<SdmxObjectNameDescription>() { new SdmxObjectNameDescription() { Lingua = "en", Name = comp.Id } });
            //            _concept.AttributeAttachmentLevelType = ((AttributeObjectCore)comp).AttachmentLevel;
            //            _concept.AssignmentStatusType = (AssignmentStatusTypeEnum)Enum.Parse(typeof(AssignmentStatusTypeEnum), ((AttributeObjectCore)comp).AssignmentStatus);
            //            concepts.Add(_concept);
            //            break;
            //        case SdmxStructureEnumType.PrimaryMeasure:
            //            concepts.Add(new AttributeConcept(comp.Id, new List<SdmxObjectNameDescription>() { new SdmxObjectNameDescription() { Lingua = "en", Name = comp.Id } }) { IsValueAttribute = true });
            //            break;
            //    }

            //}

            //return concepts;
        }

        /// <summary>
        /// Returns the concept scheme.
        /// </summary>
        /// <param name="xref">
        /// The maintainable object reference.
        /// </param>
        /// <returns>
        /// The <see cref="IConceptSchemeObject"/>.
        /// </returns>
        public IConceptSchemeObject GetConceptScheme(IMaintainableRefObject xref)
        {
            ISet<IConceptSchemeObject> objects = this._sdmxObjects.GetConceptSchemes(xref);
            return objects.FirstOrDefault();
        }

        /// <summary>
        /// Returns ConceptSchemeObjects that match the parameters in the ref object.  If the ref object is null or
        /// has no attributes set, then this will be interpreted as a search for all ConceptSchemeObjects
        /// </summary>
        /// <param name="xref">
        /// The reference object defining the search parameters, can be empty or null
        /// </param>
        /// <param name="returnLatest">
        /// If true then the latest versions of the structures that match the query will be returned.  If version information is supplied
        /// then it will be ignored
        /// </param>
        /// <param name="returnStub">
        /// If true then a stub object will be returned
        /// </param>
        /// <returns>
        /// Set of sdmxObjects that match the search criteria
        /// </returns>
        public virtual ISet<IConceptSchemeObject> GetConceptSchemeObjects(IMaintainableRefObject xref, bool returnLatest, bool returnStub)
        {
            return this._sdmxObjects.GetConceptSchemes(xref);
        }


        /// <summary>
        /// Returns the content constraint.
        /// </summary>
        /// <param name="xref">
        /// The maintainable object reference.
        /// </param>
        /// <returns>
        /// The <see cref="IContentConstraintObject"/>.
        /// </returns>
        public IContentConstraintObject GetContentConstraint(IMaintainableRefObject xref)
        {
            ISet<IContentConstraintObject> objects = this._sdmxObjects.GetContentConstraintObjects(xref);
            return objects.FirstOrDefault();
        }


        /// <summary>
        /// Returns the content constraints.
        /// </summary>
        /// <param name="xref">
        /// The maintainable object reference.
        /// </param>
        /// <param name="returnLatest">
        /// If true then the latest version is returned, regardless of whether version information is supplied
        /// </param>
        /// <param name="returnStub">
        /// If true then a stub object will be returned
        /// </param>
        /// <returns>
        /// The set of requested objects.
        /// </returns>
        public ISet<IContentConstraintObject> GetContentConstraints(IMaintainableRefObject xref, bool returnLatest, bool returnStub)
        {
            return this._sdmxObjects.GetContentConstraintObjects(xref);
        }




        /// <summary>
        /// Returns the data structure.
        /// </summary>
        /// <param name="xref">
        /// The maintainable object reference.
        /// </param>
        /// <returns>
        /// The <see cref="DataStructureObjectImpl"/>.
        /// </returns>
        public IDataStructureObject GetDataStructure(IMaintainableRefObject xref)
        {
            ISet<IDataStructureObject> objects = this._sdmxObjects.GetDataStructures(xref);
            return objects.FirstOrDefault();
        }


        /// <summary>
        /// Returns the data structure beans.
        /// </summary>
        /// <param name="xref">
        /// The maintainable object reference.
        /// </param>
        /// <param name="returnLatest">
        /// If true then the latest version is returned, regardless of whether version information is supplied
        /// </param>
        /// <param name="returnStub">
        /// If true then a stub object will be returned
        /// </param>
        /// <returns>
        /// The set of requested objects.
        /// </returns>
        public ISet<IDataStructureObject> GetDataStructureObjects(IMaintainableRefObject xref, bool returnLatest, bool returnStub)
        {
            return this._sdmxObjects.GetDataStructures(xref);
        }


      
        /// <summary>
        /// Returns the dataflow.
        /// </summary>
        /// <param name="xref">
        /// The maintainable object reference.
        /// </param>
        /// <returns>
        /// The <see cref="IDataflowObject"/>.
        /// </returns>
        public IDataflowObject GetDataflow(IMaintainableRefObject xref)
        {
            ISet<IDataflowObject> dataflowObjects = this._sdmxObjects.GetDataflows(xref);
            return dataflowObjects.FirstOrDefault();
        }

        /// <summary>
        /// Returns the dataflow.
        /// </summary>
        /// <returns>
        /// The <see cref="IDataflowObject"/>.
        /// </returns>
        public IDataflowObject GetDataflowFromSdmxObject()
        {
            ISet<IDataflowObject> dataflowObjects = this._sdmxObjects.Dataflows;
            return dataflowObjects.FirstOrDefault();
        }


        /// <summary>
        /// Returns the dataflow beans.
        /// </summary>
        /// <param name="xref">
        /// The maintainable object reference.
        /// </param>
        /// <param name="returnLatest">
        /// If true then the latest version is returned, regardless of whether version information is supplied
        /// </param>
        /// <param name="returnStub">
        /// If true then a stub object will be returned
        /// </param>
        /// <returns>
        /// The set of requested objects.
        /// </returns>
        public ISet<IDataflowObject> GetDataflowObjects(IMaintainableRefObject xref, bool returnLatest, bool returnStub)
        {
            return this._sdmxObjects.GetDataflows(xref);
        }



        /// <summary>
        /// Returns the hierarchic code list.
        /// </summary>
        /// <param name="xref">
        /// The maintainable object reference.
        /// </param>
        /// <returns>
        /// The <see cref="IHierarchicalCodelistObject"/>.
        /// </returns>
        public IHierarchicalCodelistObject GetHierarchicCodeList(IMaintainableRefObject xref)
        {
            ISet<IHierarchicalCodelistObject> objects = this._sdmxObjects.GetHierarchicalCodelists(xref);
            return objects.FirstOrDefault();
        }


        /// <summary>
        /// Returns the hierarchic code list beans.
        /// </summary>
        /// <param name="xref">
        /// The maintainable object reference.
        /// </param>
        /// <param name="returnLatest">
        /// If true then the latest version is returned, regardless of whether version information is supplied
        /// </param>
        /// <param name="returnStub">
        /// If true then a stub object will be returned
        /// </param>
        /// <returns>
        /// The set of requested objects.
        /// </returns>
        public ISet<IHierarchicalCodelistObject> GetHierarchicCodeListObjects(IMaintainableRefObject xref, bool returnLatest, bool returnStub)
        {
            return this._sdmxObjects.GetHierarchicalCodelists(xref);
        }



        /// <summary>
        /// Gets a maintainable defined by the StructureQueryObject parameter.
        ///     <p/>
        ///     Expects only ONE maintainable to be returned from this query
        /// </summary>
        /// <param name="sRref">
        /// The query.
        /// </param>
        /// <param name="returnStub">
        /// If true then a stub object will be returned
        /// </param>
        /// <returns>
        /// The <see cref="IMaintainableObject"/> .
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Not implemented.
        /// </exception>
        public IMaintainableObject GetMaintainable(IStructureReference sRref, bool returnStub)
        {
            IMaintainableRefObject xref = sRref.MaintainableReference;
            switch (sRref.MaintainableStructureEnumType.EnumType)
            {
                case SdmxStructureEnumType.ContentConstraint:
                    return this.GetContentConstraint(xref);
                case SdmxStructureEnumType.Categorisation:
                    return this.GetCategorisation(xref);
                case SdmxStructureEnumType.CategoryScheme:
                    return this.GetCategoryScheme(xref);
                case SdmxStructureEnumType.CodeList:
                    return this.GetCodelist(xref);
                case SdmxStructureEnumType.ConceptScheme:
                    return this.GetConceptScheme(xref);
                case SdmxStructureEnumType.Dataflow:
                    return this.GetDataflow(xref);
                case SdmxStructureEnumType.HierarchicalCodelist:
                    return this.GetHierarchicCodeList(xref);
                case SdmxStructureEnumType.Dsd:
                    return this.GetDataStructure(xref);
                default:
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetMaintainableFromSdmxObject, new Exception(sRref.MaintainableStructureEnumType.EnumType.ToString()));
            }
        }


        #endregion



        /// <summary>
        /// Gets a maintainable that is of the given type, determined by T, and matches the reference parameters in the IMaintainableRefObject.
        ///     <p/>
        ///     Expects only ONE maintainable to be returned from this query
        /// </summary>
        /// <param name="maintainableReference">
        /// The reference object that must match on the returned structure. If version information is missing, then latest version is assumed
        /// </param>
        /// <param name="returnStub">
        /// If true then a stub object will be returned
        /// </param>
        /// /// <param name="returnLatest">
        /// If true then the latest version is returned, regardless of whether version information is supplied
        /// </param>
        /// <returns>
        /// The <see cref="IMaintainableObject"/> .
        /// </returns>
        public T GetMaintainableObject<T>(IMaintainableRefObject maintainableReference, bool returnStub, bool returnLatest) where T : IMaintainableObject
        {
            return ExtractFromSet(GetMaintainableObjects<T>(maintainableReference, returnStub, returnLatest));
        }

        /// <summary>
        /// If the set is of size 1, then returns the element in the set.
        ///     Returns null if the set is null or of size 0
        /// </summary>
        /// <typeparam name="T">
        /// The maintainable type
        /// </typeparam>
        /// <param name="set">
        /// set of elements
        /// </param>
        /// <returns>
        /// The first element of the set.
        ///     Throws SdmxException if the set contains more then 1 element
        /// </returns>
        private static T ExtractFromSet<T>(ICollection<T> set) where T : IMaintainableObject
        {
            if (set.Count == 1)
            {
                return set.First();
            }

            throw new SdmxException(typeof(RetrievalManager), FlyExceptionObject.FlyExceptionTypeEnum.GetMaintainableFromSdmxObject, new Exception("Did not expect more then 1 structure from query, got " + set.Count + " structures."));
        }

        /// <summary>
        /// Gets a maintainable that is of the given type, determined by T, and matches the reference parameters in the IMaintainableRefObject.
        ///     <p/>
        ///     Expects only ONE maintainable to be returned from this query
        /// </summary>
        /// <param name="maintainableReference">
        /// The reference object that must match on the returned structure. If version information is missing, then latest version is assumed
        /// </param>
        /// <returns>
        /// The <see cref="IMaintainableObject"/> .
        /// </returns>
        public T GetMaintainableObject<T>(IMaintainableRefObject maintainableReference) where T : IMaintainableObject
        {
            return GetMaintainableObject<T>(maintainableReference, false, false);
        }

        /// <summary>
        /// Gets a maintainable defined by the StructureQueryObject parameter.
        ///     <p/>
        ///     Expects only ONE maintainable to be returned from this query
        /// </summary>
        /// <param name="structureReference">
        /// The reference object defining the search parameters, this is expected to uniquely identify one MaintainableObject
        /// </param>
        /// <param name="returnStub">
        /// If true then a stub object will be returned
        /// </param>
        /// <param name="returnLatest">
        /// If true then the latest version is returned, regardless of whether version information is supplied
        /// </param>
        /// <returns>
        /// The <see cref="IMaintainableObject"/> .
        /// </returns>
        public IMaintainableObject GetMaintainableObject(IStructureReference structureReference, bool returnStub, bool returnLatest)
        {
            return GetMaintainable(structureReference, returnStub);
        }

        /// <summary>
        /// Gets a maintainable defined by the StructureQueryObject parameter.
        ///     <p/>
        ///     Expects only ONE maintainable to be returned from this query
        /// </summary>
        /// <param name="structureReference">
        /// The reference object defining the search parameters, this is expected to uniquely identify one MaintainableObject
        /// </param>
        /// <returns>
        /// The <see cref="IMaintainableObject"/> .
        /// </returns>
        public IMaintainableObject GetMaintainableObject(IStructureReference structureReference)
        {
            return GetMaintainable(structureReference, false);
        }

        /// <summary>
        /// Gets a set of all MaintainableObjects of type T that match the reference parameters in the IMaintainableRefObject argument.
        /// An empty Set will be returned if there are no matches to the query
        /// </summary>
        /// <typeparam name="T">The type of the maintainable. It is constraint  </typeparam>
        /// <param name="maintainableInterface">The maintainable interface.</param>
        /// <param name="maintainableReference">Contains the identifiers of the structures to returns, can include wild-carded values (null indicates a wild-card).</param>
        /// <returns>
        /// The set of <see cref="IMaintainableObject" /> .
        /// </returns>
        /// <remarks>This method exists only for compatibility reasons with Java implementation of this interface which uses raw types and unchecked generics.</remarks>
        public ISet<T> GetMaintainableObjects<T>(Type maintainableInterface, IMaintainableRefObject maintainableReference) where T : ISdmxObject
        {
            SdmxStructureType type = SdmxStructureType.ParseClass(maintainableInterface);
            return new HashSet<T>(GetMaintainablesOfType<IMaintainableObject>(type, maintainableReference, false, false).Cast<T>());
        }

        /// <summary>
        ///     An empty Set will be returned if there are no matches to the query
        /// </summary>
        /// <param name="xType">
        /// Contains sdmx structure enum type
        /// </param>
        /// <param name="maintainableReference">
        /// Contains the identifiers of the structures to returns, can include widcarded values (null indicates a wildcard). 
        /// </param>
        /// <param name="returnStub">
        /// If true then a stub object will be returned
        /// </param>
        /// /// <param name="returnLatest">
        /// If true then the latest version is returned, regardless of whether version information is supplied
        /// </param>
        /// <returns>
        /// The set of <see cref="IMaintainableObject"/> .
        /// </returns>
        private ISet<T> GetMaintainablesOfType<T>(SdmxStructureEnumType xType, IMaintainableRefObject maintainableReference, bool returnStub, bool returnLatest) where T : IMaintainableObject
        {
            ISet<T> returnSet;

            if (returnLatest)
                maintainableReference = new MaintainableRefObjectImpl(maintainableReference.AgencyId, maintainableReference.MaintainableId, null);

            switch (xType)
            {
                case SdmxStructureEnumType.AgencyScheme:
                    returnSet = new HashSet<T>(GetMaintainableObjects<IAgencyScheme>(maintainableReference, returnStub, returnLatest).Cast<T>());
                    break;
                case SdmxStructureEnumType.DataConsumerScheme:
                    returnSet = new HashSet<T>(GetMaintainableObjects<IDataConsumerScheme>(maintainableReference, returnStub, returnLatest).Cast<T>());
                    break;
                case SdmxStructureEnumType.AttachmentConstraint:
                    returnSet = new HashSet<T>(GetMaintainableObjects<IAttachmentConstraintObject>(maintainableReference, returnStub, returnLatest).Cast<T>());
                    break;
                case SdmxStructureEnumType.ContentConstraint:
                    returnSet = new HashSet<T>(GetMaintainableObjects<IContentConstraintObject>(maintainableReference, returnStub, returnLatest).Cast<T>());
                    break;
                case SdmxStructureEnumType.DataProviderScheme:
                    returnSet = new HashSet<T>(GetMaintainableObjects<IDataProviderScheme>(maintainableReference, returnStub, returnLatest).Cast<T>());
                    break;
                case SdmxStructureEnumType.Categorisation:
                    returnSet = new HashSet<T>(GetMaintainableObjects<ICategorisationObject>(maintainableReference, returnStub, returnLatest).Cast<T>());
                    break;
                case SdmxStructureEnumType.CategoryScheme:
                    returnSet = new HashSet<T>(GetMaintainableObjects<ICategorySchemeObject>(maintainableReference, returnStub, returnLatest).Cast<T>());
                    break;
                case SdmxStructureEnumType.CodeList:
                    returnSet = new HashSet<T>(GetMaintainableObjects<ICodelistObject>(maintainableReference, returnStub, returnLatest).Cast<T>());
                    break;
                case SdmxStructureEnumType.ConceptScheme:
                    returnSet = new HashSet<T>(GetMaintainableObjects<IConceptSchemeObject>(maintainableReference, returnStub, returnLatest).Cast<T>());
                    break;
                case SdmxStructureEnumType.Dataflow:
                    returnSet = new HashSet<T>(GetMaintainableObjects<IDataflowObject>(maintainableReference, returnStub, returnLatest).Cast<T>());
                    break;
                case SdmxStructureEnumType.HierarchicalCodelist:
                    returnSet = new HashSet<T>(GetMaintainableObjects<IHierarchicalCodelistObject>(maintainableReference, returnStub, returnLatest).Cast<T>());
                    break;
                case SdmxStructureEnumType.Dsd:
                    returnSet = new HashSet<T>(GetMaintainableObjects<IDataStructureObject>(maintainableReference, returnStub, returnLatest).Cast<T>());
                    break;
                case SdmxStructureEnumType.MetadataFlow:
                    returnSet = new HashSet<T>(GetMaintainableObjects<IMetadataFlow>(maintainableReference, returnStub, returnLatest).Cast<T>());
                    break;
                case SdmxStructureEnumType.Msd:
                    returnSet = new HashSet<T>(GetMaintainableObjects<IMetadataStructureDefinitionObject>(maintainableReference, returnStub, returnLatest).Cast<T>());
                    break;
                case SdmxStructureEnumType.OrganisationUnitScheme:
                    returnSet = new HashSet<T>(GetMaintainableObjects<IOrganisationUnitSchemeObject>(maintainableReference, returnStub, returnLatest).Cast<T>());
                    break;
                case SdmxStructureEnumType.Process:
                    returnSet = new HashSet<T>(GetMaintainableObjects<IProcessObject>(maintainableReference, returnStub, returnLatest).Cast<T>());
                    break;
                case SdmxStructureEnumType.ReportingTaxonomy:
                    returnSet = new HashSet<T>(GetMaintainableObjects<IReportingTaxonomyObject>(maintainableReference, returnStub, returnLatest).Cast<T>());
                    break;
                case SdmxStructureEnumType.StructureSet:
                    returnSet = new HashSet<T>(GetMaintainableObjects<IStructureSetObject>(maintainableReference, returnStub, returnLatest).Cast<T>());
                    break;
                case SdmxStructureEnumType.ProvisionAgreement:
                    returnSet = new HashSet<T>(GetMaintainableObjects<IProvisionAgreementObject>(maintainableReference, returnStub, returnLatest).Cast<T>());
                    break;
                default:
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetMaintainableFromSdmxObject, new Exception(xType.ToString()));
            }

            //if (returnStub && _serviceRetrievalManager != null)
            //{
            //    ISet<T> stubSet = new HashSet<T>();
            //    foreach (T returnItm in returnSet)
            //    {
            //        if (returnItm.IsExternalReference.IsTrue)
            //            stubSet.Add(returnItm);
            //        else
            //            stubSet.Add((T)_serviceRetrievalManager.CreateStub(returnItm));
            //    }
            //    returnSet = stubSet;
            //}
            return returnSet;
        }

        /// <summary>
        ///     An empty Set will be returned if there are no matches to the query
        /// </summary>
        /// <param name="maintainableReference">
        /// Contains the identifiers of the structures to returns, can include widcarded values (null indicates a wildcard). 
        /// </param>
        /// <param name="returnStub">
        /// If true then a stub object will be returned
        /// </param>
        /// /// <param name="returnLatest">
        /// If true then the latest version is returned, regardless of whether version information is supplied
        /// </param>
        /// <returns>
        /// The set of <see cref="IMaintainableObject"/> .
        /// </returns>
        public ISet<T> GetMaintainableObjects<T>(IMaintainableRefObject maintainableReference, bool returnStub, bool returnLatest) where T : IMaintainableObject
        {
            ISet<T> returnSet;

            if (returnLatest)
                maintainableReference = new MaintainableRefObjectImpl(maintainableReference.AgencyId, maintainableReference.MaintainableId, null);

            SdmxStructureType type = SdmxStructureType.ParseClass(typeof(T));
            IStructureReference sRef = new StructureReferenceImpl(maintainableReference, type);
            switch (sRef.TargetReference.EnumType)
            {
                //case SdmxStructureEnumType.AgencyScheme:
                //    returnSet = new HashSet<T>(base.GetAgencySchemeObjects(maintainableReference, returnStub).Cast<T>());
                //    break;
                //case SdmxStructureEnumType.DataConsumerScheme:
                //    returnSet = new HashSet<T>(this.GetDataConsumerSchemeObjects(maintainableReference, returnStub).Cast<T>());
                //    break;
                //case SdmxStructureEnumType.AttachmentConstraint:
                //    returnSet = new HashSet<T>(this.GetAttachmentConstraints(maintainableReference, returnLatest, returnStub).Cast<T>());
                //    break;
                case SdmxStructureEnumType.ContentConstraint:
                    returnSet = new HashSet<T>(this.GetContentConstraints(maintainableReference, returnLatest, returnStub).Cast<T>());
                    break;
                //case SdmxStructureEnumType.DataProviderScheme:
                //    returnSet = new HashSet<T>(this.GetDataProviderSchemeObjects(maintainableReference, returnStub).Cast<T>());
                //    break;
                case SdmxStructureEnumType.Categorisation:
                    returnSet = new HashSet<T>(this.GetCategorisationObjects(maintainableReference, returnStub).Cast<T>());
                    break;
                case SdmxStructureEnumType.CategoryScheme:
                    returnSet = new HashSet<T>(this.GetCategorySchemeObjects(maintainableReference, returnLatest, returnStub).Cast<T>());
                    break;
                case SdmxStructureEnumType.CodeList:
                    returnSet = new HashSet<T>(this.GetCodelistObjects(maintainableReference, returnLatest, returnStub).Cast<T>());
                    break;
                case SdmxStructureEnumType.ConceptScheme:
                    returnSet = new HashSet<T>(this.GetConceptSchemeObjects(maintainableReference, returnLatest, returnStub).Cast<T>());
                    break;
                case SdmxStructureEnumType.Dataflow:
                    returnSet = new HashSet<T>(this.GetDataflowObjects(maintainableReference, returnLatest, returnStub).Cast<T>());
                    break;
                case SdmxStructureEnumType.HierarchicalCodelist:
                    returnSet = new HashSet<T>(this.GetHierarchicCodeListObjects(maintainableReference, returnLatest, returnStub).Cast<T>());
                    break;
                case SdmxStructureEnumType.Dsd:
                    returnSet = new HashSet<T>(this.GetDataStructureObjects(maintainableReference, returnLatest, returnStub).Cast<T>());
                    break;
                //case SdmxStructureEnumType.MetadataFlow:
                //    returnSet = new HashSet<T>(this.GetMetadataflowObjects(maintainableReference, returnLatest, returnStub).Cast<T>());
                //    break;
                //case SdmxStructureEnumType.Msd:
                //    returnSet = new HashSet<T>(this.GetMetadataStructureObjects(maintainableReference, returnLatest, returnStub).Cast<T>());
                //    break;
                //case SdmxStructureEnumType.OrganisationUnitScheme:
                //    returnSet = new HashSet<T>(this.GetOrganisationUnitSchemeObjects(maintainableReference, returnLatest, returnStub).Cast<T>());
                //    break;
                //case SdmxStructureEnumType.Process:
                //    returnSet = new HashSet<T>(this.GetProcessObjects(maintainableReference, returnLatest, returnStub).Cast<T>());
                //    break;
                //case SdmxStructureEnumType.ReportingTaxonomy:
                //    returnSet = new HashSet<T>(this.GetReportingTaxonomyObjects(maintainableReference, returnLatest, returnStub).Cast<T>());
                //    break;
                //case SdmxStructureEnumType.StructureSet:
                //    returnSet = new HashSet<T>(this.GetStructureSetObjects(maintainableReference, returnLatest, returnStub).Cast<T>());
                //    break;
                //case SdmxStructureEnumType.ProvisionAgreement:
                //    returnSet = new HashSet<T>(this.GetProvisionAgreementObjects(maintainableReference, returnLatest, returnStub).Cast<T>());
                //    break;
                default:
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.RetrivalParsingError, new Exception(sRef.TargetReference.EnumType.ToString()));
            }

            //if (returnStub && _serviceRetrievalManager != null)
            //{
            //    ISet<T> stubSet = new HashSet<T>();
            //    foreach (T returnItm in returnSet)
            //    {
            //        if (returnItm.IsExternalReference.IsTrue)
            //            stubSet.Add(returnItm);
            //        else
            //            stubSet.Add((T)_serviceRetrievalManager.CreateStub(returnItm));
            //    }
            //    returnSet = stubSet;
            //}
            return returnSet;
        }

        /// <summary>
        /// An empty Set will be returned if there are no matches to the query
        /// </summary>
        /// <param name="maintainableReference">
        /// Contains the identifiers of the structures to returns, can include widcarded values (null indicates a wildcard). 
        /// </param>
        /// <returns>
        /// The set of <see cref="IMaintainableObject"/> .
        /// </returns>
        public ISet<T> GetMaintainableObjects<T>(IMaintainableRefObject maintainableReference) where T : IMaintainableObject
        {
            return GetMaintainableObjects<T>(maintainableReference, false, false);
        }

        /// <summary>
        /// Gets a set of all MaintainableObjects of type T
        ///     <p/>
        ///     An empty Set will be returned if there are no matches to the query
        /// </summary>
        /// <returns>
        /// The set of <see cref="IMaintainableObject"/> .
        /// </returns>
        public ISet<T> GetMaintainableObjects<T>() where T : IMaintainableObject
        {
            return GetMaintainableObjects<T>(null);
        }

        /// <summary>
        /// Get all the maintainable that match the <paramref name="restquery"/>
        /// </summary>
        /// <param name="restquery">The REST structure query.</param>
        /// <returns>the maintainable that match the <paramref name="restquery"/></returns>
        public ISdmxObjects GetMaintainables(Org.Sdmxsource.Sdmx.Api.Model.Query.IRestStructureQuery restquery)
        {
            throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetMaintainableFromSdmxObject, null);
        }

        /// <summary>
        /// Gets the SDMX objects.
        /// </summary>
        /// <param name="structureReference">The <see cref="IStructureReference"/> which must not be null.</param>
        /// <param name="resolveCrossReferences">either 'do not resolve', 'resolve all' or 'resolve all excluding agencies'. If not set to 'do not resolve' then all the structures that are referenced by the resulting structures are also returned (and also their children).  This will be equivalent to descendants for a <c>RESTful</c> query..</param>
        /// <returns>Returns a <see cref="ISdmxObjects"/> container containing all the Maintainable Objects that match the query parameters as defined by the <paramref name="structureReference"/>.</returns>
        public ISdmxObjects GetSdmxObjects(IStructureReference structureReference, Org.Sdmxsource.Sdmx.Api.Model.ResolveCrossReferences resolveCrossReferences)
        {
            return _sdmxObjects;
        }

        /// <summary>
        /// Resolves an reference to a Object of type T, this will return the Object of the given type, throwing an exception if e
        ///     Object is not of type T
        /// </summary>
        /// <typeparam name="T">Generic type parameter.
        /// </typeparam>
        /// <param name="crossReference">Structure-reference object
        /// </param>
        /// <returns>
        /// SdmxException Not Implemented
        /// </returns>
        public T GetIdentifiableObject<T>(IStructureReference crossReference)
        {
            throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetMaintainableFromSdmxObject, null);

        }

        /// <summary>
        /// Resolves an reference to a Object of type T, this will return the Object of the given type, throwing an exception if either the
        ///     Object can not be resolved or if it is not of type T
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="crossReference">
        ///     Cross-reference object
        /// </param>
        /// <returns>
        /// SdmxException Not Implemented
        /// </returns>
        public T GetIdentifiableObject<T>(ICrossReference crossReference)
        {
            throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetMaintainableFromSdmxObject, null);

        }
        /// <summary>
        /// Resolves an identifiable reference
        /// </summary>
        /// <param name="crossReference"> Cross-reference object
        /// </param>
        /// <returns>
        /// SdmxException Not Implemented
        /// </returns>
        public IIdentifiableObject GetIdentifiableObject(ICrossReference crossReference)
        {
            throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetMaintainableFromSdmxObject, null);

        }

        /// <summary>
        /// Gets the identifiable objects.
        /// </summary>
        /// <typeparam name="T">The type of the identifiable objects to return.</typeparam>
        /// <param name="structureReference">The structure reference.</param>
        /// <returns>SdmxException Not Implemented</returns>
        public ISet<T> GetIdentifiableObjects<T>(IStructureReference structureReference) where T : IIdentifiableObject
        {
            throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetMaintainableFromSdmxObject, null);
        }


        internal IDataflowObject GetDataflow()
        {
            throw new NotImplementedException();
        }
    }
}
