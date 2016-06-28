using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyController.Model
{
    /// <summary>
    /// Object representing Group parsed in DSD RetrievalManager
    /// </summary>
    public class DataGroupObject
    {
        /// <summary>
        /// Initialize a new instance of DataGroupObject
        /// </summary>
        /// <param name="groupId">Group Id</param>
        public DataGroupObject(string groupId)
        {
            this.GroupId = groupId;
            DimensionReferences = new List<GroupReferenceObject>();
            AttributeReferences = new List<GroupReferenceObject>();
        }
        /// <summary>
        /// Group Id
        /// </summary>
        public string GroupId { get; set; }
        /// <summary>
        /// All Dimension Associated
        /// </summary>
        public List<GroupReferenceObject> DimensionReferences { get; set; }
        /// <summary>
        /// All Attribute Associated
        /// </summary>
        public List<GroupReferenceObject> AttributeReferences { get; set; }

       
    }
    /// <summary>
    /// Object representing a list of Concept (Dimension and Attribute) associated in Group
    /// </summary>
    public class GroupReferenceObject
    {
        /// <summary>
        /// Concept Code
        /// </summary>
        public string ConceptCode { get; set; }
        /// <summary>
        /// Value of Concept in group
        /// </summary>
        public object ConceptValue { get; set; }
    }
}
