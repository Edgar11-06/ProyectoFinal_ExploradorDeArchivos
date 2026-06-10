// ─────────────────────────────────────────────────────────────────────────────
// Archivo generado por el diseñador — MainForm.Designer.cs
// ─────────────────────────────────────────────────────────────────────────────
namespace VisorEditorDocumentos
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null!;

        private System.Windows.Forms.Panel       panelToolbar;
        private System.Windows.Forms.Panel       panelPdfNav;
        private System.Windows.Forms.Panel       panelContent;
        private System.Windows.Forms.Panel       panelStatus;

        private System.Windows.Forms.Panel       panelWordTools;
        private System.Windows.Forms.Button      btnWordEdit;
        private System.Windows.Forms.Button      btnWordPreview;

        private System.Windows.Forms.Button      btnOpen;
        private System.Windows.Forms.Button      btnSave;
        private System.Windows.Forms.Button      btnSaveAs;
        private System.Windows.Forms.Label       lblFileName;
        private System.Windows.Forms.ToolTip     toolTip;

        private System.Windows.Forms.Button      btnFirst;
        private System.Windows.Forms.Button      btnPrev;
        private System.Windows.Forms.Button      btnNext;
        private System.Windows.Forms.Button      btnLast;
        private System.Windows.Forms.Button      btnZoomIn;
        private System.Windows.Forms.Button      btnZoomOut;
        private System.Windows.Forms.Label       lblPageInfo;
        private System.Windows.Forms.Label       lblZoomInfo;

        private System.Windows.Forms.Label       lblStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            toolTip = new ToolTip(components);
            btnOpen = new Button();
            btnSave = new Button();
            btnSaveAs = new Button();
            btnWordEdit = new Button();
            btnWordPreview = new Button();
            btnFirst = new Button();
            btnPrev = new Button();
            btnNext = new Button();
            btnLast = new Button();
            btnZoomOut = new Button();
            btnZoomIn = new Button();
            _documentViewer = new VisorEditorDocumentos.Controls.BrowserDocumentViewerControl();
            panelContent = new Panel();
            panelWordTools = new Panel();
            panelToolbar = new Panel();
            lblFileName = new Label();
            panelPdfNav = new Panel();
            lblSep = new Label();
            lblPageInfo = new Label();
            lblZoomInfo = new Label();
            panelStatus = new Panel();
            lblStatus = new Label();
            panelContent.SuspendLayout();
            panelWordTools.SuspendLayout();
            panelToolbar.SuspendLayout();
            panelPdfNav.SuspendLayout();
            panelStatus.SuspendLayout();
            SuspendLayout();
            // 
            // btnOpen
            // 
            btnOpen.BackColor = Color.White;
            btnOpen.Location = new Point(8, 9);
            btnOpen.Name = "btnOpen";
            btnOpen.Size = new Size(160, 34);
            btnOpen.TabIndex = 0;
            btnOpen.Text = "📂  Abrir Documento";
            toolTip.SetToolTip(btnOpen, "Abrir un archivo PDF o Word (.docx)");
            btnOpen.UseVisualStyleBackColor = false;
            btnOpen.Click += btnOpen_Click;
            // 
            // btnSave
            // 
            btnSave.BackColor = Color.White;
            btnSave.Location = new Point(178, 9);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(110, 34);
            btnSave.TabIndex = 1;
            btnSave.Text = "💾  Guardar";
            toolTip.SetToolTip(btnSave, "Guardar cambios en el documento Word actual");
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            // 
            // btnSaveAs
            // 
            btnSaveAs.BackColor = Color.White;
            btnSaveAs.Location = new Point(298, 9);
            btnSaveAs.Name = "btnSaveAs";
            btnSaveAs.Size = new Size(140, 34);
            btnSaveAs.TabIndex = 2;
            btnSaveAs.Text = "📄  Guardar Como";
            toolTip.SetToolTip(btnSaveAs, "Guardar el documento Word como otro archivo .docx");
            btnSaveAs.UseVisualStyleBackColor = false;
            btnSaveAs.Click += btnSaveAs_Click;
            // 
            // btnWordEdit
            // 
            btnWordEdit.BackColor = Color.White;
            btnWordEdit.Enabled = false;
            btnWordEdit.Location = new Point(8, 6);
            btnWordEdit.Name = "btnWordEdit";
            btnWordEdit.Size = new Size(130, 32);
            btnWordEdit.TabIndex = 0;
            btnWordEdit.Text = "✏  Editar";
            toolTip.SetToolTip(btnWordEdit, "Volver al modo edición");
            btnWordEdit.UseVisualStyleBackColor = false;
            btnWordEdit.Click += btnWordEdit_Click;
            // 
            // btnWordPreview
            // 
            btnWordPreview.BackColor = Color.White;
            btnWordPreview.Location = new Point(148, 6);
            btnWordPreview.Name = "btnWordPreview";
            btnWordPreview.Size = new Size(160, 32);
            btnWordPreview.TabIndex = 1;
            btnWordPreview.Text = "👁  Vista previa";
            toolTip.SetToolTip(btnWordPreview, "Ver el documento sin editar (solo lectura)");
            btnWordPreview.UseVisualStyleBackColor = false;
            btnWordPreview.Click += btnWordPreview_Click;
            // 
            // btnFirst
            // 
            btnFirst.BackColor = Color.White;
            btnFirst.ForeColor = Color.Black;
            btnFirst.Location = new Point(12, 8);
            btnFirst.Name = "btnFirst";
            btnFirst.Size = new Size(43, 35);
            btnFirst.TabIndex = 0;
            btnFirst.Text = "⏮";
            btnFirst.UseVisualStyleBackColor = false;
            // 
            // btnPrev
            // 
            btnPrev.BackColor = Color.White;
            btnPrev.ForeColor = Color.Black;
            btnPrev.Location = new Point(63, 8);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(43, 35);
            btnPrev.TabIndex = 1;
            btnPrev.Text = "◀";
            btnPrev.UseVisualStyleBackColor = false;
            // 
            // btnNext
            // 
            btnNext.BackColor = Color.White;
            btnNext.Location = new Point(259, 8);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(43, 35);
            btnNext.TabIndex = 3;
            btnNext.Text = "▶";
            btnNext.UseVisualStyleBackColor = false;
            // 
            // btnLast
            // 
            btnLast.BackColor = Color.White;
            btnLast.Location = new Point(310, 8);
            btnLast.Name = "btnLast";
            btnLast.Size = new Size(43, 35);
            btnLast.TabIndex = 4;
            btnLast.Text = "⏭";
            btnLast.UseVisualStyleBackColor = false;
            // 
            // btnZoomOut
            // 
            btnZoomOut.BackColor = Color.White;
            btnZoomOut.Location = new Point(361, 8);
            btnZoomOut.Name = "btnZoomOut";
            btnZoomOut.Size = new Size(67, 35);
            btnZoomOut.TabIndex = 6;
            btnZoomOut.Text = "🔍 −";
            btnZoomOut.UseVisualStyleBackColor = false;
            // 
            // btnZoomIn
            // 
            btnZoomIn.BackColor = Color.White;
            btnZoomIn.Location = new Point(506, 8);
            btnZoomIn.Name = "btnZoomIn";
            btnZoomIn.Size = new Size(67, 35);
            btnZoomIn.TabIndex = 8;
            btnZoomIn.Text = "🔍 +";
            btnZoomIn.UseVisualStyleBackColor = false;
            // 
            // _documentViewer
            // 
            _documentViewer.BackColor = Color.FromArgb(55, 55, 55);
            _documentViewer.Dock = DockStyle.Fill;
            _documentViewer.Location = new Point(0, 0);
            _documentViewer.Name = "_documentViewer";
            _documentViewer.Size = new Size(1178, 601);
            _documentViewer.TabIndex = 0;
            // 
            // panelContent
            // 
            panelContent.Controls.Add(_documentViewer);
            panelContent.Dock = DockStyle.Fill;
            panelContent.Location = new Point(0, 97);
            panelContent.Name = "panelContent";
            panelContent.Size = new Size(1178, 601);
            panelContent.TabIndex = 0;
            // 
            // panelWordTools
            // 
            panelWordTools.Controls.Add(btnWordEdit);
            panelWordTools.Controls.Add(btnWordPreview);
            panelWordTools.Dock = DockStyle.Top;
            panelWordTools.Location = new Point(0, 52);
            panelWordTools.Name = "panelWordTools";
            panelWordTools.Padding = new Padding(8, 4, 8, 4);
            panelWordTools.Size = new Size(1178, 45);
            panelWordTools.TabIndex = 4;
            panelWordTools.Visible = false;
            // 
            // panelToolbar
            // 
            panelToolbar.Controls.Add(btnOpen);
            panelToolbar.Controls.Add(btnSave);
            panelToolbar.Controls.Add(btnSaveAs);
            panelToolbar.Controls.Add(lblFileName);
            panelToolbar.Dock = DockStyle.Top;
            panelToolbar.Location = new Point(0, 0);
            panelToolbar.Name = "panelToolbar";
            panelToolbar.Padding = new Padding(8, 8, 8, 4);
            panelToolbar.Size = new Size(1178, 52);
            panelToolbar.TabIndex = 2;
            // 
            // lblFileName
            // 
            lblFileName.Font = new Font("Segoe UI", 9.5F, FontStyle.Italic);
            lblFileName.ForeColor = Color.White;
            lblFileName.Location = new Point(450, 9);
            lblFileName.Name = "lblFileName";
            lblFileName.Size = new Size(700, 34);
            lblFileName.TabIndex = 3;
            lblFileName.Text = "Ningún archivo abierto";
            lblFileName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panelPdfNav
            // 
            panelPdfNav.Controls.Add(lblSep);
            panelPdfNav.Controls.Add(btnFirst);
            panelPdfNav.Controls.Add(btnPrev);
            panelPdfNav.Controls.Add(lblPageInfo);
            panelPdfNav.Controls.Add(btnNext);
            panelPdfNav.Controls.Add(btnLast);
            panelPdfNav.Controls.Add(btnZoomOut);
            panelPdfNav.Controls.Add(lblZoomInfo);
            panelPdfNav.Controls.Add(btnZoomIn);
            panelPdfNav.Dock = DockStyle.Top;
            panelPdfNav.Location = new Point(0, 52);
            panelPdfNav.Name = "panelPdfNav";
            panelPdfNav.Padding = new Padding(8, 5, 8, 5);
            panelPdfNav.Size = new Size(1178, 0);
            panelPdfNav.TabIndex = 1;
            panelPdfNav.Visible = false;
            // 
            // lblSep
            // 
            lblSep.Location = new Point(650, 8);
            lblSep.Name = "lblSep";
            lblSep.Size = new Size(107, 28);
            lblSep.TabIndex = 5;
            // 
            // lblPageInfo
            // 
            lblPageInfo.Font = new Font("Segoe UI", 9F);
            lblPageInfo.ForeColor = Color.White;
            lblPageInfo.Location = new Point(114, 8);
            lblPageInfo.Name = "lblPageInfo";
            lblPageInfo.Size = new Size(137, 35);
            lblPageInfo.TabIndex = 2;
            lblPageInfo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblZoomInfo
            // 
            lblZoomInfo.Font = new Font("Segoe UI", 9F);
            lblZoomInfo.ForeColor = Color.White;
            lblZoomInfo.Location = new Point(436, 8);
            lblZoomInfo.Name = "lblZoomInfo";
            lblZoomInfo.Size = new Size(62, 35);
            lblZoomInfo.TabIndex = 7;
            lblZoomInfo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panelStatus
            // 
            panelStatus.BackColor = Color.FromArgb(20, 20, 20);
            panelStatus.Controls.Add(lblStatus);
            panelStatus.Dock = DockStyle.Bottom;
            panelStatus.Location = new Point(0, 698);
            panelStatus.Name = "panelStatus";
            panelStatus.Size = new Size(1178, 26);
            panelStatus.TabIndex = 3;
            // 
            // lblStatus
            // 
            lblStatus.Dock = DockStyle.Fill;
            lblStatus.Font = new Font("Segoe UI", 8.5F);
            lblStatus.ForeColor = Color.FromArgb(160, 160, 160);
            lblStatus.Location = new Point(0, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Padding = new Padding(8, 0, 0, 0);
            lblStatus.Size = new Size(1178, 26);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "Bienvenido. Haga clic en 'Abrir Documento' para comenzar.";
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // MainForm
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 30);
            ClientSize = new Size(1178, 724);
            Controls.Add(panelContent);
            Controls.Add(panelWordTools);
            Controls.Add(panelPdfNav);
            Controls.Add(panelToolbar);
            Controls.Add(panelStatus);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(800, 500);
            Name = "MainForm";
            StartPosition = FormStartPosition.Manual;
            Text = "Visor y Editor de Documentos";
            WindowState = FormWindowState.Maximized;
            panelContent.ResumeLayout(false);
            panelWordTools.ResumeLayout(false);
            panelToolbar.ResumeLayout(false);
            panelPdfNav.ResumeLayout(false);
            panelStatus.ResumeLayout(false);
            ResumeLayout(false);
        }

        private Label lblSep;
    }
}
