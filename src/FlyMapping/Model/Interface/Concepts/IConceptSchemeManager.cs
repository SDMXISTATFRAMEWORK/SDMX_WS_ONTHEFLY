using FlyEngine.Model;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyMapping.Model
{
    /// <summary>
    /// Interface for Concept Scheme
    /// </summary>
    public interface IConceptSchemeManager : IBaseManager
    {
        /// <summary>
        /// Build a ConceptSchemes
        /// </summary>
        /// <returns>list of IConceptSchemeObject for SdmxObject</returns>
        List<IConceptSchemeObject> GetConceptSchemes();


        /// <summary>
        /// Build a ConceptSchemes
        /// </summary>
        /// <param name="refObj">Referenced Objects</param>
        /// <returns>list of IConceptSchemeObject for SdmxObject</returns>
        List<IConceptSchemeObject> GetConceptSchemesReferences(IReferencesObject refObj);
    }
}
