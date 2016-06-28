using System;
namespace FlyController.Model
{
    /// <summary>
    /// Special Concept for request a Codelist Constrained of type Special 
    /// </summary>
    public interface ISpecialConcept:IConceptObjectImpl
    {
        /// <summary>
        /// List of Member for Codelist Constrained (Other concept value in constrain, call a Special Concept CL_CONSTRAIN)
        /// </summary>
        System.Collections.Generic.Dictionary<string, System.Collections.Generic.IList<Org.Sdmx.Resources.SdmxMl.Schemas.V20.common.MemberValueType>> ContrainConceptREF { get; set; }
        /// <summary>
        /// Add Names at Codelist Constrained
        /// </summary>
        /// <param name="names">the names <see cref="SdmxObjectNameDescription"/></param>
        void SetNames(System.Collections.Generic.List<SdmxObjectNameDescription> names);
        /// <summary>
        /// Special Type Codelist <see cref="SpecialTypeEnum"/>
        /// </summary>
        SpecialTypeEnum SpecialType { get; set; }
       
        /// <summary>
        /// Save a TimeDimension Ref for return a Constrained Codelist whitout recall a Concepts list
        /// </summary>
        IConceptObjectImpl TimeDimensionRef { get; set; }
    }
}
