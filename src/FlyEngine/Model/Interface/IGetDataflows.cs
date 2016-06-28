using FlyController.Builder;
using FlyEngine.Model;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using System;
using System.Collections.Generic;
namespace FlyEngine.Engine.Metadata
{
    /// <summary>
    /// GetDataflows implementation of SDMXObjectBuilder for creation of DataFlows
    /// </summary>
    public interface IGetDataflows : ISDMXObjectBuilder
    {
       /// <summary>
        /// Populate a list of Dataflows property of SDMXObjectBuilder for insert this in DataStructure response
        /// Currently the Dataflow is only one
        /// </summary>
        new void Build();
        /// <summary>
        /// Create a list of DataflowBuilder
        /// Currently the Dataflow is only one
        /// </summary>
        /// <returns>List of <see cref="IDataflowObject"/></returns>
        List<IDataflowObject> Dataflows();
    }
}
