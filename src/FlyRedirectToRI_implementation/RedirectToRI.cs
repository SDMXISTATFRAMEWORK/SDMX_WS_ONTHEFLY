using Estat.Sdmxsource.Extension.Builder;
using Estat.Sri.CustomRequests.Factory;
using Estat.Sri.CustomRequests.Manager;
using Estat.Sri.CustomRequests.Model;
using FlyController;
using FlyController.Model;
using FlyController.Model.Error;
using FlyRedirectToRI_implementation.Builder;
using FlyRedirectToRI_implementation.Interfaces;
using FlySDDSLoader_implementation.Manager;
using Org.Sdmxsource.Sdmx.Api.Builder;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Manager.Query;
using Org.Sdmxsource.Sdmx.Api.Model.Data.Query;
using Org.Sdmxsource.Sdmx.Api.Model.Data.Query.Complex;
using Org.Sdmxsource.Sdmx.Api.Model.Format;
using Org.Sdmxsource.Sdmx.Api.Model.Query;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Data.Query;
using Org.Sdmxsource.Sdmx.SdmxQueryBuilder.Factory;
using Org.Sdmxsource.Sdmx.SdmxQueryBuilder.Manager;
using Org.Sdmxsource.Sdmx.SdmxQueryBuilder.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace FlyRedirectToRI_implementation
{
    /// <summary>
    /// Check redirect to Ri WebServices and provides the methods for make the request and process the response
    /// </summary>
    public class RedirectToRI
    {
        private string DataFlowElementId { get; set; }
        private string DataFlowAgencyId { get; set; }
        private string DataFlowVersion { get; set; }
        private string DataSetCode { get; set; }

        /// <summary>
        /// Initialize <see cref="RedirectToRI"/>
        /// </summary>
        /// <param name="_DataFlowElementId">Dataflow Code</param>
        /// <param name="_DataFlowAgencyId">Dataflow Agency</param>
        /// <param name="_DataFlowVersion">Dataflow version</param>
        public RedirectToRI(string _DataFlowElementId, string _DataFlowAgencyId, string _DataFlowVersion)
        {
            this.DataFlowElementId = _DataFlowElementId;
            this.DataFlowAgencyId = _DataFlowAgencyId;
            this.DataFlowVersion = _DataFlowVersion;
        }
        /// <summary>
        /// Initialize <see cref="RedirectToRI"/>
        /// </summary>
        /// <param name="_DataSetCode">Dataset Code</param>
        public RedirectToRI(string _DataSetCode)
        {
            this.DataSetCode = _DataSetCode;
        }

        /// <summary>
        /// Check Redirect To RI
        /// if is OnTheFly 2.0 Version and Call a SP for check id DataflowId is of type MappingAssistant
        /// </summary>
        /// <returns></returns>
        public bool CheckRESTRedirectToRI()
        {
            if (FlyConfiguration.OnTheFlyVersion == OnTheFlyVersionEnum.OnTheFly2
                && FlyConfiguration.RIWebServices != null
                && !string.IsNullOrEmpty(FlyConfiguration.RIWebServices.RestUri)
                && !string.IsNullOrEmpty(this.DataFlowElementId))
            {
                Uri resUri;
                if (!Uri.TryCreate(FlyConfiguration.RIWebServices.RestUri, UriKind.RelativeOrAbsolute, out resUri))
                    return false;
                else if (!FlyConfiguration.RIWebServices.RestUri.EndsWith("/") && !FlyConfiguration.RIWebServices.RestUri.EndsWith("\\"))
                    FlyConfiguration.RIWebServices.RestUri += "/";

                return CheckDataflowIsMA();
            }
            return false;
        }
        /// <summary>
        /// Check Redirect To RI
        /// if is OnTheFly 2.0 Version and Call a SP for check id DataflowId is of type MappingAssistant
        /// </summary>
        /// <returns></returns>
        public bool CheckSOAPRedirectToRI(SdmxSchemaEnumType VersionTypeResp)
        {
            if (FlyConfiguration.OnTheFlyVersion == OnTheFlyVersionEnum.OnTheFly2
                && FlyConfiguration.RIWebServices != null
                && (!string.IsNullOrEmpty(this.DataFlowElementId) || !string.IsNullOrEmpty(this.DataSetCode)))
            {
                Uri resUri;

                if (VersionTypeResp == SdmxSchemaEnumType.VersionTwo)
                {
                    if (string.IsNullOrEmpty(FlyConfiguration.RIWebServices.Soap20Uri) || !Uri.TryCreate(FlyConfiguration.RIWebServices.Soap20Uri, UriKind.RelativeOrAbsolute, out resUri))
                        return false;
                    else if (!FlyConfiguration.RIWebServices.Soap20Uri.EndsWith("/") && !FlyConfiguration.RIWebServices.Soap20Uri.EndsWith("\\"))
                        FlyConfiguration.RIWebServices.Soap20Uri += "/";
                }
                else if (VersionTypeResp == SdmxSchemaEnumType.VersionTwoPointOne)
                {
                    if (string.IsNullOrEmpty(FlyConfiguration.RIWebServices.Soap21Uri) || !Uri.TryCreate(FlyConfiguration.RIWebServices.Soap21Uri, UriKind.RelativeOrAbsolute, out resUri))
                        return false;
                    else if (!FlyConfiguration.RIWebServices.Soap21Uri.EndsWith("/") && !FlyConfiguration.RIWebServices.Soap21Uri.EndsWith("\\"))
                        FlyConfiguration.RIWebServices.Soap21Uri += "/";
                }

                return CheckDataflowIsMA();
            }
            return false;
        }

        private bool CheckDataflowIsMA()
        {
            ISdmxParsingObject parsingobj = new SdmxParsingObject(SdmxStructureEnumType.Dataflow);

            FlySDDSLoader_implementation.Manager.Metadata.DataflowsManager dfman = new FlySDDSLoader_implementation.Manager.Metadata.DataflowsManager(parsingobj, SdmxSchemaEnumType.VersionTwoPointOne);
            List<MSDataflow> dataflows = dfman.GetMSDataflows(new BuilderParameter());
            if (dataflows == null || dataflows.Count == 0)
                return false;

            MSDataflow dataflowFound = null;
            if (!string.IsNullOrEmpty(DataFlowElementId))
            {
                List<MSDataflow> dataflowsFound = dataflows.FindAll(df => df.DFCode.Trim().ToUpper() == this.DataFlowElementId.Trim().ToUpper());
                if (dataflowsFound == null || dataflowsFound.Count == 0)
                    return false;
                if (!string.IsNullOrEmpty(DataFlowAgencyId) && DataFlowAgencyId != "*" && DataFlowAgencyId.Trim().ToUpper() != "ALL")
                {
                    dataflowsFound = dataflowsFound.FindAll(df => df.DFAgency.Trim().ToUpper() == DataFlowAgencyId.Trim().ToUpper());
                    if (dataflowsFound == null || dataflowsFound.Count == 0)
                        return false;
                }
                if (!string.IsNullOrEmpty(DataFlowVersion) && DataFlowVersion != "*" && DataFlowVersion.Trim().ToUpper() != "ALL")
                {
                    dataflowsFound = dataflowsFound.FindAll(df => df.DFVersion.Trim().ToUpper() == DataFlowVersion.Trim().ToUpper());
                }
                if (dataflowsFound == null || dataflowsFound.Count == 0)
                    return false;
                dataflowFound = dataflowsFound[0];
            }
            else if (!string.IsNullOrEmpty(DataSetCode))
            {
                MSDataflow dataSetFound = dataflows.Find(df => df.DatasetList != null && df.DatasetList.Exists(ds => ds.Trim().ToUpper() == this.DataSetCode.Trim().ToUpper()));
                if (dataSetFound == null)
                    return false;
                dataflowFound = dataSetFound;
            }

            if (dataflowFound == null)
                return false;
            return !string.IsNullOrEmpty(dataflowFound.DFProduction)
                && dataflowFound.DFProduction.Trim().ToUpper() == "MA";
        }

        /// <summary>
        /// Build <see cref="IFlyWriterBody"/> response Message for Soap request
        /// </summary>
        /// <param name="xmlQuery">Element query whitout Envelop header</param>
        /// <param name="messageType">Type of request both SDMX 2.0 and Sdmx 2.1</param>
        /// <param name="versionTypeResp">Sdmx Version</param>
        /// <returns></returns>
        public IFlyWriterBody GetSOAPResultFromRI(XElement xmlQuery, string messageType, SdmxSchemaEnumType versionTypeResp)
        {
            IRequestManagement SoapRM = new SoapRequestManagement()
            {
                XmlQuery = xmlQuery,
                MessageType = messageType,
                VersionTypeResp = versionTypeResp
            };
            return SoapRM.CreateResponseMessage();
        }

        /// <summary>
        /// Build <see cref="IFlyWriterBody"/> response Message for Rest request
        /// </summary>
        /// <param name="query">Contains all object and methods for writing a response <see cref="IRestStructureQuery"/></param>
        /// <param name="requestAccept">entire request string</param>
        /// <returns></returns>
        public IFlyWriterBody GetRESTResultFromRI(string query, string requestAccept)
        {
            IRequestManagement RestRM = new RestRequestManagement()
            {
                RestQuery = query,
                RequestAccept = requestAccept
            };
            return RestRM.CreateResponseMessage();
        }

        /// <summary>
        /// Build <see cref="IFlyWriterBody"/> response Message for Rest request
        /// </summary>
        /// <param name="query">Contains all object and methods for writing a response <see cref="IRestStructureQuery"/></param>
        /// <param name="RestmessageType">Call type</param>
        /// <returns></returns>
        public IFlyWriterBody GetRESTResultFromRIToSoap(IDataQuery query, MessageTypeEnum RestmessageType)
        {
            string MessageType = "";
            SdmxSchemaEnumType VersionTypeResp;
            switch (RestmessageType)
            {
                case MessageTypeEnum.Compact_20:
                    MessageType = "GetCompactData";
                    VersionTypeResp = SdmxSchemaEnumType.VersionTwo;
                    break;
                case MessageTypeEnum.Generic_20:
                    MessageType = "GetGenericData";
                    VersionTypeResp = SdmxSchemaEnumType.VersionTwo;
                    break;
                case MessageTypeEnum.GenericData_21:
                    MessageType = "GetGenericData";
                    VersionTypeResp = SdmxSchemaEnumType.VersionTwoPointOne;
                    break;
                case MessageTypeEnum.StructureSpecific_21:
                    MessageType = "GetStructureSpecificData";
                    VersionTypeResp = SdmxSchemaEnumType.VersionTwoPointOne;
                    break;
                default:
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.NotImplemented);
            }


            XDocument xdoc;
            if (VersionTypeResp == SdmxSchemaEnumType.VersionTwo)
            {
                IDataQueryBuilderManager dataQueryBuilderManager = new DataQueryBuilderManager(new DataQueryFactory());
                xdoc = dataQueryBuilderManager.BuildDataQuery(query, new QueryMessageV2Format());
            }
            else
            {
                IDataQueryFormat<XDocument> queryFormat = new StructSpecificDataFormatV21();
                switch (RestmessageType)
                {
                    case MessageTypeEnum.GenericData_21:
                        queryFormat = new GenericDataDocumentFormatV21();
                        break;
                    case MessageTypeEnum.StructureSpecific_21:
                        queryFormat = new StructSpecificDataFormatV21();
                        break;
                }
                IBuilder<IComplexDataQuery, IDataQuery> transformer = new DataQuery2ComplexQueryBuilder(true);
                IComplexDataQuery complexDataQuery = transformer.Build(query);

                IComplexDataQueryBuilderManager complexDataQueryBuilderManager = new ComplexDataQueryBuilderManager(new ComplexDataQueryFactoryV21());
                xdoc = complexDataQueryBuilderManager.BuildComplexDataQuery(complexDataQuery, queryFormat);
            }


            IRequestManagement SoapRM = new SoapRequestManagement()
            {
                XmlQuery = xdoc.Root,
                MessageType = MessageType,
                VersionTypeResp = VersionTypeResp
            };
            return SoapRM.CreateResponseMessage();
        }


        /// <summary>
        /// Convert a Rest Request codelist constrain in Soap 2.0 Codelist Constrain and send this to Ri WebServices
        /// </summary>
        /// <param name="parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <returns></returns>
        public IFlyWriterBody GetRESTCodelistConstrain(ISdmxParsingObject parsingObject)
        {
            IRequestManagement SoapRM = new SoapRequestManagement()
            {
                XmlQuery = ParseCodelistCostrain(parsingObject),
                MessageType = "QueryStructure",
                VersionTypeResp = SdmxSchemaEnumType.VersionTwo
            };
            return SoapRM.CreateResponseMessage();
        }

        private XElement ParseCodelistCostrain(ISdmxParsingObject parsingObject)
        {
            string CostrainCodelist= @"<RegistryInterface xmlns=""http://www.SDMX.org/resources/SDMXML/schemas/v2_0/message"" xmlns:common=""http://www.SDMX.org/resources/SDMXML/schemas/v2_0/common"" xmlns:registry=""http://www.SDMX.org/resources/SDMXML/schemas/v2_0/registry"" xmlns:structure=""http://www.SDMX.org/resources/SDMXML/schemas/v2_0/structure"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://www.SDMX.org/resources/SDMXML/schemas/v2_0/message SDMXMessage.xsd"">
  <Header>
     <ID>OnTheFly</ID>
    <Test>false</Test>
    <Prepared>2015-03-04T10:40:38</Prepared>
    <Sender id=""OnTheFly"" />
    <Receiver id=""OnTheFly"" />
  </Header>
  <QueryStructureRequest resolveReferences=""false"">
    <registry:CodelistRef>
      <registry:AgencyID>###AGENCYID###</registry:AgencyID>
      <registry:CodelistID>###CL_CODELIST###</registry:CodelistID>
      <registry:Version>###VERSION###</registry:Version>
    </registry:CodelistRef>
    <registry:DataflowRef>
      <registry:AgencyID>###DFAGENCYID###</registry:AgencyID>
      <registry:DataflowID>###DATAFLOWID###</registry:DataflowID>
      <registry:Version>###DFVERSION###</registry:Version>
      <registry:Constraint ConstraintType=""Content"">
        <common:ConstraintID>###DIMENSION###</common:ConstraintID>
        <common:CubeRegion isIncluded=""true"">
          <common:Member isIncluded=""true"">
            <common:ComponentRef>###DIMENSION###</common:ComponentRef>
          </common:Member>
        </common:CubeRegion>
      </registry:Constraint>
    </registry:DataflowRef>
  </QueryStructureRequest>
</RegistryInterface>
";

            CostrainCodelist = CostrainCodelist.Replace("###AGENCYID###", parsingObject.AgencyId);
            CostrainCodelist = CostrainCodelist.Replace("###CL_CODELIST###", parsingObject.MaintainableId);
            CostrainCodelist = CostrainCodelist.Replace("###VERSION###", parsingObject._version);

            CostrainCodelist = CostrainCodelist.Replace("###DFAGENCYID###", parsingObject.ConstrainDataFlowAgency);
            CostrainCodelist = CostrainCodelist.Replace("###DATAFLOWID###", parsingObject.ConstrainDataFlow);
            CostrainCodelist = CostrainCodelist.Replace("###DFVERSION###", parsingObject.ConstrainDataFlowVersion);

            CostrainCodelist = CostrainCodelist.Replace("###DIMENSION###", parsingObject.ConstrainConcept);

            return XElement.Parse(CostrainCodelist);
        }
    }
}
