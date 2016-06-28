using FlyController;
using FlyController.Model;
using FlyController.Model.DbSetting;
using FlyDotStatExtra_implementation.Manager.Metadata;
using FlyMapping.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyDotStatExtra_implementation.Manager
{
    /// <summary>
    /// Management Instance of FlyMapping
    /// </summary>
    public class MetadataFactory : IMetadataFactory
    {
        /// <summary>
        /// initializes the correct class for the IAgencyManager interface
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        /// <returns><see cref="IAgencyManager"/> Instance</returns>
        public IAgencyManager InstanceAgencyManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
        {
            return new FlyDotStat_implementation.Manager.Metadata.AgencyManager(_parsingObject, _versionTypeResp);
        }

        /// <summary>
        /// initializes the correct class for the ICategoriesManager interface
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        /// <returns><see cref="ICategoriesManager"/> Instance</returns>
        public ICategoriesManager InstanceCategoriesManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
        {
            //if (string.IsNullOrEmpty(FlyConfiguration.ConnectionStringCategory))
            //    return new FlyDotStat_implementation.Manager.Metadata.CategoriesManagerSP(_parsingObject, _versionTypeResp);
            //else
            //    return new FlyDotStat_implementation.Manager.Metadata.CategoriesManager(_parsingObject, _versionTypeResp);

            return new CategoriesManager(_parsingObject, _versionTypeResp);
        }

        /// <summary>
        /// initializes the correct class for the IDataflowsManager interface
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        /// <returns><see cref="IDataflowsManager"/> Instance</returns>
        public IDataflowsManager InstanceDataflowsManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
        {

            IDataflowsManager dfManager = new FlyDotStat_implementation.Manager.Metadata.DatasetsManager(_parsingObject, _versionTypeResp);
            if (dfManager.DbAccess.CheckExistStoreProcedure(DBOperationEnum.GetDataflows))
                return new FlyDotStat_implementation.Manager.Metadata.DataflowsManager(_parsingObject, _versionTypeResp);
            return dfManager;
        }

        /// <summary>
        /// initializes the correct class for the IConceptSchemeManager interface
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        /// <returns><see cref="IConceptSchemeManager"/> Instance</returns>
        public IConceptSchemeManager InstanceConceptSchemeManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
        {
            IConceptSchemeManager csManager = new ConceptSchemeManager(_parsingObject, _versionTypeResp);
            return csManager;
        }

        /// <summary>
        /// initializes the correct class for the ICodelistsManager interface
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        /// <returns><see cref="ICodelistManager"/> Instance</returns>
        public ICodelistManager InstanceCodelistsManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
        {
            return new CodelistManager(_parsingObject, _versionTypeResp);
        }

        /// <summary>
        /// initializes the correct class for the IDsdManager interface
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        /// <returns><see cref="IDsdManager"/> Instance</returns>
        public IDsdManager InstanceDsdManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
        {
            return new DsdManager(_parsingObject, _versionTypeResp);
        }

       
    }
}
