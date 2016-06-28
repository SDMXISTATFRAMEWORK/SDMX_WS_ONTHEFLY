using FlyController.Builder;
using FlyController.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.CategoryScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.ConceptScheme;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using System;
using System.Collections.Generic;
namespace FlyEngine.Model
{
    /// <summary>
    /// Interface Model base for develop Metadata Builder
    /// </summary>
    public interface ISDMXObjectBuilder
    {
        /// <summary>
        /// Abstract void for build the External References
        /// </summary>
        void AddReferences();
        /// <summary>
        /// Abstract void for build this property with correct parameter
        /// </summary>
        void Build();
        /// <summary>
        /// Create a SdmxObjects from all parameter configured
        /// </summary>
        /// <returns>SdmxObject for write message</returns>
        ISdmxObjects CreateDSD();
        /// <summary>
        /// Call CreateDSD and Write SdmxObject in XElement Streaming to return with processed metadata result
        /// </summary>
        /// <returns>Object for Write response in streaming <see cref="IFlyWriterBody"/></returns>
        IFlyWriterBody WriteDSD();

        /// <summary>
        /// List of immutable instance of Categorisation
        /// </summary>
        List<ICategorisationObject> _CategorisationObject { get; set; }
        /// <summary>
        /// Immutable instance of CategoryScheme
        /// </summary>
        List<ICategorySchemeObject> _CategorySchemeObject { get; set; }
        /// <summary>
        /// Immutable instance of AgencyScheme
        /// </summary>
        IAgencyScheme _AgencyScheme { get; set; }
        /// <summary>
        /// List of immutable instance of Dataflow
        /// currently is only one
        /// </summary>
        List<IDataflowObject> _Dataflows { get; set; }
        /// <summary>
        ///  Immutable instance of ConceptsScheme
        /// </summary>
        List<IConceptSchemeObject> _Conceptscheme { get; set; }
        /// <summary>
        /// List of immutable instance of Codelist
        /// </summary>
        List<ICodelistMutableObject> _Codelists { get; set; }
        /// <summary>
        /// List of immutable instance of DataStructure (KeyFamily for Sdmx 2.0, Structure for Sdmx 2.1)
        /// </summary>
        List<DataStructureObjectImpl> _KeyFamily { get; set; }

        /// <summary>
        /// Processed request
        /// </summary>
        ISdmxParsingObject ParsingObject { get; set; }
        /// <summary>
        /// Sdmx Version
        /// </summary>
        SdmxSchemaEnumType VersionTypeResp { get; set; }


      
    }
}
