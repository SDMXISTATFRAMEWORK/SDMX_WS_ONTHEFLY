using FlyController.Model;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using System;
using System.Collections.Generic;
namespace FlyMapping.Model
{
    /// <summary>
    /// Interface for Agency Manager
    /// </summary>
    public interface IAgencyManager : IBaseManager
    {
        /// <summary>
        /// Build a AgencyScheme
        /// </summary>
        /// <returns>IAgencyScheme for SdmxObject</returns>
        IAgencyScheme GetAgencyScheme();
    }
}
