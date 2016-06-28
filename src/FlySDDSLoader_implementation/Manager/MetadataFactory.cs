using FlyController;
using FlyController.Model;
using FlyController.Model.DbSetting;
using FlySDDSLoader_implementation.Manager.Metadata;
using FlyMapping.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;

namespace FlySDDSLoader_implementation.Manager
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
            return new FlyDotStatExtra_implementation.Manager.MetadataFactory().InstanceAgencyManager(_parsingObject, _versionTypeResp);
        }

        /// <summary>
        /// initializes the correct class for the ICategoriesManager interface
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        /// <returns><see cref="ICategoriesManager"/> Instance</returns>
        public ICategoriesManager InstanceCategoriesManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
        {
           /* ICategoriesManager catman = null;
            if (string.IsNullOrEmpty(FlyConfiguration.ConnectionStringCategory))
                catman = new FlyDotStat_implementation.Manager.Metadata.CategoriesManagerSP(_parsingObject, _versionTypeResp);
            else
                catman = new FlyDotStat_implementation.Manager.Metadata.CategoriesManager(_parsingObject, _versionTypeResp);
           
            catman.ReferencesObject = new FlyEngine.Model.IReferencesObject();

            parsingObject = _parsingObject;
            versionTypeResp = _versionTypeResp;
            ((FlyDotStat_implementation.Manager.Metadata.BaseCategoriesManager)catman).GetDataFlowDelegate =
                new FlyDotStat_implementation.Manager.Metadata.BaseCategoriesManager.InternalGetDataFlowDelegate(InternalGetDataFlow);

            return catman;*/
            return new CategoriesManager(_parsingObject, _versionTypeResp);

        }

        //private ISdmxParsingObject parsingObject;
        //private SdmxSchemaEnumType versionTypeResp;
        //internal List<IDataflowObject> InternalGetDataFlow()
        //{
        //    IDataflowsManager dfman = InstanceDataflowsManager(parsingObject, versionTypeResp);
        //    return dfman.GetDataFlows();
        //}

        /// <summary>
        /// initializes the correct class for the IDataflowsManager interface
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        /// <returns><see cref="IDataflowsManager"/> Instance</returns>
        public IDataflowsManager InstanceDataflowsManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
        {
            return new DataflowsManager(_parsingObject, _versionTypeResp);
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
