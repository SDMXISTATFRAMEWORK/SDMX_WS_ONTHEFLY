using Org.Sdmxsource.Sdmx.Api.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace FlyController.Model.Error
{
    /// <summary>
    /// FlyExceptionObject contains all Error caused from Application
    /// Association beetwen Internal Exception, Sdmx Exception, Internal Exception Description
    /// </summary>
    public class FlyExceptionObject
    {
        /// <summary>
        /// Internal Exception type Enumerator (All Exception that this Application can return)
        /// </summary>
        public enum FlyExceptionTypeEnum
        {
            /// <summary>
            /// Parsing Query Error
            /// </summary>
            ParsingQueryError,
            /// <summary>
            /// Soap Message Parsing Error
            /// </summary>
            SoapParseError,
            /// <summary>
            /// REST Message Parsing Error
            /// </summary>
            RESTParseError,
            /// <summary>
            /// Dataflow or Dataset not found in received structure
            /// </summary>
            DataflowNotFound,
            /// <summary>
            /// WS Internal Error
            /// </summary>
            InternalError,
            /// <summary>
            /// DB Connection Error
            /// </summary>
            ErrorConnectionDB,
            /// <summary>
            /// AgencyId not foud
            /// </summary>
            DifferentAgency,
            /// <summary>
            /// Version not found
            /// </summary>
            DifferentVersion,
            /// <summary>
            /// ConceptSchema format error
            /// </summary>
            ConceptSchemaFormatError,
            /// <summary>
            /// ConceptSchema invalid query value
            /// </summary>
            ConceptSchemaInvalid,
            /// <summary>
            /// Codelist format error
            /// </summary>
            CodelistFormatError,
            /// <summary>
            /// Codelist invalid query value
            /// </summary>
            CodelistInvalid,
            /// <summary>
            /// KeyFamily format error
            /// </summary>
            KeyFamilyFormatError,
            /// <summary>
            /// KeyFamily invalid query value
            /// </summary>
            KeyFamilyInvalid,
            /// <summary>
            /// Registry type not supported
            /// </summary>
            NotSupportedRegistryType,
            /// <summary>
            /// Structure not found
            /// </summary>
            StructureNotFound,
            /// <summary>
            /// DB Response Error
            /// </summary>
            DBErrorResponse,
            /// <summary>
            /// Dimension not found
            /// </summary>
            DimensionNotFound,
            /// <summary>
            /// Create Concept Reference Error
            /// </summary>
            CreateConceptReferenceError,
            /// <summary>
            /// Create ConceptScheme Reference Error
            /// </summary>
            CreateConceptSchemaReferenceError,
            /// <summary>
            /// Create Codelist Reference Error
            /// </summary>
            CreateCodelistReferenceError,
            /// <summary>
            /// Create Structure ReferenceError
            /// </summary>
            CreateStructureReferenceError,
            /// <summary>
            /// Create Category Reference Error
            /// </summary>
            CreateCategoryReferenceError,
            /// <summary>
            /// Create Data Structure Reference Error
            /// </summary>
            CreateDSDReferenceError,
            /// <summary>
            /// Create Message Response Error
            /// </summary>
            CreateMessageResponseError,
            /// <summary>
            /// Write Data Message Response
            /// </summary>
            WriteDataMessage,
            /// <summary>
            /// Create Sdmx Object Error
            /// </summary>
            CreateSdmxObjectError,
            /// <summary>
            /// Retrive Category data from DB Error
            /// </summary>
            DBCreateCategoryObjects,
            /// <summary>
            /// Get Attribute Error
            /// </summary>
            GetAttributeError,
            /// <summary>
            /// Get Frequency Codelist
            /// </summary>
            GetFrequencyCodelist,
            /// <summary>
            /// Error during trasform sdmxObject to ImmutableInstance
            /// </summary>
            CreateImmutable,
            /// <summary>
            /// Error during retrive Configuration from fileconfig
            /// </summary>
            InitConfigError,
            /// <summary>
            /// Error during create Header in Response object
            /// </summary>
            CreateHeader,
            /// <summary>
            /// Get Maintainable From SdmxObject
            /// </summary>
            GetMaintainableFromSdmxObject,
            /// <summary>
            /// Not implemented Attribute with AttachmentLevel=group
            /// </summary>
            AttributeErrorAttachmentGroup,
            /// <summary>
            /// Not implemented required output version 
            /// </summary>
            UnrecognizedVersion,
            /// <summary>
            /// Not correctly parsed Sdmx Query
            /// </summary>
            RetrivalParsingError,
            /// <summary>
            /// Error while writing DataMessage
            /// </summary>
            ErrorWriteDataMessage,
            /// <summary>
            /// Error while writing a TimeSeries DataMessage
            /// </summary>
            ErrorWriteTimeSeriesData,
            /// <summary>
            /// Error while writing a Flat DataMessage
            /// </summary>
            ErrorWriteFlatData,
            /// <summary>
            /// Error retrieving Series Keys in DataMessage response
            /// </summary>
            GetSeriesKey,
            /// <summary>
            /// Error retrieving Series Attributes in DataMessage response
            /// </summary>
            GetSeriesAttribute,
            /// <summary>
            /// Error retrieving Observation Attributes in DataMessage response
            /// </summary>
            GetObservationAttribute,
            /// <summary>
            /// Error retrieving Dataset Attributes in DataMessage response
            /// </summary>
            GetDataSetAttribute,
            /// <summary>
            /// Error in ChangeSeries during creation of DataMessage response
            /// </summary>
            CheckSeriesChange,
            /// <summary>
            /// Error while generating Agency Object
            /// </summary>
            GetAgencyScheme,
            /// <summary>
            /// Error while generating Agency Builder
            /// </summary>
            BuildAgencyScheme,
            /// <summary>
            /// Error while generating Category Builder
            /// </summary>
            BuildCategoryScheme,
            /// <summary>
            /// Error while generating Codelists Builder
            /// </summary>
            BuildCodelist,
            /// <summary>
            /// Error while generating Codelists Objects
            /// </summary>
            GetCodelist,
            /// <summary>
            /// Error while generating ConceptsScheme Builder
            /// </summary>
            BuildConceptsScheme,
            /// <summary>
            /// Error while generating ConceptsScheme objects
            /// </summary>
            GetConceptsScheme,
            /// <summary>
            /// Error while generating Dataflows Builder
            /// </summary>
            BuildDataflows,
            /// <summary>
            /// Error while generating Dataflows objects
            /// </summary>
            GetDataFlows,
            /// <summary>
            /// Error while generating DataStructure Builder
            /// </summary>
            BuildDSD,
            /// <summary>
            /// Error retrieving and formatting the results
            /// </summary>
            DataMessageEngineGetResults,
            /// <summary>
            /// Error during creation a SdmxObject
            /// </summary>
            CreateDSDObject,
            /// <summary>
            /// Error while parsing the request and the creation of the builder
            /// </summary>
            CreateBuilder,
            /// <summary>
            /// Error in parsing Format of ConceptsScheme Code
            /// </summary>
            GetDataFlowCodeFromConceptSchema,
            /// <summary>
            /// Error in parsing Format of Codelist Code
            /// </summary>
            GetConceptCodeFromCodelist,
            /// <summary>
            /// Error in parsing Format of DataFlow Code
            /// </summary>
            GetDataFlowCodeFromKeyFamily,
            /// <summary>
            /// Error during creation results table message
            /// </summary>
            GetTableMessage,
            /// <summary>
            /// Error during creation a IAgencyMutableObject
            /// </summary>
            CreateIAgencyMutableObject,
            /// <summary>
            /// Error during retreival Concepts
            /// </summary>
            GetListConcepts,
            /// <summary>
            /// Error during creation Category tree
            /// </summary>
            RecursiveCreateCategoryHierarchy,
            /// <summary>
            /// Error during creation a ICategoryMutableObject
            /// </summary>
            CreateICategoryMutableObject,
            /// <summary>
            /// Error in convert Iso-Code name in standard languages
            /// </summary>
            ParsingLanguagesIsoCode,
            /// <summary>
            /// Error during creation a ICodeMutableObject
            /// </summary>
            CreateICodeMutableObject,
            /// <summary>
            /// Error during Dataflows building
            /// </summary>
            CreateDataflowBuilder,
            /// <summary>
            /// Error during get Dimension Concepts
            /// </summary>
            GetDimensionConceptObjects,
            /// <summary>
            /// Error during creation Database store procedure parameter
            /// </summary>
            CreateDBParameter,
            /// <summary>
            /// Error during cast a time value for db data required
            /// </summary>
            CastingTimeFormat,
            /// <summary>
            /// Error during Find soap Body element in request
            /// </summary>
            FindSoapBobyElement,
            /// <summary>
            /// Error during Change soap Action element in response building
            /// </summary>
            ChangeSoapActionElement,
            /// <summary>
            /// Get Metadata Generic Error
            /// </summary>
            GetMetadata,
            /// <summary>
            /// Get Data Generic Error
            /// </summary>
            GetData,
            /// <summary>
            /// Error during dispose object used for retreival data
            /// </summary>
            DestroyObjects,
            /// <summary>
            /// Error during Add References into SdmxObject
            /// </summary>
            AddReferences,
            /// <summary>
            /// Method not implemented
            /// </summary>
            NotImplemented,
            /// <summary>
            /// Error during parse contrain parameters
            /// </summary>
            ConstrainParsingError,
            /// <summary>
            /// OnTheFly project require a Codelist Query whit a Contrain
            /// </summary>
            CodelistContrainRequired,
            /// <summary>
            /// In REST sdmx v2.0 the detail parameter must be full
            /// </summary>
            RestDetail20,
            /// <summary>
            /// Error while retreive groups from Database
            /// </summary>
            RetreiveGroups,
        }



        #region Init & Property
        /// <summary>
        /// Create FlyExceptionObject Instace
        /// </summary>
        /// <param name="_flyExceptionType">Internal Exception type</param>
        /// <param name="_sdmxErrorCodeType">SDMX Standard Error Type</param>
        /// <param name="_flyExceptionText">Additional exception description</param>
        public FlyExceptionObject(FlyExceptionTypeEnum _flyExceptionType, SdmxErrorCodeEnumType _sdmxErrorCodeType, string _flyExceptionText)
        {
            this.FlyExceptionType = _flyExceptionType;
            this.SDMXException = SDMXExceptionObject.Get(_sdmxErrorCodeType);
            this.FlyExceptionText = _flyExceptionText;
        }
        /// <summary>
        /// Internal Exception type
        /// </summary>
        public FlyExceptionTypeEnum FlyExceptionType { get; set; }
        /// <summary>
        /// SDMX Standard Error Type
        /// </summary>
        public SDMXExceptionObject SDMXException { get; set; }
        /// <summary>
        /// Additional exception description
        /// </summary>
        public string FlyExceptionText { get; set; }
        #endregion

        /// <summary>
        /// Parsed file ErrorDescriptions.xml contain all errors descriptions
        /// </summary>
        private static XElement ErrorsDescription = null;


        /// <summary>
        /// Find this istance in Static ErrorMapping
        /// </summary>
        /// <param name="_flyExceptionType">Internal Exception type</param>
        /// <returns>Founded FlyExceptionObject</returns>
        public static FlyExceptionObject Get(FlyExceptionTypeEnum _flyExceptionType)
        {
            try
            {
                if (ErrorsDescription == null)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(FlyConfiguration.ErrorDescriptionFile);
                    ErrorsDescription = XElement.Load(new XmlNodeReader(doc));
                }
                XElement err = (from el in ErrorsDescription.Elements()
                                where el.Attribute("Code").Value == _flyExceptionType.ToString()
                                select el).FirstOrDefault<XElement>();

                SdmxErrorCodeEnumType tipoErr = SdmxErrorCodeEnumType.InternalServerError;
                Enum.TryParse(err.Attribute("SdmxType").Value, true, out tipoErr);

                XElement Description = (from el in err.Elements()
                                        where el.Name == "Description" && el.Attribute("LocaleIsoCode").Value == "en"
                                        select el).FirstOrDefault<XElement>();

                return new FlyExceptionObject(_flyExceptionType, tipoErr, (string)Description);

            }
            catch (Exception ex)
            {
                return new FlyExceptionObject(FlyExceptionTypeEnum.InternalError, SdmxErrorCodeEnumType.InternalServerError, string.Format("No Error description configured --> Error: {0} Message: {1}", _flyExceptionType.ToString(), ex.Message));
            }
        }

    }
}
