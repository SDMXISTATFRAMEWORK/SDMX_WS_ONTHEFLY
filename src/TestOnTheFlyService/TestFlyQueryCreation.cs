using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace TestOnTheFlyService
{
    public partial class TestFlyQueryCreation : Form
    {
        #region Property & Init
        private SendQueryStreaming _SendQuery { get; set; }
        private SdmxObject _ChoosesDf { get; set; }
        private XElement _SelectedDSD { get; set; }
        private Dictionary<string, List<SdmxQueryInput>> _DataQuery { get; set; }
        private bool lockCheck = false;
        private List<Categorisation> Categorisations = new List<Categorisation>();
        public List<OrderDim> DimensionOrdered { get; set; }

        public static DirectoryInfo ExportPath = new DirectoryInfo(Path.GetTempPath() + "\\OnTheFly\\ExportedSdmxObject");
        public enum SdmxVersionEnum
        {
            Sdmx20,
            Sdmx21
        }
        public SdmxVersionEnum SdmxVersion { get; set; }
        public enum ComunicationTypeEnum
        {
            SOAP,
            REST
        }
        public ComunicationTypeEnum ComunicationType { get; set; }
        public IQueryCreator QueryCreator { get; set; }

        public TestFlyQueryCreation()
        {
            try
            {

                InitializeComponent();
                cmbLanguages.SelectedIndex = 0;

                SDMXtree.DrawMode = TreeViewDrawMode.OwnerDrawText;
                LstCodelist.DrawMode = TreeViewDrawMode.OwnerDrawText;
                SDMXtree.DrawNode += SDMXtree_DrawNode;
                LstCodelist.DrawNode += SDMXCodelist_DrawNode;
                _SendQuery = new SendQueryStreaming();

                UriUtils.FixSystemUriDotBug();

                //Inizializzazione immagini tree
                ImageList imglist = new ImageList();
                imglist.Images.AddRange(new Image[3]{
                global::TestOnTheFlyService.Properties.Resources.category,
                global::TestOnTheFlyService.Properties.Resources.dfd,
                global::TestOnTheFlyService.Properties.Resources.dfd2});
                SDMXtree.ImageList = imglist;


                ComunicationType = ComunicationTypeEnum.SOAP;
                SdmxVersion = SdmxVersionEnum.Sdmx20;
                updateVersion();

                cmbDetail.Items.Add("Full");
                cmbDetail.Items.Add("AllStubs");
                cmbDetail.Items.Add("ReferencedStubs");
                cmbDetail.SelectedIndex = 0;

                cmbReferences.Items.Add("None");
                cmbReferences.Items.Add("Parents");
                cmbReferences.Items.Add("ParentsAndSiblings");
                cmbReferences.Items.Add("Children");
                cmbReferences.Items.Add("Descendants");
                cmbReferences.Items.Add("All");
                cmbReferences.Items.Add("Specific");
                cmbReferences.SelectedIndex = 0;

                //lblAttribute.Visible = false;
                //LstAttributes.Visible = false;
                EndPoint21 = System.Configuration.ConfigurationManager.AppSettings["EndPoint21"];
            }
            catch (Exception ex)
            {
                ShowError(string.Format("Client Application Error: {0}", ex.Message));
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            //Prima chiamata per farlo andare piu veloce
            BtnGo.Enabled = false;
            Application.DoEvents();
            new Thread(new ThreadStart(FirstCall)).Start();
            base.OnLoad(e);
        }

        public void FirstCall()
        {
            try
            {
                if (EndPoint21 != null && EndPoint21.Trim().ToUpper() == "SOAPSDMX21")
                    _SendQuery.GetData(SendQueryStreaming.StructTypeEnum.Agency, null, SdmxVersionEnum.Sdmx20, ComunicationTypeEnum.SOAP, SendQueryStreaming.ReferencesEnum.None.ToString(), SendQueryStreaming.DetailEnum.AllStubs, null);
            }
            catch (Exception)
            {
                //Applicazione terminata
            }
            finally
            {
                this.Invoke((MethodInvoker)delegate
                {
                    BtnGo.Enabled = true;
                });
            }
        }
        #endregion

        #region Create Tree
        private void BtnGo_Click(object sender, EventArgs e)
        {
            try
            {

                this.Cursor = Cursors.WaitCursor;
                SDMXtree.Nodes.Clear();
                Clear();
                SDMXtree.Visible = false;

                #region Category and Dataflow
                XElement l = _SendQuery.GetData(SendQueryStreaming.StructTypeEnum.Category, null, this.SdmxVersion, this.ComunicationType, SendQueryStreaming.ReferencesEnum.ParentsAndSiblings.ToString(), SendQueryStreaming.DetailEnum.Full, MetadataView);
                if (l == null)
                {
                    ShowError(_SendQuery.ErrorString);
                    return;
                }

                XElement df = l.Descendants().FirstOrDefault(ca => ca.Name.LocalName == "Dataflows");
                if (df == null)
                {
                    SdmxObject CategoryNotFoundNode = new SdmxObject("Categories", null, false);
                    CategoryNotFoundNode.SetNames(new List<SdmxObjectName>() { new SdmxObjectName() { Lingua = "en", Nome = "No Data found" } });

                    SDMXtree.Nodes.Add(CategoryNotFoundNode);
                    return;
                }
                List<XElement> DataFlows = (from dfls in df.Descendants()
                                            where dfls.Name.LocalName == "Dataflow"
                                            select dfls).ToList();

                Categorisations = new List<Categorisation>();
                if (this.SdmxVersion == SdmxVersionEnum.Sdmx21)
                {
                    var dtflowsref =
                                  from vw in l.Descendants()
                                  where vw.Name.LocalName == "Categorisation"
                                  select vw;
                    foreach (var Categorisationitem in dtflowsref)
                    {
                        Categorisations.Add(new Categorisation()
                        {
                            CatId = GetAttributeRef(Categorisationitem, "Target"),
                            DataflowCode = GetAttributeRef(Categorisationitem, "Source")
                        });
                    }
                }
                else
                {
                    foreach (XElement dfitem in DataFlows)
                    {
                        var Catref =
                            from vw in dfitem.Elements()
                            where vw.Name.LocalName == "CategoryRef"
                            select vw;
                        if (Catref != null)
                        {
                            List<string> CatId = new List<string>();
                            List<XElement> CategoriesID = (from CatEl in Catref.Descendants()
                                                           where CatEl.Name.LocalName == "CategoryID"
                                                           select CatEl).ToList();
                            foreach (XElement CatEl in CategoriesID)
                                CatId.Add(CatEl.Elements().FirstOrDefault(el => el.Name.LocalName == "ID").Value);

                            if (CatId.Count > 0)
                            {
                                Categorisations.Add(new Categorisation()
                                {
                                    CatId = string.Join(".", CatId),
                                    DataflowCode = dfitem.Attribute("id").Value
                                });
                            }
                        }
                    }
                }

                List<XElement> CategorySchemes = (from ca in l.Descendants()
                                                  where ca.Name.LocalName == "CategoryScheme"
                                                  select ca).ToList();
                foreach (XElement CategoryScheme in CategorySchemes)
                {
                    SdmxObject CategoryNode = new SdmxObject("Categories", null, false);
                    CategoryNode.SetNames(GetNames(CategoryScheme));
                    RecursivePopulateTreeview(CategoryNode, CategoryScheme, ref DataFlows);
                    SDMXtree.Nodes.Add(CategoryNode);
                }



                if (DataFlows.Count > 0)
                {
                    SdmxObject UnCategorizedNode = new SdmxObject("Uncategorized", null, false);
                    UnCategorizedNode.SetNames(null);
                    foreach (var uncatElement in DataFlows)
                    {
                        SdmxObject dataflow = new SdmxObject(uncatElement.Attribute("id").Value, null, true);

                        dataflow.SetNames(GetNames(uncatElement));
                        dataflow.SetDataflowInfo(uncatElement);

                        if (this.SdmxVersion == SdmxVersionEnum.Sdmx20)
                            dataflow.SetKeyFamilyRef(uncatElement);
                        else
                            dataflow.SetStructureRef(uncatElement);

                        UnCategorizedNode.Nodes.Add(dataflow);
                    }
                    SDMXtree.Nodes.Add(UnCategorizedNode);

                }
                #endregion

                SDMXtree.ExpandAll();
                if (SDMXtree.Nodes.Count > 0)
                    SDMXtree.Nodes[0].EnsureVisible();
            }
            catch (Exception ex)
            {
                ShowError(string.Format("Client Application Error: {0}", ex.Message));
            }
            finally
            {
                SDMXtree.Visible = true;
                this.Cursor = Cursors.Default;
            }
        }

        private void RecursivePopulateTreeview(TreeNode parent, XElement CategorySchemes, ref List<XElement> DataFlows)
        {
            try
            {

                var els =
                    from vw in CategorySchemes.Elements()
                    where vw.Name.LocalName == "Category"
                    select vw;
                foreach (XElement item in els)
                {
                    SdmxObject category = new SdmxObject(item.Attribute("id").Value, GetNames(item), false);
                    //if (this.SdmxVersion == SdmxVersionEnum.Sdmx20)
                    //{

                    //    var dtflowsref =
                    //         from vw in item.Elements()
                    //         where vw.Name.LocalName == "DataflowRef"
                    //         select vw;
                    //    if (dtflowsref != null)
                    //    {
                    //        foreach (XElement dtflow in dtflowsref)
                    //        {
                    //            SdmxObject dataflow = new SdmxObject((
                    //               from vw in dtflow.Elements()
                    //               where vw.Name.LocalName == "DataflowID"
                    //               select vw.Value).FirstOrDefault(), null, true);

                    //            XElement dataflowrefNodo = DataFlows.Find(x => x.Attribute("id").Value == dataflow.Code);
                    //            if (dataflowrefNodo != null)
                    //            {
                    //                dataflow.SetNames(GetNames(dataflowrefNodo));
                    //                dataflow.SetDataflowInfo(dataflowrefNodo);
                    //                dataflow.SetKeyFamilyRef(dataflowrefNodo);

                    //                category.Nodes.Add(dataflow);
                    //                DataFlows.Remove(dataflowrefNodo);
                    //            }

                    //        }
                    //    }
                    //}
                    //else
                    //{
                    foreach (var dataflowref in Categorisations.FindAll(ca =>
                    {
                        string[] cats = ca.CatId.Split('.');
                        return cats[cats.Length - 1] == category.Code;
                    }))
                    {
                        SdmxObject dataflow = new SdmxObject(dataflowref.DataflowCode, null, true);

                        XElement dataflowrefNodo = DataFlows.Find(x => x.Attribute("id").Value == dataflowref.DataflowCode);
                        if (dataflowrefNodo != null)
                        {
                            dataflow.SetNames(GetNames(dataflowrefNodo));
                            dataflow.SetDataflowInfo(dataflowrefNodo);
                            if (this.SdmxVersion == SdmxVersionEnum.Sdmx20)
                                dataflow.SetKeyFamilyRef(dataflowrefNodo);
                            else
                                dataflow.SetStructureRef(dataflowrefNodo);

                            category.Nodes.Add(dataflow);
                            DataFlows.Remove(dataflowrefNodo);
                        }

                    }
                    //}

                    RecursivePopulateTreeview(category, item, ref DataFlows);
                    if (category.Nodes.Count > 0)
                        parent.Nodes.Add(category);
                }
            }
            catch (Exception ex)
            {
                ShowError(string.Format("Client Application Error: {0}", ex.Message));
            }
        }
        #endregion

        #region Concept
        List<SdmxObject> conceptNodes = new List<SdmxObject>();
        List<SdmxObject> AttributesNodes = new List<SdmxObject>();
        bool ConceptInLoad = false;

        private void PopolaConcept()
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                ConceptInLoad = true;
                LstConcept.DataSource = null;
                conceptNodes.Clear();

                LstAttributes.DataSource = null;
                AttributesNodes.Clear();

                DimensionOrdered = new List<OrderDim>();
                _SelectedDSD = _SendQuery.GetData(SendQueryStreaming.StructTypeEnum.DSD, _ChoosesDf.KeyFamilyId, _ChoosesDf.KeyFamilyAgency, _ChoosesDf.KeyFamilyVersion, this.SdmxVersion, this.ComunicationType, SendQueryStreaming.ReferencesEnum.None.ToString(), SendQueryStreaming.DetailEnum.Full, MetadataView);

                if (_SelectedDSD == null)
                {
                    ShowError(_SendQuery.ErrorString);
                    return;

                }
                var Concepts = from cs in _SelectedDSD.Descendants()
                               where cs.Name.LocalName == "Components" || cs.Name.LocalName == "DimensionList" || cs.Name.LocalName == "AttributeList"
                               select cs;

                var ConceptsNames = from cs in _SelectedDSD.Descendants()
                                    where cs.Name.LocalName == "Concept"
                                    select cs;

                //var Concepts = from cs in _SelectedDSD.Descendants()
                //               where cs.Name.LocalName == "Concept"
                //               select cs;

                //string timeConcept = (from cs in _SelectedDSD.Descendants()
                //                      where cs.Name.LocalName == "TimeDimension"
                //                      select cs).FirstOrDefault().Attribute("conceptRef").Value;

                SdmxObject ConceptNode = null;
                foreach (XElement Concept in Concepts.Elements())
                {
                    XElement ConceptName = null;
                    string idAttribute = "";
                    if (this.SdmxVersion == SdmxVersionEnum.Sdmx20)
                        idAttribute = "conceptRef";
                    else
                        idAttribute = "id";

                    try
                    {

                        ConceptName = (from coName in ConceptsNames
                                       where Concept.Name.LocalName.Trim().ToLower() != "group" && coName.Attribute("id").Value == Concept.Attribute(idAttribute).Value
                                       select coName).FirstOrDefault();
                    }
                    catch (Exception)
                    {
                        ConceptName = null;
                    }

                    switch (Concept.Name.LocalName)
                    {
                        case "Dimension":
                            ConceptNode = new SdmxObject(Concept.Attribute(idAttribute).Value, GetNames(ConceptName), false);
                            if (this.SdmxVersion == SdmxVersionEnum.Sdmx20)
                                ConceptNode.SetCodelist20(Concept);
                            else
                                ConceptNode.SetCodelist21(Concept);

                            conceptNodes.Add(ConceptNode);

                            int Ordinamento = 0;
                            if (!(Concept.Attribute("position") != null && Concept.Attribute("position").Value != null && int.TryParse(Concept.Attribute("position").Value, out Ordinamento)))
                                Ordinamento = DimensionOrdered.Count + 1;
                            DimensionOrdered.Add(new OrderDim() { DimCode = ConceptNode.Code, Order = Ordinamento });

                            break;
                        case "TimeDimension":
                            ConceptNode = new SdmxObject(Concept.Attribute(idAttribute).Value, GetNames(ConceptName), false);
                            ConceptNode.isTimeConcept = true;
                            conceptNodes.Add(ConceptNode);
                            break;
                        case "Attribute":
                            ConceptNode = new SdmxObject(Concept.Attribute(idAttribute).Value, GetNames(ConceptName), false);
                            if (this.SdmxVersion == SdmxVersionEnum.Sdmx20)
                                ConceptNode.SetCodelist20(Concept);
                            else
                                ConceptNode.SetCodelist21(Concept);

                            AttributesNodes.Add(ConceptNode);
                            break;

                    }

                }

                LstConcept.DataSource = conceptNodes;
                LstConcept.DisplayMember = "Text";
                LstConcept.ClearSelected();
                LstAttributes.DataSource = AttributesNodes;
                LstAttributes.DisplayMember = "Text";
                LstAttributes.ClearSelected();
                //if (this.SdmxVersion == SdmxVersionEnum.Sdmx20)
                //    Btn_ShowConstrain.Enabled = true;

            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                return;
            }
            finally
            {
                Cursor = Cursors.Default;
                ConceptInLoad = false;
            }
        }

        private void LstConcept_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ConceptInLoad)
                    return;
                this.Cursor = Cursors.WaitCursor;
                LstConcept.Enabled = false;
                if (LstConcept.SelectedItem != null)
                {
                    LstAttributes.SelectedItem = null;
                    LstAttributes.ClearSelected();
                    if (!((SdmxObject)LstConcept.SelectedItem).isTimeConcept)
                        PopolaCodelist(((SdmxObject)LstConcept.SelectedItem), false);
                    else
                        PopolaValueTime(((SdmxObject)LstConcept.SelectedItem).Code);
                }
            }
            catch (Exception ex)
            {
                ShowError(string.Format("Client Application Error: {0}", ex.Message));
            }
            finally
            {
                LstConcept.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }
        private void LstAttributes_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ConceptInLoad)
                    return;
                if (LstAttributes.SelectedItem != null)
                {
                    LstConcept.SelectedItem = null;
                    LstConcept.ClearSelected();
                    PopolaCodelist(((SdmxObject)LstAttributes.SelectedItem), true);

                }
            }
            catch (Exception ex)
            {
                ShowError(string.Format("Client Application Error: {0}", ex.Message));
            }
        }
        #endregion

        #region Codelist
        string EndPoint21 = null;
        private void PopolaCodelist(SdmxObject Concept, bool _IsAttribute)
        {
            try
            {

                PanCodelist.Visible = true;
                lblCodelist.Visible = true;
                LstCodelist.Visible = true;
                LstCodelist.CheckBoxes = !_IsAttribute;
                lblTimeCodelist.Visible = false;
                dtgTimeCodelist.Visible = false;
                //Btn_ShowConstrain.Visible = true;


                if (!PopulateTreeCodelist(Concept.Code)) return;


                //XElement df = _SendQuery.getData(SendQueryStreaming.StructTypeEnum.Codelist, "CL_" + ConceptCode);
                string risStr = null;
                if (this.SdmxVersion == SdmxVersionEnum.Sdmx21 && EndPoint21.Trim().ToUpper() != "SOAPSDMX21")
                    risStr = _SendQuery.getQuery(SendQueryStreaming.StructTypeEnum.Codelist, Concept.CodelistId, Concept.CodelistAgency, Concept.CodelistVersion, this.SdmxVersion, this.ComunicationType, SendQueryStreaming.ReferencesEnum.None.ToString(), SendQueryStreaming.DetailEnum.Full);
                else
                    risStr = _SendQuery.getQuery(SendQueryStreaming.StructTypeEnum.ConstrainedCodelist, ConstrainedCodelistVal.GetStringVal(_ChoosesDf, Concept), this.SdmxVersion, this.ComunicationType, SendQueryStreaming.ReferencesEnum.None.ToString(), SendQueryStreaming.DetailEnum.Full);
                XElement df = null;
                if (this.ComunicationType == ComunicationTypeEnum.REST)
                    df = _SendQuery.GetElement(_SendQuery.getRestData(risStr, _SendQuery.GetRestHeader(SendQueryStreaming.StructTypeEnum.ConstrainedCodelist, this.SdmxVersion)));
                else
                    df = _SendQuery.GetElement(_SendQuery.getSoapData(risStr, this.SdmxVersion, SendQueryStreaming.StructTypeEnum.ConstrainedCodelist));

                //XElement df = (from dsd in _SelectedDSD.Descendants()
                //               where dsd.Name.LocalName.ToLower() == "codelist" &&
                //               dsd.Attribute("id").Value == Concept.CodelistId &&
                //               dsd.Attribute("agencyID").Value == Concept.CodelistAgency &&
                //               dsd.Attribute("version").Value == Concept.CodelistVersion
                //               select dsd).FirstOrDefault();
                if (df == null)
                {

                    return;
                }
                var Codelists = from cl in df.Descendants()
                                where cl.Name.LocalName == "Code"
                                select cl;

                List<SdmxQueryInput> Codelist = new List<SdmxQueryInput>();
                string Codeid = "value";
                string ParentCodeid = "parentCode";
                if (this.SdmxVersion == SdmxVersionEnum.Sdmx21)
                {
                    ParentCodeid = "Parent";
                    Codeid = "id";
                }

                foreach (XElement Codeobj in Codelists)
                {
                    SdmxQueryInput CodeobjNode = new SdmxQueryInput()
                    {
                        Code = Codeobj.Attribute(Codeid).Value,
                        Names = GetNames(Codeobj),
                        IsAttribute = _IsAttribute,
                        ParentCode = GetAttributeRef(Codeobj, ParentCodeid)
                    };
                    Codelist.Add(CodeobjNode);
                }

                _DataQuery[Concept.Code] = Codelist;
                PopulateTreeCodelist(Concept.Code);

            }
            catch (Exception ex)
            {
                ShowError(string.Format("Client Application Error: {0}", ex.Message));
            }
        }

        private void LstCodelist_AfterCheck(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (lockCheck) return;

                if (LstCodelist.Tag != null && _DataQuery.ContainsKey(LstCodelist.Tag.ToString()) && _DataQuery[LstCodelist.Tag.ToString()].Exists(s => s.Code == ((SdmxQueryInput)e.Node).Code))
                {
                    lockCheck = true;
                    _DataQuery[LstCodelist.Tag.ToString()].Find(s => s.Code == ((SdmxQueryInput)e.Node).Code).Value = e.Node.Checked;
                    this.QueryCreator.UpdateQuery(_ChoosesDf, _DataQuery, DimensionOrdered);
                    lockCheck = false;
                }
            }
            catch (Exception ex)
            {
                ShowError(string.Format("Client Application Error: {0}", ex.Message));
            }
        }

        private void PopolaValueTime(string ConceptCode)
        {
            try
            {
                PanCodelist.Visible = false;
                lblCodelist.Visible = false;
                LstCodelist.Visible = false;
                //Btn_ShowConstrain.Visible = false;

                lblTimeCodelist.Visible = true;
                dtgTimeCodelist.Visible = true;

                dtgTimeCodelist.Tag = ConceptCode;
                DtgTimeClear();
                if (_DataQuery.ContainsKey(ConceptCode) && _DataQuery[ConceptCode] != null && _DataQuery[ConceptCode].Count == 1 && _DataQuery[ConceptCode][0].TimeWhereValue != null)
                {
                    _DataQuery[ConceptCode][0].TimeWhereValue.ForEach(tp =>
                    {
                        dtgTimeCodelist.Rows.Clear();
                        dtgTimeCodelist.Rows.Add(new object[2]
                        {
                            tp.StartTime,tp.EndTime
                        });
                    });

                    return;
                }
                _DataQuery[ConceptCode] = new List<SdmxQueryInput>()
                 {
                     new SdmxQueryInput()
                     {
                         Code=ConceptCode,
                         TimeWhereValue= new List<TimeWhere>()
                     }
                 };
            }
            catch (Exception ex)
            {
                ShowError(string.Format("Client Application Error: {0}", ex.Message));
            }
        }

        private void DtgTimeClear()
        {
            dtgTimeCodelist.Rows.Clear();
            dtgTimeCodelist.Rows.Add(new object[2] { "", "" });
        }

        private void dtgTimeCodelist_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                List<TimeWhere> selectedTimePeriod = new List<TimeWhere>();
                for (int i = dtgTimeCodelist.Rows.Count - 1; i >= 0; i--)
                {
                    DataGridViewRow riga = dtgTimeCodelist.Rows[i];
                    if (string.IsNullOrEmpty((string)riga.Cells[ColStart.Index].Value) && string.IsNullOrEmpty((string)riga.Cells[ColEnd.Index].Value) && i + 1 < dtgTimeCodelist.Rows.Count)
                    {
                        try
                        {
                            dtgTimeCodelist.Rows.RemoveAt(i);
                        }
                        catch (Exception)
                        {
                            //non succede niente ma stò provando a buttare l'ultima riga di un datagrid  a cui posso aggiungere righe
                        }
                        continue;
                    }

                    riga.Cells[ColStart.Index].Value = riga.Cells[ColStart.Index].Value != null ? riga.Cells[ColStart.Index].Value.ToString().Trim() : "";
                    riga.Cells[ColEnd.Index].Value = riga.Cells[ColEnd.Index].Value != null ? riga.Cells[ColEnd.Index].Value.ToString().Trim() : "";

                    selectedTimePeriod.Add(new TimeWhere()
                    {
                        StartTime = ((string)riga.Cells[ColStart.Index].Value).Trim(),
                        EndTime = ((string)riga.Cells[ColEnd.Index].Value).Trim(),
                    });
                }
                _DataQuery[dtgTimeCodelist.Tag.ToString()][0].TimeWhereValue = selectedTimePeriod;
                QueryCreator.UpdateQuery(_ChoosesDf, _DataQuery, DimensionOrdered);
            }
            catch (Exception ex)
            {
                ShowError(string.Format("Client Application Error: {0}", ex.Message));
            }
        }

        private bool PopulateTreeCodelist(string ConceptCode)
        {
            try
            {
                LstCodelist.Visible = false;
                LstCodelist.Nodes.Clear();
                LstCodelist.SuspendLayout();

                if (ShowConstrain && !_DataQuery.ContainsKey(ConceptCode))
                    _DataQuery.Add(ConceptCode, AddCodelistConstrain());


                if (_DataQuery.ContainsKey(ConceptCode))
                {
                    lockCheck = true;
                    foreach (SdmxQueryInput item in _DataQuery[ConceptCode].FindAll(c => string.IsNullOrEmpty(c.ParentCode) ||  
                                                                                    _DataQuery[ConceptCode].FindAll(s=> s.Code == c.ParentCode) == null))
                    {
                        LstCodelist.Nodes.Add(item);
                        item.Nodes.Clear();
                        RecursiveAddChildNodeCodelist(ConceptCode, item);
                    }

                    //for (int i = 0; i < LstCodelist.Items.Count; i++)
                    //    LstCodelist.SetItemChecked(i, (LstCodelist.Items[i] as SdmxQueryInput).Value);
                    LstCodelist.Tag = ConceptCode;
                    LstCodelist.ExpandAll();
                    scrollTree();

                    lockCheck = false;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                ShowError(string.Format("Client Application Error: {0}", ex.Message));
                return true;
            }
            finally
            {
                LstCodelist.Visible = true;
                LstCodelist.ResumeLayout();
            }
        }


        private void RecursiveAddChildNodeCodelist(string ConceptCode, SdmxQueryInput parentNode)
        {
            try
            {
                foreach (SdmxQueryInput item in _DataQuery[ConceptCode].FindAll(c => c.ParentCode == parentNode.Code))
                {
                    parentNode.Nodes.Add(item);
                    item.Nodes.Clear();
                    RecursiveAddChildNodeCodelist(ConceptCode, item);
                }
            }
            catch (Exception ex)
            {
                ShowError(string.Format("Client Application Error: {0}", ex.Message));
            }
        }

        public bool ShowConstrain { get; set; }
        private void Btn_ShowConstrain_Click(object sender, EventArgs e)
        {
            ShowConstrain = !ShowConstrain;
            //ShowConstrain_Click();
        }
        //private void ShowConstrain_Click()
        //{
        //    if (ShowConstrain)
        //    {
        //        Btn_ShowConstrain.BackColor = Color.Yellow;
        //        Btn_ShowConstrain.Text = "Show Entire Codelist";
        //    }
        //    else
        //    {
        //        Btn_ShowConstrain.BackColor = SystemColors.Control;
        //        Btn_ShowConstrain.Text = "Show Constrain Codelist";
        //    }

        //    LstCodelist.Nodes.Clear();
        //    _DataQuery = new Dictionary<string, List<SdmxQueryInput>>();
        //    if (this.QueryCreator != null)
        //        this.QueryCreator.UpdateQuery(_ChoosesDf, _DataQuery, DimensionOrdered);

        //    if (LstConcept.SelectedItem == null && LstAttributes.SelectedItem == null)
        //        return;

        //    bool IsAttribute = false;
        //    SdmxObject codeobj = null;
        //    if (LstConcept.SelectedItem != null)
        //        codeobj = ((SdmxObject)LstConcept.SelectedItem);
        //    else if (LstAttributes.SelectedItem != null)
        //    {
        //        codeobj = ((SdmxObject)LstAttributes.SelectedItem);
        //        IsAttribute = true;
        //    }
        //    if ((codeobj == null || string.IsNullOrEmpty(codeobj.CodelistId)) && !codeobj.isTimeConcept)
        //        return;

        //    PopolaCodelist(codeobj, IsAttribute);
        //}

        //public void ResetConstrain()
        //{
        //    ShowConstrain = false;
        //    Btn_ShowConstrain.Enabled = false;
        //    Btn_ShowConstrain.BackColor = SystemColors.Control;
        //    Btn_ShowConstrain.Text = "Show Constrain Codelist";
        //}

        private List<SdmxQueryInput> AddCodelistConstrain()
        {
            if (LstConcept.SelectedItem == null && LstAttributes.SelectedItem == null)
                return null;

            bool IsAttribute = false;
            SdmxObject codeobj = null;
            if (LstConcept.SelectedItem != null)
                codeobj = ((SdmxObject)LstConcept.SelectedItem);
            else if (LstAttributes.SelectedItem != null)
            {
                codeobj = ((SdmxObject)LstAttributes.SelectedItem);
                IsAttribute = true;
            }
            if ((codeobj == null || string.IsNullOrEmpty(codeobj.CodelistId)) && !codeobj.isTimeConcept)
                return null;

            string risStr = _SendQuery.getQuery(SendQueryStreaming.StructTypeEnum.ConstrainedCodelist, ConstrainedCodelistVal.GetStringVal(_ChoosesDf, codeobj), this.SdmxVersion, this.ComunicationType, SendQueryStreaming.ReferencesEnum.None.ToString(), SendQueryStreaming.DetailEnum.Full);
            XElement ris = null;
            if (this.ComunicationType == ComunicationTypeEnum.REST)
                ris = _SendQuery.GetElement(_SendQuery.getRestData(risStr, _SendQuery.GetRestHeader(SendQueryStreaming.StructTypeEnum.ConstrainedCodelist, this.SdmxVersion)));
            else
                ris = _SendQuery.GetElement(_SendQuery.getSoapData(risStr, this.SdmxVersion, SendQueryStreaming.StructTypeEnum.ConstrainedCodelist));

            var Codelists = from cl in ris.Descendants()
                            where cl.Name.LocalName == "Code"
                            select cl;

            List<SdmxQueryInput> Codelist = new List<SdmxQueryInput>();
            string Codeid = "value";
            string ParentCodeid = "parentCode";
            foreach (XElement Codeobj in Codelists)
            {
                SdmxQueryInput CodeobjNode = new SdmxQueryInput()
                {
                    Code = Codeobj.Attribute(Codeid).Value,
                    Names = GetNames(Codeobj),
                    IsAttribute = IsAttribute,
                    ParentCode = GetAttributeRef(Codeobj, ParentCodeid)
                };
                Codelist.Add(CodeobjNode);
            }

            return Codelist;
        }
        #endregion

        #region QueryCreator Event
        void QueryCreator_ResetEvent()
        {
            try
            {
                LstConcept.ClearSelected();
                LstAttributes.ClearSelected();
                LstCodelist.Nodes.Clear();
                _DataQuery = new Dictionary<string, List<SdmxQueryInput>>();

                //PopolaConcept();
                this.QueryCreator.UpdateQuery(_ChoosesDf, _DataQuery, DimensionOrdered);

            }
            catch (Exception ex)
            {
                ShowError(string.Format("Client Application Error: {0}", ex.Message));
            }
        }

        void QueryCreator_UpdateFlatEvent()
        {
            try
            {
                this.QueryCreator.UpdateQuery(_ChoosesDf, _DataQuery, DimensionOrdered);
            }
            catch (Exception ex)
            {
                ShowError(string.Format("Client Application Error: {0}", ex.Message));
            }
        }

        void QueryCreator_GetDataEvent(string Text, string _typeOperation)
        {
            try
            {
                if (string.IsNullOrEmpty(Text))
                {
                    MessageBox.Show("Select a query or Dataflow", "On the fly");
                    return;
                }

                this.Cursor = Cursors.WaitCursor;
                txtResult.Testo = "Loading....";
                tabControl1.SelectedTab = tpRIS;
                Application.DoEvents();

                if (!ExportPath.Exists) ExportPath.Create();

                string filename = string.Format("{0}\\OnTheFlyResponse.xml", ExportPath.FullName);

                XElement l = null;
                if (this.ComunicationType == ComunicationTypeEnum.SOAP)
                {
                    TestOnTheFlyService.SendQueryStreaming.StructTypeEnum TypeOperation = (SendQueryStreaming.StructTypeEnum)Enum.Parse(typeof(SendQueryStreaming.StructTypeEnum), _typeOperation);

                    l = _SendQuery.GetData(TypeOperation, Text, this.SdmxVersion, this.ComunicationType, SendQueryStreaming.ReferencesEnum.None.ToString(), SendQueryStreaming.DetailEnum.Full, MetadataView);
                }
                else
                {
                    l = _SendQuery.GetElement(_SendQuery.getRestData(Text, _typeOperation));
                    if (l == null && (_typeOperation == "application/dspl" ||_typeOperation == "application/json") )
                    {
                        #region Download Zip
                        txtResult.Testo = "Download in progress...";
                        string filenameDspl = string.Format("{0}\\OnTheFlyResponse.xml", ExportPath.FullName);
                        if (File.Exists(filenameDspl))
                        {
                            SaveFileDialog sfd = new SaveFileDialog();
                            sfd.Title = "'On The Fly' Result file";
                            if (_typeOperation == "application/dspl")
                            {
                                sfd.FileName = "OnTheFly_ResultDSPL.zip";
                                sfd.Filter = "Zip File|*.zip";
                            }
                            else
                            {
                                sfd.FileName = "OnTheFly_Result.json";
                                sfd.Filter = "Json File|*.json";
                            }
                            sfd.AddExtension = true;
                            if (sfd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                            {
                                txtResult.Testo = "Download annulled!";
                                return;
                            }

                            try
                            {
                                this.Cursor = Cursors.Default;
                                FileInfo fi = new FileInfo(filenameDspl);
                                fi.CopyTo(sfd.FileName, true);
                                this.Cursor = Cursors.Default;
                                txtResult.Testo = "Download file Complete Success";
                            }
                            catch (Exception ex)
                            {
                                txtResult.Testo = string.Format("Error during Download File: {0}", ex.Message);
                            }
                        }
                        return;
                        #endregion
                    }
                    else
                        txtResult.FileSavedPath = filename;
                }

                if (l == null)
                {
                    ShowError(_SendQuery.ErrorString);
                    return;
                }


                WriteResult(l, filename);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

        }
        #endregion

        #region Other
        public void Clear()
        {
            try
            {
                LstConcept.DataSource = null;
                LstAttributes.DataSource = null;
                LstCodelist.Nodes.Clear();
                DtgTimeClear();
                lblCodelist.Visible = true;
                LstCodelist.Visible = true;
                //Btn_ShowConstrain.Visible = true;

                lblTimeCodelist.Visible = false;
                dtgTimeCodelist.Visible = false;

                //ResetConstrain();

                _ChoosesDf = null;
                _SelectedDSD = null;
                _DataQuery = new Dictionary<string, List<SdmxQueryInput>>();
                if (this.QueryCreator != null)
                    this.QueryCreator.UpdateQuery(_ChoosesDf, _DataQuery, DimensionOrdered);
            }
            catch (Exception ex)
            {
                ShowError(string.Format("Client Application Error: {0}", ex.Message));
            }
        }

        private List<SdmxObjectName> GetNames(XElement item)
        {

            if (item == null) return null;
            List<SdmxObjectName> Nomi = new List<SdmxObjectName>();
            try
            {
                var nomi =
                    from vw in item.Elements()
                    where vw.Name.LocalName == "Name" || vw.Name.LocalName == "Description"
                    select vw;
                foreach (XElement nome in nomi)
                {
                    Nomi.Add(new SdmxObjectName()
                    {
                        Lingua = nome.Attributes().FirstOrDefault().Value,
                        Nome = nome.Value,
                    });
                }
            }
            catch (Exception ex)
            {
                ShowError(string.Format("Client Application Error: {0}", ex.Message));
            }
            return Nomi;
        }

        private void btnChangeTest_Click(object sender, EventArgs e)
        {
            Config Conf = new Config();
            Conf.ShowDialog();
            //Test t = new Test();
            //t.ShowDialog();
        }

        private void SDMXtree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                Clear();
                if (e.Node is SdmxObject && ((SdmxObject)e.Node).IsDataset)
                {
                    lblDatasetChooses.Text = string.Format("{0} ({1})", ((SdmxObject)e.Node).Code, ((SdmxObject)e.Node).Text);
                    _ChoosesDf = ((SdmxObject)e.Node);
                    PopolaConcept();
                }
                else
                {
                    lblDatasetChooses.Text = "[Dataset Choose]";
                }
                this.QueryCreator.UpdateQuery(_ChoosesDf, _DataQuery, DimensionOrdered);
            }
            catch (Exception ex)
            {
                ShowError(string.Format("Client Application Error: {0}", ex.Message));
            }
        }
        #endregion



        private string GetAttributeRef(XElement element, string ParentCodeid)
        {
            try
            {

                if (this.SdmxVersion == SdmxVersionEnum.Sdmx20)
                {
                    return (element.Attributes(ParentCodeid).Count() > 0 ? element.Attribute(ParentCodeid).Value : null);
                }
                else
                {
                    return (element.Elements().Count(el => el.Name.LocalName == ParentCodeid) > 0 ? element.Elements().FirstOrDefault(el => el.Name.LocalName == ParentCodeid).Elements().FirstOrDefault(el => el.Name.LocalName == "Ref").Attribute("id").Value : null);
                }
            }
            catch (Exception ex)
            {
                ShowError(string.Format("Client Application Error: {0}", ex.Message));
                return null;
            }
        }

        private void ShowError(string ApplicationErr)
        {
            try
            {
                //webResult.Navigate("about:blank");
                if (!string.IsNullOrEmpty(ApplicationErr))
                {

                    //txtResult.Visible = true;
                    //webResult.Visible = false;
                    txtResult.Testo = ApplicationErr;
                    tabControl1.SelectedTab = tpRIS;
                }
                else
                    MessageBox.Show("Generic Error", "On the Fly");//non ci dovrebbe mai entrare
            }
            catch (Exception ex)
            {
                ShowError(string.Format("Client Application Error: {0}", ex.Message));
            }
        }



        private void BtnSdmx20_Click(object sender, EventArgs e)
        {
            this.SdmxVersion = SdmxVersionEnum.Sdmx20;
            updateVersion();
        }

        private void BtnSdmx21_Click(object sender, EventArgs e)
        {
            this.SdmxVersion = SdmxVersionEnum.Sdmx21;
            updateVersion();
        }
        private void BtnSoap_Click(object sender, EventArgs e)
        {
            this.ComunicationType = ComunicationTypeEnum.SOAP;
            updateVersion();
        }

        private void BtnRest_Click(object sender, EventArgs e)
        {
            this.ComunicationType = ComunicationTypeEnum.REST;
            updateVersion();
        }

        private void updateVersion()
        {
            try
            {
                SDMXtree.Nodes.Clear();
                lblDatasetChooses.Text = "[Dataset Choose]";
                Clear();
                BtnSdmx20.BackColor = Color.White;
                BtnSdmx20.Checked = false;
                BtnSdmx21.BackColor = Color.White;
                BtnSdmx21.Checked = false;
                BtnSoap.BackColor = Color.White;
                BtnSoap.Checked = false;
                BtnRest.BackColor = Color.White;
                BtnRest.Checked = false;

                groupBoxMetadata20.Visible = false;
                groupBoxMetadata21.Visible = false;

                Rdn_AgencySchema.Checked = false;
                Rdn_CategorySchema.Checked = false;
                Rdn_Dataflows.Checked = false;
                Rdn_ConceptsSchema.Checked = false;
                Rdn_Codelist.Checked = false;
                Rdn_ConstrainedCodelist.Checked = false;
                Rdn_DataStructure.Checked = false;
                Rdn_AgencySchema21.Checked = false;
                Rdn_Categorisation21.Checked = false;
                Rdn_CategorySchema21.Checked = false;
                Rdn_Dataflows21.Checked = false;
                Rdn_ConceptsSchema21.Checked = false;
                Rdn_Codelist21.Checked = false;
                Rdn_ConstrainedCodelist21.Checked = false;
                Rdn_ConstrainedCodelist21.Visible = false;
                Rdn_DataStructure21.Checked = false;
                chkResolveReferences.Checked = false;
                cmbDetail.Enabled = true;
                if (cmbDetail.Items != null && cmbDetail.Items.Count > 0)
                    cmbDetail.SelectedIndex = 0;
                if (cmbReferences.Items != null && cmbReferences.Items.Count > 0)
                    cmbReferences.SelectedIndex = 0;

                MetadataView.Testo = null;

                switch (this.SdmxVersion)
                {
                    case SdmxVersionEnum.Sdmx20:
                        BtnSdmx20.BackColor = Color.Yellow;
                        BtnSdmx20.Checked = true;
                        groupBoxMetadata20.Visible = true;
                        break;
                    case SdmxVersionEnum.Sdmx21:
                        BtnSdmx21.BackColor = Color.Yellow;
                        BtnSdmx21.Checked = true;
                        groupBoxMetadata21.Visible = true;
                        break;
                }
                switch (this.ComunicationType)
                {
                    case ComunicationTypeEnum.SOAP:
                        BtnSoap.BackColor = Color.LightGreen;
                        BtnSoap.Checked = true;
                        break;
                    case ComunicationTypeEnum.REST:
                        BtnRest.BackColor = Color.LightGreen;
                        BtnRest.Checked = true;
                        groupBoxMetadata20.Visible = false;
                        groupBoxMetadata21.Visible = true;
                        if (this.SdmxVersion == SdmxVersionEnum.Sdmx20)
                            cmbDetail.Enabled = false;
                        Rdn_ConstrainedCodelist21.Visible = true;
                        break;
                }
                CreaQueryCreator();
            }
            catch (Exception ex)
            {
                ShowError(string.Format("Client Application Error: {0}", ex.Message));
            }
        }
        public void CreaQueryCreator()
        {
            try
            {
                this.tabPageData.Controls.Clear();
                switch (this.ComunicationType)
                {
                    case ComunicationTypeEnum.SOAP:
                        switch (this.SdmxVersion)
                        {
                            case SdmxVersionEnum.Sdmx20:
                                QueryCreator = new TestOnTheFlyService.QueryCreator();
                                break;
                            case SdmxVersionEnum.Sdmx21:
                                QueryCreator = new TestOnTheFlyService.QueryCreator21();
                                break;
                        }
                        break;
                    case ComunicationTypeEnum.REST:
                        QueryCreator = new TestOnTheFlyService.QueryCreatorRest()
                        {
                            SdmxVersion = this.SdmxVersion
                        };
                        break;
                }


                this.tabPageData.Controls.Add(((UserControl)QueryCreator));
                // 
                // QueryCreator
                // 
                ((UserControl)QueryCreator).BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                ((UserControl)QueryCreator).Dock = System.Windows.Forms.DockStyle.Fill;
                ((UserControl)QueryCreator).Location = new System.Drawing.Point(3, 3);
                ((UserControl)QueryCreator).Margin = new System.Windows.Forms.Padding(7);
                ((UserControl)QueryCreator).Name = "QueryCreator";
                ((UserControl)QueryCreator).Size = new System.Drawing.Size(387, 418);
                ((UserControl)QueryCreator).TabIndex = 9;

                QueryCreator.ResetEvent -= QueryCreator_ResetEvent;
                QueryCreator.ResetEvent += QueryCreator_ResetEvent;
                QueryCreator.UpdateFlatEvent -= QueryCreator_UpdateFlatEvent;
                QueryCreator.UpdateFlatEvent += QueryCreator_UpdateFlatEvent;
                QueryCreator.GetDataEvent -= QueryCreator_GetDataEvent;
                QueryCreator.GetDataEvent += QueryCreator_GetDataEvent;
            }
            catch (Exception ex)
            {
                ShowError(string.Format("Client Application Error: {0}", ex.Message));
            }
        }

        #region Handle Gest treeview
        private const int WM_SCROLL = 276; // Horizontal scroll
        private const int WM_VSCROLL = 277; // Vertical scroll
        private const int SB_LINEUP = 0; // Scrolls one line up
        private const int SB_LINELEFT = 0;// Scrolls one cell left
        private const int SB_LINEDOWN = 1; // Scrolls one line down
        private const int SB_LINERIGHT = 1;// Scrolls one cell right
        private const int SB_PAGEUP = 2; // Scrolls one page up
        private const int SB_PAGELEFT = 2;// Scrolls one page left
        private const int SB_PAGEDOWN = 3; // Scrolls one page down
        private const int SB_PAGERIGTH = 3; // Scrolls one page right
        private const int SB_PAGETOP = 6; // Scrolls to the upper left
        private const int SB_LEFT = 6; // Scrolls to the left
        private const int SB_PAGEBOTTOM = 7; // Scrolls to the upper right
        private const int SB_RIGHT = 7; // Scrolls to the right
        private const int SB_ENDSCROLL = 8; // Ends scroll

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        private void scrollTree()
        {
            if (LstCodelist.Nodes.Count > 0)
                LstCodelist.Nodes[0].EnsureVisible();
            SendMessage(LstCodelist.Handle, WM_VSCROLL, (IntPtr)SB_PAGEUP, IntPtr.Zero);
            SendMessage(LstCodelist.Handle, WM_SCROLL, (IntPtr)SB_PAGELEFT, IntPtr.Zero);
        }

        #endregion


        private void WriteResult(XElement result, string filename)
        {
            try
            {
                txtResult.Testo = "Writing and Formatting Data...";
                Application.DoEvents();

                // result.Save(filename);
                txtResult.Testo = result.ToString();
                txtResult.FileSavedPath = filename;
            }
            catch (Exception ex)
            {
                ShowError(string.Format("Client Application Error: {0}", ex.Message));
            }
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtResult.FileSavedPath) && File.Exists(txtResult.FileSavedPath))
                {
                    Process pp = new Process();
                    pp.StartInfo = new ProcessStartInfo(txtResult.FileSavedPath);
                    pp.Start();
                }
            }
            catch (Exception ex)
            {
                ShowError(string.Format("Client Application Error: {0}", ex.Message));
            }
        }

        private void BtnOpenFolder_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtResult.FileSavedPath) && File.Exists(txtResult.FileSavedPath))
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Title = "'On The Fly' Result file";
                    sfd.FileName = "OnTheFly_Result";
                    sfd.Filter = "Xml File|*.xml|Json file|*.json|Text file|*.txt";
                    sfd.AddExtension = true;
                    if (sfd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                        return;

                    try
                    {
                        bool CopyUgual = false;
                        this.Cursor = Cursors.Default;

                        try
                        {
                            StreamDownloadXML(sfd.FileName);
                        }
                        catch (Exception)
                        {
                            CopyUgual = true;
                        }

                        if (CopyUgual)
                        {
                            FileInfo fi = new FileInfo(txtResult.FileSavedPath);
                            fi.CopyTo(sfd.FileName, true);
                        }
                        this.Cursor = Cursors.Default;
                        MessageBox.Show("Download file Complete Success", "On The Fly");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("Error during Download File: {0}", ex.Message), "On The Fly");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(string.Format("Client Application Error: {0}", ex.Message));
            }
        }

        private void StreamDownloadXML(string DownFilename)
        {

            using (StreamWriter eeewriter = new StreamWriter(DownFilename))
            {
                using (XmlWriter writer = XmlWriter.Create(eeewriter))
                {
                    using (StreamReader responseStream = new StreamReader(txtResult.FileSavedPath))
                    {
                        using (XmlTextReader reader = new XmlTextReader(responseStream))
                        {
                            if (reader.Read())
                            {
                                while (reader.NodeType != XmlNodeType.Element)
                                    reader.Read();

                                if (reader.LocalName.Trim().ToLower() == "envelope")
                                {
                                    while (reader.LocalName.Trim().ToLower() != "body")
                                        reader.Read();
                                    reader.Read();
                                    if (reader.Prefix == "web")
                                        reader.Read();
                                }

                                writer.WriteNode(reader, true);
                                writer.Flush();
                            }
                        }
                    }
                }
            }
        }

        private void btnOpenTest_Click(object sender, EventArgs e)
        {
            TestQuery tq = new TestQuery();
            tq.ShowDialog();
        }


        #region Metadata
        private void btnGetMetadata_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                ShowMetadataQuery();
                if (!MetadataView.Testo.StartsWith("<") && MetadataView.Testo.StartsWith("Warning:"))
                {
                    MessageBox.Show(MetadataView.Testo);
                    return;
                }

                this.Cursor = Cursors.WaitCursor;
                txtResult.Testo = "Loading....";
                //txtResult.Visible = true;
                //webResult.Visible = false;
                //webResult.Navigate("about:blank");
                tabControl1.SelectedTab = tpRIS;
                Application.DoEvents();
                if (string.IsNullOrEmpty(MetadataView.Testo))
                {
                    ShowError("Query is Empty");
                    return;
                }
                XElement ris = null;
                if (this.ComunicationType == ComunicationTypeEnum.REST)
                    ris = _SendQuery.GetElement(_SendQuery.getRestData(MetadataView.Testo, _SendQuery.GetRestHeader(GetTipo(), this.SdmxVersion)));
                else
                    ris = _SendQuery.GetElement(_SendQuery.getSoapData(MetadataView.Testo, this.SdmxVersion, GetTipo()));

                if (ris == null)
                {
                    if (!string.IsNullOrEmpty(_SendQuery.ErrorString))
                        ShowError(_SendQuery.ErrorString);
                    else
                        ShowError("Not recognized Query");
                    return;
                }

                if (!ExportPath.Exists) ExportPath.Create();

                string filename = string.Format("{0}\\OnTheFlyResponse.xml", ExportPath.FullName);



                WriteResult(ris, filename);

                sw.Stop();
                Console.WriteLine(string.Format("AllData in {0} ms", sw.ElapsedMilliseconds));
            }
            catch (Exception ex)
            {
                ShowError(string.Format("Client Application Error: {0}", ex.Message));
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void ShowMetadataQuery()
        {
            try
            {

                string query = null;
                if (this.ComunicationType == ComunicationTypeEnum.REST)
                {
                    string reference = cmbReferences.Text;
                    SendQueryStreaming.DetailEnum detail = (SendQueryStreaming.DetailEnum)Enum.Parse(typeof(SendQueryStreaming.DetailEnum), cmbDetail.SelectedItem.ToString());
                    query = GetMetadata21(reference, detail);
                }
                else
                {
                    if (this.SdmxVersion == SdmxVersionEnum.Sdmx20)
                    {
                        string reference = SendQueryStreaming.ReferencesEnum.None.ToString();
                        if (chkResolveReferences.Checked)
                            reference = SendQueryStreaming.ReferencesEnum.All.ToString();
                        SendQueryStreaming.DetailEnum detail = SendQueryStreaming.DetailEnum.Full;

                        query = GetMetadata20(reference, detail);
                    }
                    else
                    {
                        string reference = cmbReferences.Text;
                        SendQueryStreaming.DetailEnum detail = (SendQueryStreaming.DetailEnum)Enum.Parse(typeof(SendQueryStreaming.DetailEnum), cmbDetail.SelectedItem.ToString());

                        query = GetMetadata21(reference, detail);
                    }
                }

                if (query == null)
                    return;


                MetadataView.Testo = query;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private string GetMetadata20(string reference, SendQueryStreaming.DetailEnum detail)
        {

            string ris = null;
            try
            {


                if (Rdn_AgencySchema.Checked)
                {
                    ris = _SendQuery.getQuery(SendQueryStreaming.StructTypeEnum.Agency, null, this.SdmxVersion, this.ComunicationType, reference, detail);
                }
                else if (Rdn_CategorySchema.Checked)
                {
                    ris = _SendQuery.getQuery(SendQueryStreaming.StructTypeEnum.Category, null, this.SdmxVersion, this.ComunicationType, reference, detail);
                }
                else if (Rdn_Dataflows.Checked)
                {
                    ris = _SendQuery.getQuery(SendQueryStreaming.StructTypeEnum.Dataflow, null, this.SdmxVersion, this.ComunicationType, reference, detail);
                }
                else if (Rdn_ConceptsSchema.Checked)
                {
                    ris = _SendQuery.getQuery(SendQueryStreaming.StructTypeEnum.Concepts, null, this.SdmxVersion, this.ComunicationType, reference, detail);
                }
                else if (Rdn_Codelist.Checked)
                {
                    if (LstConcept.SelectedItem == null && LstAttributes.SelectedItem == null)
                        return "Warning: Choose Concept";

                    SdmxObject codeobj = null;
                    if (LstConcept.SelectedItem != null)
                        codeobj = ((SdmxObject)LstConcept.SelectedItem);
                    else if (LstAttributes.SelectedItem != null)
                        codeobj = ((SdmxObject)LstAttributes.SelectedItem);
                    if (codeobj == null || string.IsNullOrEmpty(codeobj.CodelistId))
                        return "Warning: Chooses Concept not contains Codelist";
                    ris = _SendQuery.getQuery(SendQueryStreaming.StructTypeEnum.Codelist, codeobj.CodelistId, codeobj.CodelistAgency, codeobj.CodelistVersion, this.SdmxVersion, this.ComunicationType, reference, detail);

                }
                else if (Rdn_ConstrainedCodelist.Checked)
                {
                    if (LstConcept.SelectedItem == null && LstAttributes.SelectedItem == null)
                        return "Warning: Choose Concept";

                    SdmxObject codeobj = null;
                    if (LstConcept.SelectedItem != null)
                        codeobj = ((SdmxObject)LstConcept.SelectedItem);
                    else if (LstAttributes.SelectedItem != null)
                        codeobj = ((SdmxObject)LstAttributes.SelectedItem);
                    if ((codeobj == null || string.IsNullOrEmpty(codeobj.CodelistId)) && !codeobj.isTimeConcept)
                        return "Warning: Chooses Concept not contains Codelist";

                    ris = _SendQuery.getQuery(SendQueryStreaming.StructTypeEnum.ConstrainedCodelist, ConstrainedCodelistVal.GetStringVal(_ChoosesDf, codeobj), this.SdmxVersion, this.ComunicationType, reference, detail);

                }
                else if (Rdn_DataStructure.Checked)
                {
                    if (_ChoosesDf == null)
                        return "Warning: Choose Dataflow";

                    ris = _SendQuery.getQuery(SendQueryStreaming.StructTypeEnum.DSD, _ChoosesDf.KeyFamilyId, _ChoosesDf.KeyFamilyAgency, _ChoosesDf.KeyFamilyVersion, this.SdmxVersion, this.ComunicationType, reference, detail);
                }
                else
                {
                    return "Warning: Metadata Type not selected";
                }
            }
            catch (Exception ex)
            {
                ShowError(string.Format("Client Application Error: {0}", ex.Message));
            }
            return ris;
        }
        private string GetMetadata21(string reference, SendQueryStreaming.DetailEnum detail)
        {
            string ris = null;
            try
            {


                if (Rdn_AgencySchema21.Checked)
                {
                    ris = _SendQuery.getQuery(SendQueryStreaming.StructTypeEnum.Agency, null, this.SdmxVersion, this.ComunicationType, reference, detail);
                }
                else if (Rdn_Categorisation21.Checked)
                {
                    if (this.ComunicationType == ComunicationTypeEnum.REST && this.SdmxVersion == SdmxVersionEnum.Sdmx20)
                        return "Warning: Rest Sdmx v 2.0 is not possible return a Categorisation. The result will be ever empty";
                    ris = _SendQuery.getQuery(SendQueryStreaming.StructTypeEnum.Categorisation, null, this.SdmxVersion, this.ComunicationType, reference, detail);
                }
                else if (Rdn_CategorySchema21.Checked)
                {
                    ris = _SendQuery.getQuery(SendQueryStreaming.StructTypeEnum.Category, null, this.SdmxVersion, this.ComunicationType, reference, detail);
                }
                else if (Rdn_Dataflows21.Checked)
                {
                    ris = _SendQuery.getQuery(SendQueryStreaming.StructTypeEnum.Dataflow, null, this.SdmxVersion, this.ComunicationType, reference, detail);
                }
                else if (Rdn_ConceptsSchema21.Checked)
                {
                    ris = _SendQuery.getQuery(SendQueryStreaming.StructTypeEnum.Concepts, null, this.SdmxVersion, this.ComunicationType, reference, detail);
                }
                else if (Rdn_Codelist21.Checked)
                {
                    if (LstConcept.SelectedItem == null && LstAttributes.SelectedItem == null)
                        return "Warning: Choose Concept";

                    SdmxObject codeobj = null;
                    if (LstConcept.SelectedItem != null)
                        codeobj = ((SdmxObject)LstConcept.SelectedItem);
                    else if (LstAttributes.SelectedItem != null)
                        codeobj = ((SdmxObject)LstAttributes.SelectedItem);
                    if (codeobj == null || string.IsNullOrEmpty(codeobj.CodelistId))
                        return "Warning: Chooses Concept not contains Codelist";
                    ris = _SendQuery.getQuery(SendQueryStreaming.StructTypeEnum.Codelist, codeobj.CodelistId, codeobj.CodelistAgency, codeobj.CodelistVersion, this.SdmxVersion, this.ComunicationType, reference, detail);
                }
                else if (Rdn_ConstrainedCodelist21.Checked)
                {
                    if (LstConcept.SelectedItem == null && LstAttributes.SelectedItem == null)
                        return "Warning: Choose Concept";

                    SdmxObject codeobj = null;
                    if (LstConcept.SelectedItem != null)
                        codeobj = ((SdmxObject)LstConcept.SelectedItem);
                    else if (LstAttributes.SelectedItem != null)
                        codeobj = ((SdmxObject)LstAttributes.SelectedItem);
                    if ((codeobj == null || string.IsNullOrEmpty(codeobj.CodelistId)) && !codeobj.isTimeConcept)
                        return "Warning: Chooses Concept not contains Codelist";

                    ris = _SendQuery.getQuery(SendQueryStreaming.StructTypeEnum.ConstrainedCodelist, ConstrainedCodelistVal.GetStringVal(_ChoosesDf, codeobj), this.SdmxVersion, this.ComunicationType, reference, detail);

                }
                else if (Rdn_DataStructure21.Checked)
                {
                    if (_ChoosesDf == null)
                        return "Warning: Choose Dataflow";
                    ris = _SendQuery.getQuery(SendQueryStreaming.StructTypeEnum.DSD, _ChoosesDf.KeyFamilyId, _ChoosesDf.KeyFamilyAgency, _ChoosesDf.KeyFamilyVersion, this.SdmxVersion, this.ComunicationType, reference, detail);
                }
                else
                {
                    return "Warning: Metadata Type not selected";
                }
                if (ris == null)
                {
                    ShowError(_SendQuery.ErrorString);
                    return null;
                }
            }
            catch (Exception ex)
            {
                ShowError(string.Format("Client Application Error: {0}", ex.Message));
            }
            return ris;
        }

        private SendQueryStreaming.StructTypeEnum GetTipo()
        {
            SdmxVersionEnum ver = this.SdmxVersion;
            if (this.ComunicationType == ComunicationTypeEnum.REST)
                ver = SdmxVersionEnum.Sdmx21;
            switch (ver)
            {
                case SdmxVersionEnum.Sdmx20:
                    if (Rdn_AgencySchema.Checked)
                        return SendQueryStreaming.StructTypeEnum.Agency;
                    else if (Rdn_Categorisation21.Checked)
                        return SendQueryStreaming.StructTypeEnum.Categorisation;
                    else if (Rdn_CategorySchema.Checked)
                        return SendQueryStreaming.StructTypeEnum.Category;
                    else if (Rdn_Dataflows.Checked)
                        return SendQueryStreaming.StructTypeEnum.Dataflow;
                    else if (Rdn_ConceptsSchema.Checked)
                        return SendQueryStreaming.StructTypeEnum.Concepts;
                    else if (Rdn_Codelist.Checked)
                        return SendQueryStreaming.StructTypeEnum.Codelist;
                    else if (Rdn_ConstrainedCodelist.Checked)
                        return SendQueryStreaming.StructTypeEnum.ConstrainedCodelist;
                    else if (Rdn_DataStructure.Checked)
                        return SendQueryStreaming.StructTypeEnum.DSD;
                    break;
                case SdmxVersionEnum.Sdmx21:
                    if (Rdn_AgencySchema21.Checked)
                        return SendQueryStreaming.StructTypeEnum.Agency;
                    else if (Rdn_Categorisation21.Checked)
                        return SendQueryStreaming.StructTypeEnum.Categorisation;
                    else if (Rdn_CategorySchema21.Checked)
                        return SendQueryStreaming.StructTypeEnum.Category;
                    else if (Rdn_Dataflows21.Checked)
                        return SendQueryStreaming.StructTypeEnum.Dataflow;
                    else if (Rdn_ConceptsSchema21.Checked)
                        return SendQueryStreaming.StructTypeEnum.Concepts;
                    else if (Rdn_Codelist21.Checked)
                        return SendQueryStreaming.StructTypeEnum.Codelist;
                    else if (Rdn_ConstrainedCodelist21.Checked)
                        return SendQueryStreaming.StructTypeEnum.ConstrainedCodelist;
                    else if (Rdn_DataStructure21.Checked)
                        return SendQueryStreaming.StructTypeEnum.DSD;
                    break;
            }
            throw new Exception("Metadata Type not selected");

        }

        private void Rdn_Metadata_CheckedChanged(object sender, EventArgs e)
        {
            ShowMetadataQuery();
        }

        private void cmbDetail_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowMetadataQuery();

        }

        private void cmbReferences_TextChanged(object sender, EventArgs e)
        {
            ShowMetadataQuery();

        }

        private void chkResolveReferences_CheckedChanged(object sender, EventArgs e)
        {
            ShowMetadataQuery();

        }

        #endregion

        private void cmbLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeLanguages.Lang = cmbLanguages.SelectedItem.ToString();
            this.Cursor = Cursors.WaitCursor;

            SDMXtree.Refresh();
            foreach (var item in conceptNodes)
                ((SdmxObject)item).SetNames(((SdmxObject)item).Names);
            LstConcept.DataSource = null;
            LstConcept.DataSource = conceptNodes;
            LstConcept.DisplayMember = "Text";

            foreach (var item in AttributesNodes)
                ((SdmxObject)item).SetNames(((SdmxObject)item).Names);
            LstAttributes.DataSource = null;
            LstAttributes.DataSource = AttributesNodes;
            LstAttributes.DisplayMember = "Text";

            LstCodelist.Refresh();
            this.Cursor = Cursors.Default;
        }



        #region Redraw


        void SDMXtree_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if ((e.State & TreeNodeStates.Focused) != 0)
            {
                Rectangle focusBounds = new Rectangle(e.Node.Bounds.Location, new Size(e.Node.Bounds.Size.Width + 20, e.Node.Bounds.Size.Height));
                e.Graphics.FillRectangle(Brushes.LightGray, focusBounds);
            }
            if (e.Node.IsSelected)
            {
                Rectangle focusBounds = new Rectangle(e.Node.Bounds.Location, new Size(e.Node.Bounds.Size.Width + 20, e.Node.Bounds.Size.Height));
                e.Graphics.FillRectangle(Brushes.LightGray, focusBounds);
            }
            if (e.Node is SdmxObject)
                RedrawNode(((SdmxObject)e.Node), e.Graphics, e.Bounds);
            else if (e.Node is SdmxQueryInput)
                RedrawNode(((SdmxQueryInput)e.Node), e.Graphics, e.Bounds);
        }

        void SDMXCodelist_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            Rectangle focusBounds = new Rectangle(e.Node.Bounds.Location, new Size(e.Node.Bounds.Size.Width + 20, e.Node.Bounds.Size.Height));
            e.Graphics.FillRectangle(Brushes.White, focusBounds);
            if (e.Node is SdmxObject)
                RedrawNode(((SdmxObject)e.Node), e.Graphics, e.Bounds);
            else if (e.Node is SdmxQueryInput)
                RedrawNode(((SdmxQueryInput)e.Node), e.Graphics, e.Bounds);
        }

        void RedrawNode(SdmxObject nodo, Graphics graphics, Rectangle rec)
        {
            string testo = ChangeLanguages.GetCorrectLanguages(nodo.Code, nodo.Names);
            RedrawNode(testo, graphics, rec);
        }
        void RedrawNode(SdmxQueryInput nodo, Graphics graphics, Rectangle rec)
        {
            string testo = ChangeLanguages.GetCorrectLanguages(nodo.Code, nodo.Names);
            RedrawNode(testo, graphics, rec);
        }
        void RedrawNode(string txtNodo, Graphics graphics, Rectangle rec)
        {
            graphics.DrawString(txtNodo, LstCodelist.Font, Brushes.Black, new Point(rec.X, rec.Y + 4));
        }
        #endregion





    }
}
