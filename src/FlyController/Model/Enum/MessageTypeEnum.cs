using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyController.Model
{
    /// <summary>
    /// All Type of request both SDMX 2.0 and Sdmx 2.1
    /// </summary>
    public enum MessageTypeEnum
    {
        /// <summary>
        /// Unrecognized request message
        /// </summary>
        None,

        /// <summary>
        /// Get Compact data (request query message Sdmx 2.0 version)
        /// </summary>
        Compact_20,
        /// <summary>
        /// Get Generic data (request query message Sdmx 2.0 version)
        /// </summary>
        Generic_20,
        /// <summary>
        /// Get CrossSectional data (request query message Sdmx 2.0 version)
        /// </summary>
        CrossSectional_20,


        /// <summary>
        /// Get Generic data (request query message Sdmx 2.1 version)
        /// </summary>
        GenericData_21,
        /// <summary>
        /// Get Generic TimeSeries data (request query message Sdmx 2.1 version)
        /// </summary>
        GenericTimeSeries_21,
        /// <summary>
        /// Get StructureSpecific data (request query message Sdmx 2.1 version)
        /// </summary>
        StructureSpecific_21,
        /// <summary>
        /// Get StructureSpecific TimeSeries data (request query message Sdmx 2.1 version)
        /// </summary>
        StructureSpecificTimeSeries_21,

        /// <summary>
        /// Get Data in RDF format
        /// </summary>
        Rdf,
        /// <summary>
        /// Get Data in Dspl format
        /// </summary>
        Dspl,
        /// <summary>
        /// Get Data in Json format
        /// </summary>
        Json
    }
}
