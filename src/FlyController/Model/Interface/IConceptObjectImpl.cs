using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Base;
using System;
namespace FlyController.Model
{
    /// <summary>
    /// Abstract Extension of ConceptMutableCore for separate Dimension and Attribute
    /// </summary>
    public interface IConceptObjectImpl : INameableMutableObject, IIdentifiableMutableObject
    {
        /// <summary>
        ///  Identificable Code
        /// </summary>
        string ConceptObjectCode { get; set; }
        /// <summary>
        ///  Descriptions Names
        /// </summary>
        System.Collections.Generic.List<SdmxObjectNameDescription> ConceptObjectNames { get; set; }
        /// <summary>
        /// Concept Type (Dimension or Attribute)
        /// </summary>
        ConceptTypeEnum ConceptType { get; set; }

        /// <summary>
        /// Used only for DotStatExtra implementation specify a code of codelist whitout using configuration settings
        /// </summary>
        string CodelistCode { get; set; }

        /// <summary>
        /// Support Information on Concept in DSD rappresentation
        /// </summary>
        ConceptDSDInfoObject ConceptDSDInfo { get; set; }

    }
}
