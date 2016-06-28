using System;
using System.Collections.Generic;
namespace FlyController.Model
{
    /// <summary>
    /// SdmxParsingObject is a Object Model used for parse a query request
    /// </summary>
    public interface ISdmxParsingObject : ICloneable
    {
        /// <summary>
        /// Structure Version
        /// </summary>
        string _version { get; set; }
        /// <summary>
        /// Structure Agency Code
        /// </summary>
        string AgencyId { get; set; }
        
        /// <summary>
        /// Create a Clone of <see cref="ISdmxParsingObject"/> but with isReferenceOf to True for declare that the structure is a reference
        /// </summary>
        /// <returns></returns>
        ISdmxParsingObject CloneForReferences();
        /// <summary>
        /// Concept Code for Codelist Constrained
        /// </summary>
        string ConstrainConcept { get; set; }
        /// <summary>
        /// Dataflow Code for Codelist Constrained
        /// </summary>
        string ConstrainDataFlow { get; set; }
        /// <summary>
        /// Dataflow Code for Codelist Constrained
        /// </summary>
        string ConstrainDataFlowAgency { get; set; }
        /// <summary>
        /// Dataflow Code for Codelist Constrained
        /// </summary>
        string ConstrainDataFlowVersion { get; set; }
        /// <summary>
        /// List of Member for Codelist Constrained (Other concept value in constrain, call a Special Concept CL_CONSTRAIN)
        /// </summary>
        System.Collections.Generic.Dictionary<string, System.Collections.Generic.IList<Org.Sdmx.Resources.SdmxMl.Schemas.V20.common.MemberValueType>> ContrainConceptREF { get; set; }
       
        /// <summary>
        /// Determinate if this class is using for build a reference
        /// </summary>
        bool isReferenceOf { get; set; }
        /// <summary>
        /// Structure Maintainable Code (properties that change the type of value according to the type required)
        /// </summary>
        string MaintainableId { get; set; }
        /// <summary>
        /// Check if AgencyId of request is equals to AgencyId configured in File Config
        /// </summary>
        void PreliminarCheck();
        /// <summary>
        /// Not load a full Object (For Sdmx 2.1 or REST)
        /// </summary>
        Org.Sdmxsource.Sdmx.Api.Constants.StructureQueryDetailEnumType QueryDetail { get; set; }
        /// <summary>
        /// Determinate what structure references return
        /// </summary>
        Org.Sdmxsource.Sdmx.Api.Constants.StructureReferenceDetailEnumType References { get; set; }
        /// <summary>
        /// Determinate what is a ResolveReference property of a Query in Sdmx 2.0
        /// </summary>
        bool ResolveReferenceSdmx20 { get; set; }
        /// <summary>
        /// Determines whether the result is in STUB format or not: if detail is AllStubs or if detail is ReferencedStubs and is writing a reference
        /// </summary>
        bool ReturnStub { get; }
        /// <summary>
        /// Sdmx Version
        /// </summary>
        Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType SdmxStructureType { get; set; }
        /// <summary>
        /// List of Structure required for references
        /// </summary>
        System.Collections.Generic.List<Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureType> SpecificReference { get; set; }
        /// <summary>
        /// LastUpdate parameter request only observation from this date onwards
        /// </summary>
        string TimeStamp { get; set; }

        /// <summary>
        /// If the Query contains other Registry this field contains a type of other regitry requested
        /// </summary>
        List<ISdmxParsingObject> OtherRegistry { get; set; }

        /// <summary>
        /// Check if agency have a consistent Value
        /// </summary>
        /// <returns></returns>
        bool isValidAgency();
       
        /// <summary>
        /// Check if version have a consistent Value
        /// </summary>
        /// <returns></returns>
        bool isValidVersion();
    }
}
