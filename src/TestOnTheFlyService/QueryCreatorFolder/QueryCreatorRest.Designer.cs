namespace TestOnTheFlyService
{
    partial class QueryCreatorRest
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
            this.txtQuery = new System.Windows.Forms.TextBox();
            this.ChkFlat = new System.Windows.Forms.CheckBox();
            this.txtHeader = new System.Windows.Forms.TextBox();
            this.lblHeader = new System.Windows.Forms.Label();
            this.cmbTypeOP = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.btnReset, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnGetData, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtQuery, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.ChkFlat, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtHeader, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblHeader, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.cmbTypeOP, 3, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
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
            this.btnGetData.AutoSize = true;
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
            this.txtQuery.Location = new System.Drawing.Point(7, 34);
            this.txtQuery.Margin = new System.Windows.Forms.Padding(7);
            this.txtQuery.Multiline = true;
            this.txtQuery.Name = "txtQuery";
            this.txtQuery.Size = new System.Drawing.Size(639, 172);
            this.txtQuery.TabIndex = 11;
            // 
            // ChkFlat
            // 
            this.ChkFlat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.ChkFlat.AutoSize = true;
            this.ChkFlat.Location = new System.Drawing.Point(598, 5);
            this.ChkFlat.Name = "ChkFlat";
            this.ChkFlat.Size = new System.Drawing.Size(52, 17);
            this.ChkFlat.TabIndex = 16;
            this.ChkFlat.Text = "FLAT";
            this.ChkFlat.UseVisualStyleBackColor = true;
            this.ChkFlat.CheckedChanged += new System.EventHandler(this.ChkFlat_CheckedChanged);
            // 
            // txtHeader
            // 
            this.txtHeader.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.txtHeader, 2);
            this.txtHeader.Location = new System.Drawing.Point(89, 3);
            this.txtHeader.Name = "txtHeader";
            this.txtHeader.Size = new System.Drawing.Size(329, 20);
            this.txtHeader.TabIndex = 17;
            // 
            // lblHeader
            // 
            this.lblHeader.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblHeader.AutoSize = true;
            this.lblHeader.Location = new System.Drawing.Point(36, 7);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(47, 13);
            this.lblHeader.TabIndex = 18;
            this.lblHeader.Text = "Headers";
            // 
            // cmbTypeOP
            // 
            this.cmbTypeOP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTypeOP.FormattingEnabled = true;
            this.cmbTypeOP.Location = new System.Drawing.Point(424, 3);
            this.cmbTypeOP.Name = "cmbTypeOP";
            this.cmbTypeOP.Size = new System.Drawing.Size(168, 21);
            this.cmbTypeOP.TabIndex = 19;
            this.cmbTypeOP.SelectedIndexChanged += new System.EventHandler(this.cmbTypeOP_SelectedIndexChanged);
            // 
            // QueryCreatorRest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "QueryCreatorRest";
            this.Size = new System.Drawing.Size(653, 259);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnGetData;
        private System.Windows.Forms.TextBox txtQuery;
        private System.Windows.Forms.CheckBox ChkFlat;
        private System.Windows.Forms.TextBox txtHeader;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.ComboBox cmbTypeOP;
    }
}
