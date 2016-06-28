using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOAPSdmx.Model
{
    /// <summary>
    /// Indicates what is the request that was made
    /// </summary>
    public class QueryOperation
    {
        /// <summary>
        /// Create a Instance of QueryOperation
        /// </summary>
        /// <param name="QueryType">indicates from which the request was made entrypoint</param>
        /// <param name="QueryVersion">Sdmx Version</param>
        public QueryOperation(MessageTypeEnum QueryType, MessageVersionEnum QueryVersion)
        {
            this.MessageType = QueryType;
            this.MessageVersion = QueryVersion;
        }

        /// <summary>
        /// Indicates from which the request was made entrypoint
        /// </summary>
        public MessageTypeEnum MessageType { get; set; }
        /// <summary>
        /// Sdmx Version
        /// </summary>
        public MessageVersionEnum MessageVersion { get; set; }
        /// <summary>
        /// Type of response (according to the request which was made)
        /// </summary>
        public ResponseMessageTypeEnum ResponseType { get { return GetResponse(); } }

        /// <summary>
        /// Sdmx possible versions
        /// </summary>
        public enum MessageVersionEnum
        {
            /// <summary>
            /// Sdmx 2.0 Version
            /// </summary>
            Version2_0,
            /// <summary>
            /// Sdmx 2.0 Version
            /// </summary>
            Version2_1,
        }

        /// <summary>
        /// All Message Entrypont for all SDMX version
        /// </summary>
        public enum MessageTypeEnum
        {
            //Tipologie 2.0
            /// <summary>
            /// Unrecognized Type
            /// </summary>
            Null = 0,
            /// <summary>
            /// GetCompactData entrypont for SDMX 2.0 version
            /// </summary>
            GetCompactData,
            /// <summary>
            /// GetCrossSectionalData entrypont for SDMX 2.0 version
            /// </summary>
            GetCrossSectionalData,
            /// <summary>
            /// QueryStructure entrypont for SDMX 2.0 version
            /// </summary>
            QueryStructure,

            //Tipologie 2.1
            /// <summary>
            /// GetGenericTimeSeriesData entrypont for SDMX 2.1 version
            /// </summary>
            GetGenericTimeSeriesData,
            /// <summary>
            /// GetStructureSpecificData entrypont for SDMX 2.1 version
            /// </summary>
            GetStructureSpecificData,
            /// <summary>
            /// GetStructureSpecificTimeSeriesData entrypont for SDMX 2.1 version
            /// </summary>
            GetStructureSpecificTimeSeriesData,
            /// <summary>
            /// GetStructures entrypont for SDMX 2.1 version
            /// </summary>
            GetStructures,
            /// <summary>
            /// GetDataflow entrypont for SDMX 2.1 version
            /// </summary>
            GetDataflow,
            /// <summary>
            /// GetCategorisation entrypont for SDMX 2.1 version
            /// </summary>
            GetCategorisation,
            /// <summary>
            /// GetCategoryScheme entrypont for SDMX 2.1 version
            /// </summary>
            GetCategoryScheme,
            /// <summary>
            /// GetConceptScheme entrypont for SDMX 2.1 version
            /// </summary>
            GetConceptScheme,
            /// <summary>
            /// GetCodelist entrypont for SDMX 2.1 version
            /// </summary>
            GetCodelist,
            /// <summary>
            /// GetHierarchicalCodelist entrypont for SDMX 2.1 version
            /// </summary>
            GetHierarchicalCodelist,
            /// <summary>
            /// GetOrganisationScheme entrypont for SDMX 2.1 version
            /// </summary>
            GetOrganisationScheme,

            //Comuni
            /// <summary>
            /// GetGenericData entrypont for SDMX 2.0 and SDMX 2.1 version
            /// </summary>
            GetGenericData,

        }

        /// <summary>
        /// All types of response to the soap version
        /// </summary>
        public enum ResponseMessageTypeEnum
        {
            /// <summary>
            ///     The default value.
            /// </summary>
            Null = 0,

            /// <summary>
            ///     The get compact data response.
            /// </summary>
            GetCompactDataResponse,

            /// <summary>
            ///     The get utility data response.
            /// </summary>
            GetUtilityDataResponse,

            /// <summary>
            ///     The get cross sectional data response.
            /// </summary>
            GetCrossSectionalDataResponse,

            /// <summary>
            ///     The query structure response.
            /// </summary>
            QueryStructureResponse,

            /// <summary>
            ///     The get generic data response.
            /// </summary>
            GetGenericDataResponse,

            /// <summary>
            ///     The get generic time series data response.
            /// </summary>
            GetGenericTimeSeriesDataResponse,

            /// <summary>
            ///     The get structure specific data response.
            /// </summary>
            GetStructureSpecificDataResponse,

            /// <summary>
            ///     The get structure specific time series data response.
            /// </summary>
            GetStructureSpecificTimeSeriesDataResponse,

            /// <summary>
            ///     The get generic metadata response.
            /// </summary>
            GetGenericMetadataResponse,

            /// <summary>
            ///     The get structure specific metadata response.
            /// </summary>
            GetStructureSpecificMetadataResponse,

            /// <summary>
            ///     The get structures response.
            /// </summary>
            GetStructuresResponse,

            /// <summary>
            ///     The get dataflow response.
            /// </summary>
            GetDataflowResponse,

            /// <summary>
            ///     The get metadataflow response.
            /// </summary>
            GetMetadataflowResponse,

            /// <summary>
            ///     The get data structure response.
            /// </summary>
            GetDataStructureResponse,

            /// <summary>
            ///     The get metadata structure response.
            /// </summary>
            GetMetadataStructureResponse,

            /// <summary>
            ///     The get category scheme response.
            /// </summary>
            GetCategorySchemeResponse,

            /// <summary>
            ///     The get concept scheme response.
            /// </summary>
            GetConceptSchemeResponse,

            /// <summary>
            ///     The get codelist response.
            /// </summary>
            GetCodelistResponse,

            /// <summary>
            ///     The get hierarchical codelist response.
            /// </summary>
            GetHierarchicalCodelistResponse,

            /// <summary>
            ///     The get organisation scheme response.
            /// </summary>
            GetOrganisationSchemeResponse,

            /// <summary>
            ///     The get reporting taxonomy response.
            /// </summary>
            GetReportingTaxonomyResponse,

            /// <summary>
            ///     The get structure set response.
            /// </summary>
            GetStructureSetResponse,

            /// <summary>
            ///     The get process response.
            /// </summary>
            GetProcessResponse,

            /// <summary>
            ///     The get categorisation response.
            /// </summary>
            GetCategorisationResponse,

            /// <summary>
            ///     The get provision agreement response.
            /// </summary>
            GetProvisionAgreementResponse,

            /// <summary>
            ///     The get constraint response.
            /// </summary>
            GetConstraintResponse,

            /// <summary>
            ///     The get data schema response.
            /// </summary>
            GetDataSchemaResponse,

            /// <summary>
            ///     The get metadata schema response.
            /// </summary>
            GetMetadataSchemaResponse
        }
        /// <summary>
        /// Get the Response (according to the request which was made)
        /// </summary>
        /// <returns>
        /// The <see cref="ResponseMessageTypeEnum"/>.
        /// </returns>
        private ResponseMessageTypeEnum GetResponse()
        {
            switch (MessageType)
            {
                case MessageTypeEnum.Null:
                    return ResponseMessageTypeEnum.Null;
                case MessageTypeEnum.QueryStructure:
                    return ResponseMessageTypeEnum.QueryStructureResponse;
                case MessageTypeEnum.GetCompactData:
                    return ResponseMessageTypeEnum.GetCompactDataResponse;
                case MessageTypeEnum.GetCrossSectionalData:
                    return ResponseMessageTypeEnum.GetCrossSectionalDataResponse;
                case MessageTypeEnum.GetGenericData:
                    return ResponseMessageTypeEnum.GetGenericDataResponse;
                case MessageTypeEnum.GetGenericTimeSeriesData:
                    return ResponseMessageTypeEnum.GetGenericTimeSeriesDataResponse;
                case MessageTypeEnum.GetStructureSpecificData:
                    return ResponseMessageTypeEnum.GetStructureSpecificDataResponse;
                case MessageTypeEnum.GetStructureSpecificTimeSeriesData:
                    return ResponseMessageTypeEnum.GetStructureSpecificTimeSeriesDataResponse;
                case MessageTypeEnum.GetStructures:
                    return ResponseMessageTypeEnum.GetStructuresResponse;
                case MessageTypeEnum.GetDataflow:
                    return ResponseMessageTypeEnum.GetDataflowResponse;
                case MessageTypeEnum.GetCategoryScheme:
                    return ResponseMessageTypeEnum.GetCategorySchemeResponse;
                case MessageTypeEnum.GetConceptScheme:
                    return ResponseMessageTypeEnum.GetConceptSchemeResponse;
                case MessageTypeEnum.GetCodelist:
                    return ResponseMessageTypeEnum.GetCodelistResponse;
                case MessageTypeEnum.GetHierarchicalCodelist:
                    return ResponseMessageTypeEnum.GetHierarchicalCodelistResponse;
                case MessageTypeEnum.GetOrganisationScheme:
                    return ResponseMessageTypeEnum.GetOrganisationSchemeResponse;
                case MessageTypeEnum.GetCategorisation:
                    return ResponseMessageTypeEnum.GetCategorisationResponse;
                //case MessageTypeEnum.GetGenericMetadata:
                //    return ResponseMessageTypeEnum.GetGenericMetadataResponse;
                //case MessageTypeEnum.GetStructureSpecificMetadata:
                //    return ResponseMessageTypeEnum.GetStructureSpecificMetadataResponse;
                //case MessageTypeEnum.GetMetadataflow:
                //    return ResponseMessageTypeEnum.GetMetadataflowResponse;
                //case MessageTypeEnum.GetDataStructure:
                //    return ResponseMessageTypeEnum.GetDataStructureResponse;
                //case MessageTypeEnum.GetMetadataStructure:
                //    return ResponseMessageTypeEnum.GetMetadataStructureResponse;
                //case MessageTypeEnum.GetReportingTaxonomy:
                //    return ResponseMessageTypeEnum.GetReportingTaxonomyResponse;
                //case MessageTypeEnum.GetStructureSet:
                //    return ResponseMessageTypeEnum.GetStructureSetResponse;
                //case MessageTypeEnum.GetProcess:
                //    return ResponseMessageTypeEnum.GetProcessResponse;
                //case MessageTypeEnum.GetProvisionAgreement:
                //    return ResponseMessageTypeEnum.GetProvisionAgreementResponse;
                //case MessageTypeEnum.GetConstraint:
                //    return ResponseMessageTypeEnum.GetConstraintResponse;
                //case MessageTypeEnum.GetDataSchema:
                //    return ResponseMessageTypeEnum.GetDataSchemaResponse;
                //case MessageTypeEnum.GetMetadataSchema:
                //    return ResponseMessageTypeEnum.GetMetadataSchemaResponse;
                default:
                    return ResponseMessageTypeEnum.Null;
            }
        }
    }
}