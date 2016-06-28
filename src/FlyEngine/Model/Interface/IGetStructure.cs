using FlyEngine.Model;
using System;
namespace FlyEngine.Engine.Metadata
{
    /// <summary>
    /// GetStructure implementation of SDMXObjectBuilder for creation of DataStructure
    /// </summary>
    public interface IGetStructure : ISDMXObjectBuilder
    {
        /// <summary>
        /// Populate DataStructure (KeyFamyly for SDMX 2.0, Structure for SDMX 2.1)
        /// property of SDMXObjectBuilder for insert this elements in DataStructure response
        /// </summary>
        new void Build();
    }
}
