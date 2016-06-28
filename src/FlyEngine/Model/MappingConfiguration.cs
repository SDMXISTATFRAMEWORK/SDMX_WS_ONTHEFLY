using FlyController.Model;
using FlyController.Model.Error;
using FlyMapping.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyEngine.Model
{
    /// <summary>
    /// Configure Implementation of Interfaces of FlyMapping
    /// </summary>
    public class MappingConfiguration
    {
        /// <summary>
        /// Instance of <see cref="IMetadataFactory"/>
        /// </summary>
        public static IMetadataFactory MetadataFactory
        {
            get
            {
                switch (FlyController.FlyConfiguration.OnTheFlyVersion)
                {
                    case FlyController.Model.OnTheFlyVersionEnum.OnTheFly1:
                        return new FlyDotStat_implementation.Manager.MetadataFactory();
                    case FlyController.Model.OnTheFlyVersionEnum.OnTheFly15:
                        return new FlyDotStatExtra_implementation.Manager.MetadataFactory();
                    case FlyController.Model.OnTheFlyVersionEnum.OnTheFly2:
                        return new FlySDDSLoader_implementation.Manager.MetadataFactory();
                    default:
                        throw new SdmxException(typeof(MappingConfiguration), FlyExceptionObject.FlyExceptionTypeEnum.InitConfigError, new Exception("On the Fly Version Error"));
                }
            }
        }


        /// <summary>
        /// Instance of <see cref="IDataMessageManager"/>
        /// </summary>
        /// <param name="VersionTypeResp">Sdmx Version of response</param>
        /// <returns></returns>
        public static IDataMessageManager DataFactory(SdmxSchemaEnumType VersionTypeResp)
        {
            switch (FlyController.FlyConfiguration.OnTheFlyVersion)
            {
                case FlyController.Model.OnTheFlyVersionEnum.OnTheFly1:
                    return new FlyDotStat_implementation.Manager.Data.DataMessageManager(VersionTypeResp);
                case FlyController.Model.OnTheFlyVersionEnum.OnTheFly15:
                    return new FlyDotStat_implementation.Manager.Data.DataMessageManager(VersionTypeResp);
                case FlyController.Model.OnTheFlyVersionEnum.OnTheFly2:
                    return new FlyDotStat_implementation.Manager.Data.DataMessageManager(VersionTypeResp);
                default:
                    throw new SdmxException(typeof(MappingConfiguration), FlyExceptionObject.FlyExceptionTypeEnum.InitConfigError, new Exception("On the Fly Version Error"));
            }
        }

        /// <summary>
        /// Instance of <see cref="IDataMessageManager"/>
        /// </summary>
        /// <param name="TimeStamp">LastUpdate parameter request only data from this date onwards</param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        /// <returns></returns>
        public static IGroupsManager GroupFactory(string TimeStamp, SdmxSchemaEnumType _versionTypeResp)
        {
            switch (FlyController.FlyConfiguration.OnTheFlyVersion)
            {
                case FlyController.Model.OnTheFlyVersionEnum.OnTheFly1:
                    return new FlyDotStat_implementation.Manager.Data.GroupsManager(TimeStamp, _versionTypeResp);
                case FlyController.Model.OnTheFlyVersionEnum.OnTheFly15:
                    return new FlyDotStat_implementation.Manager.Data.GroupsManager(TimeStamp, _versionTypeResp);
                case FlyController.Model.OnTheFlyVersionEnum.OnTheFly2:
                    return new FlyDotStat_implementation.Manager.Data.GroupsManager(TimeStamp, _versionTypeResp);
                default:
                    throw new SdmxException(typeof(MappingConfiguration), FlyExceptionObject.FlyExceptionTypeEnum.InitConfigError, new Exception("On the Fly Version Error"));
            }
        }
    }
}

