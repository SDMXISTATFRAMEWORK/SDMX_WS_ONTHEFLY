using FlyController;
using FlyController.Builder;
using FlyController.Model;
using FlyController.Model.DbSetting;
using FlyController.Model.Error;
using FlyEngine.Model;
using FlyMapping.Build;
using FlyMapping.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.MetadataStructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;

namespace FlyDotStat_implementation.Manager.Metadata
{
    /// <summary>
    /// DatasetsManagerManager retrieves the data for build DataFlows
    /// </summary>
    public class DatasetsManager : BaseManager, IDataflowsManager
    {
        /// <summary>
        /// Inizialize for BaseManager Class
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        public DatasetsManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
            : base(_parsingObject, _versionTypeResp) { }

        /// <summary>
        /// Referenced objects 
        /// </summary>
        public override IReferencesObject ReferencesObject { get; set; }
       
        /// <summary>
        /// Build a DataFlows
        /// </summary>
        /// <returns>list of IDataflowObject for SdmxObject</returns>
        public List<IDataflowObject> GetDataFlows()
        {
            if (ReferencesObject == null)
                ReferencesObject = new IReferencesObject();

           
                ReferencesObject.FoundedDataflows = new List<IDataflowObject>();     
 
            string DataflowCodeMaintenableId = this.parsingObject.MaintainableId;
            Dictionary<string, List<SdmxObjectNameDescription>> df = GetDataflows(DataflowCodeMaintenableId);
            List<DataflowBuilder> res = new List<DataflowBuilder>();
            foreach (var item in df)
            {
                if (string.IsNullOrEmpty(DataflowCodeMaintenableId) || item.Key.Trim().ToUpper() == DataflowCodeMaintenableId.Trim().ToUpper())
                    res.Add(new DataflowBuilder(item.Key, item.Value, this.parsingObject, this.versionTypeResp));
            }

            foreach (DataflowBuilder dfbuilder in res)
                ReferencesObject.FoundedDataflows.Add(dfbuilder.BuildDataflow(FlyConfiguration.MainAgencyId, FlyConfiguration.Version));

            return ReferencesObject.FoundedDataflows;
        }

       
        /// <summary>
        /// retrieves the Dataflows from database
        /// </summary>
        /// <returns>list of Dataflow in Dictionary structure (Code, Descriptions)</returns>
        public Dictionary<string, List<SdmxObjectNameDescription>> GetDataflows(string DataflowCodeMaintenableId)
        {
            try
            {
               Dictionary<string,List<SdmxObjectNameDescription>> _Dataflows = new Dictionary<string,List<SdmxObjectNameDescription>>();

               List<IParameterValue> parametri = new List<IParameterValue>()
                {
                    new ParameterValue() {Item="UserName",Value=FlyConfiguration.UserName},
                    new ParameterValue() {Item="Domain",Value=FlyConfiguration.Domain},
                };

                if (!string.IsNullOrEmpty(this.parsingObject.TimeStamp))
                    parametri.Add(new ParameterValue() { Item = "TimeStamp", Value = this.parsingObject.TimeStamp, SqlType = SqlDbType.DateTime });
                
                //EFFETTUO LA RICHIESTA AL DB
                List<XmlNode> risposta = this.DbAccess.Execute(DBOperationEnum.GetDatasets, parametri);

                //PARSO LA RISPOSTA E CREO L'OGGETTO
                if (risposta.Count > 0)
                {
                    foreach (XmlNode dataset in risposta)
                    {
                        if (dataset.Attributes == null || dataset.Attributes["Code"] == null)
                            continue;

                        _Dataflows.Add(dataset.Attributes["Code"].Value, SdmxObjectNameDescription.GetNameDescriptions(dataset));
                    }
                }
                return _Dataflows;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateDataflowBuilder, ex);
            }

        }

        /// <summary>
        /// Build a DataFlows
        /// </summary>
        /// <param name="refObj">Referenced Objects</param>
        /// <returns>list of IDataflowObject for SdmxObject</returns>
        public List<IDataflowObject> GetDataFlowsReferences(IReferencesObject refObj)
        {

            if (refObj.FoundedDataflows != null)
                return refObj.FoundedDataflows;
            else
                return GetDataFlows();//Chiunque referenzia i dataflow gia li ha gia istanziati

        }
       
    }
}
