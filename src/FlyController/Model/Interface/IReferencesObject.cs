using FlyController.Model;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyEngine.Model
{
    /// <summary>
    /// Interface of object to use for references
    /// </summary>
    public class IReferencesObject
    {
        /// <summary>
        /// Saved CategoryScheme
        /// </summary>
        public List<ICategorySchemeObject> CategoryScheme { get; set; }
        /// <summary>
        /// Saved Categorisation
        /// </summary>
        public List<ICategorisationObject> Categorisation { get; set; }

        /// <summary>
        /// Founded Codelist
        /// </summary>
        public List<ICodelistMutableObject> Codelists { get; set; }

        /// <summary>
        /// Founded ConceptSchemes
        /// </summary>
        public List<IConceptSchemeObject> ConceptSchemes { get; set; }

        /// <summary>
        /// Founded Concepts Key=ConceptScheme Code / Value = Concepts
        /// </summary>
        public Dictionary<string, List<IConceptObjectImpl>> Concepts { get; set; }
        /// <summary>
        /// List of DataFlows founded
        /// </summary>
        public List<IDataflowObject> FoundedDataflows { get; set; }

        /// <summary>
        /// Founded DSD
        /// </summary>
        public List<DataStructureObjectImpl> DSDs { get; set; }
    }
}
