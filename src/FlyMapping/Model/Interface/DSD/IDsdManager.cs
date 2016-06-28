using FlyController.Model;
using FlyEngine.Model;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyMapping.Model
{
    /// <summary>
    /// Interface for DSD Manager
    /// DsdManager retrieves the data for build  KeyFamilies (sdmx v 2.0) or DataStructures (sdmx v 2.1)
    /// </summary>
    public interface IDsdManager : IBaseManager
    {
        /// <summary>
        /// retrieves the DSD from database
        /// </summary>
        /// <returns>list of DataStructure for SDMXObject</returns>
        List<DataStructureObjectImpl> GetDSDs();


        /// <summary>
        /// retrieves the DSD from database
        /// </summary>
        /// <param name="refObj">Referenced Objects</param>
        /// <returns>list of DataStructure for SDMXObject</returns>
        List<DataStructureObjectImpl> GetDSDsReferences(IReferencesObject refObj);
    }
}
