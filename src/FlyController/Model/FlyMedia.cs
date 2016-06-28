using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyController.Model
{
    /// <summary>
    /// Supported response Media type
    /// </summary>
    public enum FlyMediaEnum
    {
        /// <summary>
        /// Sdmx v2.0 and v2.1 XML
        /// </summary>
        Sdmx,
        /// <summary>
        /// Rdf NoVersion XML
        /// </summary>
        Rdf,
        /// <summary>
        /// Dspl NoVersion XML
        /// </summary>
        Dspl,
        /// <summary>
        /// Json only data
        /// </summary>
        Json
    }
}
