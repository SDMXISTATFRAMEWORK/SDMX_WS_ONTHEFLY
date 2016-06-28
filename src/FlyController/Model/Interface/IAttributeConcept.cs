using System;
using System.Collections.Generic;
namespace FlyController.Model
{
    /// <summary>
    /// AttributeConcept is Implementation of ConceptObjectImpl that representing an  Attribute
    /// </summary>
    public interface IAttributeConcept : IConceptObjectImpl
    {
        /// <summary>
        /// Attribute Assignment Status
        /// </summary>
        AssignmentStatusTypeEnum AssignmentStatusType { get; set; }
        /// <summary>
        /// Attribute AttachmentLevel
        /// </summary>
        Org.Sdmxsource.Sdmx.Api.Constants.AttributeAttachmentLevel AttributeAttachmentLevelType { get; set; }
        /// <summary>
        /// Get Reference in case of DimensionGroup AttachmentLevel
        /// </summary>
        /// <param name="_Concepts">All Concept to reference</param>
        /// <returns>List of Code Dimension to attach in DimensionReferences</returns>
        System.Collections.Generic.List<string> GetDimensionsReference(List<IConceptObjectImpl> _Concepts);
        /// <summary>
        /// When AttributeAttachmentLevelType is Group, determinate a GroupName
        /// </summary>
        string GroupName { get; set; }
         /// <summary>
        /// Flag that representing if this is a FLAGAttribute (OBS_STATUS)
        /// </summary>
        bool IsFlagAttribute { get; set; }
        /// <summary>
        /// Flag that representing if this is a ValueAttribute (OBS_VALUE)
        /// </summary>
        bool IsValueAttribute { get; set; }
    }
}
