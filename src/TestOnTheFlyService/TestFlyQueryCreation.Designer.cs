using System.Windows.Forms;
namespace TestOnTheFlyService
{
    partial class TestFlyQueryCreation
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestFlyQueryCreation));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpQueryGen = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnOpenTest = new System.Windows.Forms.Button();
            this.lblTimeCodelist = new System.Windows.Forms.Label();
            this.lblCodelist = new System.Windows.Forms.Label();
            this.SDMXtree = new System.Windows.Forms.TreeView();
            this.btnChangeTest = new System.Windows.Forms.Button();
            this.tabControlTypeQueryCreator = new System.Windows.Forms.TabControl();
            this.tabPageData = new System.Windows.Forms.TabPage();
            this.tabPageMetadata = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.MetadataView = new TestOnTheFlyService.WinXmlViewer();
            this.groupBoxMetadata21 = new System.Windows.Forms.GroupBox();
            this.Rdn_ConstrainedCodelist21 = new System.Windows.Forms.RadioButton();
            this.Rdn_Categorisation21 = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbReferences = new System.Windows.Forms.ComboBox();
            this.cmbDetail = new System.Windows.Forms.ComboBox();
            this.Rdn_AgencySchema21 = new System.Windows.Forms.RadioButton();
            this.Rdn_DataStructure21 = new System.Windows.Forms.RadioButton();
            this.Rdn_Codelist21 = new System.Windows.Forms.RadioButton();
            this.Rdn_ConceptsSchema21 = new System.Windows.Forms.RadioButton();
            this.Rdn_Dataflows21 = new System.Windows.Forms.RadioButton();
            this.Rdn_CategorySchema21 = new System.Windows.Forms.RadioButton();
            this.groupBoxMetadata20 = new System.Windows.Forms.GroupBox();
            this.Rdn_ConstrainedCodelist = new System.Windows.Forms.RadioButton();
            this.chkResolveReferences = new System.Windows.Forms.CheckBox();
            this.Rdn_DataStructure = new System.Windows.Forms.RadioButton();
            this.Rdn_Codelist = new System.Windows.Forms.RadioButton();
            this.Rdn_ConceptsSchema = new System.Windows.Forms.RadioButton();
            this.Rdn_Dataflows = new System.Windows.Forms.RadioButton();
            this.Rdn_CategorySchema = new System.Windows.Forms.RadioButton();
            this.Rdn_AgencySchema = new System.Windows.Forms.RadioButton();
            this.btnGetMetadata = new System.Windows.Forms.Button();
            this.dtgTimeCodelist = new System.Windows.Forms.DataGridView();
            this.ColStart = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColEnd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.LstConcept = new System.Windows.Forms.ListBox();
            this.lblAttribute = new System.Windows.Forms.Label();
            this.LstAttributes = new System.Windows.Forms.ListBox();
            this.lblDatasetChooses = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.BtnSdmx20 = new System.Windows.Forms.RadioButton();
            this.BtnSdmx21 = new System.Windows.Forms.RadioButton();
            this.BtnGo = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.BtnRest = new System.Windows.Forms.RadioButton();
            this.BtnSoap = new System.Windows.Forms.RadioButton();
            this.PanCodelist = new System.Windows.Forms.Panel();
            this.LstCodelist = new TestOnTheFlyService.TreeNoDBClick();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbLanguages = new System.Windows.Forms.ComboBox();
            this.tpRIS = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.BtnOpenFolder = new System.Windows.Forms.Button();
            this.txtResult = new TestOnTheFlyService.WinXmlViewer();
            this.tabControl1.SuspendLayout();
            this.tpQueryGen.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabControlTypeQueryCreator.SuspendLayout();
            this.tabPageMetadata.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.groupBoxMetadata21.SuspendLayout();
            this.groupBoxMetadata20.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtgTimeCodelist)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.PanCodelist.SuspendLayout();
            this.tpRIS.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tpQueryGen);
            this.tabControl1.Controls.Add(this.tpRIS);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1289, 606);
            this.tabControl1.TabIndex = 2;
            // 
            // tpQueryGen
            // 
            this.tpQueryGen.Controls.Add(this.tableLayoutPanel1);
            this.tpQueryGen.Location = new System.Drawing.Point(4, 22);
            this.tpQueryGen.Name = "tpQueryGen";
            this.tpQueryGen.Padding = new System.Windows.Forms.Padding(3);
            this.tpQueryGen.Size = new System.Drawing.Size(1281, 580);
            this.tpQueryGen.TabIndex = 0;
            this.tpQueryGen.Text = "Query Creator";
            this.tpQueryGen.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 9;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.btnOpenTest, 7, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblTimeCodelist, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblCodelist, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.SDMXtree, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.btnChangeTest, 8, 0);
            this.tableLayoutPanel1.Controls.Add(this.tabControlTypeQueryCreator, 5, 1);
            this.tableLayoutPanel1.Controls.Add(this.dtgTimeCodelist, 4, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblDatasetChooses, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.PanCodelist, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.label4, 5, 0);
            this.tableLayoutPanel1.Controls.Add(this.cmbLanguages, 6, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1275, 574);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // btnOpenTest
            // 
            this.btnOpenTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenTest.Image = global::TestOnTheFlyService.Properties.Resources.add;
            this.btnOpenTest.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOpenTest.Location = new System.Drawing.Point(1055, 3);
            this.btnOpenTest.Name = "btnOpenTest";
            this.btnOpenTest.Size = new System.Drawing.Size(111, 40);
            this.btnOpenTest.TabIndex = 17;
            this.btnOpenTest.Text = "Test Query   ";
            this.btnOpenTest.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOpenTest.UseVisualStyleBackColor = true;
            this.btnOpenTest.Click += new System.EventHandler(this.btnOpenTest_Click);
            // 
            // lblTimeCodelist
            // 
            this.lblTimeCodelist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTimeCodelist.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimeCodelist.Location = new System.Drawing.Point(715, 46);
            this.lblTimeCodelist.Name = "lblTimeCodelist";
            this.lblTimeCodelist.Size = new System.Drawing.Size(200, 30);
            this.lblTimeCodelist.TabIndex = 12;
            this.lblTimeCodelist.Text = "Time Concept";
            this.lblTimeCodelist.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTimeCodelist.Visible = false;
            // 
            // lblCodelist
            // 
            this.lblCodelist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCodelist.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCodelist.Location = new System.Drawing.Point(509, 46);
            this.lblCodelist.Name = "lblCodelist";
            this.lblCodelist.Size = new System.Drawing.Size(200, 30);
            this.lblCodelist.TabIndex = 3;
            this.lblCodelist.Text = "Codelist";
            this.lblCodelist.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SDMXtree
            // 
            this.SDMXtree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SDMXtree.FullRowSelect = true;
            this.SDMXtree.ItemHeight = 22;
            this.SDMXtree.Location = new System.Drawing.Point(3, 122);
            this.SDMXtree.Name = "SDMXtree";
            this.SDMXtree.Size = new System.Drawing.Size(300, 449);
            this.SDMXtree.TabIndex = 4;
            this.SDMXtree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.SDMXtree_NodeMouseClick);
            // 
            // btnChangeTest
            // 
            this.btnChangeTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChangeTest.Image = global::TestOnTheFlyService.Properties.Resources.settings;
            this.btnChangeTest.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnChangeTest.Location = new System.Drawing.Point(1172, 3);
            this.btnChangeTest.Name = "btnChangeTest";
            this.btnChangeTest.Size = new System.Drawing.Size(100, 40);
            this.btnChangeTest.TabIndex = 7;
            this.btnChangeTest.Text = "Settings   ";
            this.btnChangeTest.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnChangeTest.UseVisualStyleBackColor = true;
            this.btnChangeTest.Click += new System.EventHandler(this.btnChangeTest_Click);
            // 
            // tabControlTypeQueryCreator
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.tabControlTypeQueryCreator, 4);
            this.tabControlTypeQueryCreator.Controls.Add(this.tabPageData);
            this.tabControlTypeQueryCreator.Controls.Add(this.tabPageMetadata);
            this.tabControlTypeQueryCreator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlTypeQueryCreator.Location = new System.Drawing.Point(921, 49);
            this.tabControlTypeQueryCreator.Name = "tabControlTypeQueryCreator";
            this.tableLayoutPanel1.SetRowSpan(this.tabControlTypeQueryCreator, 3);
            this.tabControlTypeQueryCreator.SelectedIndex = 0;
            this.tabControlTypeQueryCreator.Size = new System.Drawing.Size(351, 522);
            this.tabControlTypeQueryCreator.TabIndex = 10;
            // 
            // tabPageData
            // 
            this.tabPageData.Location = new System.Drawing.Point(4, 22);
            this.tabPageData.Name = "tabPageData";
            this.tabPageData.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageData.Size = new System.Drawing.Size(343, 496);
            this.tabPageData.TabIndex = 0;
            this.tabPageData.Text = "Data";
            this.tabPageData.UseVisualStyleBackColor = true;
            // 
            // tabPageMetadata
            // 
            this.tabPageMetadata.Controls.Add(this.tableLayoutPanel5);
            this.tabPageMetadata.Location = new System.Drawing.Point(4, 22);
            this.tabPageMetadata.Name = "tabPageMetadata";
            this.tabPageMetadata.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMetadata.Size = new System.Drawing.Size(343, 496);
            this.tabPageMetadata.TabIndex = 1;
            this.tabPageMetadata.Text = "Metadata";
            this.tabPageMetadata.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.MetadataView, 0, 2);
            this.tableLayoutPanel5.Controls.Add(this.groupBoxMetadata21, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.groupBoxMetadata20, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.btnGetMetadata, 0, 3);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 4;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.Size = new System.Drawing.Size(337, 490);
            this.tableLayoutPanel5.TabIndex = 9;
            // 
            // MetadataView
            // 
            this.MetadataView.AutoScroll = true;
            this.MetadataView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MetadataView.FileSavedPath = null;
            this.MetadataView.Location = new System.Drawing.Point(3, 420);
            this.MetadataView.Name = "MetadataView";
            this.MetadataView.Size = new System.Drawing.Size(331, 41);
            this.MetadataView.SolaLettura = false;
            this.MetadataView.TabIndex = 10;
            this.MetadataView.Testo = "";
            this.MetadataView.Visible = false;
            // 
            // groupBoxMetadata21
            // 
            this.groupBoxMetadata21.Controls.Add(this.Rdn_ConstrainedCodelist21);
            this.groupBoxMetadata21.Controls.Add(this.Rdn_Categorisation21);
            this.groupBoxMetadata21.Controls.Add(this.label3);
            this.groupBoxMetadata21.Controls.Add(this.label1);
            this.groupBoxMetadata21.Controls.Add(this.cmbReferences);
            this.groupBoxMetadata21.Controls.Add(this.cmbDetail);
            this.groupBoxMetadata21.Controls.Add(this.Rdn_AgencySchema21);
            this.groupBoxMetadata21.Controls.Add(this.Rdn_DataStructure21);
            this.groupBoxMetadata21.Controls.Add(this.Rdn_Codelist21);
            this.groupBoxMetadata21.Controls.Add(this.Rdn_ConceptsSchema21);
            this.groupBoxMetadata21.Controls.Add(this.Rdn_Dataflows21);
            this.groupBoxMetadata21.Controls.Add(this.Rdn_CategorySchema21);
            this.groupBoxMetadata21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxMetadata21.Location = new System.Drawing.Point(3, 199);
            this.groupBoxMetadata21.Name = "groupBoxMetadata21";
            this.groupBoxMetadata21.Size = new System.Drawing.Size(331, 215);
            this.groupBoxMetadata21.TabIndex = 9;
            this.groupBoxMetadata21.TabStop = false;
            this.groupBoxMetadata21.Text = "Metadata Type";
            // 
            // Rdn_ConstrainedCodelist21
            // 
            this.Rdn_ConstrainedCodelist21.AutoSize = true;
            this.Rdn_ConstrainedCodelist21.Location = new System.Drawing.Point(23, 193);
            this.Rdn_ConstrainedCodelist21.Name = "Rdn_ConstrainedCodelist21";
            this.Rdn_ConstrainedCodelist21.Size = new System.Drawing.Size(121, 17);
            this.Rdn_ConstrainedCodelist21.TabIndex = 12;
            this.Rdn_ConstrainedCodelist21.TabStop = true;
            this.Rdn_ConstrainedCodelist21.Text = "Constrained Codelist";
            this.Rdn_ConstrainedCodelist21.UseVisualStyleBackColor = true;
            // 
            // Rdn_Categorisation21
            // 
            this.Rdn_Categorisation21.AutoSize = true;
            this.Rdn_Categorisation21.Location = new System.Drawing.Point(23, 55);
            this.Rdn_Categorisation21.Name = "Rdn_Categorisation21";
            this.Rdn_Categorisation21.Size = new System.Drawing.Size(92, 17);
            this.Rdn_Categorisation21.TabIndex = 11;
            this.Rdn_Categorisation21.TabStop = true;
            this.Rdn_Categorisation21.Text = "Categorisation";
            this.Rdn_Categorisation21.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(189, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "References";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(189, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Details";
            // 
            // cmbReferences
            // 
            this.cmbReferences.FormattingEnabled = true;
            this.cmbReferences.Location = new System.Drawing.Point(192, 97);
            this.cmbReferences.Name = "cmbReferences";
            this.cmbReferences.Size = new System.Drawing.Size(121, 21);
            this.cmbReferences.TabIndex = 8;
            this.cmbReferences.TextChanged += new System.EventHandler(this.cmbReferences_TextChanged);
            // 
            // cmbDetail
            // 
            this.cmbDetail.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDetail.FormattingEnabled = true;
            this.cmbDetail.Location = new System.Drawing.Point(192, 51);
            this.cmbDetail.Name = "cmbDetail";
            this.cmbDetail.Size = new System.Drawing.Size(121, 21);
            this.cmbDetail.TabIndex = 7;
            this.cmbDetail.SelectedIndexChanged += new System.EventHandler(this.cmbDetail_SelectedIndexChanged);
            // 
            // Rdn_AgencySchema21
            // 
            this.Rdn_AgencySchema21.AutoSize = true;
            this.Rdn_AgencySchema21.Location = new System.Drawing.Point(23, 31);
            this.Rdn_AgencySchema21.Name = "Rdn_AgencySchema21";
            this.Rdn_AgencySchema21.Size = new System.Drawing.Size(123, 17);
            this.Rdn_AgencySchema21.TabIndex = 6;
            this.Rdn_AgencySchema21.TabStop = true;
            this.Rdn_AgencySchema21.Text = "OrganisationScheme";
            this.Rdn_AgencySchema21.UseVisualStyleBackColor = true;
            this.Rdn_AgencySchema21.CheckedChanged += new System.EventHandler(this.Rdn_Metadata_CheckedChanged);
            // 
            // Rdn_DataStructure21
            // 
            this.Rdn_DataStructure21.AutoSize = true;
            this.Rdn_DataStructure21.Location = new System.Drawing.Point(23, 170);
            this.Rdn_DataStructure21.Name = "Rdn_DataStructure21";
            this.Rdn_DataStructure21.Size = new System.Drawing.Size(94, 17);
            this.Rdn_DataStructure21.TabIndex = 5;
            this.Rdn_DataStructure21.TabStop = true;
            this.Rdn_DataStructure21.Text = "Data Structure";
            this.Rdn_DataStructure21.UseVisualStyleBackColor = true;
            this.Rdn_DataStructure21.CheckedChanged += new System.EventHandler(this.Rdn_Metadata_CheckedChanged);
            // 
            // Rdn_Codelist21
            // 
            this.Rdn_Codelist21.AutoSize = true;
            this.Rdn_Codelist21.Location = new System.Drawing.Point(23, 147);
            this.Rdn_Codelist21.Name = "Rdn_Codelist21";
            this.Rdn_Codelist21.Size = new System.Drawing.Size(62, 17);
            this.Rdn_Codelist21.TabIndex = 4;
            this.Rdn_Codelist21.TabStop = true;
            this.Rdn_Codelist21.Text = "Codelist";
            this.Rdn_Codelist21.UseVisualStyleBackColor = true;
            this.Rdn_Codelist21.CheckedChanged += new System.EventHandler(this.Rdn_Metadata_CheckedChanged);
            // 
            // Rdn_ConceptsSchema21
            // 
            this.Rdn_ConceptsSchema21.AutoSize = true;
            this.Rdn_ConceptsSchema21.Location = new System.Drawing.Point(23, 124);
            this.Rdn_ConceptsSchema21.Name = "Rdn_ConceptsSchema21";
            this.Rdn_ConceptsSchema21.Size = new System.Drawing.Size(109, 17);
            this.Rdn_ConceptsSchema21.TabIndex = 3;
            this.Rdn_ConceptsSchema21.TabStop = true;
            this.Rdn_ConceptsSchema21.Text = "ConceptsScheme";
            this.Rdn_ConceptsSchema21.UseVisualStyleBackColor = true;
            this.Rdn_ConceptsSchema21.CheckedChanged += new System.EventHandler(this.Rdn_Metadata_CheckedChanged);
            // 
            // Rdn_Dataflows21
            // 
            this.Rdn_Dataflows21.AutoSize = true;
            this.Rdn_Dataflows21.Location = new System.Drawing.Point(23, 101);
            this.Rdn_Dataflows21.Name = "Rdn_Dataflows21";
            this.Rdn_Dataflows21.Size = new System.Drawing.Size(72, 17);
            this.Rdn_Dataflows21.TabIndex = 2;
            this.Rdn_Dataflows21.TabStop = true;
            this.Rdn_Dataflows21.Text = "Dataflows";
            this.Rdn_Dataflows21.UseVisualStyleBackColor = true;
            this.Rdn_Dataflows21.CheckedChanged += new System.EventHandler(this.Rdn_Metadata_CheckedChanged);
            // 
            // Rdn_CategorySchema21
            // 
            this.Rdn_CategorySchema21.AutoSize = true;
            this.Rdn_CategorySchema21.Location = new System.Drawing.Point(23, 78);
            this.Rdn_CategorySchema21.Name = "Rdn_CategorySchema21";
            this.Rdn_CategorySchema21.Size = new System.Drawing.Size(106, 17);
            this.Rdn_CategorySchema21.TabIndex = 1;
            this.Rdn_CategorySchema21.TabStop = true;
            this.Rdn_CategorySchema21.Text = "CategoryScheme";
            this.Rdn_CategorySchema21.UseVisualStyleBackColor = true;
            this.Rdn_CategorySchema21.CheckedChanged += new System.EventHandler(this.Rdn_Metadata_CheckedChanged);
            // 
            // groupBoxMetadata20
            // 
            this.groupBoxMetadata20.Controls.Add(this.Rdn_ConstrainedCodelist);
            this.groupBoxMetadata20.Controls.Add(this.chkResolveReferences);
            this.groupBoxMetadata20.Controls.Add(this.Rdn_DataStructure);
            this.groupBoxMetadata20.Controls.Add(this.Rdn_Codelist);
            this.groupBoxMetadata20.Controls.Add(this.Rdn_ConceptsSchema);
            this.groupBoxMetadata20.Controls.Add(this.Rdn_Dataflows);
            this.groupBoxMetadata20.Controls.Add(this.Rdn_CategorySchema);
            this.groupBoxMetadata20.Controls.Add(this.Rdn_AgencySchema);
            this.groupBoxMetadata20.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxMetadata20.Location = new System.Drawing.Point(3, 3);
            this.groupBoxMetadata20.Name = "groupBoxMetadata20";
            this.groupBoxMetadata20.Size = new System.Drawing.Size(331, 190);
            this.groupBoxMetadata20.TabIndex = 0;
            this.groupBoxMetadata20.TabStop = false;
            this.groupBoxMetadata20.Text = "Metadata Type";
            // 
            // Rdn_ConstrainedCodelist
            // 
            this.Rdn_ConstrainedCodelist.AutoSize = true;
            this.Rdn_ConstrainedCodelist.Location = new System.Drawing.Point(23, 167);
            this.Rdn_ConstrainedCodelist.Name = "Rdn_ConstrainedCodelist";
            this.Rdn_ConstrainedCodelist.Size = new System.Drawing.Size(121, 17);
            this.Rdn_ConstrainedCodelist.TabIndex = 7;
            this.Rdn_ConstrainedCodelist.TabStop = true;
            this.Rdn_ConstrainedCodelist.Text = "Constrained Codelist";
            this.Rdn_ConstrainedCodelist.UseVisualStyleBackColor = true;
            // 
            // chkResolveReferences
            // 
            this.chkResolveReferences.AutoSize = true;
            this.chkResolveReferences.Location = new System.Drawing.Point(202, 30);
            this.chkResolveReferences.Name = "chkResolveReferences";
            this.chkResolveReferences.Size = new System.Drawing.Size(123, 17);
            this.chkResolveReferences.TabIndex = 6;
            this.chkResolveReferences.Text = "Resolve References";
            this.chkResolveReferences.UseVisualStyleBackColor = true;
            this.chkResolveReferences.CheckedChanged += new System.EventHandler(this.chkResolveReferences_CheckedChanged);
            // 
            // Rdn_DataStructure
            // 
            this.Rdn_DataStructure.AutoSize = true;
            this.Rdn_DataStructure.Location = new System.Drawing.Point(23, 144);
            this.Rdn_DataStructure.Name = "Rdn_DataStructure";
            this.Rdn_DataStructure.Size = new System.Drawing.Size(94, 17);
            this.Rdn_DataStructure.TabIndex = 5;
            this.Rdn_DataStructure.TabStop = true;
            this.Rdn_DataStructure.Text = "Data Structure";
            this.Rdn_DataStructure.UseVisualStyleBackColor = true;
            this.Rdn_DataStructure.CheckedChanged += new System.EventHandler(this.Rdn_Metadata_CheckedChanged);
            // 
            // Rdn_Codelist
            // 
            this.Rdn_Codelist.AutoSize = true;
            this.Rdn_Codelist.Location = new System.Drawing.Point(23, 121);
            this.Rdn_Codelist.Name = "Rdn_Codelist";
            this.Rdn_Codelist.Size = new System.Drawing.Size(62, 17);
            this.Rdn_Codelist.TabIndex = 4;
            this.Rdn_Codelist.TabStop = true;
            this.Rdn_Codelist.Text = "Codelist";
            this.Rdn_Codelist.UseVisualStyleBackColor = true;
            this.Rdn_Codelist.CheckedChanged += new System.EventHandler(this.Rdn_Metadata_CheckedChanged);
            // 
            // Rdn_ConceptsSchema
            // 
            this.Rdn_ConceptsSchema.AutoSize = true;
            this.Rdn_ConceptsSchema.Location = new System.Drawing.Point(23, 98);
            this.Rdn_ConceptsSchema.Name = "Rdn_ConceptsSchema";
            this.Rdn_ConceptsSchema.Size = new System.Drawing.Size(109, 17);
            this.Rdn_ConceptsSchema.TabIndex = 3;
            this.Rdn_ConceptsSchema.TabStop = true;
            this.Rdn_ConceptsSchema.Text = "ConceptsScheme";
            this.Rdn_ConceptsSchema.UseVisualStyleBackColor = true;
            this.Rdn_ConceptsSchema.CheckedChanged += new System.EventHandler(this.Rdn_Metadata_CheckedChanged);
            // 
            // Rdn_Dataflows
            // 
            this.Rdn_Dataflows.AutoSize = true;
            this.Rdn_Dataflows.Location = new System.Drawing.Point(23, 75);
            this.Rdn_Dataflows.Name = "Rdn_Dataflows";
            this.Rdn_Dataflows.Size = new System.Drawing.Size(72, 17);
            this.Rdn_Dataflows.TabIndex = 2;
            this.Rdn_Dataflows.TabStop = true;
            this.Rdn_Dataflows.Text = "Dataflows";
            this.Rdn_Dataflows.UseVisualStyleBackColor = true;
            this.Rdn_Dataflows.CheckedChanged += new System.EventHandler(this.Rdn_Metadata_CheckedChanged);
            // 
            // Rdn_CategorySchema
            // 
            this.Rdn_CategorySchema.AutoSize = true;
            this.Rdn_CategorySchema.Location = new System.Drawing.Point(23, 52);
            this.Rdn_CategorySchema.Name = "Rdn_CategorySchema";
            this.Rdn_CategorySchema.Size = new System.Drawing.Size(106, 17);
            this.Rdn_CategorySchema.TabIndex = 1;
            this.Rdn_CategorySchema.TabStop = true;
            this.Rdn_CategorySchema.Text = "CategoryScheme";
            this.Rdn_CategorySchema.UseVisualStyleBackColor = true;
            this.Rdn_CategorySchema.CheckedChanged += new System.EventHandler(this.Rdn_Metadata_CheckedChanged);
            // 
            // Rdn_AgencySchema
            // 
            this.Rdn_AgencySchema.AutoSize = true;
            this.Rdn_AgencySchema.Location = new System.Drawing.Point(23, 29);
            this.Rdn_AgencySchema.Name = "Rdn_AgencySchema";
            this.Rdn_AgencySchema.Size = new System.Drawing.Size(100, 17);
            this.Rdn_AgencySchema.TabIndex = 0;
            this.Rdn_AgencySchema.TabStop = true;
            this.Rdn_AgencySchema.Text = "AgencyScheme";
            this.Rdn_AgencySchema.UseVisualStyleBackColor = true;
            this.Rdn_AgencySchema.CheckedChanged += new System.EventHandler(this.Rdn_Metadata_CheckedChanged);
            // 
            // btnGetMetadata
            // 
            this.btnGetMetadata.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGetMetadata.Location = new System.Drawing.Point(234, 467);
            this.btnGetMetadata.Name = "btnGetMetadata";
            this.btnGetMetadata.Size = new System.Drawing.Size(100, 20);
            this.btnGetMetadata.TabIndex = 8;
            this.btnGetMetadata.Text = "Get Metadata";
            this.btnGetMetadata.UseVisualStyleBackColor = true;
            this.btnGetMetadata.Click += new System.EventHandler(this.btnGetMetadata_Click);
            // 
            // dtgTimeCodelist
            // 
            this.dtgTimeCodelist.AllowUserToAddRows = false;
            this.dtgTimeCodelist.AllowUserToDeleteRows = false;
            this.dtgTimeCodelist.AllowUserToResizeColumns = false;
            this.dtgTimeCodelist.AllowUserToResizeRows = false;
            this.dtgTimeCodelist.BackgroundColor = System.Drawing.Color.White;
            this.dtgTimeCodelist.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dtgTimeCodelist.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtgTimeCodelist.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColStart,
            this.ColEnd});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dtgTimeCodelist.DefaultCellStyle = dataGridViewCellStyle1;
            this.dtgTimeCodelist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtgTimeCodelist.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dtgTimeCodelist.Location = new System.Drawing.Point(715, 80);
            this.dtgTimeCodelist.MultiSelect = false;
            this.dtgTimeCodelist.Name = "dtgTimeCodelist";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtgTimeCodelist.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dtgTimeCodelist.RowHeadersWidth = 15;
            this.dtgTimeCodelist.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.tableLayoutPanel1.SetRowSpan(this.dtgTimeCodelist, 2);
            this.dtgTimeCodelist.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dtgTimeCodelist.Size = new System.Drawing.Size(200, 491);
            this.dtgTimeCodelist.TabIndex = 11;
            this.dtgTimeCodelist.Visible = false;
            this.dtgTimeCodelist.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtgTimeCodelist_CellEndEdit);
            // 
            // ColStart
            // 
            this.ColStart.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColStart.DataPropertyName = "StartTime";
            this.ColStart.HeaderText = "Start Date";
            this.ColStart.Name = "ColStart";
            // 
            // ColEnd
            // 
            this.ColEnd.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColEnd.DataPropertyName = "EndTime";
            this.ColEnd.HeaderText = "End Date";
            this.ColEnd.Name = "ColEnd";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.LstConcept, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.lblAttribute, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.LstAttributes, 0, 3);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(306, 46);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 4;
            this.tableLayoutPanel1.SetRowSpan(this.tableLayoutPanel3, 3);
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(200, 528);
            this.tableLayoutPanel3.TabIndex = 15;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(194, 30);
            this.label2.TabIndex = 2;
            this.label2.Text = "Dimensions";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LstConcept
            // 
            this.LstConcept.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LstConcept.FormattingEnabled = true;
            this.LstConcept.Location = new System.Drawing.Point(3, 33);
            this.LstConcept.Name = "LstConcept";
            this.LstConcept.Size = new System.Drawing.Size(194, 266);
            this.LstConcept.TabIndex = 5;
            this.LstConcept.SelectedIndexChanged += new System.EventHandler(this.LstConcept_SelectedIndexChanged);
            // 
            // lblAttribute
            // 
            this.lblAttribute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAttribute.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAttribute.Location = new System.Drawing.Point(3, 302);
            this.lblAttribute.Name = "lblAttribute";
            this.lblAttribute.Size = new System.Drawing.Size(194, 20);
            this.lblAttribute.TabIndex = 14;
            this.lblAttribute.Text = "Attributes";
            this.lblAttribute.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LstAttributes
            // 
            this.LstAttributes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LstAttributes.FormattingEnabled = true;
            this.LstAttributes.Location = new System.Drawing.Point(3, 325);
            this.LstAttributes.Name = "LstAttributes";
            this.LstAttributes.Size = new System.Drawing.Size(194, 200);
            this.LstAttributes.TabIndex = 13;
            this.LstAttributes.SelectedIndexChanged += new System.EventHandler(this.LstAttributes_SelectedIndexChanged);
            // 
            // lblDatasetChooses
            // 
            this.lblDatasetChooses.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDatasetChooses.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lblDatasetChooses, 3);
            this.lblDatasetChooses.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDatasetChooses.Location = new System.Drawing.Point(309, 16);
            this.lblDatasetChooses.MaximumSize = new System.Drawing.Size(400, 13);
            this.lblDatasetChooses.Name = "lblDatasetChooses";
            this.lblDatasetChooses.Size = new System.Drawing.Size(400, 13);
            this.lblDatasetChooses.TabIndex = 8;
            this.lblDatasetChooses.Text = "[Dataset Choose]";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel4);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.tableLayoutPanel1.SetRowSpan(this.panel1, 3);
            this.panel1.Size = new System.Drawing.Size(300, 113);
            this.panel1.TabIndex = 16;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 3;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.groupBox2, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.BtnGo, 2, 0);
            this.tableLayoutPanel4.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 4;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(300, 113);
            this.tableLayoutPanel4.TabIndex = 2;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.BtnSdmx20);
            this.groupBox2.Controls.Add(this.BtnSdmx21);
            this.groupBox2.Location = new System.Drawing.Point(3, 59);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(187, 50);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Version";
            // 
            // BtnSdmx20
            // 
            this.BtnSdmx20.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnSdmx20.FlatAppearance.BorderSize = 0;
            this.BtnSdmx20.Location = new System.Drawing.Point(12, 15);
            this.BtnSdmx20.Margin = new System.Windows.Forms.Padding(0);
            this.BtnSdmx20.Name = "BtnSdmx20";
            this.BtnSdmx20.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.BtnSdmx20.Size = new System.Drawing.Size(89, 32);
            this.BtnSdmx20.TabIndex = 1;
            this.BtnSdmx20.Text = "Sdmx 2.0";
            this.BtnSdmx20.UseVisualStyleBackColor = true;
            this.BtnSdmx20.Click += new System.EventHandler(this.BtnSdmx20_Click);
            // 
            // BtnSdmx21
            // 
            this.BtnSdmx21.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnSdmx21.FlatAppearance.BorderSize = 0;
            this.BtnSdmx21.Location = new System.Drawing.Point(110, 15);
            this.BtnSdmx21.Margin = new System.Windows.Forms.Padding(0);
            this.BtnSdmx21.Name = "BtnSdmx21";
            this.BtnSdmx21.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.BtnSdmx21.Size = new System.Drawing.Size(75, 32);
            this.BtnSdmx21.TabIndex = 3;
            this.BtnSdmx21.Text = "Sdmx 2.1";
            this.BtnSdmx21.UseVisualStyleBackColor = true;
            this.BtnSdmx21.Click += new System.EventHandler(this.BtnSdmx21_Click);
            // 
            // BtnGo
            // 
            this.BtnGo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.BtnGo.Image = global::TestOnTheFlyService.Properties.Resources.treeview;
            this.BtnGo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BtnGo.Location = new System.Drawing.Point(196, 36);
            this.BtnGo.Name = "BtnGo";
            this.tableLayoutPanel4.SetRowSpan(this.BtnGo, 2);
            this.BtnGo.Size = new System.Drawing.Size(100, 40);
            this.BtnGo.TabIndex = 2;
            this.BtnGo.Text = "Get Tree  ";
            this.BtnGo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BtnGo.UseVisualStyleBackColor = true;
            this.BtnGo.Click += new System.EventHandler(this.BtnGo_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.BtnRest);
            this.groupBox1.Controls.Add(this.BtnSoap);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(187, 50);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Comunication Type";
            // 
            // BtnRest
            // 
            this.BtnRest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnRest.FlatAppearance.BorderSize = 0;
            this.BtnRest.Location = new System.Drawing.Point(110, 15);
            this.BtnRest.Margin = new System.Windows.Forms.Padding(0);
            this.BtnRest.Name = "BtnRest";
            this.BtnRest.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.BtnRest.Size = new System.Drawing.Size(75, 32);
            this.BtnRest.TabIndex = 5;
            this.BtnRest.Text = "REST";
            this.BtnRest.UseVisualStyleBackColor = true;
            this.BtnRest.Click += new System.EventHandler(this.BtnRest_Click);
            // 
            // BtnSoap
            // 
            this.BtnSoap.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnSoap.FlatAppearance.BorderSize = 0;
            this.BtnSoap.Location = new System.Drawing.Point(12, 15);
            this.BtnSoap.Margin = new System.Windows.Forms.Padding(0);
            this.BtnSoap.Name = "BtnSoap";
            this.BtnSoap.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.BtnSoap.Size = new System.Drawing.Size(89, 32);
            this.BtnSoap.TabIndex = 4;
            this.BtnSoap.Text = "SOAP";
            this.BtnSoap.UseVisualStyleBackColor = true;
            this.BtnSoap.Click += new System.EventHandler(this.BtnSoap_Click);
            // 
            // PanCodelist
            // 
            this.PanCodelist.Controls.Add(this.LstCodelist);
            this.PanCodelist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PanCodelist.Location = new System.Drawing.Point(509, 80);
            this.PanCodelist.Name = "PanCodelist";
            this.tableLayoutPanel1.SetRowSpan(this.PanCodelist, 2);
            this.PanCodelist.Size = new System.Drawing.Size(200, 491);
            this.PanCodelist.TabIndex = 18;
            // 
            // LstCodelist
            // 
            this.LstCodelist.CheckBoxes = true;
            this.LstCodelist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LstCodelist.HideSelection = false;
            this.LstCodelist.ItemHeight = 22;
            this.LstCodelist.Location = new System.Drawing.Point(0, 0);
            this.LstCodelist.Name = "LstCodelist";
            this.LstCodelist.Size = new System.Drawing.Size(200, 491);
            this.LstCodelist.TabIndex = 6;
            this.LstCodelist.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.LstCodelist_AfterCheck);
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(921, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Lang.";
            // 
            // cmbLanguages
            // 
            this.cmbLanguages.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cmbLanguages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLanguages.FormattingEnabled = true;
            this.cmbLanguages.Items.AddRange(new object[] {
            "En",
            "Fr",
            "It"});
            this.cmbLanguages.Location = new System.Drawing.Point(961, 12);
            this.cmbLanguages.Name = "cmbLanguages";
            this.cmbLanguages.Size = new System.Drawing.Size(88, 21);
            this.cmbLanguages.TabIndex = 19;
            this.cmbLanguages.SelectedIndexChanged += new System.EventHandler(this.cmbLanguages_SelectedIndexChanged);
            // 
            // tpRIS
            // 
            this.tpRIS.Controls.Add(this.tableLayoutPanel2);
            this.tpRIS.Location = new System.Drawing.Point(4, 22);
            this.tpRIS.Name = "tpRIS";
            this.tpRIS.Padding = new System.Windows.Forms.Padding(3);
            this.tpRIS.Size = new System.Drawing.Size(1281, 580);
            this.tpRIS.TabIndex = 1;
            this.tpRIS.Text = "Results";
            this.tpRIS.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.btnOpenFile, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.BtnOpenFolder, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.txtResult, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1275, 574);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnOpenFile.Image = global::TestOnTheFlyService.Properties.Resources.Print;
            this.btnOpenFile.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOpenFile.Location = new System.Drawing.Point(1056, 531);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(100, 40);
            this.btnOpenFile.TabIndex = 4;
            this.btnOpenFile.Text = "Open File  ";
            this.btnOpenFile.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // BtnOpenFolder
            // 
            this.BtnOpenFolder.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.BtnOpenFolder.Image = global::TestOnTheFlyService.Properties.Resources.DownloadIcon;
            this.BtnOpenFolder.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BtnOpenFolder.Location = new System.Drawing.Point(1162, 531);
            this.BtnOpenFolder.Name = "BtnOpenFolder";
            this.BtnOpenFolder.Size = new System.Drawing.Size(110, 40);
            this.BtnOpenFolder.TabIndex = 3;
            this.BtnOpenFolder.Text = "Download   ";
            this.BtnOpenFolder.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BtnOpenFolder.UseVisualStyleBackColor = true;
            this.BtnOpenFolder.Click += new System.EventHandler(this.BtnOpenFolder_Click);
            // 
            // txtResult
            // 
            this.txtResult.AutoScroll = true;
            this.tableLayoutPanel2.SetColumnSpan(this.txtResult, 2);
            this.txtResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResult.FileSavedPath = null;
            this.txtResult.Location = new System.Drawing.Point(3, 3);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(1269, 522);
            this.txtResult.SolaLettura = false;
            this.txtResult.TabIndex = 5;
            this.txtResult.Testo = "";
            // 
            // TestFlyQueryCreation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1289, 606);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TestFlyQueryCreation";
            this.Text = "Query Creator And Test OnTheFly WebService";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tabControl1.ResumeLayout(false);
            this.tpQueryGen.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tabControlTypeQueryCreator.ResumeLayout(false);
            this.tabPageMetadata.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.groupBoxMetadata21.ResumeLayout(false);
            this.groupBoxMetadata21.PerformLayout();
            this.groupBoxMetadata20.ResumeLayout(false);
            this.groupBoxMetadata20.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtgTimeCodelist)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.PanCodelist.ResumeLayout(false);
            this.tpRIS.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

       

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpQueryGen;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
       
        private System.Windows.Forms.Label lblCodelist;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TreeView SDMXtree;
        private System.Windows.Forms.ListBox LstConcept;
        private System.Windows.Forms.TabPage tpRIS;
        private System.Windows.Forms.Button btnChangeTest;
        private System.Windows.Forms.Label lblDatasetChooses;
        private System.Windows.Forms.TabControl tabControlTypeQueryCreator;
        private System.Windows.Forms.TabPage tabPageData;
        private System.Windows.Forms.TabPage tabPageMetadata;
        private System.Windows.Forms.Button btnGetMetadata;
        private System.Windows.Forms.GroupBox groupBoxMetadata20;
        private System.Windows.Forms.RadioButton Rdn_DataStructure;
        private System.Windows.Forms.RadioButton Rdn_Codelist;
        private System.Windows.Forms.RadioButton Rdn_ConceptsSchema;
        private System.Windows.Forms.RadioButton Rdn_Dataflows;
        private System.Windows.Forms.RadioButton Rdn_CategorySchema;
        private System.Windows.Forms.RadioButton Rdn_AgencySchema;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.DataGridView dtgTimeCodelist;
        private System.Windows.Forms.Label lblTimeCodelist;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColStart;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColEnd;
        private System.Windows.Forms.ListBox LstAttributes;
        private System.Windows.Forms.Label lblAttribute;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton BtnSdmx21;
        private System.Windows.Forms.Button BtnGo;
        private System.Windows.Forms.RadioButton BtnSdmx20;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.GroupBox groupBoxMetadata21;
        private System.Windows.Forms.RadioButton Rdn_DataStructure21;
        private System.Windows.Forms.RadioButton Rdn_Codelist21;
        private System.Windows.Forms.RadioButton Rdn_ConceptsSchema21;
        private System.Windows.Forms.RadioButton Rdn_Dataflows21;
        private System.Windows.Forms.RadioButton Rdn_CategorySchema21;
        private System.Windows.Forms.RadioButton Rdn_AgencySchema21;

        private TreeNoDBClick LstCodelist;
        private Panel PanCodelist;
        private WinXmlViewer txtResult;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.Button BtnOpenFolder;
        private System.Windows.Forms.RadioButton BtnRest;
        private System.Windows.Forms.RadioButton BtnSoap;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Button btnOpenTest;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbReferences;
        private System.Windows.Forms.ComboBox cmbDetail;
        private System.Windows.Forms.CheckBox chkResolveReferences;
        private WinXmlViewer MetadataView;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton Rdn_Categorisation21;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbLanguages;
        private System.Windows.Forms.RadioButton Rdn_ConstrainedCodelist21;
        private System.Windows.Forms.RadioButton Rdn_ConstrainedCodelist;

    }
}