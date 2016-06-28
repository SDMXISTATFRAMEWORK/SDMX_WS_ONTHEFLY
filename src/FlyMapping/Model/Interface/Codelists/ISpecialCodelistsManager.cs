using FlyController.Model;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using System;
using System.Collections.Generic;
namespace FlyMapping.Model
{
    /// <summary>
    /// Interface for Special Codelists Manager
    /// SpecialCodelistsManager retrieves the data for build  Codelist:
    /// Frequency where not Dimension is present (From file), CL_COUNT, CL_TIME_PERIOD...
    /// </summary>
    public interface ISpecialCodelistsManager : IBaseManager
    {
        /// <summary>
        /// retrieves the codelist of Frequency dimension from the file "FrequencyCodelist.xml"
        /// </summary>
        /// <returns>list of Mutable Code Object</returns>
        List<ICodeMutableObject> GetFrequencyCodelist();
         /// <summary>
        /// retrieves the codelist constrained
        /// </summary>
        /// <param name="_dataFlowCode">Dataflow Code</param>
        /// <param name="specialConcept">the <see cref="ISpecialConcept"/></param>
        /// <returns>list of Mutable Code Object</returns>
        List<ICodeMutableObject> GetSpecialCodelist(string _dataFlowCode, ISpecialConcept specialConcept);
    }
}
