using FlyController.Model;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using System;
using System.Collections.Generic;
namespace FlyMapping.Model
{
    /// <summary>
    /// Interface for Dimension Codelists Manager
    /// DimensionCodelistsManager retrieves the data for build  Codelist
    /// </summary>
    public interface IDimensionCodelistsManager : IBaseManager
    {
         /// <summary>
        /// retrieves the codelist Contrain of Dimension from database
        /// </summary>
        /// <param name="DataflowCode">>Dataflow Code</param>
        /// <param name="dimension">Instance of Dimension "DimensionConcept"</param>
        /// <returns>list of Mutable Code Object</returns>
        List<ICodeMutableObject> GetDimensionCodelistContrain(string DataflowCode, IDimensionConcept dimension);

        /// <summary>
        /// retrieves the codelist Contrain of Dimension from database
        /// </summary>
        /// <param name="dimension">Instance of Dimension "DimensionConcept"</param>
        /// <returns>list of Mutable Code Object</returns>
        List<ICodeMutableObject> GetDimensionCodelistNoContrain(IDimensionConcept dimension);
      
    }
}
