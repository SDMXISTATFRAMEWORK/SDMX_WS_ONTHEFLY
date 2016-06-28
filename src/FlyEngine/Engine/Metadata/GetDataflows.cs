using FlyEngine.Model;
using FlyController.Builder;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.Sdmxsource.Sdmx.Api.Constants;
using FlyController.Model.Error;
using FlyController.Model;
using OnTheFlyLog;
using FlyMapping.Model;

namespace FlyEngine.Engine.Metadata
{
    /// <summary>
    /// GetDataflows implementation of SDMXObjectBuilder for creation of DataFlows
    /// </summary>
    public class GetDataflows : SDMXObjectBuilder, IGetDataflows
    {
        /// <summary>
        /// create a GetConcepts instance
        /// </summary>
        /// <param name="_parsingObject">Sdmx Parsing Object</param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        public GetDataflows(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
            : base(_parsingObject, _versionTypeResp)
        { }

        /// <summary>
        /// Populate a list of Dataflows property of SDMXObjectBuilder for insert this in DataStructure response
        /// Currently the Dataflow is only one
        /// </summary>
        public override void Build()
        {
            try
            {
                this._Dataflows = Dataflows();
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.BuildDataflows, ex);
            }
        }

        /// <summary>
        /// Create a list of DataflowBuilder
        /// Currently the Dataflow is only one
        /// </summary>
        /// <returns>List of <see cref="IDataflowObject"/></returns>
        public List<IDataflowObject> Dataflows()
        {
            try
            {
                if(DataflowManager==null)
                    DataflowManager = MappingConfiguration.MetadataFactory.InstanceDataflowsManager(this.ParsingObject, this.VersionTypeResp);
                return DataflowManager.GetDataFlows();
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetDataFlows, ex);
            }
        }

        #region References
        internal IDataflowsManager DataflowManager { get; set; }

        /// <summary>
        /// Add External reference into SdmxObject
        /// </summary>
        public override void AddReferences()
        {
            try
            {
                RetrievalReferences Mr = new RetrievalReferences(this);
                Mr.ReferencesObject=this.DataflowManager.ReferencesObject;
              
                Mr.AddReferences(MetadataReferences.ReferenceTreeEnum.Dataflow);
                //Destroy Obj
                Mr = null;
                DataflowManager = null;

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
