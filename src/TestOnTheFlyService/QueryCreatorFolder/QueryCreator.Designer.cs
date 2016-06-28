namespace TestOnTheFlyService
{
    partial class QueryCreator
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
            this.btnCompact = new System.Windows.Forms.Button();
            this.BtnGeneric = new System.Windows.Forms.Button();
            this.txtQuery = new WinXmlViewer();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.btnReset, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnCompact, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.BtnGeneric, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtQuery, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(460, 259);
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
            this.btnReset.Size = new System.Drawing.Size(85, 40);
            this.btnReset.TabIndex = 10;
            this.btnReset.Text = "Reset  ";
            this.btnReset.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnCompact
            // 
            this.btnCompact.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnCompact.AutoSize = true;
            this.btnCompact.Image = global::TestOnTheFlyService.Properties.Resources.GetData;
            this.btnCompact.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCompact.Location = new System.Drawing.Point(325, 216);
            this.btnCompact.Name = "btnCompact";
            this.btnCompact.Size = new System.Drawing.Size(132, 40);
            this.btnCompact.TabIndex = 8;
            this.btnCompact.Text = "Get CompactData";
            this.btnCompact.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCompact.UseVisualStyleBackColor = true;
            this.btnCompact.Click += new System.EventHandler(this.btnCompact_Click);
            // 
            // BtnGeneric
            // 
            this.BtnGeneric.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.BtnGeneric.AutoSize = true;
            this.BtnGeneric.Image = global::TestOnTheFlyService.Properties.Resources.GetData;
            this.BtnGeneric.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BtnGeneric.Location = new System.Drawing.Point(189, 216);
            this.BtnGeneric.MinimumSize = new System.Drawing.Size(130, 40);
            this.BtnGeneric.Name = "BtnGeneric";
            this.BtnGeneric.Size = new System.Drawing.Size(130, 40);
            this.BtnGeneric.TabIndex = 9;
            this.BtnGeneric.Text = "Get GenericData";
            this.BtnGeneric.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BtnGeneric.UseVisualStyleBackColor = true;
            this.BtnGeneric.Click += new System.EventHandler(this.BtnGeneric_Click);
            // 
            // txtQuery
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.txtQuery, 3);
            this.txtQuery.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtQuery.Location = new System.Drawing.Point(7, 7);
            this.txtQuery.Margin = new System.Windows.Forms.Padding(7);
            this.txtQuery.Name = "txtQuery";
            this.txtQuery.Size = new System.Drawing.Size(446, 199);
            this.txtQuery.TabIndex = 11;
            // 
            // QueryCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "QueryCreator";
            this.Size = new System.Drawing.Size(460, 259);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnCompact;
        private System.Windows.Forms.Button BtnGeneric;
        private WinXmlViewer txtQuery;
    }
}
