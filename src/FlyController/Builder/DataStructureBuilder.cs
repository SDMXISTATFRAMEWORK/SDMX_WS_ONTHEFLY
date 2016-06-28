using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlyController.Model;
using FlyController;
using FlyController.Builder;
using FlyController.Model.Error;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Util.Objects.Reference;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;

namespace FlyController.Builder
{
    /// <summary>
    /// DataStructureBuilder Create a ImmutableInstance of DataStructure with all concepts and codelist Association
    /// </summary>
    public class DataStructureBuilder : ObjectBuilder
    {
        #region IObjectBuilder Property
        /// <summary>
        ///  Identificable Code
        /// </summary>
        public override string Code { get; set; }

        /// <summary>
        ///  Descriptions Names
        /// </summary>
        public override List<SdmxObjectNameDescription> Names { get; set; }


        #endregion

        /// <summary>
        /// list of all concepts
        /// </summary>
        public List<IConceptObjectImpl> _Concepts { get; set; }


        /// <summary>
        /// Create DataStructureBuilder Istance 
        /// </summary>
        /// <param name="concepts"> list of all concepts</param>
        /// <param name="parsingObject">Parsing Object <see cref="ISdmxParsingObject"/></param>
        /// <param name="versionTypeResp">Sdmx Version</param>
        public DataStructureBuilder(List<IConceptObjectImpl> concepts, ISdmxParsingObject parsingObject, SdmxSchemaEnumType versionTypeResp) :
            base(parsingObject, versionTypeResp)
        {
            _Concepts = concepts;
        }

       
        /// <summary>
        /// Create a ImmutableInstance of DataStructure (Keyfamily in sdmx 2.0)
        /// </summary>
        /// <param name="components">list of IComponentMutableObject that compose dsd</param>
        /// <param name="Groups">list of Groups (ever Null for OnTheFly version 1.0)</param>
        /// <param name="AgencyId">Agency Id</param>
        /// <param name="Version">Artefact Version</param>
        /// <returns>DataStructureObjectImpl</returns>
        public DataStructureObjectImpl BuildDataStructure(List<IComponentMutableObject> components, List<IGroupMutableObject> Groups, string AgencyId, string Version)
        {
            try
            {
                DataStructureObjectImpl dsd = new DataStructureObjectImpl();
                dsd._Concepts = this._Concepts;
                dsd.AgencyId = AgencyId;
                dsd.Version = Version;
                dsd.Id = this.Code;
                foreach (var nomi in this.Names)
                    dsd.AddName(nomi.Lingua, nomi.Name);

                if (!this.ParsingObject.ReturnStub)
                {
                    foreach (IComponentMutableObject comp in components)
                    {
                        if (comp.StructureType.EnumType == SdmxStructureEnumType.PrimaryMeasure)
                        {
                            dsd.AddPrimaryMeasure(comp.ConceptRef);
                        }
                        else if (comp is IAttributeMutableObject)
                        {
                            dsd.AddAttribute((IAttributeMutableObject)comp);
                        }
                        else if (comp is IDimensionMutableObject)
                        {
                            dsd.AddDimension((IDimensionMutableObject)comp);
                        }
                    }

                    if (Groups != null)
                    {
                        foreach (GroupMutableCore group in Groups)
                            dsd.AddGroup(group);
                    }

                }
                dsd.FinalStructure = TertiaryBool.ParseBoolean(true);

                if (this.ParsingObject.isReferenceOf || this.ParsingObject.ReturnStub)
                {
                    dsd.ExternalReference = TertiaryBool.ParseBoolean(true);
                    dsd.StructureURL = RetreivalStructureUrl.Get(this, dsd.Id, dsd.AgencyId, dsd.Version);
                }


                dsd.Immutated = dsd.ImmutableInstance;
                return dsd;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateImmutable, ex);
            }
        }
    }
}
