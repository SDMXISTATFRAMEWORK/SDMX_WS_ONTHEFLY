using FlyController.Model;
using FlyEngine.Model;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using System;
using System.Collections.Generic;
namespace FlyEngine.Engine.Metadata
{
    /// <summary>
    /// GetConcepts implementation of SDMXObjectBuilder for creation of ConceptScheme
    /// </summary>
    public interface IGetConcepts : ISDMXObjectBuilder
    {
        /// <summary>
        /// Populate a Conceptscheme property of SDMXObjectBuilder for insert this in DataStructure response
        /// </summary>
        new void Build();
         /// <summary>
        /// Build a ConceptSchemes
        /// </summary>
        /// <returns>list of IConceptSchemeObject for SdmxObject</returns>
        List<IConceptSchemeObject> GetConceptSchemes();
    }
}
