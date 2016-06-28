using FlyCallWS.Streaming;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;


namespace TestOnTheFlyService
{
    public partial class TestQuery : Form
    {
        public enum TipoQueryEndPoint
        {
            Soap20,
            Soap21,
            Rest20,
            Rest21,
        }
        public enum tipooperazione
        {
            Rdn_AgencySchema,
            Rdn_CategorySchema,
            Rdn_Dataflows,
            Rdn_ConceptsSchema,
            Rdn_Codelist,
            Rdn_ConstrainedCodelist,
            Rdn_DataStructure,
            rdn_GenericData,
            rdn_CompactData,

            Rdn_AgencySchema21,
            Rdn_Categorisation21,
            Rdn_CategorySchema21,
            Rdn_Dataflows21,
            Rdn_ConceptsSchema21,
            Rdn_Codelist21,
            Rdn_DataStructure21,
            rdn_GenericData21,
            rdn_StructureSpecificData21,

            Rdn_DataStructure_REST20,
            rdn_GenericData_REST20,
            rdn_CompactData_REST20,

            Rdn_DataStructure_REST21,
            rdn_GenericData_REST21,
            rdn_StructureSpecific_REST21,
        }


        public TipoQueryEndPoint LastTipoQuery { get; set; }
        public tipooperazione LastTipoOperazione { get; set; }
        public TestQuery()
        {
            InitializeComponent();
            cmbEndPoint.Items.Add(new cmbval() { Id = "Soap Sdmx 2.0", tipo = TipoQueryEndPoint.Soap20 });
            cmbEndPoint.Items.Add(new cmbval() { Id = "Soap Sdmx 2.1", tipo = TipoQueryEndPoint.Soap21 });
            cmbEndPoint.Items.Add(new cmbval() { Id = "Rest Sdmx 2.0", tipo = TipoQueryEndPoint.Rest20 });
            cmbEndPoint.Items.Add(new cmbval() { Id = "Rest Sdmx 2.1", tipo = TipoQueryEndPoint.Rest21 });

            Rdn_AgencySchema.Tag = tipooperazione.Rdn_AgencySchema;
            Rdn_CategorySchema.Tag = tipooperazione.Rdn_CategorySchema;
            Rdn_Dataflows.Tag = tipooperazione.Rdn_Dataflows;
            Rdn_ConceptsSchema.Tag = tipooperazione.Rdn_ConceptsSchema;
            Rdn_Codelist.Tag = tipooperazione.Rdn_Codelist;
            Rdn_ConstrainedCodelist.Tag = tipooperazione.Rdn_ConstrainedCodelist;
            Rdn_DataStructure.Tag = tipooperazione.Rdn_DataStructure;
            rdn_GenericData.Tag = tipooperazione.rdn_GenericData;
            rdn_CompactData.Tag = tipooperazione.rdn_CompactData;

            Rdn_AgencySchema21.Tag = tipooperazione.Rdn_AgencySchema21;
            Rdn_Categorisation21.Tag = tipooperazione.Rdn_Categorisation21;
            Rdn_CategorySchema21.Tag = tipooperazione.Rdn_CategorySchema21;
            Rdn_Dataflows21.Tag = tipooperazione.Rdn_Dataflows21;
            Rdn_ConceptsSchema21.Tag = tipooperazione.Rdn_ConceptsSchema21;
            Rdn_Codelist21.Tag = tipooperazione.Rdn_Codelist21;
            Rdn_DataStructure21.Tag = tipooperazione.Rdn_DataStructure21;
            rdn_GenericData21.Tag = tipooperazione.rdn_GenericData21;
            rdn_StructureSpecificData21.Tag = tipooperazione.rdn_StructureSpecificData21;

            Rdn_DataStructure_REST20.Tag = tipooperazione.Rdn_DataStructure_REST20;
            rdn_GenericData_REST20.Tag = tipooperazione.rdn_GenericData_REST20;
            rdn_CompactData_REST20.Tag = tipooperazione.rdn_CompactData_REST20;

            Rdn_DataStructure_REST21.Tag = tipooperazione.Rdn_DataStructure_REST21;
            rdn_GenericData_REST21.Tag = tipooperazione.rdn_GenericData_REST21;
            rdn_StructureSpecific_REST21.Tag = tipooperazione.rdn_StructureSpecific_REST21;

            cmbEndPoint.SelectedIndex = 0;

        }


        private void cmbEndPoint_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtQuery.Testo = "";
            txtRes.Testo = "";
            txtHeader.Text = "";
            txtHeader.Visible = false;

            grSOAP20.Visible = false;
            grSOAP21.Visible = false;
            grREST20.Visible = false;
            grREST21.Visible = false;

            Rdn_AgencySchema.Checked = false;
            Rdn_CategorySchema.Checked = false;
            Rdn_Dataflows.Checked = false;
            Rdn_ConceptsSchema.Checked = false;
            Rdn_Codelist.Checked = false;
            Rdn_DataStructure.Checked = false;
            rdn_GenericData.Checked = false;
            rdn_CompactData.Checked = false;

            Rdn_AgencySchema21.Checked = false;
            Rdn_CategorySchema21.Checked = false;
            Rdn_Dataflows21.Checked = false;
            Rdn_ConceptsSchema21.Checked = false;
            Rdn_Codelist21.Checked = false;
            Rdn_DataStructure21.Checked = false;
            rdn_GenericData21.Checked = false;
            rdn_StructureSpecificData21.Checked = false;

            Rdn_DataStructure_REST20.Checked = false;
            rdn_GenericData_REST20.Checked = false;
            rdn_CompactData_REST20.Checked = false;

            Rdn_DataStructure_REST21.Checked = false;
            rdn_GenericData_REST21.Checked = false;
            rdn_StructureSpecific_REST21.Checked = false;
            LastTipoQuery = ((cmbval)cmbEndPoint.SelectedItem).tipo;
            switch (LastTipoQuery)
            {
                case TipoQueryEndPoint.Soap20:
                    grSOAP20.Visible = true;
                    Rdn_AgencySchema.Checked = true;
                    break;
                case TipoQueryEndPoint.Soap21:
                    grSOAP21.Visible = true;
                    Rdn_AgencySchema21.Checked = true;
                    break;
                case TipoQueryEndPoint.Rest20:
                    grREST20.Visible = true;
                    Rdn_DataStructure_REST20.Checked = true;
                    txtHeader.Visible = true;
                    break;
                case TipoQueryEndPoint.Rest21:
                    grREST21.Visible = true;
                    Rdn_DataStructure_REST21.Checked = true;
                    txtHeader.Visible = true;
                    break;
            }
        }

        private void Rdn_CheckedChanged(object sender, EventArgs e)
        {
            txtQuery.Testo = "";
            txtRes.Testo = "";
            if (!(sender is RadioButton) || ((RadioButton)sender).Tag == null || !((RadioButton)sender).Checked)
                return;

            LastTipoOperazione = (tipooperazione)((RadioButton)sender).Tag;

            DirectoryInfo dirStruct = new DirectoryInfo(Application.StartupPath.Replace(@"TestOnTheFlyService\bin\x86\Debug", "") + @"\Utils\");

            FileInfo fi = null;
            switch (LastTipoOperazione)
            {
                case tipooperazione.Rdn_AgencySchema:
                    fi = new FileInfo(dirStruct.FullName + @"QueriesStructure\AgencySchemeStructure.xml");
                    break;
                case tipooperazione.Rdn_CategorySchema:
                    fi = new FileInfo(dirStruct.FullName + @"QueriesStructure\CategorySchemaStructure.xml");
                    break;
                case tipooperazione.Rdn_Dataflows:
                    fi = new FileInfo(dirStruct.FullName + @"QueriesStructure\DataFlowsStructure.xml");
                    break;
                case tipooperazione.Rdn_ConceptsSchema:
                    fi = new FileInfo(dirStruct.FullName + @"QueriesStructure\ConceptSchemeStructure.xml");
                    break;
                case tipooperazione.Rdn_Codelist:
                    fi = new FileInfo(dirStruct.FullName + @"QueriesStructure\CodelistStructure.xml");
                    break;
                case tipooperazione.Rdn_ConstrainedCodelist:
                    fi = new FileInfo(dirStruct.FullName + @"QueriesStructure\ConstrainedCodelistStructure.xml");
                    break;
                case tipooperazione.Rdn_DataStructure:
                    fi = new FileInfo(dirStruct.FullName + @"QueriesStructure\DSDStructure.xml");
                    break;
                case tipooperazione.rdn_GenericData:
                    fi = new FileInfo(dirStruct.FullName + @"QueryMessage\QueryStructure20.xml");
                    break;
                case tipooperazione.rdn_CompactData:
                    fi = new FileInfo(dirStruct.FullName + @"QueryMessage\QueryStructure20.xml");
                    break;
                case tipooperazione.Rdn_AgencySchema21:
                    fi = new FileInfo(dirStruct.FullName + @"QueriesStructure 21\AgencySchemeStructure.xml");
                    break;
                case tipooperazione.Rdn_Categorisation21:
                    fi = new FileInfo(dirStruct.FullName + @"QueriesStructure 21\CategorisationStructure.xml");
                    break;
                case tipooperazione.Rdn_CategorySchema21:
                    fi = new FileInfo(dirStruct.FullName + @"QueriesStructure 21\CategorySchemaStructure.xml");
                    break;
                case tipooperazione.Rdn_Dataflows21:
                    fi = new FileInfo(dirStruct.FullName + @"QueriesStructure 21\DataFlowsStructure.xml");
                    break;
                case tipooperazione.Rdn_ConceptsSchema21:
                    fi = new FileInfo(dirStruct.FullName + @"QueriesStructure 21\ConceptSchemeStructure.xml");
                    break;
                case tipooperazione.Rdn_Codelist21:
                    fi = new FileInfo(dirStruct.FullName + @"QueriesStructure 21\CodelistStructure.xml");
                    break;
                case tipooperazione.Rdn_DataStructure21:
                    fi = new FileInfo(dirStruct.FullName + @"QueriesStructure 21\DSDStructure.xml");
                    break;
                case tipooperazione.rdn_GenericData21:
                    fi = new FileInfo(dirStruct.FullName + @"QueryMessage\QueryStructure21.xml");
                    break;
                case tipooperazione.rdn_StructureSpecificData21:
                    fi = new FileInfo(dirStruct.FullName + @"QueryMessage\QueryStructure21.xml");
                    break;
                case tipooperazione.Rdn_DataStructure_REST20:
                    txtHeader.Text = GetRestHeader(SendQueryStreaming.StructTypeEnum.DSD, TestFlyQueryCreation.SdmxVersionEnum.Sdmx20);
                    break;
                case tipooperazione.rdn_GenericData_REST20:
                    txtHeader.Text = GetRestHeader(SendQueryStreaming.StructTypeEnum.GenericData, TestFlyQueryCreation.SdmxVersionEnum.Sdmx20);
                    txtQuery.Testo = "data/";
                    break;
                case tipooperazione.rdn_CompactData_REST20:
                    txtHeader.Text = GetRestHeader(SendQueryStreaming.StructTypeEnum.Compact, TestFlyQueryCreation.SdmxVersionEnum.Sdmx20);
                    txtQuery.Testo = "data/";
                    break;
                case tipooperazione.Rdn_DataStructure_REST21:
                    txtHeader.Text = GetRestHeader(SendQueryStreaming.StructTypeEnum.DSD, TestFlyQueryCreation.SdmxVersionEnum.Sdmx21);
                    break;
                case tipooperazione.rdn_GenericData_REST21:
                    txtHeader.Text = GetRestHeader(SendQueryStreaming.StructTypeEnum.GenericData, TestFlyQueryCreation.SdmxVersionEnum.Sdmx21);
                    txtQuery.Testo = "data/";
                    break;
                case tipooperazione.rdn_StructureSpecific_REST21:
                    txtHeader.Text = GetRestHeader(SendQueryStreaming.StructTypeEnum.StructureSpecificData, TestFlyQueryCreation.SdmxVersionEnum.Sdmx21);
                    txtQuery.Testo = "data/";
                    break;
                default:
                    break;
            }
            if (fi != null)
            {
                string xmlQuery = "";
                using (StreamReader rea = new StreamReader(fi.FullName))
                {
                    xmlQuery = rea.ReadToEnd();
                }
               
                if (LastTipoOperazione==tipooperazione.rdn_GenericData21)
                    xmlQuery = xmlQuery.Replace("###StructType###", "GenericTimeSeriesDataQuery" );
                if (LastTipoOperazione==tipooperazione.rdn_StructureSpecificData21)
                    xmlQuery = xmlQuery.Replace("###StructType###", "StructureSpecificTimeSeriesDataQuery");


                txtQuery.Testo = xmlQuery;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtRes.Testo = SendRequest();
        }
        public string SendRequest()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                CallWS cw = new CallWS(string.Format("{0}\\OnTheFlyResponse.xml", TestFlyQueryCreation.ExportPath.FullName), CallWebServices.MAX_OUTPUT_LENGTH);
                //    public TipoQueryEndPoint LastTipoQuery { get; set; }
                //public tipooperazione LastTipoOperazione { get; set; }
                string resp = null;
                string operation = "";
                #region Operation & Header
                switch (LastTipoOperazione)
                {
                    case tipooperazione.Rdn_AgencySchema:
                    case tipooperazione.Rdn_CategorySchema:
                    case tipooperazione.Rdn_Dataflows:
                    case tipooperazione.Rdn_ConceptsSchema:
                    case tipooperazione.Rdn_Codelist:
                    case tipooperazione.Rdn_ConstrainedCodelist:
                    case tipooperazione.Rdn_DataStructure:
                        operation = "QueryStructure";
                        break;
                    case tipooperazione.rdn_GenericData:
                        operation = "GetGenericData";
                        break;
                    case tipooperazione.rdn_CompactData:
                        operation = "GetCompactData";
                        break;
                    case tipooperazione.Rdn_AgencySchema21:
                        operation = "GetOrganisationScheme";
                        break;
                    case tipooperazione.Rdn_Categorisation21:
                        operation = "GetCategorisation";
                        break;
                    case tipooperazione.Rdn_CategorySchema21:
                        operation = "GetCategoryScheme";
                        break;
                    case tipooperazione.Rdn_Dataflows21:
                        operation = "GetDataflow";
                        break;
                    case tipooperazione.Rdn_ConceptsSchema21:
                        operation = "GetConceptScheme";
                        break;
                    case tipooperazione.Rdn_Codelist21:
                        operation = "GetCodelist";
                        break;
                    case tipooperazione.Rdn_DataStructure21:
                        operation = "GetDataStructure";
                        break;
                    case tipooperazione.rdn_GenericData21:
                        operation = "GetGenericTimeSeriesData";
                        break;
                    case tipooperazione.rdn_StructureSpecificData21:
                        operation = "GetStructureSpecificTimeSeriesData";
                        break;
                    case tipooperazione.Rdn_DataStructure_REST20:
                        break;
                    case tipooperazione.rdn_GenericData_REST20:
                        break;
                    case tipooperazione.rdn_CompactData_REST20:
                        break;
                    case tipooperazione.Rdn_DataStructure_REST21:
                        break;
                    case tipooperazione.rdn_GenericData_REST21:
                        break;
                    case tipooperazione.rdn_StructureSpecific_REST21:
                        break;
                    default:
                        break;
                }
                #endregion

                string endpoint = ""; 
                string mainUri = ConfigurationManager.AppSettings["MainUri"];
                switch (LastTipoQuery)
                {
                    case TipoQueryEndPoint.Soap20:
                        endpoint = mainUri+ ConfigurationManager.AppSettings["EndPoint20"];
                        XmlDocument doc20 = new XmlDocument();
                        doc20.LoadXml(txtQuery.Testo);
                        resp = cw.SendSOAPQuery(doc20, CallWebServices.GetSettings(endpoint, operation));
                        break;
                    case TipoQueryEndPoint.Soap21:
                        endpoint = mainUri +ConfigurationManager.AppSettings["EndPoint21"];
                        XmlDocument doc21 = new XmlDocument();
                        doc21.LoadXml(txtQuery.Testo);
                        resp = cw.SendSOAPQuery(doc21, CallWebServices.GetSettings(endpoint, operation));
                        break;
                    case TipoQueryEndPoint.Rest20:
                    case TipoQueryEndPoint.Rest21:
                        endpoint = mainUri+ "rest/";
                        resp = cw.SendRESTQuery(txtQuery.Testo, txtHeader.Text, CallWebServices.GetSettings(endpoint, null));
                        break;
                }

                if (!string.IsNullOrEmpty(cw.LastError))
                    return cw.LastError;
                return resp;
            }
            catch (Exception ex)
            {
                return "Client Error: " + ex.Message;
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void btnOpenIE_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRes.Testo))
            {
                Process pp = new Process();
                string FileSavedPath = string.Format("{0}\\OnTheFlyResponse.xml", TestFlyQueryCreation.ExportPath.FullName);
                pp.StartInfo = new ProcessStartInfo(FileSavedPath);
                pp.Start();
            }
        }


        public string GetRestHeader(TestOnTheFlyService.SendQueryStreaming.StructTypeEnum tipo, TestFlyQueryCreation.SdmxVersionEnum SdmxVersion)
        {
            string Header = "application/vnd.sdmx.structure+xml";
            switch (tipo)
            {

                case SendQueryStreaming.StructTypeEnum.Generic:
                case SendQueryStreaming.StructTypeEnum.GenericData:
                    Header = "application/vnd.sdmx.genericdata+xml";
                    break;
                case SendQueryStreaming.StructTypeEnum.Compact:
                    Header = "application/vnd.sdmx.compactdata+xml";
                    break;
                case SendQueryStreaming.StructTypeEnum.StructureSpecificData:
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

       
    }
    public class cmbval
    {
        public string Id { get; set; }
        public TestQuery.TipoQueryEndPoint tipo { get; set; }

        public override string ToString()
        {
            return Id;
        }

    }
}
