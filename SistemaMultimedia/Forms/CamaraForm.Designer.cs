namespace SistemaMultimedia.Forms
{
    partial class CamaraForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.PictureBox pbCamera;
        private System.Windows.Forms.ComboBox comboBoxCameras;
        private System.Windows.Forms.ComboBox comboBoxMicrophones;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.Timer timerRecording;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Label labelRecordingTime;

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CamaraForm));
            pbCamera = new PictureBox();
            comboBoxCameras = new ComboBox();
            comboBoxMicrophones = new ComboBox();
            labelStatus = new Label();
            statusStrip = new StatusStrip();
            timerRecording = new System.Windows.Forms.Timer(components);
            saveFileDialog = new SaveFileDialog();
            openFileDialog = new OpenFileDialog();
            labelRecordingTime = new Label();
            pbTakePhoto = new PictureBox();
            pbStartRecording = new PictureBox();
            pbPreview = new PictureBox();
            pbStopRecording = new PictureBox();
            lblTakePhoto = new Label();
            lblStartRecording = new Label();
            lblStopRecording = new Label();
            ((System.ComponentModel.ISupportInitialize)pbCamera).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbTakePhoto).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbStartRecording).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbPreview).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbStopRecording).BeginInit();
            SuspendLayout();
            // 
            // pbCamera
            // 
            pbCamera.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pbCamera.Location = new Point(17, 31);
            pbCamera.Margin = new Padding(4, 5, 4, 5);
            pbCamera.Name = "pbCamera";
            pbCamera.Size = new Size(800, 600);
            pbCamera.SizeMode = PictureBoxSizeMode.Zoom;
            pbCamera.TabIndex = 0;
            pbCamera.TabStop = false;
            // 
            // comboBoxCameras
            // 
            comboBoxCameras.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            comboBoxCameras.BackColor = SystemColors.ButtonFace;
            comboBoxCameras.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxCameras.FormattingEnabled = true;
            comboBoxCameras.Location = new Point(17, 651);
            comboBoxCameras.Margin = new Padding(4, 5, 4, 5);
            comboBoxCameras.Name = "comboBoxCameras";
            comboBoxCameras.Size = new Size(284, 33);
            comboBoxCameras.TabIndex = 1;
            // 
            // comboBoxMicrophones
            // 
            comboBoxMicrophones.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            comboBoxMicrophones.BackColor = SystemColors.ButtonFace;
            comboBoxMicrophones.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxMicrophones.FormattingEnabled = true;
            comboBoxMicrophones.Location = new Point(533, 651);
            comboBoxMicrophones.Margin = new Padding(4, 5, 4, 5);
            comboBoxMicrophones.Name = "comboBoxMicrophones";
            comboBoxMicrophones.Size = new Size(284, 33);
            comboBoxMicrophones.TabIndex = 2;
            // 
            // labelStatus
            // 
            labelStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelStatus.AutoSize = true;
            labelStatus.ForeColor = SystemColors.Control;
            labelStatus.Location = new Point(17, 1);
            labelStatus.Margin = new Padding(4, 0, 4, 0);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(66, 25);
            labelStatus.TabIndex = 8;
            labelStatus.Text = "Estado";
            // 
            // statusStrip
            // 
            statusStrip.ImageScalingSize = new Size(24, 24);
            statusStrip.Location = new Point(0, 728);
            statusStrip.Name = "statusStrip";
            statusStrip.Padding = new Padding(1, 0, 20, 0);
            statusStrip.Size = new Size(1149, 22);
            statusStrip.TabIndex = 9;
            statusStrip.Text = "statusStrip1";
            // 
            // timerRecording
            // 
            timerRecording.Interval = 1000;
            timerRecording.Tick += timerRecording_Tick;
            // 
            // saveFileDialog
            // 
            saveFileDialog.Filter = "Video MP4|*.mp4|Video AVI|*.avi|Imagen JPG|*.jpg|Imagen PNG|*.png";
            // 
            // openFileDialog
            // 
            openFileDialog.Filter = "Todos los archivos|*.*";
            // 
            // labelRecordingTime
            // 
            labelRecordingTime.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelRecordingTime.AutoSize = true;
            labelRecordingTime.ForeColor = SystemColors.Control;
            labelRecordingTime.Location = new Point(377, 659);
            labelRecordingTime.Margin = new Padding(4, 0, 4, 0);
            labelRecordingTime.Name = "labelRecordingTime";
            labelRecordingTime.Size = new Size(80, 25);
            labelRecordingTime.TabIndex = 10;
            labelRecordingTime.Text = "00:00:00";
            // 
            // pbTakePhoto
            // 
            pbTakePhoto.Cursor = Cursors.Hand;
            pbTakePhoto.Image = (Image)resources.GetObject("pbTakePhoto.Image");
            pbTakePhoto.Location = new Point(898, 73);
            pbTakePhoto.Name = "pbTakePhoto";
            pbTakePhoto.Size = new Size(159, 79);
            pbTakePhoto.SizeMode = PictureBoxSizeMode.Zoom;
            pbTakePhoto.TabIndex = 11;
            pbTakePhoto.TabStop = false;
            pbTakePhoto.Click += pbTakePhoto_Click;
            // 
            // pbStartRecording
            // 
            pbStartRecording.Cursor = Cursors.Hand;
            pbStartRecording.Image = Properties.Resources.rec;
            pbStartRecording.Location = new Point(898, 201);
            pbStartRecording.Name = "pbStartRecording";
            pbStartRecording.Size = new Size(159, 79);
            pbStartRecording.SizeMode = PictureBoxSizeMode.Zoom;
            pbStartRecording.TabIndex = 12;
            pbStartRecording.TabStop = false;
            pbStartRecording.Click += pbStartRecording_Click;
            // 
            // pbPreview
            // 
            pbPreview.BackColor = Color.FromArgb(35, 35, 35);
            pbPreview.Cursor = Cursors.Hand;
            pbPreview.Location = new Point(865, 511);
            pbPreview.Name = "pbPreview";
            pbPreview.Size = new Size(230, 173);
            pbPreview.SizeMode = PictureBoxSizeMode.Zoom;
            pbPreview.TabIndex = 13;
            pbPreview.TabStop = false;
            pbPreview.Click += pbPreview_Click;
            pbPreview.DoubleClick += pbPreview_DoubleClick;
            // 
            // pbStopRecording
            // 
            pbStopRecording.Cursor = Cursors.Hand;
            pbStopRecording.Image = Properties.Resources.stop_camera;
            pbStopRecording.Location = new Point(898, 333);
            pbStopRecording.Name = "pbStopRecording";
            pbStopRecording.Size = new Size(159, 79);
            pbStopRecording.SizeMode = PictureBoxSizeMode.Zoom;
            pbStopRecording.TabIndex = 14;
            pbStopRecording.TabStop = false;
            pbStopRecording.Click += pbStopRecording_Click;
            // 
            // lblTakePhoto
            // 
            lblTakePhoto.AutoSize = true;
            lblTakePhoto.ForeColor = Color.White;
            lblTakePhoto.Location = new Point(926, 37);
            lblTakePhoto.Name = "lblTakePhoto";
            lblTakePhoto.Size = new Size(103, 25);
            lblTakePhoto.TabIndex = 16;
            lblTakePhoto.Text = "Tomar Foto";
            // 
            // lblStartRecording
            // 
            lblStartRecording.AutoSize = true;
            lblStartRecording.ForeColor = Color.White;
            lblStartRecording.Location = new Point(906, 165);
            lblStartRecording.Name = "lblStartRecording";
            lblStartRecording.Size = new Size(143, 25);
            lblStartRecording.TabIndex = 17;
            lblStartRecording.Text = "Iniciar Grabación";
            // 
            // lblStopRecording
            // 
            lblStopRecording.AutoSize = true;
            lblStopRecording.ForeColor = Color.White;
            lblStopRecording.Location = new Point(898, 297);
            lblStopRecording.Name = "lblStopRecording";
            lblStopRecording.Size = new Size(159, 25);
            lblStopRecording.TabIndex = 18;
            lblStopRecording.Text = "Detener Grabación";
            // 
            // CamaraForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 30);
            ClientSize = new Size(1149, 750);
            Controls.Add(lblStopRecording);
            Controls.Add(lblStartRecording);
            Controls.Add(lblTakePhoto);
            Controls.Add(pbStopRecording);
            Controls.Add(pbPreview);
            Controls.Add(pbStartRecording);
            Controls.Add(pbTakePhoto);
            Controls.Add(labelRecordingTime);
            Controls.Add(statusStrip);
            Controls.Add(labelStatus);
            Controls.Add(comboBoxMicrophones);
            Controls.Add(comboBoxCameras);
            Controls.Add(pbCamera);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 5, 4, 5);
            MinimumSize = new Size(1162, 778);
            Name = "CamaraForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Cámara";
            Load += CamaraForm_Load;
            ((System.ComponentModel.ISupportInitialize)pbCamera).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbTakePhoto).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbStartRecording).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbPreview).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbStopRecording).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private PictureBox pbTakePhoto;
        private PictureBox pbStartRecording;
        private PictureBox pbPreview;
        private PictureBox pbStopRecording;
        private Label lblTakePhoto;
        private Label lblStartRecording;
        private Label lblStopRecording;
    }
}
