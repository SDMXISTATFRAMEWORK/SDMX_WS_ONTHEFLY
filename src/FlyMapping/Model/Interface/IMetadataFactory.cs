using FlyController.Model;
using FlyMapping.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using System;
namespace FlyMapping.Model
{

    /// <summary>
    /// Management Instance of FlyMapping
    /// </summary>
    public interface IMetadataFactory
    {
        /// <summary>
        /// initializes the correct class for the IAgencyManager interface
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        /// <returns><see cref="IAgencyManager"/> Instance</returns>        
        IAgencyManager InstanceAgencyManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp);
        /// <summary>
        /// initializes the correct class for the ICategoriesManager interface
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        /// <returns><see cref="ICategoriesManager"/> Instance</returns>
        ICategoriesManager InstanceCategoriesManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp);
        /// <summary>
        /// initializes the correct class for the ICodelistsManager interface
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        /// <returns><see cref="ICodelistManager"/> Instance</returns>
        ICodelistManager InstanceCodelistsManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp);
        /// <summary>
        /// initializes the correct class for the IDataflowsManager interface
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        /// <returns><see cref="IDataflowsManager"/> Instance</returns>
        IDataflowsManager InstanceDataflowsManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp);
        /// <summary>
        /// initializes the correct class for the IConceptSchemeManager interface
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        /// <returns><see cref="IConceptSchemeManager"/> Instance</returns>
        IConceptSchemeManager InstanceConceptSchemeManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp);
        /// <summary>
        /// initializes the correct class for the IDsdManager interface
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        /// <returns><see cref="IDsdManager"/> Instance</returns>
        IDsdManager InstanceDsdManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp);
     
    }
}
