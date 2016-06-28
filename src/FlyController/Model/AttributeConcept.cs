using FlyController.Model.Error;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.DataStructure;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyController.Model
{
    /// <summary>
    /// AttributeConcept is Implementation of ConceptObjectImpl that representing an  Attribute
    /// </summary>
    public class AttributeConcept : ConceptObjectImpl, FlyController.Model.IAttributeConcept
    {
        /// <summary>
        /// Attribute AttachmentLevel
        /// </summary>
        public AttributeAttachmentLevel AttributeAttachmentLevelType { get; set; }
        /// <summary>
        /// Attribute Assignment Status
        /// </summary>
        public AssignmentStatusTypeEnum AssignmentStatusType { get; set; }
      
        /// <summary>
        /// Flag that representing if this is a FLAGAttribute (OBS_STATUS)
        /// </summary>
        public bool IsFlagAttribute { get; set; }
        /// <summary>
        /// Flag that representing if this is a ValueAttribute (OBS_VALUE)
        /// </summary>
        public bool IsValueAttribute { get; set; }

        /// <summary>
        /// create a instace of AttributeConcept
        /// </summary>
        /// <param name="code">Attribute Code</param>
        /// <param name="names">Attribute Descriptions Names</param>
        public AttributeConcept(string code, List<SdmxObjectNameDescription> names)
            : base(code, names, ConceptTypeEnum.Attribute)
        {
            this.IsValueAttribute = false;
        }

        /// <summary>
        /// When AttributeAttachmentLevelType is Group, determinate a GroupName
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Get Reference in case of DimensionGroup AttachmentLevel
        /// </summary>
        /// <param name="_Concepts">All Concept to reference</param>
        /// <returns>List of Code Dimension to attach in DimensionReferences</returns>
        public List<string> GetDimensionsReference(List<IConceptObjectImpl> _Concepts)//lo lego a tutti
        {
            List<string> AssociateDimensions= new List<string>();
            foreach (var item in _Concepts.FindAll(c => c.ConceptType == ConceptTypeEnum.Dimension && ((IDimensionConcept)c).DimensionType != DimensionTypeEnum.Time))
                AssociateDimensions.Add(item.ConceptObjectCode);
            return AssociateDimensions;
        }
    }
}
