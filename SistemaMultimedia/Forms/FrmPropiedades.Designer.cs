namespace SistemaMultimedia.Forms
{
    partial class FrmPropiedades
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

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmPropiedades));
            picIcon = new PictureBox();
            tabControl1 = new TabControl();
            tabGeneral = new TabPage();
            lblType = new Label();
            lblSize = new Label();
            lblAttributes = new Label();
            lblModified = new Label();
            lblCreated = new Label();
            lblFullPath = new Label();
            lblName = new Label();
            tabDetails = new TabPage();
            lblFilesCount = new Label();
            lblFoldersCount = new Label();
            lblImageResolution = new Label();
            lblImageFormat = new Label();
            lblImageDepth = new Label();
            btnAccept = new Button();
            btnCancel = new Button();
            ((System.ComponentModel.ISupportInitialize)picIcon).BeginInit();
            tabControl1.SuspendLayout();
            tabGeneral.SuspendLayout();
            tabDetails.SuspendLayout();
            SuspendLayout();
            // 
            // picIcon
            // 
            picIcon.Location = new Point(12, 12);
            picIcon.Name = "picIcon";
            picIcon.Size = new Size(48, 48);
            picIcon.SizeMode = PictureBoxSizeMode.CenterImage;
            picIcon.TabIndex = 0;
            picIcon.TabStop = false;
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(tabGeneral);
            tabControl1.Controls.Add(tabDetails);
            tabControl1.Location = new Point(12, 70);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(640, 355);
            tabControl1.TabIndex = 1;
            // 
            // tabGeneral
            // 
            tabGeneral.BackColor = Color.FromArgb(30, 30, 30);
            tabGeneral.Controls.Add(lblType);
            tabGeneral.Controls.Add(lblSize);
            tabGeneral.Controls.Add(lblAttributes);
            tabGeneral.Controls.Add(lblModified);
            tabGeneral.Controls.Add(lblCreated);
            tabGeneral.Controls.Add(lblFullPath);
            tabGeneral.Controls.Add(lblName);
            tabGeneral.ForeColor = Color.White;
            tabGeneral.Location = new Point(4, 34);
            tabGeneral.Name = "tabGeneral";
            tabGeneral.Padding = new Padding(3);
            tabGeneral.Size = new Size(632, 317);
            tabGeneral.TabIndex = 0;
            tabGeneral.Text = "General";
            // 
            // lblType
            // 
            lblType.AutoSize = true;
            lblType.Location = new Point(12, 170);
            lblType.Name = "lblType";
            lblType.Size = new Size(47, 25);
            lblType.TabIndex = 6;
            lblType.Text = "Tipo";
            // 
            // lblSize
            // 
            lblSize.AutoSize = true;
            lblSize.Location = new Point(12, 140);
            lblSize.Name = "lblSize";
            lblSize.Size = new Size(74, 25);
            lblSize.TabIndex = 5;
            lblSize.Text = "Tamaño";
            // 
            // lblAttributes
            // 
            lblAttributes.AutoSize = true;
            lblAttributes.Location = new Point(12, 110);
            lblAttributes.Name = "lblAttributes";
            lblAttributes.Size = new Size(86, 25);
            lblAttributes.TabIndex = 4;
            lblAttributes.Text = "Atributos";
            // 
            // lblModified
            // 
            lblModified.AutoSize = true;
            lblModified.Location = new Point(12, 80);
            lblModified.Name = "lblModified";
            lblModified.Size = new Size(153, 25);
            lblModified.TabIndex = 3;
            lblModified.Text = "Fecha modificado";
            // 
            // lblCreated
            // 
            lblCreated.AutoSize = true;
            lblCreated.Location = new Point(12, 50);
            lblCreated.Name = "lblCreated";
            lblCreated.Size = new Size(116, 25);
            lblCreated.TabIndex = 2;
            lblCreated.Text = "Fecha creado";
            // 
            // lblFullPath
            // 
            lblFullPath.AutoSize = true;
            lblFullPath.Location = new Point(12, 20);
            lblFullPath.Name = "lblFullPath";
            lblFullPath.Size = new Size(127, 25);
            lblFullPath.TabIndex = 1;
            lblFullPath.Text = "Ruta completa";
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblName.Location = new Point(12, 0);
            lblName.Name = "lblName";
            lblName.Size = new Size(81, 25);
            lblName.TabIndex = 0;
            lblName.Text = "Nombre";
            // 
            // tabDetails
            // 
            tabDetails.BackColor = Color.FromArgb(30, 30, 30);
            tabDetails.Controls.Add(lblFilesCount);
            tabDetails.Controls.Add(lblFoldersCount);
            tabDetails.Controls.Add(lblImageResolution);
            tabDetails.Controls.Add(lblImageFormat);
            tabDetails.Controls.Add(lblImageDepth);
            tabDetails.ForeColor = Color.White;
            tabDetails.Location = new Point(4, 34);
            tabDetails.Name = "tabDetails";
            tabDetails.Padding = new Padding(3);
            tabDetails.Size = new Size(632, 317);
            tabDetails.TabIndex = 1;
            tabDetails.Text = "Detalles";
            // 
            // lblFilesCount
            // 
            lblFilesCount.AutoSize = true;
            lblFilesCount.Location = new Point(12, 10);
            lblFilesCount.Name = "lblFilesCount";
            lblFilesCount.Size = new Size(99, 25);
            lblFilesCount.TabIndex = 0;
            lblFilesCount.Text = "Archivos: 0";
            // 
            // lblFoldersCount
            // 
            lblFoldersCount.AutoSize = true;
            lblFoldersCount.Location = new Point(12, 30);
            lblFoldersCount.Name = "lblFoldersCount";
            lblFoldersCount.Size = new Size(128, 25);
            lblFoldersCount.TabIndex = 1;
            lblFoldersCount.Text = "Subcarpetas: 0";
            // 
            // lblImageResolution
            // 
            lblImageResolution.AutoSize = true;
            lblImageResolution.Location = new Point(12, 60);
            lblImageResolution.Name = "lblImageResolution";
            lblImageResolution.Size = new Size(134, 25);
            lblImageResolution.TabIndex = 2;
            lblImageResolution.Text = "Resolución: 0x0";
            // 
            // lblImageFormat
            // 
            lblImageFormat.AutoSize = true;
            lblImageFormat.Location = new Point(12, 80);
            lblImageFormat.Name = "lblImageFormat";
            lblImageFormat.Size = new Size(127, 25);
            lblImageFormat.TabIndex = 3;
            lblImageFormat.Text = "Formato: (des)";
            // 
            // lblImageDepth
            // 
            lblImageDepth.AutoSize = true;
            lblImageDepth.Location = new Point(12, 100);
            lblImageDepth.Name = "lblImageDepth";
            lblImageDepth.Size = new Size(141, 25);
            lblImageDepth.TabIndex = 4;
            lblImageDepth.Text = "Profundidad: (d)";
            // 
            // btnAccept
            // 
            btnAccept.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnAccept.Location = new Point(430, 431);
            btnAccept.Name = "btnAccept";
            btnAccept.Size = new Size(108, 29);
            btnAccept.TabIndex = 2;
            btnAccept.Text = "Aceptar";
            btnAccept.UseVisualStyleBackColor = true;
            btnAccept.Click += btnAccept_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.Location = new Point(544, 431);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(108, 29);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "Cancelar";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // FrmPropiedades
            // 
            BackColor = Color.FromArgb(30, 30, 30);
            ClientSize = new Size(664, 466);
            Controls.Add(btnCancel);
            Controls.Add(btnAccept);
            Controls.Add(tabControl1);
            Controls.Add(picIcon);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmPropiedades";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Propiedades";
            ((System.ComponentModel.ISupportInitialize)picIcon).EndInit();
            tabControl1.ResumeLayout(false);
            tabGeneral.ResumeLayout(false);
            tabGeneral.PerformLayout();
            tabDetails.ResumeLayout(false);
            tabDetails.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picIcon;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.TabPage tabDetails;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblFullPath;
        private System.Windows.Forms.Label lblCreated;
        private System.Windows.Forms.Label lblModified;
        private System.Windows.Forms.Label lblAttributes;
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.Label lblFilesCount;
        private System.Windows.Forms.Label lblFoldersCount;
        private System.Windows.Forms.Label lblImageResolution;
        private System.Windows.Forms.Label lblImageFormat;
        private System.Windows.Forms.Label lblImageDepth;
    }
}
