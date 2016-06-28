using FlyEngine.Model;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using System;
namespace FlyEngine.Engine.Metadata
{
    /// <summary>
    /// GetAgencySchema implementation of SDMXObjectBuilder for creation of OrganisationScheme and AgencyScheme
    /// </summary>
    public interface IGetAgencyScheme : ISDMXObjectBuilder
    {
        /// <summary>
        /// Create a immutable instance of IAgencyScheme
        /// </summary>
        /// <returns>IAgencyScheme</returns>
        IAgencyScheme _GetAgencyScheme();
       
        /// <summary>
        /// Populate a AgencyScheme property of SDMXObjectBuilder for insert this in DataStructure response
        /// </summary>
        new void Build();
    }
}
