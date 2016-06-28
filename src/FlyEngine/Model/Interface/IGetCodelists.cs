using FlyEngine.Model;
using System;
namespace FlyEngine.Engine.Metadata
{
    /// <summary>
    /// GetCodelists implementation of SDMXObjectBuilder for creation of Codelist
    /// </summary>
    public interface IGetCodelists : ISDMXObjectBuilder
    {
         /// <summary>
        /// Populate a list of Codelist property of SDMXObjectBuilder for insert this in DataStructure response
        /// </summary>
        new void Build();
    }
}
