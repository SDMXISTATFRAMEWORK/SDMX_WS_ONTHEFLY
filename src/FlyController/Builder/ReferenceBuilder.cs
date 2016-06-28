using FlyController;
using FlyController.Model.Error;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyController.Builder
{
    /// <summary>
    /// ReferenceBuilder contains all Static function for StructureReference creation 
    /// </summary>
    public class ReferenceBuilder
    {

        /// <summary>
        /// Create StructureReference of SdmxStructureEnumType.Dataflow
        /// </summary>
        /// <param name="id">Dataflow Code</param>
        /// <param name="Agency">Dataflow Agency Id</param>
        /// <param name="Version">Dataflow Version</param>
        /// <returns></returns>
        public static IStructureReference CreateDataflowReference(string id, string Agency, string Version)
        {
            try
            {
                return new Org.Sdmxsource.Sdmx.Util.Objects.Reference.StructureReferenceImpl(Agency, id, Version, SdmxStructureEnumType.Dataflow);
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(typeof(FlyController.Builder.ReferenceBuilder), FlyExceptionObject.FlyExceptionTypeEnum.CreateConceptReferenceError, ex);
            }
        }
        /// <summary>
        /// Create StructureReference of SdmxStructureEnumType.Concept
        /// </summary>
        /// <param name="ConceptSchemaId">ConceptSchema Code</param>
        /// <param name="id">Concept Code</param>
        /// <returns></returns>
        public static IStructureReference CreateConceptReference(string ConceptSchemaId, string id)
        {
            try
            {
                return new Org.Sdmxsource.Sdmx.Util.Objects.Reference.StructureReferenceImpl(FlyConfiguration.MainAgencyId, ConceptSchemaId, FlyConfiguration.Version, SdmxStructureEnumType.Concept, id);
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(typeof(FlyController.Builder.ReferenceBuilder), FlyExceptionObject.FlyExceptionTypeEnum.CreateConceptReferenceError, ex);
            }
        }

         /// <summary>
        /// Create StructureReference of SdmxStructureEnumType.Codelist
        /// </summary>
        /// <param name="id">Codelist Code</param>
        /// <returns></returns>
        public static IStructureReference CreateCodelistReference(string id)
        {
            try
            {
                return new Org.Sdmxsource.Sdmx.Util.Objects.Reference.StructureReferenceImpl(FlyConfiguration.MainAgencyId, id, FlyConfiguration.Version, SdmxStructureEnumType.CodeList);
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(typeof(FlyController.Builder.ReferenceBuilder), FlyExceptionObject.FlyExceptionTypeEnum.CreateCodelistReferenceError, ex);
            }
        }

      
        /// <summary>
        /// Create StructureReference of SdmxStructureEnumType.Category
        /// </summary>
        /// <param name="CategorySchemeCode">CategoryScheme Code</param>
        /// <param name="CategorySchemeAgency">CategoryScheme Agency</param>
        /// <param name="CategorySchemeVersion">CategoryScheme Version</param>
        /// <param name="CategoryCode">Category Code</param>
        /// <returns></returns>
        public static IStructureReference CreateCategoryReference(string CategorySchemeCode,string CategorySchemeAgency, string CategorySchemeVersion, params string[] CategoryCode)
        {
            try
            {
                IStructureReference Sref = new Org.Sdmxsource.Sdmx.Util.Objects.Reference.StructureReferenceImpl(CategorySchemeAgency, CategorySchemeCode, CategorySchemeVersion, SdmxStructureEnumType.Category, CategoryCode);
                return Sref;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(typeof(FlyController.Builder.ReferenceBuilder), FlyExceptionObject.FlyExceptionTypeEnum.CreateCategoryReferenceError, ex);
            }
        }

        /// <summary>
        /// Create StructureReference of SdmxStructureEnumType.Dsd
        /// </summary>
        /// <param name="DataflowId">Dataflow Code</param>
        /// <returns></returns>
        public static IStructureReference CreateDSDStructureReference(string DataflowId)
        {
            try
            {
                return new Org.Sdmxsource.Sdmx.Util.Objects.Reference.StructureReferenceImpl(
                    FlyConfiguration.MainAgencyId,
                    string.Format(FlyConfiguration.DsdFormat,DataflowId),
                    FlyConfiguration.Version,
                    Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureEnumType.Dsd);
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(typeof(FlyController.Builder.ReferenceBuilder), FlyExceptionObject.FlyExceptionTypeEnum.CreateDSDReferenceError, ex);
            }
        }

    }
}
