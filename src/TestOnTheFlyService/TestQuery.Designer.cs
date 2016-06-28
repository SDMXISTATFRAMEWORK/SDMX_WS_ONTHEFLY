namespace TestOnTheFlyService
{
    partial class TestQuery
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestQuery));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cmbEndPoint = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnOpenIE = new System.Windows.Forms.Button();
            this.txtQuery = new TestOnTheFlyService.WinXmlViewer();
            this.txtRes = new TestOnTheFlyService.WinXmlViewer();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.grREST21 = new System.Windows.Forms.GroupBox();
            this.rdn_StructureSpecific_REST21 = new System.Windows.Forms.RadioButton();
            this.rdn_GenericData_REST21 = new System.Windows.Forms.RadioButton();
            this.Rdn_DataStructure_REST21 = new System.Windows.Forms.RadioButton();
            this.grREST20 = new System.Windows.Forms.GroupBox();
            this.rdn_CompactData_REST20 = new System.Windows.Forms.RadioButton();
            this.rdn_GenericData_REST20 = new System.Windows.Forms.RadioButton();
            this.Rdn_DataStructure_REST20 = new System.Windows.Forms.RadioButton();
            this.grSOAP21 = new System.Windows.Forms.GroupBox();
            this.Rdn_Categorisation21 = new System.Windows.Forms.RadioButton();
            this.rdn_StructureSpecificData21 = new System.Windows.Forms.RadioButton();
            this.rdn_GenericData21 = new System.Windows.Forms.RadioButton();
            this.Rdn_AgencySchema21 = new System.Windows.Forms.RadioButton();
            this.Rdn_DataStructure21 = new System.Windows.Forms.RadioButton();
            this.Rdn_Codelist21 = new System.Windows.Forms.RadioButton();
            this.Rdn_ConceptsSchema21 = new System.Windows.Forms.RadioButton();
            this.Rdn_Dataflows21 = new System.Windows.Forms.RadioButton();
            this.Rdn_CategorySchema21 = new System.Windows.Forms.RadioButton();
            this.grSOAP20 = new System.Windows.Forms.GroupBox();
            this.Rdn_ConstrainedCodelist = new System.Windows.Forms.RadioButton();
            this.rdn_CompactData = new System.Windows.Forms.RadioButton();
            this.rdn_GenericData = new System.Windows.Forms.RadioButton();
            this.Rdn_DataStructure = new System.Windows.Forms.RadioButton();
            this.Rdn_Codelist = new System.Windows.Forms.RadioButton();
            this.Rdn_ConceptsSchema = new System.Windows.Forms.RadioButton();
            this.Rdn_Dataflows = new System.Windows.Forms.RadioButton();
            this.Rdn_CategorySchema = new System.Windows.Forms.RadioButton();
            this.Rdn_AgencySchema = new System.Windows.Forms.RadioButton();
            this.txtHeader = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.grREST21.SuspendLayout();
            this.grREST20.SuspendLayout();
            this.grSOAP21.SuspendLayout();
            this.grSOAP20.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.cmbEndPoint, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.button1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnOpenIE, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtQuery, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtRes, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtHeader, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1165, 733);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // cmbEndPoint
            // 
            this.cmbEndPoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbEndPoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEndPoint.FormattingEnabled = true;
            this.cmbEndPoint.Location = new System.Drawing.Point(3, 5);
            this.cmbEndPoint.Name = "cmbEndPoint";
            this.cmbEndPoint.Size = new System.Drawing.Size(200, 21);
            this.cmbEndPoint.TabIndex = 0;
            this.cmbEndPoint.SelectedIndexChanged += new System.EventHandler(this.cmbEndPoint_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.button1.Location = new System.Drawing.Point(569, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(113, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Send Message";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnOpenIE
            // 
            this.btnOpenIE.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenIE.AutoSize = true;
            this.btnOpenIE.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOpenIE.FlatAppearance.BorderSize = 0;
            this.btnOpenIE.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenIE.Location = new System.Drawing.Point(688, 3);
            this.btnOpenIE.Name = "btnOpenIE";
            this.btnOpenIE.Size = new System.Drawing.Size(474, 25);
            this.btnOpenIE.TabIndex = 2;
            this.btnOpenIE.Text = "Result";
            this.btnOpenIE.Click += new System.EventHandler(this.btnOpenIE_Click);
            // 
            // txtQuery
            // 
            this.txtQuery.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtQuery.FileSavedPath = null;
            this.txtQuery.Location = new System.Drawing.Point(209, 60);
            this.txtQuery.Name = "txtQuery";
            this.txtQuery.Size = new System.Drawing.Size(473, 670);
            this.txtQuery.SolaLettura = false;
            this.txtQuery.TabIndex = 3;
            this.txtQuery.Testo = "";
            // 
            // txtRes
            // 
            this.txtRes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRes.FileSavedPath = null;
            this.txtRes.Location = new System.Drawing.Point(688, 34);
            this.txtRes.Name = "txtRes";
            this.tableLayoutPanel1.SetRowSpan(this.txtRes, 2);
            this.txtRes.Size = new System.Drawing.Size(474, 696);
            this.txtRes.SolaLettura = false;
            this.txtRes.TabIndex = 4;
            this.txtRes.Testo = "";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.grREST21, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.grREST20, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.grSOAP21, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.grSOAP20, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 34);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel1.SetRowSpan(this.tableLayoutPanel2, 2);
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(200, 696);
            this.tableLayoutPanel2.TabIndex = 5;
            // 
            // grREST21
            // 
            this.grREST21.Controls.Add(this.rdn_StructureSpecific_REST21);
            this.grREST21.Controls.Add(this.rdn_GenericData_REST21);
            this.grREST21.Controls.Add(this.Rdn_DataStructure_REST21);
            this.grREST21.Location = new System.Drawing.Point(3, 618);
            this.grREST21.Name = "grREST21";
            this.grREST21.Size = new System.Drawing.Size(194, 108);
            this.grREST21.TabIndex = 3;
            this.grREST21.TabStop = false;
            this.grREST21.Text = "Rest Sdmx 2.1";
            // 
            // rdn_StructureSpecific_REST21
            // 
            this.rdn_StructureSpecific_REST21.AutoSize = true;
            this.rdn_StructureSpecific_REST21.Location = new System.Drawing.Point(15, 73);
            this.rdn_StructureSpecific_REST21.Name = "rdn_StructureSpecific_REST21";
            this.rdn_StructureSpecific_REST21.Size = new System.Drawing.Size(132, 17);
            this.rdn_StructureSpecific_REST21.TabIndex = 18;
            this.rdn_StructureSpecific_REST21.TabStop = true;
            this.rdn_StructureSpecific_REST21.Text = "StructureSpecific Data";
            this.rdn_StructureSpecific_REST21.UseVisualStyleBackColor = true;
            this.rdn_StructureSpecific_REST21.CheckedChanged += new System.EventHandler(this.Rdn_CheckedChanged);
            // 
            // rdn_GenericData_REST21
            // 
            this.rdn_GenericData_REST21.AutoSize = true;
            this.rdn_GenericData_REST21.Location = new System.Drawing.Point(15, 50);
            this.rdn_GenericData_REST21.Name = "rdn_GenericData_REST21";
            this.rdn_GenericData_REST21.Size = new System.Drawing.Size(88, 17);
            this.rdn_GenericData_REST21.TabIndex = 17;
            this.rdn_GenericData_REST21.TabStop = true;
            this.rdn_GenericData_REST21.Text = "Generic Data";
            this.rdn_GenericData_REST21.UseVisualStyleBackColor = true;
            this.rdn_GenericData_REST21.CheckedChanged += new System.EventHandler(this.Rdn_CheckedChanged);
            // 
            // Rdn_DataStructure_REST21
            // 
            this.Rdn_DataStructure_REST21.AutoSize = true;
            this.Rdn_DataStructure_REST21.Location = new System.Drawing.Point(15, 19);
            this.Rdn_DataStructure_REST21.Name = "Rdn_DataStructure_REST21";
            this.Rdn_DataStructure_REST21.Size = new System.Drawing.Size(94, 17);
            this.Rdn_DataStructure_REST21.TabIndex = 16;
            this.Rdn_DataStructure_REST21.TabStop = true;
            this.Rdn_DataStructure_REST21.Text = "Data Structure";
            this.Rdn_DataStructure_REST21.UseVisualStyleBackColor = true;
            this.Rdn_DataStructure_REST21.CheckedChanged += new System.EventHandler(this.Rdn_CheckedChanged);
            // 
            // grREST20
            // 
            this.grREST20.Controls.Add(this.rdn_CompactData_REST20);
            this.grREST20.Controls.Add(this.rdn_GenericData_REST20);
            this.grREST20.Controls.Add(this.Rdn_DataStructure_REST20);
            this.grREST20.Location = new System.Drawing.Point(3, 509);
            this.grREST20.Name = "grREST20";
            this.grREST20.Size = new System.Drawing.Size(194, 103);
            this.grREST20.TabIndex = 2;
            this.grREST20.TabStop = false;
            this.grREST20.Text = "Rest Sdmx 2.0";
            // 
            // rdn_CompactData_REST20
            // 
            this.rdn_CompactData_REST20.AutoSize = true;
            this.rdn_CompactData_REST20.Location = new System.Drawing.Point(14, 73);
            this.rdn_CompactData_REST20.Name = "rdn_CompactData_REST20";
            this.rdn_CompactData_REST20.Size = new System.Drawing.Size(93, 17);
            this.rdn_CompactData_REST20.TabIndex = 15;
            this.rdn_CompactData_REST20.TabStop = true;
            this.rdn_CompactData_REST20.Text = "Compact Data";
            this.rdn_CompactData_REST20.UseVisualStyleBackColor = true;
            this.rdn_CompactData_REST20.CheckedChanged += new System.EventHandler(this.Rdn_CheckedChanged);
            // 
            // rdn_GenericData_REST20
            // 
            this.rdn_GenericData_REST20.AutoSize = true;
            this.rdn_GenericData_REST20.Location = new System.Drawing.Point(14, 50);
            this.rdn_GenericData_REST20.Name = "rdn_GenericData_REST20";
            this.rdn_GenericData_REST20.Size = new System.Drawing.Size(88, 17);
            this.rdn_GenericData_REST20.TabIndex = 14;
            this.rdn_GenericData_REST20.TabStop = true;
            this.rdn_GenericData_REST20.Text = "Generic Data";
            this.rdn_GenericData_REST20.UseVisualStyleBackColor = true;
            this.rdn_GenericData_REST20.CheckedChanged += new System.EventHandler(this.Rdn_CheckedChanged);
            // 
            // Rdn_DataStructure_REST20
            // 
            this.Rdn_DataStructure_REST20.AutoSize = true;
            this.Rdn_DataStructure_REST20.Location = new System.Drawing.Point(14, 19);
            this.Rdn_DataStructure_REST20.Name = "Rdn_DataStructure_REST20";
            this.Rdn_DataStructure_REST20.Size = new System.Drawing.Size(94, 17);
            this.Rdn_DataStructure_REST20.TabIndex = 12;
            this.Rdn_DataStructure_REST20.TabStop = true;
            this.Rdn_DataStructure_REST20.Text = "Data Structure";
            this.Rdn_DataStructure_REST20.UseVisualStyleBackColor = true;
            this.Rdn_DataStructure_REST20.CheckedChanged += new System.EventHandler(this.Rdn_CheckedChanged);
            // 
            // grSOAP21
            // 
            this.grSOAP21.Controls.Add(this.Rdn_Categorisation21);
            this.grSOAP21.Controls.Add(this.rdn_StructureSpecificData21);
            this.grSOAP21.Controls.Add(this.rdn_GenericData21);
            this.grSOAP21.Controls.Add(this.Rdn_AgencySchema21);
            this.grSOAP21.Controls.Add(this.Rdn_DataStructure21);
            this.grSOAP21.Controls.Add(this.Rdn_Codelist21);
            this.grSOAP21.Controls.Add(this.Rdn_ConceptsSchema21);
            this.grSOAP21.Controls.Add(this.Rdn_Dataflows21);
            this.grSOAP21.Controls.Add(this.Rdn_CategorySchema21);
            this.grSOAP21.Location = new System.Drawing.Point(3, 249);
            this.grSOAP21.Name = "grSOAP21";
            this.grSOAP21.Size = new System.Drawing.Size(194, 254);
            this.grSOAP21.TabIndex = 1;
            this.grSOAP21.TabStop = false;
            this.grSOAP21.Text = "Soap Sdmx 2.1";
            // 
            // Rdn_Categorisation21
            // 
            this.Rdn_Categorisation21.AutoSize = true;
            this.Rdn_Categorisation21.Location = new System.Drawing.Point(14, 51);
            this.Rdn_Categorisation21.Name = "Rdn_Categorisation21";
            this.Rdn_Categorisation21.Size = new System.Drawing.Size(92, 17);
            this.Rdn_Categorisation21.TabIndex = 16;
            this.Rdn_Categorisation21.TabStop = true;
            this.Rdn_Categorisation21.Text = "Categorisation";
            this.Rdn_Categorisation21.UseVisualStyleBackColor = true;
            this.Rdn_Categorisation21.CheckedChanged += new System.EventHandler(this.Rdn_CheckedChanged);
            // 
            // rdn_StructureSpecificData21
            // 
            this.rdn_StructureSpecificData21.AutoSize = true;
            this.rdn_StructureSpecificData21.Location = new System.Drawing.Point(15, 229);
            this.rdn_StructureSpecificData21.Name = "rdn_StructureSpecificData21";
            this.rdn_StructureSpecificData21.Size = new System.Drawing.Size(132, 17);
            this.rdn_StructureSpecificData21.TabIndex = 15;
            this.rdn_StructureSpecificData21.TabStop = true;
            this.rdn_StructureSpecificData21.Text = "StructureSpecific Data";
            this.rdn_StructureSpecificData21.UseVisualStyleBackColor = true;
            this.rdn_StructureSpecificData21.CheckedChanged += new System.EventHandler(this.Rdn_CheckedChanged);
            // 
            // rdn_GenericData21
            // 
            this.rdn_GenericData21.AutoSize = true;
            this.rdn_GenericData21.Location = new System.Drawing.Point(15, 206);
            this.rdn_GenericData21.Name = "rdn_GenericData21";
            this.rdn_GenericData21.Size = new System.Drawing.Size(88, 17);
            this.rdn_GenericData21.TabIndex = 14;
            this.rdn_GenericData21.TabStop = true;
            this.rdn_GenericData21.Text = "Generic Data";
            this.rdn_GenericData21.UseVisualStyleBackColor = true;
            this.rdn_GenericData21.CheckedChanged += new System.EventHandler(this.Rdn_CheckedChanged);
            // 
            // Rdn_AgencySchema21
            // 
            this.Rdn_AgencySchema21.AutoSize = true;
            this.Rdn_AgencySchema21.Location = new System.Drawing.Point(15, 28);
            this.Rdn_AgencySchema21.Name = "Rdn_AgencySchema21";
            this.Rdn_AgencySchema21.Size = new System.Drawing.Size(123, 17);
            this.Rdn_AgencySchema21.TabIndex = 12;
            this.Rdn_AgencySchema21.TabStop = true;
            this.Rdn_AgencySchema21.Text = "OrganisationScheme";
            this.Rdn_AgencySchema21.UseVisualStyleBackColor = true;
            this.Rdn_AgencySchema21.CheckedChanged += new System.EventHandler(this.Rdn_CheckedChanged);
            // 
            // Rdn_DataStructure21
            // 
            this.Rdn_DataStructure21.AutoSize = true;
            this.Rdn_DataStructure21.Location = new System.Drawing.Point(14, 168);
            this.Rdn_DataStructure21.Name = "Rdn_DataStructure21";
            this.Rdn_DataStructure21.Size = new System.Drawing.Size(94, 17);
            this.Rdn_DataStructure21.TabIndex = 11;
            this.Rdn_DataStructure21.TabStop = true;
            this.Rdn_DataStructure21.Text = "Data Structure";
            this.Rdn_DataStructure21.UseVisualStyleBackColor = true;
            this.Rdn_DataStructure21.CheckedChanged += new System.EventHandler(this.Rdn_CheckedChanged);
            // 
            // Rdn_Codelist21
            // 
            this.Rdn_Codelist21.AutoSize = true;
            this.Rdn_Codelist21.Location = new System.Drawing.Point(15, 143);
            this.Rdn_Codelist21.Name = "Rdn_Codelist21";
            this.Rdn_Codelist21.Size = new System.Drawing.Size(62, 17);
            this.Rdn_Codelist21.TabIndex = 10;
            this.Rdn_Codelist21.TabStop = true;
            this.Rdn_Codelist21.Text = "Codelist";
            this.Rdn_Codelist21.UseVisualStyleBackColor = true;
            this.Rdn_Codelist21.CheckedChanged += new System.EventHandler(this.Rdn_CheckedChanged);
            // 
            // Rdn_ConceptsSchema21
            // 
            this.Rdn_ConceptsSchema21.AutoSize = true;
            this.Rdn_ConceptsSchema21.Location = new System.Drawing.Point(15, 120);
            this.Rdn_ConceptsSchema21.Name = "Rdn_ConceptsSchema21";
            this.Rdn_ConceptsSchema21.Size = new System.Drawing.Size(109, 17);
            this.Rdn_ConceptsSchema21.TabIndex = 9;
            this.Rdn_ConceptsSchema21.TabStop = true;
            this.Rdn_ConceptsSchema21.Text = "ConceptsScheme";
            this.Rdn_ConceptsSchema21.UseVisualStyleBackColor = true;
            this.Rdn_ConceptsSchema21.CheckedChanged += new System.EventHandler(this.Rdn_CheckedChanged);
            // 
            // Rdn_Dataflows21
            // 
            this.Rdn_Dataflows21.AutoSize = true;
            this.Rdn_Dataflows21.Location = new System.Drawing.Point(15, 97);
            this.Rdn_Dataflows21.Name = "Rdn_Dataflows21";
            this.Rdn_Dataflows21.Size = new System.Drawing.Size(72, 17);
            this.Rdn_Dataflows21.TabIndex = 8;
            this.Rdn_Dataflows21.TabStop = true;
            this.Rdn_Dataflows21.Text = "Dataflows";
            this.Rdn_Dataflows21.UseVisualStyleBackColor = true;
            this.Rdn_Dataflows21.CheckedChanged += new System.EventHandler(this.Rdn_CheckedChanged);
            // 
            // Rdn_CategorySchema21
            // 
            this.Rdn_CategorySchema21.AutoSize = true;
            this.Rdn_CategorySchema21.Location = new System.Drawing.Point(15, 74);
            this.Rdn_CategorySchema21.Name = "Rdn_CategorySchema21";
            this.Rdn_CategorySchema21.Size = new System.Drawing.Size(106, 17);
            this.Rdn_CategorySchema21.TabIndex = 7;
            this.Rdn_CategorySchema21.TabStop = true;
            this.Rdn_CategorySchema21.Text = "CategoryScheme";
            this.Rdn_CategorySchema21.UseVisualStyleBackColor = true;
            this.Rdn_CategorySchema21.CheckedChanged += new System.EventHandler(this.Rdn_CheckedChanged);
            // 
            // grSOAP20
            // 
            this.grSOAP20.Controls.Add(this.Rdn_ConstrainedCodelist);
            this.grSOAP20.Controls.Add(this.rdn_CompactData);
            this.grSOAP20.Controls.Add(this.rdn_GenericData);
            this.grSOAP20.Controls.Add(this.Rdn_DataStructure);
            this.grSOAP20.Controls.Add(this.Rdn_Codelist);
            this.grSOAP20.Controls.Add(this.Rdn_ConceptsSchema);
            this.grSOAP20.Controls.Add(this.Rdn_Dataflows);
            this.grSOAP20.Controls.Add(this.Rdn_CategorySchema);
            this.grSOAP20.Controls.Add(this.Rdn_AgencySchema);
            this.grSOAP20.Location = new System.Drawing.Point(3, 3);
            this.grSOAP20.Name = "grSOAP20";
            this.grSOAP20.Size = new System.Drawing.Size(194, 240);
            this.grSOAP20.TabIndex = 0;
            this.grSOAP20.TabStop = false;
            this.grSOAP20.Text = "Soap Sdmx 2.0";
            // 
            // Rdn_ConstrainedCodelist
            // 
            this.Rdn_ConstrainedCodelist.AutoSize = true;
            this.Rdn_ConstrainedCodelist.Location = new System.Drawing.Point(15, 160);
            this.Rdn_ConstrainedCodelist.Name = "Rdn_ConstrainedCodelist";
            this.Rdn_ConstrainedCodelist.Size = new System.Drawing.Size(121, 17);
            this.Rdn_ConstrainedCodelist.TabIndex = 14;
            this.Rdn_ConstrainedCodelist.TabStop = true;
            this.Rdn_ConstrainedCodelist.Text = "Constrained Codelist";
            this.Rdn_ConstrainedCodelist.UseVisualStyleBackColor = true;
            this.Rdn_ConstrainedCodelist.CheckedChanged += new System.EventHandler(this.Rdn_CheckedChanged);
            // 
            // rdn_CompactData
            // 
            this.rdn_CompactData.AutoSize = true;
            this.rdn_CompactData.Location = new System.Drawing.Point(13, 217);
            this.rdn_CompactData.Name = "rdn_CompactData";
            this.rdn_CompactData.Size = new System.Drawing.Size(93, 17);
            this.rdn_CompactData.TabIndex = 13;
            this.rdn_CompactData.TabStop = true;
            this.rdn_CompactData.Text = "Compact Data";
            this.rdn_CompactData.UseVisualStyleBackColor = true;
            this.rdn_CompactData.CheckedChanged += new System.EventHandler(this.Rdn_CheckedChanged);
            // 
            // rdn_GenericData
            // 
            this.rdn_GenericData.AutoSize = true;
            this.rdn_GenericData.Location = new System.Drawing.Point(13, 194);
            this.rdn_GenericData.Name = "rdn_GenericData";
            this.rdn_GenericData.Size = new System.Drawing.Size(88, 17);
            this.rdn_GenericData.TabIndex = 12;
            this.rdn_GenericData.TabStop = true;
            this.rdn_GenericData.Text = "Generic Data";
            this.rdn_GenericData.UseVisualStyleBackColor = true;
            this.rdn_GenericData.CheckedChanged += new System.EventHandler(this.Rdn_CheckedChanged);
            // 
            // Rdn_DataStructure
            // 
            this.Rdn_DataStructure.AutoSize = true;
            this.Rdn_DataStructure.Location = new System.Drawing.Point(15, 137);
            this.Rdn_DataStructure.Name = "Rdn_DataStructure";
            this.Rdn_DataStructure.Size = new System.Drawing.Size(94, 17);
            this.Rdn_DataStructure.TabIndex = 11;
            this.Rdn_DataStructure.TabStop = true;
            this.Rdn_DataStructure.Text = "Data Structure";
            this.Rdn_DataStructure.UseVisualStyleBackColor = true;
            this.Rdn_DataStructure.CheckedChanged += new System.EventHandler(this.Rdn_CheckedChanged);
            // 
            // Rdn_Codelist
            // 
            this.Rdn_Codelist.AutoSize = true;
            this.Rdn_Codelist.Location = new System.Drawing.Point(15, 114);
            this.Rdn_Codelist.Name = "Rdn_Codelist";
            this.Rdn_Codelist.Size = new System.Drawing.Size(62, 17);
            this.Rdn_Codelist.TabIndex = 10;
            this.Rdn_Codelist.TabStop = true;
            this.Rdn_Codelist.Text = "Codelist";
            this.Rdn_Codelist.UseVisualStyleBackColor = true;
            this.Rdn_Codelist.CheckedChanged += new System.EventHandler(this.Rdn_CheckedChanged);
            // 
            // Rdn_ConceptsSchema
            // 
            this.Rdn_ConceptsSchema.AutoSize = true;
            this.Rdn_ConceptsSchema.Location = new System.Drawing.Point(15, 91);
            this.Rdn_ConceptsSchema.Name = "Rdn_ConceptsSchema";
            this.Rdn_ConceptsSchema.Size = new System.Drawing.Size(109, 17);
            this.Rdn_ConceptsSchema.TabIndex = 9;
            this.Rdn_ConceptsSchema.TabStop = true;
            this.Rdn_ConceptsSchema.Text = "ConceptsScheme";
            this.Rdn_ConceptsSchema.UseVisualStyleBackColor = true;
            this.Rdn_ConceptsSchema.CheckedChanged += new System.EventHandler(this.Rdn_CheckedChanged);
            // 
            // Rdn_Dataflows
            // 
            this.Rdn_Dataflows.AutoSize = true;
            this.Rdn_Dataflows.Location = new System.Drawing.Point(15, 68);
            this.Rdn_Dataflows.Name = "Rdn_Dataflows";
            this.Rdn_Dataflows.Size = new System.Drawing.Size(72, 17);
            this.Rdn_Dataflows.TabIndex = 8;
            this.Rdn_Dataflows.TabStop = true;
            this.Rdn_Dataflows.Text = "Dataflows";
            this.Rdn_Dataflows.UseVisualStyleBackColor = true;
            this.Rdn_Dataflows.CheckedChanged += new System.EventHandler(this.Rdn_CheckedChanged);
            // 
            // Rdn_CategorySchema
            // 
            this.Rdn_CategorySchema.AutoSize = true;
            this.Rdn_CategorySchema.Location = new System.Drawing.Point(15, 45);
            this.Rdn_CategorySchema.Name = "Rdn_CategorySchema";
            this.Rdn_CategorySchema.Size = new System.Drawing.Size(106, 17);
            this.Rdn_CategorySchema.TabIndex = 7;
            this.Rdn_CategorySchema.TabStop = true;
            this.Rdn_CategorySchema.Text = "CategoryScheme";
            this.Rdn_CategorySchema.UseVisualStyleBackColor = true;
            this.Rdn_CategorySchema.CheckedChanged += new System.EventHandler(this.Rdn_CheckedChanged);
            // 
            // Rdn_AgencySchema
            // 
            this.Rdn_AgencySchema.AutoSize = true;
            this.Rdn_AgencySchema.Location = new System.Drawing.Point(15, 22);
            this.Rdn_AgencySchema.Name = "Rdn_AgencySchema";
            this.Rdn_AgencySchema.Size = new System.Drawing.Size(100, 17);
            this.Rdn_AgencySchema.TabIndex = 6;
            this.Rdn_AgencySchema.TabStop = true;
            this.Rdn_AgencySchema.Text = "AgencyScheme";
            this.Rdn_AgencySchema.UseVisualStyleBackColor = true;
            this.Rdn_AgencySchema.CheckedChanged += new System.EventHandler(this.Rdn_CheckedChanged);
            // 
            // txtHeader
            // 
            this.txtHeader.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHeader.Location = new System.Drawing.Point(209, 34);
            this.txtHeader.Name = "txtHeader";
            this.txtHeader.Size = new System.Drawing.Size(473, 20);
            this.txtHeader.TabIndex = 6;
            this.txtHeader.Visible = false;
            // 
            // TestQuery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1165, 733);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TestQuery";
            this.Text = "Test Query";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.grREST21.ResumeLayout(false);
            this.grREST21.PerformLayout();
            this.grREST20.ResumeLayout(false);
            this.grREST20.PerformLayout();
            this.grSOAP21.ResumeLayout(false);
            this.grSOAP21.PerformLayout();
            this.grSOAP20.ResumeLayout(false);
            this.grSOAP20.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox cmbEndPoint;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnOpenIE;
        private WinXmlViewer txtQuery;
        private WinXmlViewer txtRes;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.GroupBox grREST20;
        private System.Windows.Forms.GroupBox grSOAP21;
        private System.Windows.Forms.GroupBox grSOAP20;
        private System.Windows.Forms.GroupBox grREST21;
        private System.Windows.Forms.RadioButton Rdn_DataStructure;
        private System.Windows.Forms.RadioButton Rdn_Codelist;
        private System.Windows.Forms.RadioButton Rdn_ConceptsSchema;
        private System.Windows.Forms.RadioButton Rdn_Dataflows;
        private System.Windows.Forms.RadioButton Rdn_CategorySchema;
        private System.Windows.Forms.RadioButton Rdn_AgencySchema;
        private System.Windows.Forms.RadioButton rdn_CompactData_REST20;
        private System.Windows.Forms.RadioButton rdn_GenericData_REST20;
        private System.Windows.Forms.RadioButton Rdn_DataStructure_REST20;
        private System.Windows.Forms.RadioButton rdn_StructureSpecificData21;
        private System.Windows.Forms.RadioButton rdn_GenericData21;
        private System.Windows.Forms.RadioButton Rdn_AgencySchema21;
        private System.Windows.Forms.RadioButton Rdn_DataStructure21;
        private System.Windows.Forms.RadioButton Rdn_Codelist21;
        private System.Windows.Forms.RadioButton Rdn_ConceptsSchema21;
        private System.Windows.Forms.RadioButton Rdn_Dataflows21;
        private System.Windows.Forms.RadioButton Rdn_CategorySchema21;
        private System.Windows.Forms.RadioButton rdn_CompactData;
        private System.Windows.Forms.RadioButton rdn_GenericData;
        private System.Windows.Forms.RadioButton rdn_StructureSpecific_REST21;
        private System.Windows.Forms.RadioButton rdn_GenericData_REST21;
        private System.Windows.Forms.RadioButton Rdn_DataStructure_REST21;
        private System.Windows.Forms.TextBox txtHeader;
        private System.Windows.Forms.RadioButton Rdn_Categorisation21;
        private System.Windows.Forms.RadioButton Rdn_ConstrainedCodelist;
    }
}