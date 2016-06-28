using FlyController.Model;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyMapping.Model
{
    /// <summary>
    /// Interface for FLAG Manager
    /// FLAGManager retrieves the data for FLAG Concept
    /// </summary>
    public interface IFLAGManager : IBaseManager
    {
        /// <summary>
        /// Get Flag Attribute (OBS_STATUS)
        /// </summary>
        /// <returns>Flag Attribute</returns>
        IAttributeConcept GetFlag();

        
    }
}
