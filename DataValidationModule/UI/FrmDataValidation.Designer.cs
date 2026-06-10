namespace DataValidationModule.UI
{
    partial class FrmDataValidation
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            panelTop = new System.Windows.Forms.Panel();
            lblFile = new System.Windows.Forms.Label();
            btnImport = new System.Windows.Forms.Button();
            lblTitle = new System.Windows.Forms.Label();
            splitMain = new System.Windows.Forms.SplitContainer();
            panelControls = new System.Windows.Forms.Panel();
            lblProgress = new System.Windows.Forms.Label();
            progressBar = new System.Windows.Forms.ProgressBar();
            grpStats = new System.Windows.Forms.GroupBox();
            lblStatQuality = new System.Windows.Forms.Label();
            lblStatFixed = new System.Windows.Forms.Label();
            lblStatNulls = new System.Windows.Forms.Label();
            lblStatDups = new System.Windows.Forms.Label();
            lblStatErrors = new System.Windows.Forms.Label();
            lblStatColumns = new System.Windows.Forms.Label();
            lblStatRecords = new System.Windows.Forms.Label();
            grpActions = new System.Windows.Forms.GroupBox();
            btnExportReport = new System.Windows.Forms.Button();
            btnSaveFile = new System.Windows.Forms.Button();
            btnRevert = new System.Windows.Forms.Button();
            btnApplyChanges = new System.Windows.Forms.Button();
            btnRemoveDups = new System.Windows.Forms.Button();
            btnAutoFix = new System.Windows.Forms.Button();
            btnAnalyze = new System.Windows.Forms.Button();
            splitLeft = new System.Windows.Forms.SplitContainer();
            dataGridView = new System.Windows.Forms.DataGridView();
            panelReport = new System.Windows.Forms.Panel();
            richTextBoxReport = new System.Windows.Forms.RichTextBox();
            lblReportTitle = new System.Windows.Forms.Label();
            statusStrip = new System.Windows.Forms.StatusStrip();
            toolStripStatus = new System.Windows.Forms.ToolStripStatusLabel();
            panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
            splitMain.Panel1.SuspendLayout();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            panelControls.SuspendLayout();
            grpStats.SuspendLayout();
            grpActions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitLeft).BeginInit();
            splitLeft.Panel1.SuspendLayout();
            splitLeft.Panel2.SuspendLayout();
            splitLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
            panelReport.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // panelTop
            // 
            panelTop.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            panelTop.Controls.Add(lblFile);
            panelTop.Controls.Add(btnImport);
            panelTop.Controls.Add(lblTitle);
            panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            panelTop.Location = new System.Drawing.Point(0, 0);
            panelTop.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            panelTop.Name = "panelTop";
            panelTop.Padding = new System.Windows.Forms.Padding(17, 13, 17, 13);
            panelTop.Size = new System.Drawing.Size(1829, 62);
            panelTop.TabIndex = 0;
            // 
            // lblFile
            // 
            lblFile.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            lblFile.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Italic);
            lblFile.ForeColor = System.Drawing.Color.FromArgb(149, 165, 166);
            lblFile.Location = new System.Drawing.Point(1311, 80);
            lblFile.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblFile.Name = "lblFile";
            lblFile.Size = new System.Drawing.Size(500, 30);
            lblFile.TabIndex = 3;
            lblFile.Text = "Ningún archivo cargado";
            lblFile.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnImport
            // 
            btnImport.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnImport.BackColor = System.Drawing.Color.White;
            btnImport.FlatAppearance.BorderSize = 0;
            btnImport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnImport.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            btnImport.ForeColor = System.Drawing.Color.Black;
            btnImport.Location = new System.Drawing.Point(1589, 18);
            btnImport.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnImport.Name = "btnImport";
            btnImport.Size = new System.Drawing.Size(222, 32);
            btnImport.TabIndex = 2;
            btnImport.Text = "📂  Importar Archivo";
            btnImport.UseVisualStyleBackColor = false;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.Location = new System.Drawing.Point(20, 13);
            lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new System.Drawing.Size(537, 38);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "🔍  Validación y Corrección de Datasets";
            // 
            // splitMain
            // 
            splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            splitMain.Location = new System.Drawing.Point(0, 62);
            splitMain.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            splitMain.Panel1.Controls.Add(panelControls);
            splitMain.Panel1.Padding = new System.Windows.Forms.Padding(11, 13, 11, 13);
            splitMain.Panel1MinSize = 350;
            // 
            // splitMain.Panel2
            // 
            splitMain.Panel2.Controls.Add(splitLeft);
            splitMain.Size = new System.Drawing.Size(1829, 956);
            splitMain.SplitterDistance = 731;
            splitMain.SplitterWidth = 6;
            splitMain.TabIndex = 1;
            // 
            // panelControls
            // 
            panelControls.AutoScroll = true;
            panelControls.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            panelControls.Controls.Add(lblProgress);
            panelControls.Controls.Add(progressBar);
            panelControls.Controls.Add(grpStats);
            panelControls.Controls.Add(grpActions);
            panelControls.Dock = System.Windows.Forms.DockStyle.Fill;
            panelControls.ForeColor = System.Drawing.Color.White;
            panelControls.Location = new System.Drawing.Point(11, 13);
            panelControls.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            panelControls.Name = "panelControls";
            panelControls.Padding = new System.Windows.Forms.Padding(11, 13, 11, 13);
            panelControls.Size = new System.Drawing.Size(709, 930);
            panelControls.TabIndex = 0;
            // 
            // lblProgress
            // 
            lblProgress.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            lblProgress.Dock = System.Windows.Forms.DockStyle.Top;
            lblProgress.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            lblProgress.ForeColor = System.Drawing.Color.White;
            lblProgress.Location = new System.Drawing.Point(11, 884);
            lblProgress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblProgress.Name = "lblProgress";
            lblProgress.Padding = new System.Windows.Forms.Padding(0, 0, 0, 7);
            lblProgress.Size = new System.Drawing.Size(687, 37);
            lblProgress.TabIndex = 3;
            lblProgress.Text = "Listo";
            lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // progressBar
            // 
            progressBar.Dock = System.Windows.Forms.DockStyle.Top;
            progressBar.Location = new System.Drawing.Point(11, 854);
            progressBar.Margin = new System.Windows.Forms.Padding(0, 7, 0, 0);
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(687, 30);
            progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            progressBar.TabIndex = 2;
            // 
            // grpStats
            // 
            grpStats.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            grpStats.Controls.Add(lblStatQuality);
            grpStats.Controls.Add(lblStatFixed);
            grpStats.Controls.Add(lblStatNulls);
            grpStats.Controls.Add(lblStatDups);
            grpStats.Controls.Add(lblStatErrors);
            grpStats.Controls.Add(lblStatColumns);
            grpStats.Controls.Add(lblStatRecords);
            grpStats.Dock = System.Windows.Forms.DockStyle.Top;
            grpStats.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            grpStats.ForeColor = System.Drawing.Color.White;
            grpStats.Location = new System.Drawing.Point(11, 542);
            grpStats.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            grpStats.Name = "grpStats";
            grpStats.Padding = new System.Windows.Forms.Padding(14, 23, 11, 13);
            grpStats.Size = new System.Drawing.Size(687, 312);
            grpStats.TabIndex = 1;
            grpStats.TabStop = false;
            grpStats.Text = "Estadísticas";
            // 
            // lblStatQuality
            // 
            lblStatQuality.Dock = System.Windows.Forms.DockStyle.Top;
            lblStatQuality.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            lblStatQuality.ForeColor = System.Drawing.Color.White;
            lblStatQuality.Location = new System.Drawing.Point(14, 269);
            lblStatQuality.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblStatQuality.Name = "lblStatQuality";
            lblStatQuality.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            lblStatQuality.Size = new System.Drawing.Size(662, 37);
            lblStatQuality.TabIndex = 6;
            lblStatQuality.Text = "📊 Calidad:        —";
            // 
            // lblStatFixed
            // 
            lblStatFixed.Dock = System.Windows.Forms.DockStyle.Top;
            lblStatFixed.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            lblStatFixed.ForeColor = System.Drawing.Color.White;
            lblStatFixed.Location = new System.Drawing.Point(14, 232);
            lblStatFixed.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblStatFixed.Name = "lblStatFixed";
            lblStatFixed.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            lblStatFixed.Size = new System.Drawing.Size(662, 37);
            lblStatFixed.TabIndex = 5;
            lblStatFixed.Text = "✅ Correcciones:  —";
            // 
            // lblStatNulls
            // 
            lblStatNulls.Dock = System.Windows.Forms.DockStyle.Top;
            lblStatNulls.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            lblStatNulls.ForeColor = System.Drawing.Color.White;
            lblStatNulls.Location = new System.Drawing.Point(14, 195);
            lblStatNulls.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblStatNulls.Name = "lblStatNulls";
            lblStatNulls.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            lblStatNulls.Size = new System.Drawing.Size(662, 37);
            lblStatNulls.TabIndex = 4;
            lblStatNulls.Text = "⬜ Nulos:          —";
            // 
            // lblStatDups
            // 
            lblStatDups.Dock = System.Windows.Forms.DockStyle.Top;
            lblStatDups.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            lblStatDups.ForeColor = System.Drawing.Color.White;
            lblStatDups.Location = new System.Drawing.Point(14, 158);
            lblStatDups.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblStatDups.Name = "lblStatDups";
            lblStatDups.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            lblStatDups.Size = new System.Drawing.Size(662, 37);
            lblStatDups.TabIndex = 3;
            lblStatDups.Text = "♻️ Duplicados:     —";
            // 
            // lblStatErrors
            // 
            lblStatErrors.Dock = System.Windows.Forms.DockStyle.Top;
            lblStatErrors.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            lblStatErrors.ForeColor = System.Drawing.Color.White;
            lblStatErrors.Location = new System.Drawing.Point(14, 121);
            lblStatErrors.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblStatErrors.Name = "lblStatErrors";
            lblStatErrors.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            lblStatErrors.Size = new System.Drawing.Size(662, 37);
            lblStatErrors.TabIndex = 2;
            lblStatErrors.Text = "❌ Errores:        —";
            // 
            // lblStatColumns
            // 
            lblStatColumns.Dock = System.Windows.Forms.DockStyle.Top;
            lblStatColumns.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            lblStatColumns.ForeColor = System.Drawing.Color.White;
            lblStatColumns.Location = new System.Drawing.Point(14, 84);
            lblStatColumns.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblStatColumns.Name = "lblStatColumns";
            lblStatColumns.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            lblStatColumns.Size = new System.Drawing.Size(662, 37);
            lblStatColumns.TabIndex = 1;
            lblStatColumns.Text = "📋 Columnas:       —";
            // 
            // lblStatRecords
            // 
            lblStatRecords.Dock = System.Windows.Forms.DockStyle.Top;
            lblStatRecords.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            lblStatRecords.ForeColor = System.Drawing.Color.White;
            lblStatRecords.Location = new System.Drawing.Point(14, 47);
            lblStatRecords.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblStatRecords.Name = "lblStatRecords";
            lblStatRecords.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            lblStatRecords.Size = new System.Drawing.Size(662, 37);
            lblStatRecords.TabIndex = 0;
            lblStatRecords.Text = "📝 Registros:      —";
            // 
            // grpActions
            // 
            grpActions.Controls.Add(btnExportReport);
            grpActions.Controls.Add(btnSaveFile);
            grpActions.Controls.Add(btnRevert);
            grpActions.Controls.Add(btnApplyChanges);
            grpActions.Controls.Add(btnRemoveDups);
            grpActions.Controls.Add(btnAutoFix);
            grpActions.Controls.Add(btnAnalyze);
            grpActions.Dock = System.Windows.Forms.DockStyle.Top;
            grpActions.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            grpActions.ForeColor = System.Drawing.Color.White;
            grpActions.Location = new System.Drawing.Point(11, 13);
            grpActions.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            grpActions.Name = "grpActions";
            grpActions.Padding = new System.Windows.Forms.Padding(11, 13, 11, 13);
            grpActions.Size = new System.Drawing.Size(687, 529);
            grpActions.TabIndex = 0;
            grpActions.TabStop = false;
            grpActions.Text = "Acciones";
            // 
            // btnExportReport
            // 
            btnExportReport.BackColor = System.Drawing.Color.White;
            btnExportReport.Cursor = System.Windows.Forms.Cursors.Hand;
            btnExportReport.Dock = System.Windows.Forms.DockStyle.Top;
            btnExportReport.Enabled = false;
            btnExportReport.FlatAppearance.BorderSize = 0;
            btnExportReport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnExportReport.Font = new System.Drawing.Font("Segoe UI", 9F);
            btnExportReport.ForeColor = System.Drawing.Color.Black;
            btnExportReport.Location = new System.Drawing.Point(11, 397);
            btnExportReport.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            btnExportReport.Name = "btnExportReport";
            btnExportReport.Size = new System.Drawing.Size(665, 60);
            btnExportReport.TabIndex = 6;
            btnExportReport.Text = "📄  Exportar Reporte";
            btnExportReport.UseVisualStyleBackColor = false;
            // 
            // btnSaveFile
            // 
            btnSaveFile.BackColor = System.Drawing.Color.White;
            btnSaveFile.Cursor = System.Windows.Forms.Cursors.Hand;
            btnSaveFile.Dock = System.Windows.Forms.DockStyle.Top;
            btnSaveFile.Enabled = false;
            btnSaveFile.FlatAppearance.BorderSize = 0;
            btnSaveFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnSaveFile.Font = new System.Drawing.Font("Segoe UI", 9F);
            btnSaveFile.ForeColor = System.Drawing.Color.Black;
            btnSaveFile.Location = new System.Drawing.Point(11, 337);
            btnSaveFile.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            btnSaveFile.Name = "btnSaveFile";
            btnSaveFile.Size = new System.Drawing.Size(665, 60);
            btnSaveFile.TabIndex = 5;
            btnSaveFile.Text = "💾  Guardar Archivo";
            btnSaveFile.UseVisualStyleBackColor = false;
            // 
            // btnRevert
            // 
            btnRevert.BackColor = System.Drawing.Color.White;
            btnRevert.Cursor = System.Windows.Forms.Cursors.Hand;
            btnRevert.Dock = System.Windows.Forms.DockStyle.Top;
            btnRevert.Enabled = false;
            btnRevert.FlatAppearance.BorderSize = 0;
            btnRevert.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnRevert.Font = new System.Drawing.Font("Segoe UI", 9F);
            btnRevert.ForeColor = System.Drawing.Color.Black;
            btnRevert.Location = new System.Drawing.Point(11, 277);
            btnRevert.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            btnRevert.Name = "btnRevert";
            btnRevert.Size = new System.Drawing.Size(665, 60);
            btnRevert.TabIndex = 4;
            btnRevert.Text = "↩️  Revertir Cambios";
            btnRevert.UseVisualStyleBackColor = false;
            // 
            // btnApplyChanges
            // 
            btnApplyChanges.BackColor = System.Drawing.Color.White;
            btnApplyChanges.Cursor = System.Windows.Forms.Cursors.Hand;
            btnApplyChanges.Dock = System.Windows.Forms.DockStyle.Top;
            btnApplyChanges.Enabled = false;
            btnApplyChanges.FlatAppearance.BorderSize = 0;
            btnApplyChanges.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnApplyChanges.Font = new System.Drawing.Font("Segoe UI", 9F);
            btnApplyChanges.ForeColor = System.Drawing.Color.Black;
            btnApplyChanges.Location = new System.Drawing.Point(11, 217);
            btnApplyChanges.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            btnApplyChanges.Name = "btnApplyChanges";
            btnApplyChanges.Size = new System.Drawing.Size(665, 60);
            btnApplyChanges.TabIndex = 3;
            btnApplyChanges.Text = "✅  Aplicar Cambios";
            btnApplyChanges.UseVisualStyleBackColor = false;
            // 
            // btnRemoveDups
            // 
            btnRemoveDups.BackColor = System.Drawing.Color.White;
            btnRemoveDups.Cursor = System.Windows.Forms.Cursors.Hand;
            btnRemoveDups.Dock = System.Windows.Forms.DockStyle.Top;
            btnRemoveDups.Enabled = false;
            btnRemoveDups.FlatAppearance.BorderSize = 0;
            btnRemoveDups.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnRemoveDups.Font = new System.Drawing.Font("Segoe UI", 9F);
            btnRemoveDups.ForeColor = System.Drawing.Color.Black;
            btnRemoveDups.Location = new System.Drawing.Point(11, 157);
            btnRemoveDups.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            btnRemoveDups.Name = "btnRemoveDups";
            btnRemoveDups.Size = new System.Drawing.Size(665, 60);
            btnRemoveDups.TabIndex = 2;
            btnRemoveDups.Text = "🗑️  Eliminar Duplicados";
            btnRemoveDups.UseVisualStyleBackColor = false;
            // 
            // btnAutoFix
            // 
            btnAutoFix.BackColor = System.Drawing.Color.White;
            btnAutoFix.Cursor = System.Windows.Forms.Cursors.Hand;
            btnAutoFix.Dock = System.Windows.Forms.DockStyle.Top;
            btnAutoFix.Enabled = false;
            btnAutoFix.FlatAppearance.BorderSize = 0;
            btnAutoFix.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnAutoFix.Font = new System.Drawing.Font("Segoe UI", 9F);
            btnAutoFix.ForeColor = System.Drawing.Color.Black;
            btnAutoFix.Location = new System.Drawing.Point(11, 97);
            btnAutoFix.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            btnAutoFix.Name = "btnAutoFix";
            btnAutoFix.Size = new System.Drawing.Size(665, 60);
            btnAutoFix.TabIndex = 1;
            btnAutoFix.Text = "⚙️  Corregir Automáticamente";
            btnAutoFix.UseVisualStyleBackColor = false;
            // 
            // btnAnalyze
            // 
            btnAnalyze.BackColor = System.Drawing.Color.White;
            btnAnalyze.Cursor = System.Windows.Forms.Cursors.Hand;
            btnAnalyze.Dock = System.Windows.Forms.DockStyle.Top;
            btnAnalyze.Enabled = false;
            btnAnalyze.FlatAppearance.BorderSize = 0;
            btnAnalyze.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnAnalyze.Font = new System.Drawing.Font("Segoe UI", 9F);
            btnAnalyze.ForeColor = System.Drawing.Color.Black;
            btnAnalyze.Location = new System.Drawing.Point(11, 37);
            btnAnalyze.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            btnAnalyze.Name = "btnAnalyze";
            btnAnalyze.Size = new System.Drawing.Size(665, 60);
            btnAnalyze.TabIndex = 0;
            btnAnalyze.Text = "🔍  Analizar Dataset";
            btnAnalyze.UseVisualStyleBackColor = false;
            // 
            // splitLeft
            // 
            splitLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            splitLeft.Location = new System.Drawing.Point(0, 0);
            splitLeft.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            splitLeft.Name = "splitLeft";
            splitLeft.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitLeft.Panel1
            // 
            splitLeft.Panel1.Controls.Add(dataGridView);
            splitLeft.Panel1MinSize = 200;
            // 
            // splitLeft.Panel2
            // 
            splitLeft.Panel2.Controls.Add(panelReport);
            splitLeft.Panel2MinSize = 150;
            splitLeft.Size = new System.Drawing.Size(1092, 956);
            splitLeft.SplitterDistance = 540;
            splitLeft.SplitterWidth = 7;
            splitLeft.TabIndex = 0;
            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(245, 248, 252);
            dataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.BackgroundColor = System.Drawing.Color.White;
            dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(44, 62, 80);
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.GridColor = System.Drawing.Color.FromArgb(220, 220, 220);
            dataGridView.Location = new System.Drawing.Point(0, 0);
            dataGridView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            dataGridView.Name = "dataGridView";
            dataGridView.RowHeadersWidth = 28;
            dataGridView.RowTemplate.Height = 24;
            dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dataGridView.Size = new System.Drawing.Size(1092, 540);
            dataGridView.TabIndex = 0;
            // 
            // panelReport
            // 
            panelReport.BackColor = System.Drawing.Color.White;
            panelReport.Controls.Add(richTextBoxReport);
            panelReport.Controls.Add(lblReportTitle);
            panelReport.Dock = System.Windows.Forms.DockStyle.Fill;
            panelReport.Location = new System.Drawing.Point(0, 0);
            panelReport.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            panelReport.Name = "panelReport";
            panelReport.Padding = new System.Windows.Forms.Padding(6, 7, 6, 0);
            panelReport.Size = new System.Drawing.Size(1092, 409);
            panelReport.TabIndex = 0;
            // 
            // richTextBoxReport
            // 
            richTextBoxReport.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            richTextBoxReport.BorderStyle = System.Windows.Forms.BorderStyle.None;
            richTextBoxReport.Font = new System.Drawing.Font("Consolas", 9F);
            richTextBoxReport.ForeColor = System.Drawing.Color.FromArgb(230, 230, 230);
            richTextBoxReport.Location = new System.Drawing.Point(6, 52);
            richTextBoxReport.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            richTextBoxReport.Name = "richTextBoxReport";
            richTextBoxReport.ReadOnly = true;
            richTextBoxReport.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            richTextBoxReport.Size = new System.Drawing.Size(1078, 344);
            richTextBoxReport.TabIndex = 1;
            richTextBoxReport.Text = "";
            // 
            // lblReportTitle
            // 
            lblReportTitle.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            lblReportTitle.Dock = System.Windows.Forms.DockStyle.Top;
            lblReportTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            lblReportTitle.ForeColor = System.Drawing.Color.White;
            lblReportTitle.Location = new System.Drawing.Point(6, 7);
            lblReportTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblReportTitle.Name = "lblReportTitle";
            lblReportTitle.Padding = new System.Windows.Forms.Padding(6, 7, 0, 0);
            lblReportTitle.Size = new System.Drawing.Size(1080, 40);
            lblReportTitle.TabIndex = 0;
            lblReportTitle.Text = "📋  Reporte de errores detectados";
            // 
            // statusStrip
            // 
            statusStrip.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            statusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatus });
            statusStrip.Location = new System.Drawing.Point(0, 1018);
            statusStrip.Name = "statusStrip";
            statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 20, 0);
            statusStrip.Size = new System.Drawing.Size(1829, 32);
            statusStrip.TabIndex = 2;
            // 
            // toolStripStatus
            // 
            toolStripStatus.ForeColor = System.Drawing.Color.White;
            toolStripStatus.Name = "toolStripStatus";
            toolStripStatus.Size = new System.Drawing.Size(349, 25);
            toolStripStatus.Text = "Listo  •  Importe un archivo para comenzar";
            // 
            // FrmDataValidation
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            ClientSize = new System.Drawing.Size(1829, 1050);
            Controls.Add(splitMain);
            Controls.Add(panelTop);
            Controls.Add(statusStrip);
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            MinimumSize = new System.Drawing.Size(1419, 1006);
            Name = "FrmDataValidation";
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = "Validación y Corrección de Datasets";
            WindowState = System.Windows.Forms.FormWindowState.Maximized;
            panelTop.ResumeLayout(false);
            panelTop.PerformLayout();
            splitMain.Panel1.ResumeLayout(false);
            splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
            splitMain.ResumeLayout(false);
            panelControls.ResumeLayout(false);
            grpStats.ResumeLayout(false);
            grpActions.ResumeLayout(false);
            splitLeft.Panel1.ResumeLayout(false);
            splitLeft.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitLeft).EndInit();
            splitLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
            panelReport.ResumeLayout(false);
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.SplitContainer splitLeft;
        private System.Windows.Forms.Panel panelControls;
        private System.Windows.Forms.GroupBox grpActions;
        private System.Windows.Forms.Button btnAnalyze;
        private System.Windows.Forms.Button btnAutoFix;
        private System.Windows.Forms.Button btnRemoveDups;
        private System.Windows.Forms.Button btnApplyChanges;
        private System.Windows.Forms.Button btnRevert;
        private System.Windows.Forms.Button btnExportReport;
        private System.Windows.Forms.Button btnSaveFile;
        private System.Windows.Forms.GroupBox grpStats;
        private System.Windows.Forms.Label lblStatRecords;
        private System.Windows.Forms.Label lblStatColumns;
        private System.Windows.Forms.Label lblStatErrors;
        private System.Windows.Forms.Label lblStatDups;
        private System.Windows.Forms.Label lblStatNulls;
        private System.Windows.Forms.Label lblStatFixed;
        private System.Windows.Forms.Label lblStatQuality;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Panel panelReport;
        private System.Windows.Forms.Label lblReportTitle;
        private System.Windows.Forms.RichTextBox richTextBoxReport;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatus;
    }
}
