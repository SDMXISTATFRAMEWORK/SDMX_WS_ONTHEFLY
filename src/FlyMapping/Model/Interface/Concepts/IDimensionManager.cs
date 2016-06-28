using FlyController.Model;
using System;
using System.Collections.Generic;
namespace FlyMapping.Model
{
    /// <summary>
    ///  Interface for DataMessage Manager
    /// DimensionManager retrieves the data for build conceptScheme
    /// </summary>
    public interface IDimensionManager : IBaseManager
    {
        /// <summary>
        /// retrieves all Concept Dimension from database
        /// </summary>
        /// <param name="DataflowCode">Dataflow Code</param>
        /// <param name="Names">return Dataflow NameDescription</param>
        /// <returns>list of Dimension</returns>
        List<IDimensionConcept> GetDimensionConceptObjects(string DataflowCode, out List<SdmxObjectNameDescription> Names);
    }
}
