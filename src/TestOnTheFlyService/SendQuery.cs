using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace TestOnTheFlyService
{
    public class SendQuery
    {
        private ServiceReferenceSdmx20.SOAP20Client ws;
        private ServiceReferenceSdmx21.WS_SDMX_SOAP21Client ws21;
        public string ErrorString { get; set; }
        public XElement LastError { get; set; }

        public SendQuery()
        {
            ws = new ServiceReferenceSdmx20.SOAP20Client();
            ws.Endpoint.Binding = new BasicHttpBinding { TransferMode = TransferMode.StreamedResponse, MaxBufferSize = 65536, MaxReceivedMessageSize = 67108864 };
            ws.Endpoint.Binding.CloseTimeout = new TimeSpan(1, 0, 0);
            ws.Endpoint.Binding.OpenTimeout = new TimeSpan(1, 0, 0);
            ws.Endpoint.Binding.SendTimeout = new TimeSpan(1, 0, 0);
            ws.Endpoint.Binding.ReceiveTimeout = new TimeSpan(1, 0, 0);
            ws21 = new ServiceReferenceSdmx21.WS_SDMX_SOAP21Client();
            //ws21.Endpoint.Binding = new BasicHttpBinding { CloseTimeout = new TimeSpan(1, 0, 0), OpenTimeout = new TimeSpan(1, 0, 0), SendTimeout = new TimeSpan(1, 0, 0), MaxReceivedMessageSize = 2147483647 };
        }

        #region InviaQuery
        public enum StructTypeEnum
        {
            Agency,
            DSD,
            Category,
            Dataflow,
            Concepts,
            Codelist,
            Generic,
            Compact,

            GenericData,
            GenericTimeSeriesData,
            StructureSpecificData,
            StructureSpecificTimeSeriesData,
        }
        public XElement getData(StructTypeEnum tipo, string val, TestFlyQueryCreation.SdmxVersionEnum SdmxVersion)
        {
            LastError = null;
            DirectoryInfo dirStruct = new DirectoryInfo(Application.StartupPath.Replace(@"TestOnTheFlyService\bin\x86\Debug", "") + @"\Utils\");
            FileInfo fi = null;
            switch (tipo)
            {
                case StructTypeEnum.Category:
                    if (SdmxVersion == TestFlyQueryCreation.SdmxVersionEnum.Sdmx20)
                        fi = new FileInfo(dirStruct.FullName + @"QueriesStructure\CategorySchemaStructure.xml");
                    else
                        fi = new FileInfo(dirStruct.FullName + @"QueriesStructure 21\CategorySchemaStructure.xml");
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
                case StructTypeEnum.GenericTimeSeriesData:
                case StructTypeEnum.StructureSpecificData:
                case StructTypeEnum.StructureSpecificTimeSeriesData:
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
            xmlQuery = xmlQuery.Replace("###AGENCYID###", "ISTAT");
            xmlQuery = xmlQuery.Replace("###VERSION###", "1.0");
            switch (tipo)
            {
                case StructTypeEnum.Concepts:
                case StructTypeEnum.DSD:
                    xmlQuery = xmlQuery.Replace("###DATAFLOW###", val);
                    break;
                case StructTypeEnum.Codelist:
                    xmlQuery = xmlQuery.Replace("###CODELIST###", val);
                    break;
                case StructTypeEnum.Generic:
                case StructTypeEnum.Compact:
                    xmlQuery = xmlQuery.Replace("###QUERY###", val);
                    break;
                case StructTypeEnum.GenericData:
                case StructTypeEnum.GenericTimeSeriesData:
                case StructTypeEnum.StructureSpecificData:
                case StructTypeEnum.StructureSpecificTimeSeriesData:

                    xmlQuery = xmlQuery.Replace("###StructType###", tipo.ToString() + "Query");
                    xmlQuery = xmlQuery.Replace("###QUERY###", val);
                    break;
                default:
                    break;
            }


            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlQuery);
            if (SdmxVersion == TestFlyQueryCreation.SdmxVersionEnum.Sdmx20)
            {
                if (tipo == StructTypeEnum.Compact)
                    return ProcessQuery(doc, fi.Name, TestOnTheFlyService.Test.MessageTypeEnum.CompactData);
                else if (tipo == StructTypeEnum.Generic)
                    return ProcessQuery(doc, fi.Name, TestOnTheFlyService.Test.MessageTypeEnum.GenericData);
                else
                    return ProcessQuery(doc, fi.Name, TestOnTheFlyService.Test.MessageTypeEnum.QueryStructure);
            }
            else
            {
                switch (tipo)
                {
                    case StructTypeEnum.Agency:
                        return ProcessQuery(doc, fi.Name, TestOnTheFlyService.Test.MessageTypeEnum.GetOrganisationScheme);
                    case StructTypeEnum.DSD:
                        return ProcessQuery(doc, fi.Name, TestOnTheFlyService.Test.MessageTypeEnum.GetStructures);
                    case StructTypeEnum.Category:
                        return ProcessQuery(doc, fi.Name, TestOnTheFlyService.Test.MessageTypeEnum.GetCategoryScheme);
                    case StructTypeEnum.Dataflow:
                        return ProcessQuery(doc, fi.Name, TestOnTheFlyService.Test.MessageTypeEnum.GetDataflow);
                    case StructTypeEnum.Concepts:
                        return ProcessQuery(doc, fi.Name, TestOnTheFlyService.Test.MessageTypeEnum.GetConceptScheme);
                    case StructTypeEnum.Codelist:
                        return ProcessQuery(doc, fi.Name, TestOnTheFlyService.Test.MessageTypeEnum.GetCodelist);

                    case StructTypeEnum.GenericData:
                        return ProcessQuery(doc, fi.Name, TestOnTheFlyService.Test.MessageTypeEnum.GetGenericData);
                    case StructTypeEnum.GenericTimeSeriesData:
                        return ProcessQuery(doc, fi.Name, TestOnTheFlyService.Test.MessageTypeEnum.GetGenericTimeSeriesData);
                    case StructTypeEnum.StructureSpecificData:
                        return ProcessQuery(doc, fi.Name, TestOnTheFlyService.Test.MessageTypeEnum.GetStructureSpecificData);
                    case StructTypeEnum.StructureSpecificTimeSeriesData:
                        return ProcessQuery(doc, fi.Name, TestOnTheFlyService.Test.MessageTypeEnum.GetStructureSpecificTimeSeriesData);
                    default:
                        ErrorString = "Not supported type";
                        return null;
                }
            }
        }

        public XElement ProcessQuery(XmlDocument doc, string FunctionName, Test.MessageTypeEnum tipo)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ErrorString = "Caricamento in corso...";

            System.ServiceModel.Channels.Message Risposta = null;
            try
            {
                string WSNameSpace20 = string.Format("{0}", ws.Endpoint.Contract.Namespace, ws.Endpoint.Contract.Name);// "WS_SOAP_SDMX/IWS_SDMX_SOAP20";
                string WSNameSpace21 = string.Format("{0}", ws21.Endpoint.Contract.Namespace, ws21.Endpoint.Contract.Name);// "WS_SOAP_SDMX/IWS_SDMX_SOAP20";
                switch (tipo)
                {
                    case Test.MessageTypeEnum.QueryStructure:
                        System.ServiceModel.Channels.Message mess = System.ServiceModel.Channels.Message.CreateMessage(MessageVersion.Soap11, WSNameSpace20 + "/QueryStructure", new XmlNodeReader(doc));
                         Risposta = ws.QueryStructure(mess);
                        break;
                    case Test.MessageTypeEnum.CompactData:
                        Risposta = ws.GetCompactData(System.ServiceModel.Channels.Message.CreateMessage(MessageVersion.Soap11, WSNameSpace20 + "/GetCompactData", new XmlNodeReader(doc)));
                        break;
                    case Test.MessageTypeEnum.CrossSectionalData:
                        //Risposta = ws.GetCrossSectionalData(System.ServiceModel.Channels.Message.CreateMessage(MessageVersion.Soap11, WSNameSpace20 + "/GetCrossSectionalData", new XmlNodeReader(doc)));
                        break;
                    case Test.MessageTypeEnum.GenericData:
                        Risposta = ws.GetGenericData(System.ServiceModel.Channels.Message.CreateMessage(MessageVersion.Soap11, WSNameSpace20 + "/GetGenericData", new XmlNodeReader(doc)));
                        break;

                    case Test.MessageTypeEnum.GetDataflow:
                        Risposta = ws21.GetDataflow(System.ServiceModel.Channels.Message.CreateMessage(MessageVersion.Soap11, WSNameSpace21 + "/GetDataflow", new XmlNodeReader(doc)));
                        break;
                    case Test.MessageTypeEnum.GetStructures:
                        Risposta = ws21.GetStructures(System.ServiceModel.Channels.Message.CreateMessage(MessageVersion.Soap11, WSNameSpace21 + "/GetStructures", new XmlNodeReader(doc)));
                        break;
                    case Test.MessageTypeEnum.GetCategoryScheme:
                        Risposta = ws21.GetCategoryScheme(System.ServiceModel.Channels.Message.CreateMessage(MessageVersion.Soap11, WSNameSpace21 + "/GetCategoryScheme", new XmlNodeReader(doc)));
                        break;
                    case Test.MessageTypeEnum.GetConceptScheme:
                        Risposta = ws21.GetConceptScheme(System.ServiceModel.Channels.Message.CreateMessage(MessageVersion.Soap11, WSNameSpace21 + "/GetConceptScheme", new XmlNodeReader(doc)));
                        break;
                    case Test.MessageTypeEnum.GetCodelist:
                        Risposta = ws21.GetCodelist(System.ServiceModel.Channels.Message.CreateMessage(MessageVersion.Soap11, WSNameSpace21 + "/GetCodelist", new XmlNodeReader(doc)));
                        break;
                    case Test.MessageTypeEnum.GetHierarchicalCodelist:
                        //Risposta = ws21.GetHierarchicalCodelist(System.ServiceModel.Channels.Message.CreateMessage(MessageVersion.Soap11, WSNameSpace21 + "/GetHierarchicalCodelist", new XmlNodeReader(doc)));
                        break;
                    case Test.MessageTypeEnum.GetOrganisationScheme:
                        Risposta = ws21.GetOrganisationScheme(System.ServiceModel.Channels.Message.CreateMessage(MessageVersion.Soap11, WSNameSpace21 + "/GetOrganisationScheme", new XmlNodeReader(doc)));
                        break;
                    case Test.MessageTypeEnum.GetGenericData:
                        //Risposta = ws21.GetGenericData(System.ServiceModel.Channels.Message.CreateMessage(MessageVersion.Soap11, WSNameSpace21 + "/GetGenericData", new XmlNodeReader(doc)));
                        break;
                    case Test.MessageTypeEnum.GetGenericTimeSeriesData:
                        Risposta = ws21.GetGenericTimeSeriesData(System.ServiceModel.Channels.Message.CreateMessage(MessageVersion.Soap11, WSNameSpace21 + "/GetGenericTimeSeriesData", new XmlNodeReader(doc)));
                        break;
                    case Test.MessageTypeEnum.GetStructureSpecificData:
                        //Risposta = ws21.GetStructureSpecificData(System.ServiceModel.Channels.Message.CreateMessage(MessageVersion.Soap11, WSNameSpace21 + "/GetStructureSpecificData", new XmlNodeReader(doc)));
                        break;
                    case Test.MessageTypeEnum.GetStructureSpecificTimeSeriesData:
                        Risposta = ws21.GetStructureSpecificTimeSeriesData(System.ServiceModel.Channels.Message.CreateMessage(MessageVersion.Soap11, WSNameSpace21 + "/GetStructureSpecificTimeSeriesData", new XmlNodeReader(doc)));
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                ErrorString = ("Errore Parsing della query: " + ex.Message);
                return null;
            }

            try
            {
                if (Risposta == null)
                {
                    ErrorString = "Nessuna risposta!!";
                    return null;
                }
                if (Risposta.IsFault)
                {
                    LastError = Risposta.GetBody<XElement>();
                    ErrorString = null;
                    //MessageBuffer msgbuf = Risposta.CreateBufferedCopy(int.MaxValue);
                    //System.ServiceModel.Channels.Message msg = msgbuf.CreateMessage();
                    //System.Xml.XPath.XPathNavigator nav = msgbuf.CreateNavigator();
                    //MemoryStream ms = new MemoryStream();
                    //using (XmlWriter xw = XmlWriter.Create(ms))
                    //{
                    //    nav.WriteSubtree(xw);
                    //}
                    //ms.Position = 0;
                    //LastError = XElement.Load(XmlReader.Create(ms));
                    return null;
                }
                sw.Stop();

                Console.WriteLine(string.Format("Arrivato in {0} ms", sw.ElapsedMilliseconds));
                return Risposta.GetBody<XElement>();
            }
            catch (Exception ex)
            {
                ErrorString = ("Errore Parsing Risposta della query: " + ex.Message);
            }

            return null;
        }
        #endregion
    }
}
