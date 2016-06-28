using FlyCallWS;
using FlyCallWS.Streaming;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace TestOnTheFlyService
{
    public class SendQueryStreaming
    {
        public string ErrorString { get; set; }


        #region InviaQuery
        public enum StructTypeEnum
        {
            Agency,
            DSD,
            Categorisation,
            Category,
            Dataflow,
            Concepts,
            Codelist,
            ConstrainedCodelist,
            Generic,
            Compact,

            GenericData,
            StructureSpecificData,
        }

        public enum DetailEnum
        {
            Full = 1,
            AllStubs = 2,
            ReferencedStubs = 3,
        }
        public enum ReferencesEnum
        {
            None = 1,
            Parents = 2,
            ParentsAndSiblings = 3,
            Children = 4,
            Descendants = 5,
            All = 6,
            Specific = 7,
        }
        public XElement GetData(StructTypeEnum tipo, string val, TestFlyQueryCreation.SdmxVersionEnum SdmxVersion, TestFlyQueryCreation.ComunicationTypeEnum ComunicationType, string reference, DetailEnum detail, WinXmlViewer MetadataView)
        {
            return GetData(tipo, val, "ALL", "*", SdmxVersion, ComunicationType, reference, detail, MetadataView);
        }
        public XElement GetData(StructTypeEnum tipo, string val, string agency, string version, TestFlyQueryCreation.SdmxVersionEnum SdmxVersion, TestFlyQueryCreation.ComunicationTypeEnum ComunicationType, string reference, DetailEnum detail, WinXmlViewer MetadataView)
        {
            string query = getQuery(tipo, val, agency, version, SdmxVersion, ComunicationType, reference, detail);
            if (MetadataView != null)
                MetadataView.Testo = query;
            switch (ComunicationType)
            {
                case TestFlyQueryCreation.ComunicationTypeEnum.SOAP:
                    return GetElement(getSoapData(query, SdmxVersion, tipo));
                case TestFlyQueryCreation.ComunicationTypeEnum.REST:
                    return GetElement(getRestData(query, GetRestHeader(tipo, SdmxVersion)));
            }
            return null;
        }

        public string getQuery(StructTypeEnum tipo, string val, TestFlyQueryCreation.SdmxVersionEnum SdmxVersion, TestFlyQueryCreation.ComunicationTypeEnum ComunicationType, string reference, DetailEnum detail)
        {
            return getQuery(tipo, val, "ALL", "*", SdmxVersion, ComunicationType, reference, detail);
        }
        public string getQuery(StructTypeEnum tipo, string val, string agency, string version, TestFlyQueryCreation.SdmxVersionEnum SdmxVersion, TestFlyQueryCreation.ComunicationTypeEnum ComunicationType, string reference, DetailEnum detail)
        {
            switch (ComunicationType)
            {
                case TestFlyQueryCreation.ComunicationTypeEnum.SOAP:
                    return getSOAPDataQuery(tipo, val, agency, version, SdmxVersion, reference, detail);
                case TestFlyQueryCreation.ComunicationTypeEnum.REST:
                    return getRESTDataQuery(tipo, val, agency, version, SdmxVersion, reference, detail);
            }
            ErrorString = "Not supported type";
            return null;
        }

        public string getSoapData(string query, TestFlyQueryCreation.SdmxVersionEnum SdmxVersion, StructTypeEnum tipo)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(query);
            if (SdmxVersion == TestFlyQueryCreation.SdmxVersionEnum.Sdmx20)
            {
                if (tipo == StructTypeEnum.Compact)
                    return ProcessQuery(doc, MessageTypeEnum.CompactData);
                else if (tipo == StructTypeEnum.Generic)
                    return ProcessQuery(doc, MessageTypeEnum.GenericData);
                else
                    return ProcessQuery(doc, MessageTypeEnum.QueryStructure);
            }
            else
            {
                switch (tipo)
                {
                    case StructTypeEnum.Agency:
                        return ProcessQuery(doc, MessageTypeEnum.GetOrganisationScheme);
                    case StructTypeEnum.DSD:
                        return ProcessQuery(doc, MessageTypeEnum.GetDataStructure);
                    case StructTypeEnum.Categorisation:
                        return ProcessQuery(doc, MessageTypeEnum.GetCategorisation);
                    case StructTypeEnum.Category:
                        return ProcessQuery(doc, MessageTypeEnum.GetCategoryScheme);
                    case StructTypeEnum.Dataflow:
                        return ProcessQuery(doc, MessageTypeEnum.GetDataflow);
                    case StructTypeEnum.Concepts:
                        return ProcessQuery(doc, MessageTypeEnum.GetConceptScheme);
                    case StructTypeEnum.Codelist:
                    case StructTypeEnum.ConstrainedCodelist:
                        return ProcessQuery(doc, MessageTypeEnum.GetCodelist);
                    case StructTypeEnum.GenericData:
                        return ProcessQuery(doc, MessageTypeEnum.GetGenericData);
                    case StructTypeEnum.StructureSpecificData:
                        return ProcessQuery(doc, MessageTypeEnum.GetStructureSpecificData);
                    default:
                        ErrorString = "Not supported type";
                        return null;
                }
            }

        }
        public string getRestData(string query, string Header)
        {
            return ProcessRestQuery(query, Header);
        }

        public string getSOAPDataQuery(StructTypeEnum tipo, string val, string agency, string version, TestFlyQueryCreation.SdmxVersionEnum SdmxVersion, string reference, DetailEnum detail)
        {
            ErrorString = null;
            DirectoryInfo dirStruct = new DirectoryInfo(Application.StartupPath.Replace(@"TestOnTheFlyService\bin\x86\Debug", "").Replace(@"TestOnTheFlyService\bin\x86\Release", "") + @"\Utils\");
            FileInfo fi = null;
            switch (tipo)
            {
                case StructTypeEnum.Category:
                    if (SdmxVersion == TestFlyQueryCreation.SdmxVersionEnum.Sdmx20)
                        fi = new FileInfo(dirStruct.FullName + @"QueriesStructure\CategorySchemaStructure.xml");
                    else
                        fi = new FileInfo(dirStruct.FullName + @"QueriesStructure 21\CategorySchemaStructure.xml");
                    break;
                case StructTypeEnum.Categorisation:
                    fi = new FileInfo(dirStruct.FullName + @"QueriesStructure 21\CategorisationStructure.xml");
                    break;
                case StructTypeEnum.Dataflow:
                    if (SdmxVersion == TestFlyQueryCreation.SdmxVersionEnum.Sdmx20)
                        fi = new FileInfo(dirStruct.FullName + @"QueriesStructure\DataFlowsStructure.xml");
                    else
                        fi = new FileInfo(dirStruct.FullName + @"QueriesStructure 21\DataFlowsStructure.xml");
                    break;
                case StructTypeEnum.Concepts:
                    if (SdmxVersion == TestFlyQueryCreation.SdmxVersionEnum.Sdmx20)
                        fi = new FileInfo(dirStruct.FullName + @"QueriesStructure\ConceptSchemeStructure.xml");
                    else
                        fi = new FileInfo(dirStruct.FullName + @"QueriesStructure 21\ConceptSchemeStructure.xml");
                    break;
                case StructTypeEnum.Codelist:
                    if (SdmxVersion == TestFlyQueryCreation.SdmxVersionEnum.Sdmx20)
                        fi = new FileInfo(dirStruct.FullName + @"QueriesStructure\CodelistStructure.xml");
                    else
                        fi = new FileInfo(dirStruct.FullName + @"QueriesStructure 21\CodelistStructure.xml");
                    break;
                case StructTypeEnum.ConstrainedCodelist:
                    if (SdmxVersion == TestFlyQueryCreation.SdmxVersionEnum.Sdmx20)
                        fi = new FileInfo(dirStruct.FullName + @"QueriesStructure\ConstrainedCodelistStructure.xml");
                    else
                        fi = new FileInfo(dirStruct.FullName + @"QueriesStructure 21\ConstrainedCodelistStructure.xml");
                    break;
                case StructTypeEnum.Agency:
                    if (SdmxVersion == TestFlyQueryCreation.SdmxVersionEnum.Sdmx20)
                        fi = new FileInfo(dirStruct.FullName + @"QueriesStructure\AgencySchemeStructure.xml");
                    else
                        fi = new FileInfo(dirStruct.FullName + @"QueriesStructure 21\AgencySchemeStructure.xml");
                    break;
                case StructTypeEnum.DSD:
                    if (SdmxVersion == TestFlyQueryCreation.SdmxVersionEnum.Sdmx20)
                        fi = new FileInfo(dirStruct.FullName + @"QueriesStructure\DSDStructure.xml");
                    else
                        fi = new FileInfo(dirStruct.FullName + @"QueriesStructure 21\DSDStructure.xml");
                    break;
                case StructTypeEnum.Generic:
                case StructTypeEnum.Compact:
                    fi = new FileInfo(dirStruct.FullName + @"QueryMessage\QueryStructure20.xml");
                    break;
                case StructTypeEnum.GenericData:
                case StructTypeEnum.StructureSpecificData:
                    fi = new FileInfo(dirStruct.FullName + @"QueryMessage\QueryStructure21.xml");
                    break;
            }
            if (fi == null)
            {
                ErrorString = "Tipo non supportato";
                return null;
            }
            string xmlQuery = "";
            using (StreamReader rea = new StreamReader(fi.FullName))
            {
                xmlQuery = rea.ReadToEnd();
            }

            //            xmlQuery = @"<QueryMessage xmlns=""http://www.SDMX.org/resources/SDMXML/schemas/v2_0/message"">
            //  <Header>
            //    <ID>UNKNOWN</ID>
            //    <Test>false</Test>
            //    <Prepared>2014-08-18T15:49:54.0943115+02:00</Prepared>
            //    <Sender id=""UNKNOWN"" />
            //    <Receiver id=""unknown"" />
            //  </Header>
            //  <Query>
            //    <DataWhere xmlns=""http://www.SDMX.org/resources/SDMXML/schemas/v2_0/query"">
            //      <And>
            //        <Dimension id=""FREQ"">M</Dimension>
            //        <Dataflow>158_149</Dataflow>
            //      </And>
            //    </DataWhere>
            //  </Query>
            //</QueryMessage>";


            switch (tipo)
            {
                case StructTypeEnum.Concepts:
                case StructTypeEnum.DSD:
                case StructTypeEnum.Codelist:
                    xmlQuery = xmlQuery.Replace("###RESOURCE###", val);
                    break;
                case StructTypeEnum.ConstrainedCodelist:
                    xmlQuery = ConstrainedCodelistVal.ReplaceSoapVals(val, xmlQuery);
                    break;
                case StructTypeEnum.Generic:
                case StructTypeEnum.Compact:
                    xmlQuery = xmlQuery.Replace("###QUERY###", val);
                    break;
                case StructTypeEnum.GenericData:
                case StructTypeEnum.StructureSpecificData:

                    xmlQuery = xmlQuery.Replace("###StructType###", tipo.ToString() + "Query");
                    xmlQuery = xmlQuery.Replace("###QUERY###", val);
                    break;
                default:
                    break;
            }

            xmlQuery = xmlQuery.Replace("###AGENCYID###", agency);
            xmlQuery = xmlQuery.Replace("###VERSION###", version);

            if (reference != "None")
            {
                if (SdmxVersion == TestFlyQueryCreation.SdmxVersionEnum.Sdmx20)
                    xmlQuery = xmlQuery.Replace("QueryStructureRequest resolveReferences=\"false\"", "QueryStructureRequest resolveReferences=\"true\"");
                else
                {
                    ReferencesEnum re;
                    if (Enum.TryParse<ReferencesEnum>(reference, true, out re))
                    {
                        xmlQuery = xmlQuery.Replace("<query:References><query:None/></query:References>", "<query:References><query:" + re.ToString() + "/></query:References>");
                    }
                    else
                    {
                        string[] references = reference.Split(',');
                        StringBuilder Common = new StringBuilder();
                        foreach (var item in references)
                            Common.AppendLine("<common:" + item.Trim() + "/>");
                        xmlQuery = xmlQuery.Replace("<query:References><query:None/></query:References>",
                            "<query:References><query:SpecificObjects>" + Common.ToString() + "</query:SpecificObjects></query:References>");
                    }
                }

            }
            if (detail != DetailEnum.Full)
            {
                string Dettaglio = "Full";
                switch (detail)
                {
                    case DetailEnum.Full:
                        break;
                    case DetailEnum.AllStubs:
                        Dettaglio = "Stub";
                        break;
                    case DetailEnum.ReferencedStubs:
                        Dettaglio = "CompleteStub";
                        break;
                    default:
                        break;
                }
                xmlQuery = xmlQuery.Replace("<query:ReturnDetails detail=\"Full\">", "<query:ReturnDetails detail=\"" + Dettaglio + "\">");
            }

            return xmlQuery;
        }
        public string getRESTDataQuery(StructTypeEnum tipo, string val, string agency, string version, TestFlyQueryCreation.SdmxVersionEnum SdmxVersion, string reference, DetailEnum detail)
        {
            ErrorString = null;

            string Operation = "";
            string Extra = "";
            bool isDatarequest = false;
            reference = reference.ToLower();
            //datastructure|metadatastructure|categoryscheme|conceptscheme|codelist|hierarchicalcodelist|organisationscheme|agencyscheme|dataproviderscheme|dataconsumerscheme|organisationunitscheme|dataflow|metadataflow|reportingtaxonomy|provisionagreement|structureset|process|categorisation|contentconstraint|attachmentconstraint|structure)$",
            switch (tipo)
            {
                case StructTypeEnum.Categorisation:
                    Operation = "categorisation";
                    break;
                case StructTypeEnum.Category:
                    Operation = "categoryscheme";
                    break;
                case StructTypeEnum.Dataflow:
                    Operation = "dataflow";
                    break;
                case StructTypeEnum.Concepts:
                    Operation = "conceptscheme";
                    break;
                case StructTypeEnum.Codelist:
                    Operation = "codelist";
                    Extra = val;
                    break;
                case StructTypeEnum.ConstrainedCodelist:
                    Operation = "codelist";
                    reference = ConstrainedCodelistVal.RestExtraRef(val, out Extra, out agency, out version);
                    break;
                case StructTypeEnum.Agency:
                    Operation = "agencyscheme";
                    break;
                case StructTypeEnum.DSD:
                    Operation = "datastructure";
                    Extra = val;
                    break;
                case StructTypeEnum.Generic:
                case StructTypeEnum.GenericData:
                case StructTypeEnum.Compact:
                case StructTypeEnum.StructureSpecificData:
                    Extra = val;
                    isDatarequest = true;
                    break;
            }
            string query = "";
            //Metadata compose Query
            if (!isDatarequest)
                if (string.IsNullOrEmpty(version) || version == "*")
                    query = string.Format("{0}/{4}/{1}?references={2}&detail={3}", Operation, (string.IsNullOrEmpty(Extra) ? "ALL" : Extra), reference, detail.ToString().ToLower(), agency);
                else
                    query = string.Format("{0}/{4}/{1}/{5}?references={2}&detail={3}", Operation, (string.IsNullOrEmpty(Extra) ? "ALL" : Extra), reference, detail.ToString().ToLower(), agency, version);
            else
                query = Operation + val;

            return query;

        }


        public string GetRestHeader(StructTypeEnum tipo, TestFlyQueryCreation.SdmxVersionEnum SdmxVersion)
        {
            string Header = "application/vnd.sdmx.structure+xml";
            switch (tipo)
            {

                case StructTypeEnum.Generic:
                case StructTypeEnum.GenericData:
                    Header = "application/vnd.sdmx.genericdata+xml";
                    break;
                case StructTypeEnum.Compact:
                    Header = "application/vnd.sdmx.compactdata+xml";
                    break;
                case StructTypeEnum.StructureSpecificData:
                    Header = "application/vnd.sdmx.structurespecificdata+xml";
                    break;
            }
            switch (SdmxVersion)
            {
                case TestFlyQueryCreation.SdmxVersionEnum.Sdmx20:
                    Header += ";version=2.0";
                    break;
                case TestFlyQueryCreation.SdmxVersionEnum.Sdmx21:
                    Header += ";version=2.1";
                    break;
            }
            return Header;
        }

        #endregion

        public enum MessageTypeEnum
        {
            //Tipologie 2.0
            QueryStructure,
            CompactData,
            CrossSectionalData,
            GenericData,

            //Tipologie 2.1
            GetDataflow,
            GetDataStructure,
            GetCategoryScheme,
            GetCategorisation,
            GetConceptScheme,
            GetCodelist,
            GetHierarchicalCodelist,
            GetOrganisationScheme,

            GetGenericData,
            GetGenericTimeSeriesData,
            GetStructureSpecificData,
            GetStructureSpecificTimeSeriesData,
        }


        public string ProcessQuery(XmlDocument query, MessageTypeEnum tipo)
        {
            WsConfigurationSettings _settings = GetSOAPSettings(tipo);
            CallWS cw = new CallWS(string.Format("{0}\\OnTheFlyResponse.xml", TestFlyQueryCreation.ExportPath.FullName), CallWebServices.MAX_OUTPUT_LENGTH);
            string resp = cw.SendSOAPQuery(query, _settings);
            if (!string.IsNullOrEmpty(cw.LastError))
                this.ErrorString = cw.LastError;
            return resp;
        }

        private string ProcessRestQuery(string query, string Headers)
        {
            WsConfigurationSettings _settings = GetRestSettings(query);
            CallWS cw = new CallWS(string.Format("{0}\\OnTheFlyResponse.xml", TestFlyQueryCreation.ExportPath.FullName), CallWebServices.MAX_OUTPUT_LENGTH);
            string resp = cw.SendRESTQuery(query, Headers, _settings);
            if (!string.IsNullOrEmpty(cw.LastError))
                this.ErrorString = cw.LastError;
            return resp;

        }

        #region Configuration
        private WsConfigurationSettings GetSOAPSettings(MessageTypeEnum operation)
        {
            string EndPointAction = operation.ToString();
            #region SdmxVersion
            bool isVersion20 = true;
            switch (operation)
            {
                case MessageTypeEnum.QueryStructure:
                    isVersion20 = true;
                    break;
                case MessageTypeEnum.CompactData:
                case MessageTypeEnum.CrossSectionalData:
                case MessageTypeEnum.GenericData:
                    EndPointAction = "Get" + EndPointAction;
                    isVersion20 = true;
                    break;
                case MessageTypeEnum.GetDataflow:
                case MessageTypeEnum.GetDataStructure:
                case MessageTypeEnum.GetCategorisation:
                case MessageTypeEnum.GetCategoryScheme:
                case MessageTypeEnum.GetConceptScheme:
                case MessageTypeEnum.GetCodelist:
                case MessageTypeEnum.GetHierarchicalCodelist:
                case MessageTypeEnum.GetOrganisationScheme:
                case MessageTypeEnum.GetGenericData:
                case MessageTypeEnum.GetGenericTimeSeriesData:
                case MessageTypeEnum.GetStructureSpecificData:
                case MessageTypeEnum.GetStructureSpecificTimeSeriesData:
                    isVersion20 = false;
                    break;
                default:
                    break;
            }
            #endregion
            string endpoint = "";

            if (isVersion20)
                endpoint = ConfigurationManager.AppSettings["EndPoint20"];
            else
                endpoint = ConfigurationManager.AppSettings["EndPoint21"];
            string mainUri = ConfigurationManager.AppSettings["MainUri"];
            return CallWebServices.GetSettings(mainUri + endpoint, EndPointAction);

        }
        private WsConfigurationSettings GetRestSettings(string query)
        {

            string endpoint = ConfigurationManager.AppSettings["MainUri"] + "rest/";

            return CallWebServices.GetSettings(endpoint, null);

        }





        #endregion

        internal XElement GetElement(string element)
        {
            if (element == null)
                return null;
            return XElement.Parse(element);
        }
    }


}
