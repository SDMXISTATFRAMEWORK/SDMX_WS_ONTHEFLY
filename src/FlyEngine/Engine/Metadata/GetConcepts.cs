using FlyEngine.Model;
using FlyController.Builder;
using FlyController.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.Sdmxsource.Sdmx.Api.Constants;
using FlyController.Model.Error;
using OnTheFlyLog;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using FlyController;
using FlyMapping.Model;

namespace FlyEngine.Engine.Metadata
{
    /// <summary>
    /// GetConcepts implementation of SDMXObjectBuilder for creation of ConceptScheme
    /// </summary>
    public class GetConcepts : SDMXObjectBuilder, IGetConcepts
    {
        /// <summary>
        /// create a GetConcepts instance
        /// </summary>
        /// <param name="_parsingObject">Sdmx Parsing Object</param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        public GetConcepts(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
            : base(_parsingObject, _versionTypeResp)
        { }

       
        /// <summary>
        /// Populate a Conceptscheme property of SDMXObjectBuilder for insert this in DataStructure response
        /// </summary>
        public override void Build()
        {
            try
            {
                this._Conceptscheme = GetConceptSchemes();
               
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.BuildConceptsScheme, ex);
            }
        }



        /// <summary>
        /// Build a ConceptSchemes
        /// </summary>
        /// <returns>list of IConceptSchemeObject for SdmxObject</returns>
        public List<IConceptSchemeObject> GetConceptSchemes()
        {
            try
            {
                if (ConceptSchemeManager == null)
                    ConceptSchemeManager = MappingConfiguration.MetadataFactory.InstanceConceptSchemeManager(this.ParsingObject, this.VersionTypeResp);
                return ConceptSchemeManager.GetConceptSchemes();
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.BuildConceptsScheme, ex);
            }
        }



       
        #region References
        internal IConceptSchemeManager ConceptSchemeManager { get; set; }

        /// <summary>
        /// Add External reference into SdmxObject
        /// </summary>
        public override void AddReferences()
        {
            try
            {
                RetrievalReferences Mr = new RetrievalReferences(this);
                Mr.ReferencesObject=this.ConceptSchemeManager.ReferencesObject;
               
                Mr.AddReferences(MetadataReferences.ReferenceTreeEnum.Concept);
                //Destroy Obj
                Mr = null;
                ConceptSchemeManager = null;

            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.AddReferences, ex);
            }
        }


        #endregion
    }
}
