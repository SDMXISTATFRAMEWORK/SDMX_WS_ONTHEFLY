using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyController.Model
{
    /// <summary>
    /// Support Information on Concept in DSD rappresentation
    /// </summary>
    public class ConceptDSDInfoObject
    {
        /// <summary>
        /// Codelist Code
        /// </summary>
        public string CodelistId { get; set; }
        /// <summary>
        /// Codelist Agency
        /// </summary>
        public string CodelistAgency { get; set; }
        /// <summary>
        /// Codelist Version
        /// </summary>
        public string CodelistVersion { get; set; }
    }
}
