namespace SistemaMultimedia.Forms.ImageEditor;

partial class ImageEditorForm
{
    private System.ComponentModel.IContainer? components = null;
    private Button buttonOpen = null!;
    private Panel rightPanel = null!;
    private Button btnGrayscale = null!;
    private Button btnSepia = null!;
    private Button btnResize = null!;
    private TrackBar trackBarBrightness = null!;
    private TrackBar trackBarContrast = null!;
    private Label lblBrightness = null!;
    private Label lblContrast = null!;
    private Button btnSaveFormat = null!;
    private Button btnAddText = null!;
    private Button btnDrawFree = null!;
    private PictureBox pictureBoxImage = null!;
    private TextBox txtPath = null!;
    private Button btnGeolocalizacion = null!;
    private Label lblDeshacer = null!;
    private Label lblRehacer = null!;
    private Label lblRotar = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageEditorForm));
        buttonOpen = new Button();
        rightPanel = new Panel();
        pbRedoMenu = new PictureBox();
        pbUndoMenu = new PictureBox();
        lblRotar = new Label();
        lblRehacer = new Label();
        lblDeshacer = new Label();
        btnGeolocalizacion = new Button();
        btnGrayscale = new Button();
        btnSepia = new Button();
        btnSaveFormat = new Button();
        btnDrawFree = new Button();
        btnAddText = new Button();
        btnResize = new Button();
        lblBrightness = new Label();
        lblContrast = new Label();
        trackBarBrightness = new TrackBar();
        trackBarContrast = new TrackBar();
        pictureBoxImage = new PictureBox();
        txtPath = new TextBox();
        pbRotate90 = new PictureBox();
        pictureBox1 = new PictureBox();
        pictureBox2 = new PictureBox();
        rightPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pbRedoMenu).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pbUndoMenu).BeginInit();
        ((System.ComponentModel.ISupportInitialize)trackBarBrightness).BeginInit();
        ((System.ComponentModel.ISupportInitialize)trackBarContrast).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pictureBoxImage).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pbRotate90).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
        SuspendLayout();
        // 
        // buttonOpen
        // 
        buttonOpen.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        buttonOpen.BackColor = Color.White;
        buttonOpen.ForeColor = Color.Black;
        buttonOpen.Location = new Point(988, 14);
        buttonOpen.Name = "buttonOpen";
        buttonOpen.Size = new Size(94, 31);
        buttonOpen.TabIndex = 2;
        buttonOpen.Text = "Abrir...";
        buttonOpen.UseVisualStyleBackColor = false;
        buttonOpen.Click += buttonOpen_Click;
        // 
        // rightPanel
        // 
        rightPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        rightPanel.BackColor = Color.FromArgb(30, 30, 30);
        rightPanel.Controls.Add(pictureBox2);
        rightPanel.Controls.Add(pictureBox1);
        rightPanel.Controls.Add(pbRotate90);
        rightPanel.Controls.Add(pbRedoMenu);
        rightPanel.Controls.Add(pbUndoMenu);
        rightPanel.Controls.Add(lblRotar);
        rightPanel.Controls.Add(lblRehacer);
        rightPanel.Controls.Add(lblDeshacer);
        rightPanel.Controls.Add(btnGeolocalizacion);
        rightPanel.Controls.Add(btnGrayscale);
        rightPanel.Controls.Add(btnSepia);
        rightPanel.Controls.Add(btnSaveFormat);
        rightPanel.Controls.Add(btnDrawFree);
        rightPanel.Controls.Add(btnAddText);
        rightPanel.Controls.Add(btnResize);
        rightPanel.Controls.Add(lblBrightness);
        rightPanel.Controls.Add(lblContrast);
        rightPanel.Controls.Add(trackBarBrightness);
        rightPanel.Controls.Add(trackBarContrast);
        rightPanel.Location = new Point(985, 56);
        rightPanel.Name = "rightPanel";
        rightPanel.Size = new Size(250, 644);
        rightPanel.TabIndex = 0;
        // 
        // pbRedoMenu
        // 
        pbRedoMenu.Cursor = Cursors.Hand;
        pbRedoMenu.Image = (Image)resources.GetObject("pbRedoMenu.Image");
        pbRedoMenu.Location = new Point(143, 539);
        pbRedoMenu.Name = "pbRedoMenu";
        pbRedoMenu.Size = new Size(79, 38);
        pbRedoMenu.SizeMode = PictureBoxSizeMode.Zoom;
        pbRedoMenu.TabIndex = 20;
        pbRedoMenu.TabStop = false;
        pbRedoMenu.Click += RedoMenu_Click;
        // 
        // pbUndoMenu
        // 
        pbUndoMenu.Cursor = Cursors.Hand;
        pbUndoMenu.Image = Properties.Resources.retroceder;
        pbUndoMenu.Location = new Point(26, 539);
        pbUndoMenu.Name = "pbUndoMenu";
        pbUndoMenu.Size = new Size(79, 38);
        pbUndoMenu.SizeMode = PictureBoxSizeMode.Zoom;
        pbUndoMenu.TabIndex = 19;
        pbUndoMenu.TabStop = false;
        pbUndoMenu.Click += UndoMenu_Click;
        // 
        // lblRotar
        // 
        lblRotar.ForeColor = Color.White;
        lblRotar.Location = new Point(12, 108);
        lblRotar.Name = "lblRotar";
        lblRotar.Size = new Size(225, 24);
        lblRotar.TabIndex = 0;
        lblRotar.Text = "Rotar";
        // 
        // lblRehacer
        // 
        lblRehacer.ForeColor = Color.White;
        lblRehacer.Location = new Point(149, 510);
        lblRehacer.Name = "lblRehacer";
        lblRehacer.Size = new Size(73, 26);
        lblRehacer.TabIndex = 1;
        lblRehacer.Text = "Rehacer";
        // 
        // lblDeshacer
        // 
        lblDeshacer.ForeColor = Color.White;
        lblDeshacer.Location = new Point(26, 510);
        lblDeshacer.Name = "lblDeshacer";
        lblDeshacer.Size = new Size(92, 26);
        lblDeshacer.TabIndex = 2;
        lblDeshacer.Text = "Deshacer";
        // 
        // btnGeolocalizacion
        // 
        btnGeolocalizacion.BackColor = Color.White;
        btnGeolocalizacion.ForeColor = Color.Black;
        btnGeolocalizacion.Location = new Point(12, 596);
        btnGeolocalizacion.Name = "btnGeolocalizacion";
        btnGeolocalizacion.Size = new Size(225, 34);
        btnGeolocalizacion.TabIndex = 7;
        btnGeolocalizacion.Text = "Geolocalización";
        btnGeolocalizacion.UseVisualStyleBackColor = false;
        btnGeolocalizacion.Click += BtnGeolocalizacion_Click;
        // 
        // btnGrayscale
        // 
        btnGrayscale.BackColor = Color.White;
        btnGrayscale.ForeColor = Color.Black;
        btnGrayscale.Location = new Point(12, 12);
        btnGrayscale.Name = "btnGrayscale";
        btnGrayscale.Size = new Size(225, 38);
        btnGrayscale.TabIndex = 8;
        btnGrayscale.Text = "Blanco y Negro";
        btnGrayscale.UseVisualStyleBackColor = false;
        btnGrayscale.Click += MenuGrayscale_Click;
        // 
        // btnSepia
        // 
        btnSepia.BackColor = Color.White;
        btnSepia.ForeColor = Color.Black;
        btnSepia.Location = new Point(12, 62);
        btnSepia.Name = "btnSepia";
        btnSepia.Size = new Size(225, 38);
        btnSepia.TabIndex = 9;
        btnSepia.Text = "Sepia";
        btnSepia.UseVisualStyleBackColor = false;
        btnSepia.Click += MenuSepia_Click;
        // 
        // btnSaveFormat
        // 
        btnSaveFormat.BackColor = Color.White;
        btnSaveFormat.ForeColor = Color.Black;
        btnSaveFormat.Location = new Point(12, 468);
        btnSaveFormat.Name = "btnSaveFormat";
        btnSaveFormat.Size = new Size(225, 38);
        btnSaveFormat.TabIndex = 10;
        btnSaveFormat.Text = "Guardar";
        btnSaveFormat.UseVisualStyleBackColor = false;
        btnSaveFormat.Click += MenuSave_Click;
        // 
        // btnDrawFree
        // 
        btnDrawFree.BackColor = Color.White;
        btnDrawFree.ForeColor = Color.Black;
        btnDrawFree.Location = new Point(12, 376);
        btnDrawFree.Name = "btnDrawFree";
        btnDrawFree.Size = new Size(225, 38);
        btnDrawFree.TabIndex = 12;
        btnDrawFree.Text = "Dibujar libre";
        btnDrawFree.UseVisualStyleBackColor = false;
        btnDrawFree.Click += MenuDrawLine_Click;
        // 
        // btnAddText
        // 
        btnAddText.BackColor = Color.White;
        btnAddText.ForeColor = Color.Black;
        btnAddText.Location = new Point(12, 422);
        btnAddText.Name = "btnAddText";
        btnAddText.Size = new Size(225, 38);
        btnAddText.TabIndex = 13;
        btnAddText.Text = "Añadir texto";
        btnAddText.UseVisualStyleBackColor = false;
        btnAddText.Click += MenuAddText_Click;
        // 
        // btnResize
        // 
        btnResize.BackColor = Color.White;
        btnResize.ForeColor = Color.Black;
        btnResize.Location = new Point(12, 189);
        btnResize.Name = "btnResize";
        btnResize.Size = new Size(225, 38);
        btnResize.TabIndex = 14;
        btnResize.Text = "Redimensionar";
        btnResize.UseVisualStyleBackColor = false;
        btnResize.Click += MenuResize_Click;
        // 
        // lblBrightness
        // 
        lblBrightness.ForeColor = Color.White;
        lblBrightness.Location = new Point(12, 237);
        lblBrightness.Name = "lblBrightness";
        lblBrightness.Size = new Size(225, 26);
        lblBrightness.TabIndex = 15;
        lblBrightness.Text = "Brillo: 0";
        // 
        // lblContrast
        // 
        lblContrast.ForeColor = Color.White;
        lblContrast.Location = new Point(12, 302);
        lblContrast.Name = "lblContrast";
        lblContrast.Size = new Size(225, 24);
        lblContrast.TabIndex = 16;
        lblContrast.Text = "Contraste: 0";
        // 
        // trackBarBrightness
        // 
        trackBarBrightness.Location = new Point(12, 257);
        trackBarBrightness.Maximum = 100;
        trackBarBrightness.Minimum = -100;
        trackBarBrightness.Name = "trackBarBrightness";
        trackBarBrightness.Size = new Size(225, 69);
        trackBarBrightness.TabIndex = 17;
        trackBarBrightness.TickFrequency = 10;
        trackBarBrightness.Scroll += trackBarBrightness_Scroll;
        // 
        // trackBarContrast
        // 
        trackBarContrast.Location = new Point(12, 325);
        trackBarContrast.Maximum = 100;
        trackBarContrast.Minimum = -100;
        trackBarContrast.Name = "trackBarContrast";
        trackBarContrast.Size = new Size(225, 69);
        trackBarContrast.TabIndex = 18;
        trackBarContrast.TickFrequency = 10;
        trackBarContrast.Scroll += trackBarContrast_Scroll;
        // 
        // pictureBoxImage
        // 
        pictureBoxImage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        pictureBoxImage.BackColor = Color.FromArgb(30, 30, 30);
        pictureBoxImage.Location = new Point(15, 56);
        pictureBoxImage.Name = "pictureBoxImage";
        pictureBoxImage.Size = new Size(950, 644);
        pictureBoxImage.SizeMode = PictureBoxSizeMode.Zoom;
        pictureBoxImage.TabIndex = 3;
        pictureBoxImage.TabStop = false;
        pictureBoxImage.Click += pictureBoxImage_Click;
        pictureBoxImage.MouseDown += pictureBoxImage_MouseDown;
        pictureBoxImage.MouseMove += pictureBoxImage_MouseMove;
        pictureBoxImage.MouseUp += pictureBoxImage_MouseUp;
        // 
        // txtPath
        // 
        txtPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtPath.Location = new Point(15, 14);
        txtPath.Name = "txtPath";
        txtPath.PlaceholderText = "Seleccione una imagen con Abrir...";
        txtPath.ReadOnly = true;
        txtPath.Size = new Size(949, 31);
        txtPath.TabIndex = 1;
        // 
        // pbRotate90
        // 
        pbRotate90.Cursor = Cursors.Hand;
        pbRotate90.Image = Properties.Resources.retroceder;
        pbRotate90.Location = new Point(17, 135);
        pbRotate90.Name = "pbRotate90";
        pbRotate90.Size = new Size(69, 38);
        pbRotate90.SizeMode = PictureBoxSizeMode.Zoom;
        pbRotate90.TabIndex = 20;
        pbRotate90.TabStop = false;
        pbRotate90.Click += btnRotate90_Click;
        // 
        // pictureBox1
        // 
        pictureBox1.Cursor = Cursors.Hand;
        pictureBox1.Image = Properties.Resources.adelantar;
        pictureBox1.Location = new Point(168, 135);
        pictureBox1.Name = "pictureBox1";
        pictureBox1.Size = new Size(68, 38);
        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        pictureBox1.TabIndex = 21;
        pictureBox1.TabStop = false;
        pictureBox1.Click += btnRotate270_Click;
        // 
        // pictureBox2
        // 
        pictureBox2.Cursor = Cursors.Hand;
        pictureBox2.Image = Properties.Resources.trim;
        pictureBox2.Location = new Point(93, 135);
        pictureBox2.Name = "pictureBox2";
        pictureBox2.Size = new Size(68, 38);
        pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
        pictureBox2.TabIndex = 22;
        pictureBox2.TabStop = false;
        pictureBox2.Click += btnRotate180_Click;
        // 
        // ImageEditorForm
        // 
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(30, 30, 30);
        ClientSize = new Size(1250, 713);
        Controls.Add(rightPanel);
        Controls.Add(txtPath);
        Controls.Add(pictureBoxImage);
        Controls.Add(buttonOpen);
        // NOTE: runtime UI adjustments moved to constructor to keep designer stable
        ForeColor = Color.White;
        Icon = (Icon)resources.GetObject("$this.Icon");
        Name = "ImageEditorForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Editor de Imágenes";
        rightPanel.ResumeLayout(false);
        rightPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pbRedoMenu).EndInit();
        ((System.ComponentModel.ISupportInitialize)pbUndoMenu).EndInit();
        ((System.ComponentModel.ISupportInitialize)trackBarBrightness).EndInit();
        ((System.ComponentModel.ISupportInitialize)trackBarContrast).EndInit();
        ((System.ComponentModel.ISupportInitialize)pictureBoxImage).EndInit();
        ((System.ComponentModel.ISupportInitialize)pbRotate90).EndInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private PictureBox pbRedoMenu;
    private PictureBox pbUndoMenu;
    private PictureBox pictureBox2;
    private PictureBox pictureBox1;
    private PictureBox pbRotate90;
}
