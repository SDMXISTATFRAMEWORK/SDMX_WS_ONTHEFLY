using FlyEngine.Model;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using System;
using System.Collections.Generic;
namespace FlyMapping.Model
{
    /// <summary>
    /// Interface for IDataflows Manager
    /// DataflowsManager retrieves the data for build DataFlows
    /// </summary>
    public interface IDataflowsManager : IBaseManager
    {
        /// <summary>
        /// Build a DataFlows
        /// </summary>
        /// <returns>list of IDataflowObject for SdmxObject</returns>
        List<IDataflowObject> GetDataFlows();

        /// <summary>
        /// Build a DataFlows
        /// </summary>
        /// <param name="refObj">Referenced Objects</param>
        /// <returns>list of IDataflowObject for SdmxObject</returns>
        List<IDataflowObject> GetDataFlowsReferences(IReferencesObject refObj);
    }
}
