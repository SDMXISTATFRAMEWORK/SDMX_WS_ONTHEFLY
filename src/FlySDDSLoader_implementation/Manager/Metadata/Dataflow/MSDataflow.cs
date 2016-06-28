using FlyController.Model;
using FlyController.Model.Error;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlySDDSLoader_implementation.Manager
{
    /// <summary>
    /// DataFlow Object that contains also Code and Id for Merge Metadata from 2 Database
    /// </summary>
    public class MSDataflow
    {
        /// <summary>
        /// Dataflow ID
        /// </summary>
        public int IdDf { get; set; }
        /// <summary>
        /// Dataflow Code
        /// </summary>
        public string DFCode { get; set; }
        /// <summary>
        /// Dataflow AgengyId
        /// </summary>
        public string DFAgency { get; set; }
        /// <summary>
        /// Dataflow Version
        /// </summary>
        public string DFVersion { get; set; }
        /// <summary>
        /// Dataflow Production
        /// is required in FlyRedirectToRI_implementations
        /// </summary>
        public string DFProduction { get; set; }

        /// <summary>
        /// List of associated Dataset Code
        /// </summary>
        public List<string> DatasetList { get; set; }
        
        internal List<SdmxObjectNameDescription> Descr { get; set; }

        internal IStructureReference GetDataStrunctureRef(string DsdCode, string DsdAgency, string DsdVersion)
        {
            try
            {
                if (string.IsNullOrEmpty(DsdCode))
                {
                    return new Org.Sdmxsource.Sdmx.Util.Objects.Reference.StructureReferenceImpl(
                       FlyController.FlyConfiguration.MainAgencyId,
                       "DSD_NOT_FOUND",
                       FlyController.FlyConfiguration.Version,
                       Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Dsd);
                }

                return new Org.Sdmxsource.Sdmx.Util.Objects.Reference.StructureReferenceImpl(
                    DsdAgency,
                    DsdCode,
                    DsdVersion,
                    Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Dsd);
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(typeof(FlyController.Builder.ReferenceBuilder), FlyExceptionObject.FlyExceptionTypeEnum.CreateDSDReferenceError, ex);
            }
        }

        /// <summary>
        /// Referenced DSD Code
        /// </summary>
        public string DsdCode { get; set; }

        /// <summary>
        /// Referenced DSD Agency
        /// </summary>
        public string DsdAgency { get; set; }

        /// <summary>
        /// Referenced DSD Version
        /// </summary>
        public string DsdVersion { get; set; }
    }
}
