using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyController.Model
{
    /// <summary>
    /// Concept Type Enum (Dimension or Attribute)
    /// </summary>
    public enum ConceptTypeEnum
    {
        /// <summary>
        /// Dimension Concept
        /// </summary>
        Dimension,
        /// <summary>
        /// Attribute Concept
        /// </summary>
        Attribute,
        /// <summary>
        /// Special Concept for retreival Contrained codelist
        /// </summary>
        Special,
    }

}
