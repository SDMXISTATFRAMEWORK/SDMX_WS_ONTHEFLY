using FlyController;
using FlyController.Builder;
using FlyController.Model;
using FlyController.Model.DbSetting;
using FlyController.Model.Error;
using FlyDotStat_implementation.Build;
using FlyDotStatExtra_implementation.Manager;
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

namespace FlyDotStatExtra_implementation.Manager
{
    /// <summary>
    /// DataflowsManager retrieves the data for build DataFlows
    /// </summary>
    public class DataflowsManager : BaseManager, IDataflowsManager
    {
        /// <summary>
        /// Inizialize for BaseManager Class
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        public DataflowsManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
            : base(_parsingObject, _versionTypeResp) { }

        /// <summary>
        /// Referenced objects 
        /// </summary>
        public override IReferencesObject ReferencesObject { get; set; }

        internal List<MSDataflow> MSDataflows = null;
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
            MSDataflows = GetDataflows();
            for (int i = MSDataflows.Count - 1; i >= 0; i--)
            {

                MSDataflow item = MSDataflows[i];
                if ((!string.IsNullOrEmpty(DataflowCodeMaintenableId)) && item.DFCode.Trim().ToUpper() != DataflowCodeMaintenableId.Trim().ToUpper())
                {
                    MSDataflows.RemoveAt(i);
                    continue;
                }

                DataflowBuilder dfbuilder = dfbuilder = new DataflowBuilder(item.DFCode, item.Descr, this.parsingObject, this.versionTypeResp);
                //DsdManager dsdMan = new DsdManager(this.parsingObject.CloneForReferences(), this.versionTypeResp);
                dfbuilder.DataStrunctureRef = item.GetDataStrunctureRef(MSDataflows[i].DsdCode, MSDataflows[i].DsdAgency, MSDataflows[i].DsdVersion);
                ReferencesObject.FoundedDataflows.Add(dfbuilder.BuildDataflow(item.DFAgency, item.DFVersion));
            }

            return ReferencesObject.FoundedDataflows;
        }

        /// <summary>
        /// retrieves the Dataflows from database
        /// </summary>
        /// <returns>list of Dataflow in Dictionary structure (Code, Descriptions)</returns>
        public List<MSDataflow> GetDataflows()
        {
            try
            {
                List<MSDataflow> _Dataflows = new List<MSDataflow>();

                BuilderParameter bp = new BuilderParameter()
                {
                    TimeStamp = this.parsingObject.TimeStamp,
                };

                //EFFETTUO LA RICHIESTA AL DB
                FlyDotStat_implementation.Build.DWHAccess OtherDbAccess = new FlyDotStat_implementation.Build.DWHAccess(FlyConfiguration.ConnectionString);
                List<XmlNode> risposta = OtherDbAccess.Execute(DBOperationEnum.GetDataflows, bp.GetParameter());

                //PARSO LA RISPOSTA E CREO L'OGGETTO
                if (risposta.Count > 0)
                {
                    foreach (XmlNode dataset in risposta)
                    {
                        if (dataset.Attributes == null || dataset.Attributes["Code"] == null)
                            continue;

                        _Dataflows.Add(new MSDataflow()
                        {
                            IdDf = Convert.ToInt32(dataset.Attributes["DfID"].Value),
                            DFCode = dataset.Attributes["Code"].Value,
                            DFAgency = FlyConfiguration.MainAgencyId,
                            DFVersion = FlyConfiguration.Version,
                            Descr = SdmxObjectNameDescription.GetNameDescriptions(dataset)
                        });

                    }
                }

                List<MSDataflow> Ms_Dataflows = GetMSDataflows(bp);

                if (_Dataflows != null && _Dataflows.Count > 0)
                {//Scarto i Cancellati e quelli con Production MA

                    for (int i = _Dataflows.Count - 1; i >= 0; i--)
                    {
                        MSDataflow Ms_Dataflow = Ms_Dataflows.Find(dfNode => dfNode.IdDf.ToString() == _Dataflows[i].IdDf.ToString());
                        if (Ms_Dataflow == null)
                        {
                            _Dataflows.RemoveAt(i);
                            continue;
                        }

                        if (!string.IsNullOrEmpty(Ms_Dataflow.DFProduction))
                        {
                            if (Ms_Dataflow.DFProduction.Trim().ToUpper() == "MA")
                            {
                                _Dataflows.RemoveAt(i);
                                continue;
                            }
                            _Dataflows[i].DFProduction = Ms_Dataflow.DFProduction;
                        }
                        if (!string.IsNullOrEmpty(Ms_Dataflow.DFAgency))
                            _Dataflows[i].DFAgency = Ms_Dataflow.DFAgency;
                        if (!string.IsNullOrEmpty(Ms_Dataflow.DFVersion))
                            _Dataflows[i].DFVersion = Ms_Dataflow.DFVersion;

                        if (!string.IsNullOrEmpty(Ms_Dataflow.DsdCode))
                            _Dataflows[i].DsdCode = Ms_Dataflow.DsdCode;
                        if (!string.IsNullOrEmpty(Ms_Dataflow.DsdAgency))
                            _Dataflows[i].DsdAgency = Ms_Dataflow.DsdAgency;
                        if (!string.IsNullOrEmpty(Ms_Dataflow.DsdVersion))
                            _Dataflows[i].DsdVersion = Ms_Dataflow.DsdVersion;
                    }
                }
                foreach (var MaDf in Ms_Dataflows.FindAll(madf => madf.DFProduction.Trim().ToUpper() == "MA"))
                {//Aggiungo quelli che verranno elaborati dal MA
                    _Dataflows.Add(new MSDataflow()
                       {
                           IdDf = MaDf.IdDf,
                           DFCode = MaDf.DFCode,
                           DFAgency = MaDf.DFAgency,
                           DFVersion = MaDf.DFVersion,
                           DsdCode = MaDf.DsdCode,
                           DsdAgency = MaDf.DsdAgency,
                           DsdVersion = MaDf.DsdVersion,
                           DatasetList=MaDf.DatasetList,
                           Descr = MaDf.Descr
                       });
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
        /// Get Dataflow from MS Database
        /// </summary>
        /// <param name="bp">Parameter <see cref="BuilderParameter"/></param>
        /// <returns>list of dataflow in <see cref="MSDataflow"/> structure</returns>
        public List<MSDataflow> GetMSDataflows(BuilderParameter bp)
        {
            try
            {
                List<MSDataflow> MSDataflows = new List<MSDataflow>();

                this.DbAccess = new DWHAccess(FlyConfiguration.ConnectionString);
                List<XmlNode> Ms_Dataflows = this.DbAccess.Execute(DBOperationEnum.MSGetDataflows, bp.GetParameter());

                foreach (var Ms_Dataflow in Ms_Dataflows)
                {
                    MSDataflow df = new MSDataflow();

                    if (Ms_Dataflow.Attributes["id"] != null)
                        df.IdDf = Convert.ToInt32(Ms_Dataflow.Attributes["id"].Value);

                    if (Ms_Dataflow.Attributes["Production"] != null)
                        df.DFProduction = Ms_Dataflow.Attributes["Production"].Value;
                    if (Ms_Dataflow.Attributes["Code"] != null)
                        df.DFCode = Ms_Dataflow.Attributes["Code"].Value;
                    if (Ms_Dataflow.Attributes["AgencyId"] != null)
                        df.DFAgency = Ms_Dataflow.Attributes["AgencyId"].Value;
                    if (Ms_Dataflow.Attributes["Version"] != null)
                        df.DFVersion = Ms_Dataflow.Attributes["Version"].Value;

                    if (Ms_Dataflow.Attributes["DSDCode"] != null)
                        df.DsdCode = Ms_Dataflow.Attributes["DSDCode"].Value;
                    if (Ms_Dataflow.Attributes["DSDAgencyId"] != null)
                        df.DsdAgency = Ms_Dataflow.Attributes["DSDAgencyId"].Value;
                    if (Ms_Dataflow.Attributes["DSDVersion"] != null)
                        df.DsdVersion = Ms_Dataflow.Attributes["DSDVersion"].Value;

                    XmlNode datasets = Ms_Dataflow.ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.LocalName.Trim().ToUpper() == "DATASETLIST");
                    if (datasets != null && datasets.ChildNodes != null && datasets.ChildNodes.Count > 0)
                    {
                        df.DatasetList = new List<string>();
                        foreach (XmlNode dataset in datasets.ChildNodes)
                        {
                            if (dataset.LocalName == "Dataset" && dataset.Attributes["Code"] != null)
                                df.DatasetList.Add(dataset.Attributes["Code"].Value);
                        }
                    }
                    df.Descr = SdmxObjectNameDescription.GetNameDescriptions(Ms_Dataflow);

                    MSDataflows.Add(df);
                }
                return MSDataflows;

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
            {
                List<IDataflowObject> dfs = GetDataFlows();
                if (refObj.DSDs != null)
                {
                    for (int i = dfs.Count - 1; i >= 0; i--)
                    {
                        if (!refObj.DSDs.Exists(dsd => dsd.Id == dfs[i].DataStructureRef.MaintainableId))
                            dfs.RemoveAt(i);
                    }
                }
                return dfs;
            }
        }
    }
}
