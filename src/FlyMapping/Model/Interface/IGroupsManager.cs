using FlyController.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyMapping.Model
{
    /// <summary>
    /// Interface for retreive Groups information from Database
    /// </summary>
    public interface IGroupsManager
    {
        /// <summary>
        /// Get Groups information from Database
        /// </summary>
        /// <param name="DataFlowID">Dataflow Code</param>
        /// <param name="GroupId">Group Code</param>
        /// <param name="DimensionRef">All Dimension in references of Group</param>
        /// <param name="AttributeRef">All Attribute that referenced this Group</param>
        /// <returns>List of <see cref="DataGroupObject"/></returns>
        List<DataGroupObject> GetGroups(string DataFlowID, string GroupId, List<string> DimensionRef, List<string> AttributeRef);
       
    }
}
