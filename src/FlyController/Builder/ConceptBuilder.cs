using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.ConceptScheme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FlyController;
using FlyController.Model;
using FlyController.Model.Error;

namespace FlyController.Builder
{
    /// <summary>
    /// ConceptBuilder Create a ImmutableInstance of ConceptScheme
    /// </summary>
    public class ConceptBuilder : ObjectBuilder
    {
        #region IObjectBuilder Property
        /// <summary>
        ///  Identificable Code
        /// </summary>
        public override string Code{ get; set; }
        
        /// <summary>
        ///  Descriptions Names
        /// </summary>
        public override List<SdmxObjectNameDescription> Names { get; set; }
       
        #endregion
      
        /// <summary>
        /// List of all Concept of Dataflow
        /// </summary>
        public List<IConceptObjectImpl> Concepts { get; set; }

        /// <summary>
        /// Create ConceptBuilder Istance 
        /// </summary>
        /// <param name="_names">ConceptScheme Descriptions Names</param>
        /// <param name="parsingObject">Parsing Object <see cref="ISdmxParsingObject"/></param>
        /// <param name="versionTypeResp">Sdmx Version</param>
        public ConceptBuilder(List<SdmxObjectNameDescription> _names, ISdmxParsingObject parsingObject, SdmxSchemaEnumType versionTypeResp) :
            base(parsingObject, versionTypeResp)
        {
            this.Names = _names;
        }

         /// <summary>
        ///  Create a ImmutableInstance of ConceptScheme
        /// </summary>
        /// <returns>IConceptSchemeObject</returns>
        public IConceptSchemeObject BuildConceptScheme()
        {
            return BuildConceptScheme(FlyConfiguration.MainAgencyId, FlyConfiguration.Version);
        }
        /// <summary>
        ///  Create a ImmutableInstance of ConceptScheme
        /// </summary>
        /// <param name="AgencyId">Agency Id</param>
        /// <param name="Version">Artefact Version</param>
        /// <returns>IConceptSchemeObject</returns>
        public IConceptSchemeObject BuildConceptScheme(string AgencyId, string Version)
        {
            try
            {
                IConceptSchemeMutableObject cs = new ConceptSchemeMutableCore();
                cs.AgencyId = AgencyId;
                cs.Version = Version;
                cs.Id = this.Code;
                if (this.Names != null)
                    foreach (SdmxObjectNameDescription item in this.Names)
                        cs.AddName(item.Lingua, item.Name);

                if (!this.ParsingObject.ReturnStub)
                {
                    foreach (IConceptMutableObject _dim in Concepts)
                        cs.AddItem(_dim);
                }
               

                cs.FinalStructure = TertiaryBool.ParseBoolean(true);

                if (this.ParsingObject.isReferenceOf || this.ParsingObject.ReturnStub)
                {
                    cs.ExternalReference = TertiaryBool.ParseBoolean(true);
                    cs.StructureURL = RetreivalStructureUrl.Get(this, cs.Id, cs.AgencyId, cs.Version);
                }

                return cs.ImmutableInstance;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateImmutable, ex);
            }
        }

    }
}
