namespace TestOnTheFlyService
{
    partial class QueryCreator21
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnGetData = new System.Windows.Forms.Button();
            this.txtQuery = new TestOnTheFlyService.WinXmlViewer();
            this.ChkFlat = new System.Windows.Forms.CheckBox();
            this.btnStructureSpecificTimeSeries = new System.Windows.Forms.Button();
            this.BtnGenericTimeSeries = new System.Windows.Forms.Button();
            this.btnStructureSpecific = new System.Windows.Forms.Button();
            this.btnGenericData = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.btnReset, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnGetData, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtQuery, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.ChkFlat, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnStructureSpecificTimeSeries, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.BtnGenericTimeSeries, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnStructureSpecific, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnGenericData, 3, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(653, 259);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // btnReset
            // 
            this.btnReset.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnReset.AutoSize = true;
            this.btnReset.Image = global::TestOnTheFlyService.Properties.Resources.Reload;
            this.btnReset.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReset.Location = new System.Drawing.Point(3, 216);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(80, 40);
            this.btnReset.TabIndex = 10;
            this.btnReset.Text = "Reset ";
            this.btnReset.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnGetData
            // 
            this.btnGetData.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.tableLayoutPanel1.SetColumnSpan(this.btnGetData, 4);
            this.btnGetData.Image = global::TestOnTheFlyService.Properties.Resources.GetData;
            this.btnGetData.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGetData.Location = new System.Drawing.Point(550, 216);
            this.btnGetData.MinimumSize = new System.Drawing.Size(100, 40);
            this.btnGetData.Name = "btnGetData";
            this.btnGetData.Size = new System.Drawing.Size(100, 40);
            this.btnGetData.TabIndex = 8;
            this.btnGetData.Text = "Get Data  ";
            this.btnGetData.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGetData.UseVisualStyleBackColor = true;
            this.btnGetData.Click += new System.EventHandler(this.btnGetData_Click);
            // 
            // txtQuery
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.txtQuery, 5);
            this.txtQuery.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtQuery.FileSavedPath = null;
            this.txtQuery.Location = new System.Drawing.Point(7, 36);
            this.txtQuery.Margin = new System.Windows.Forms.Padding(7);
            this.txtQuery.Name = "txtQuery";
            this.txtQuery.Size = new System.Drawing.Size(639, 170);
            this.txtQuery.SolaLettura = false;
            this.txtQuery.TabIndex = 11;
            this.txtQuery.Testo = "";
            // 
            // ChkFlat
            // 
            this.ChkFlat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.ChkFlat.AutoSize = true;
            this.ChkFlat.Location = new System.Drawing.Point(598, 6);
            this.ChkFlat.Name = "ChkFlat";
            this.ChkFlat.Size = new System.Drawing.Size(52, 17);
            this.ChkFlat.TabIndex = 16;
            this.ChkFlat.Text = "FLAT";
            this.ChkFlat.UseVisualStyleBackColor = true;
            this.ChkFlat.CheckedChanged += new System.EventHandler(this.ChkFlat_CheckedChanged);
            // 
            // btnStructureSpecificTimeSeries
            // 
            this.btnStructureSpecificTimeSeries.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnStructureSpecificTimeSeries.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStructureSpecificTimeSeries.FlatAppearance.BorderSize = 0;
            this.btnStructureSpecificTimeSeries.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStructureSpecificTimeSeries.Location = new System.Drawing.Point(3, 3);
            this.btnStructureSpecificTimeSeries.Name = "btnStructureSpecificTimeSeries";
            this.btnStructureSpecificTimeSeries.Size = new System.Drawing.Size(158, 23);
            this.btnStructureSpecificTimeSeries.TabIndex = 14;
            this.btnStructureSpecificTimeSeries.Text = "StructureSpecificTimeSeries";
            this.btnStructureSpecificTimeSeries.UseVisualStyleBackColor = true;
            this.btnStructureSpecificTimeSeries.Click += new System.EventHandler(this.btnStructureSpecificTimeSeries_Click);
            // 
            // BtnGenericTimeSeries
            // 
            this.BtnGenericTimeSeries.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.BtnGenericTimeSeries.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnGenericTimeSeries.FlatAppearance.BorderSize = 0;
            this.BtnGenericTimeSeries.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnGenericTimeSeries.Location = new System.Drawing.Point(167, 3);
            this.BtnGenericTimeSeries.Name = "BtnGenericTimeSeries";
            this.BtnGenericTimeSeries.Size = new System.Drawing.Size(114, 23);
            this.BtnGenericTimeSeries.TabIndex = 12;
            this.BtnGenericTimeSeries.Text = "GenericTimeSeries";
            this.BtnGenericTimeSeries.UseVisualStyleBackColor = true;
            this.BtnGenericTimeSeries.Click += new System.EventHandler(this.BtnGenericTimeSeries_Click);
            // 
            // btnStructureSpecific
            // 
            this.btnStructureSpecific.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnStructureSpecific.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStructureSpecific.FlatAppearance.BorderSize = 0;
            this.btnStructureSpecific.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStructureSpecific.Location = new System.Drawing.Point(287, 3);
            this.btnStructureSpecific.Name = "btnStructureSpecific";
            this.btnStructureSpecific.Size = new System.Drawing.Size(106, 23);
            this.btnStructureSpecific.TabIndex = 15;
            this.btnStructureSpecific.Text = "StructureSpecific";
            this.btnStructureSpecific.UseVisualStyleBackColor = true;
            this.btnStructureSpecific.Visible = false;
            this.btnStructureSpecific.Click += new System.EventHandler(this.btnStructureSpecific_Click);
            // 
            // btnGenericData
            // 
            this.btnGenericData.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnGenericData.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGenericData.FlatAppearance.BorderSize = 0;
            this.btnGenericData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenericData.Location = new System.Drawing.Point(399, 3);
            this.btnGenericData.Name = "btnGenericData";
            this.btnGenericData.Size = new System.Drawing.Size(85, 23);
            this.btnGenericData.TabIndex = 13;
            this.btnGenericData.Text = "GenericData";
            this.btnGenericData.UseVisualStyleBackColor = true;
            this.btnGenericData.Visible = false;
            this.btnGenericData.Click += new System.EventHandler(this.btnGenericData_Click);
            // 
            // QueryCreator21
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "QueryCreator21";
            this.Size = new System.Drawing.Size(653, 259);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnGetData;
        private WinXmlViewer txtQuery;
        private System.Windows.Forms.Button BtnGenericTimeSeries;
        private System.Windows.Forms.Button btnGenericData;
        private System.Windows.Forms.Button btnStructureSpecificTimeSeries;
        private System.Windows.Forms.Button btnStructureSpecific;
        private System.Windows.Forms.CheckBox ChkFlat;
    }
}
