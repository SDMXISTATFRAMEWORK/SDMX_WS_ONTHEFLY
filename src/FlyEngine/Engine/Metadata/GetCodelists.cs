using FlyEngine.Model;
using FlyController.Builder;
using FlyController.Model;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.Sdmxsource.Sdmx.Api.Constants;
using FlyController.Model.Error;
using OnTheFlyLog;
using FlyController;
using FlyMapping.Model;

namespace FlyEngine.Engine.Metadata
{
    /// <summary>
    /// GetCodelists implementation of SDMXObjectBuilder for creation of Codelist
    /// </summary>
    public class GetCodelists : SDMXObjectBuilder, IGetCodelists
    {
        /// <summary>
        /// create a GetCodelists instance
        /// </summary>
        /// <param name="_parsingObject">Sdmx Parsing Object</param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        public GetCodelists(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
            : base(_parsingObject, _versionTypeResp)
        { }

        /// <summary>
        /// Populate a list of Codelist property of SDMXObjectBuilder for insert this in DataStructure response
        /// </summary>
        public override void Build()
        {
            try
            {
                this._Codelists = GetCodelist();
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.BuildCodelist, ex);
            }
        }

        /// <summary>
        /// retrieves the codelist Contrain  from database
        /// </summary>
        /// <returns>list of Codelist for SDMXObject</returns>
        public List<ICodelistMutableObject> GetCodelist()
        {
            try
            {
                if(CodelistManager==null)
                    CodelistManager = MappingConfiguration.MetadataFactory.InstanceCodelistsManager(this.ParsingObject, this.VersionTypeResp);

                if ((string.IsNullOrEmpty(this.ParsingObject.ConstrainConcept) || string.IsNullOrEmpty(this.ParsingObject.ConstrainDataFlow)) && this.ParsingObject.AgencyId!="MA")
                    return CodelistManager.GetCodelistNoConstrain();
                else
                    return CodelistManager.GetCodelistConstrain();
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.BuildCodelist, ex);
            }
        }





        #region References
        internal ICodelistManager CodelistManager { get; set; }
        /// <summary>
        /// Add External reference into SdmxObject
        /// </summary>
        public override void AddReferences()
        {
            try
            {
                RetrievalReferences Mr = new RetrievalReferences(this);
                Mr.ReferencesObject=this.CodelistManager.ReferencesObject;
            
                Mr.AddReferences(MetadataReferences.ReferenceTreeEnum.Codelist);
                //Destroy Obj
                Mr = null;
                CodelistManager = null;
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
