namespace SistemaMultimedia.Forms
{
    partial class DataFusionForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView ddvDatos;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnGroup;
        private System.Windows.Forms.Button btnValidate;
        private System.Windows.Forms.Button btnSaveJson;

        // Botones para SQLite
        private System.Windows.Forms.Button btnSaveToDb;
        private System.Windows.Forms.Button btnLoadFromDb;

        // Label para resumen (total registros y suma Valor)
        private System.Windows.Forms.Label lblSummary;

        // ComboBox para seleccionar el destino de la base de datos
        private System.Windows.Forms.ComboBox cmbDB;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataFusionForm));
            ddvDatos = new DataGridView();
            btnLoad = new Button();
            btnGroup = new Button();
            btnValidate = new Button();
            btnSaveJson = new Button();
            btnSaveToDb = new Button();
            btnLoadFromDb = new Button();
            lblSummary = new Label();
            cmbDB = new ComboBox();
            ((System.ComponentModel.ISupportInitialize)ddvDatos).BeginInit();
            SuspendLayout();
            // 
            // ddvDatos
            // 
            ddvDatos.AllowUserToAddRows = false;
            ddvDatos.AllowUserToDeleteRows = false;
            ddvDatos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ddvDatos.BackgroundColor = Color.White;
            ddvDatos.ForeColor = Color.Black;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle1.ForeColor = Color.White;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            ddvDatos.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            ddvDatos.ColumnHeadersHeight = 29;
            ddvDatos.Location = new Point(15, 56);
            ddvDatos.Margin = new Padding(4);
            ddvDatos.Name = "ddvDatos";
            ddvDatos.ReadOnly = true;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.Control;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            ddvDatos.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            ddvDatos.RowHeadersWidth = 51;
            ddvDatos.Size = new Size(1069, 552);
            ddvDatos.TabIndex = 1;
            // 
            // btnLoad
            // 
            btnLoad.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLoad.BackColor = Color.FromArgb(30, 30, 30);
            btnLoad.Cursor = Cursors.Hand;
            btnLoad.ForeColor = Color.Black;
            btnLoad.Location = new Point(975, 15);
            btnLoad.Margin = new Padding(4);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(110, 34);
            btnLoad.TabIndex = 0;
            btnLoad.Text = "Cargar CSV/TXT";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += btnLoad_Click;
            // 
            // btnGroup
            // 
            btnGroup.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnGroup.BackColor = Color.FromArgb(30, 30, 30);
            btnGroup.Cursor = Cursors.Hand;
            btnGroup.ForeColor = Color.Black;
            btnGroup.Location = new Point(850, 15);
            btnGroup.Margin = new Padding(4);
            btnGroup.Name = "btnGroup";
            btnGroup.Size = new Size(118, 34);
            btnGroup.TabIndex = 2;
            btnGroup.Text = "Agrupar";
            btnGroup.UseVisualStyleBackColor = true;
            // 
            // btnValidate
            // 
            btnValidate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnValidate.BackColor = Color.FromArgb(30, 30, 30);
            btnValidate.Cursor = Cursors.Hand;
            btnValidate.ForeColor = Color.Black;
            btnValidate.Location = new Point(600, 15);
            btnValidate.Margin = new Padding(4);
            btnValidate.Name = "btnValidate";
            btnValidate.Size = new Size(118, 34);
            btnValidate.TabIndex = 19;
            btnValidate.Text = "Validar datos";
            btnValidate.UseVisualStyleBackColor = true;
            // 
            // btnSaveJson
            // 
            btnSaveJson.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSaveJson.BackColor = Color.FromArgb(30, 30, 30);
            btnSaveJson.Cursor = Cursors.Hand;
            btnSaveJson.ForeColor = Color.Black;
            btnSaveJson.Location = new Point(725, 15);
            btnSaveJson.Margin = new Padding(4);
            btnSaveJson.Name = "btnSaveJson";
            btnSaveJson.Size = new Size(118, 34);
            btnSaveJson.TabIndex = 15;
            btnSaveJson.Text = "Guardar JSON";
            btnSaveJson.UseVisualStyleBackColor = true;
            btnSaveJson.Click += btnSaveJson_Click;
            // 
            // btnSaveToDb
            // 
            btnSaveToDb.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSaveToDb.BackColor = Color.FromArgb(30, 30, 30);
            btnSaveToDb.Cursor = Cursors.Hand;
            btnSaveToDb.ForeColor = Color.Black;
            btnSaveToDb.Location = new Point(790, 617);
            btnSaveToDb.Margin = new Padding(4);
            btnSaveToDb.Name = "btnSaveToDb";
            btnSaveToDb.Size = new Size(124, 33);
            btnSaveToDb.TabIndex = 14;
            btnSaveToDb.Text = "Guardar BD";
            btnSaveToDb.UseVisualStyleBackColor = true;
            btnSaveToDb.Click += btnMigrateToSql_Click;
            // 
            // btnLoadFromDb
            // 
            btnLoadFromDb.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLoadFromDb.BackColor = Color.FromArgb(30, 30, 30);
            btnLoadFromDb.Cursor = Cursors.Hand;
            btnLoadFromDb.ForeColor = Color.Black;
            btnLoadFromDb.Location = new Point(960, 617);
            btnLoadFromDb.Margin = new Padding(4);
            btnLoadFromDb.Name = "btnLoadFromDb";
            btnLoadFromDb.Size = new Size(124, 33);
            btnLoadFromDb.TabIndex = 16;
            btnLoadFromDb.Text = "Cargar BD";
            btnLoadFromDb.UseVisualStyleBackColor = true;
            btnLoadFromDb.Click += btnLoadFromDb_Click;
            // 
            // lblSummary
            // 
            lblSummary.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblSummary.AutoSize = true;
            lblSummary.ForeColor = Color.White;
            lblSummary.Location = new Point(15, 625);
            lblSummary.Margin = new Padding(4, 0, 4, 0);
            lblSummary.Name = "lblSummary";
            lblSummary.Size = new Size(201, 25);
            lblSummary.TabIndex = 17;
            lblSummary.Text = "No hay datos cargados.";
            // 
            // cmbDB
            // 
            cmbDB.BackColor = Color.FromArgb(30, 30, 30);
            cmbDB.Cursor = Cursors.Hand;
            cmbDB.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDB.ForeColor = Color.Black;
            cmbDB.FormattingEnabled = true;
            cmbDB.Items.AddRange(new object[] { "SQLite", "SQL Server" });
            cmbDB.Location = new Point(619, 617);
            cmbDB.Margin = new Padding(4);
            cmbDB.Name = "cmbDB";
            cmbDB.Size = new Size(124, 33);
            cmbDB.TabIndex = 18;
            // 
            // DataFusionForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 30);
            ClientSize = new Size(1099, 662);
            Controls.Add(cmbDB);
            Controls.Add(lblSummary);
            Controls.Add(btnLoadFromDb);
            Controls.Add(btnSaveToDb);
            Controls.Add(btnSaveJson);
            Controls.Add(ddvDatos);
            Controls.Add(btnValidate);
            Controls.Add(btnGroup);
            Controls.Add(btnLoad);
            ForeColor = Color.White;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4);
            Name = "DataFusionForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "DataFusion Arena";
            ((System.ComponentModel.ISupportInitialize)ddvDatos).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
