namespace SistemaMultimedia.Forms
{
    partial class ReproductorVideoForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReproductorVideoForm));
            videoView = new LibVLCSharp.WinForms.VideoView();
            panelControles = new Panel();
            pbMute = new PictureBox();
            comboVelocidad = new ComboBox();
            lblVelocidad = new Label();
            lblVolumen = new Label();
            trackBarVolumen = new TrackBar();
            pictureBox1 = new PictureBox();
            pbAvanzar = new PictureBox();
            pbPlay = new PictureBox();
            pbRetroceder = new PictureBox();
            lblInfo = new Label();
            lblTiempo = new Label();
            trackBarProgress = new TrackBar();
            btnAbrir = new Button();
            ((System.ComponentModel.ISupportInitialize)videoView).BeginInit();
            panelControles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbMute).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarVolumen).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbAvanzar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbPlay).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbRetroceder).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarProgress).BeginInit();
            SuspendLayout();
            // 
            // videoView
            // 
            videoView.BackColor = Color.Black;
            videoView.Dock = DockStyle.Fill;
            videoView.Location = new Point(0, 0);
            videoView.Margin = new Padding(4, 5, 4, 5);
            videoView.MediaPlayer = null;
            videoView.Name = "videoView";
            videoView.Size = new Size(1844, 897);
            videoView.TabIndex = 0;
            videoView.Text = "videoView";
            videoView.DoubleClick += VideoView_DoubleClick;
            videoView.KeyDown += VideoView_KeyDown;
            // 
            // panelControles
            // 
            panelControles.BackColor = Color.FromArgb(30, 30, 30);
            panelControles.Controls.Add(pbMute);
            panelControles.Controls.Add(comboVelocidad);
            panelControles.Controls.Add(lblVelocidad);
            panelControles.Controls.Add(lblVolumen);
            panelControles.Controls.Add(trackBarVolumen);
            panelControles.Controls.Add(pictureBox1);
            panelControles.Controls.Add(pbAvanzar);
            panelControles.Controls.Add(pbPlay);
            panelControles.Controls.Add(pbRetroceder);
            panelControles.Controls.Add(lblInfo);
            panelControles.Controls.Add(lblTiempo);
            panelControles.Controls.Add(trackBarProgress);
            panelControles.Controls.Add(btnAbrir);
            panelControles.Dock = DockStyle.Bottom;
            panelControles.Location = new Point(0, 897);
            panelControles.Margin = new Padding(4, 5, 4, 5);
            panelControles.Name = "panelControles";
            panelControles.Size = new Size(1844, 153);
            panelControles.TabIndex = 1;
            // 
            // pbMute
            // 
            pbMute.Cursor = Cursors.Hand;
            pbMute.Image = Properties.Resources.volume;
            pbMute.Location = new Point(1397, 49);
            pbMute.Name = "pbMute";
            pbMute.Size = new Size(64, 61);
            pbMute.SizeMode = PictureBoxSizeMode.Zoom;
            pbMute.TabIndex = 23;
            pbMute.TabStop = false;
            pbMute.Click += BtnMute_Click;
            // 
            // comboVelocidad
            // 
            comboVelocidad.DropDownStyle = ComboBoxStyle.DropDownList;
            comboVelocidad.FormattingEnabled = true;
            comboVelocidad.Items.AddRange(new object[] { "0.25x", "0.5x", "0.75x", "1x", "1.25x", "1.5x", "1.75x", "2x" });
            comboVelocidad.Location = new Point(1211, 60);
            comboVelocidad.Margin = new Padding(4, 5, 4, 5);
            comboVelocidad.Name = "comboVelocidad";
            comboVelocidad.Size = new Size(113, 33);
            comboVelocidad.TabIndex = 22;
            // 
            // lblVelocidad
            // 
            lblVelocidad.AutoSize = true;
            lblVelocidad.ForeColor = Color.White;
            lblVelocidad.Location = new Point(1222, 28);
            lblVelocidad.Margin = new Padding(4, 0, 4, 0);
            lblVelocidad.Name = "lblVelocidad";
            lblVelocidad.Size = new Size(89, 25);
            lblVelocidad.TabIndex = 21;
            lblVelocidad.Text = "Velocidad";
            // 
            // lblVolumen
            // 
            lblVolumen.AutoSize = true;
            lblVolumen.ForeColor = Color.White;
            lblVolumen.Location = new Point(1565, 109);
            lblVolumen.Margin = new Padding(4, 0, 4, 0);
            lblVolumen.Name = "lblVolumen";
            lblVolumen.Size = new Size(57, 25);
            lblVolumen.TabIndex = 20;
            lblVolumen.Text = "100%";
            // 
            // trackBarVolumen
            // 
            trackBarVolumen.Location = new Point(1468, 60);
            trackBarVolumen.Margin = new Padding(4, 5, 4, 5);
            trackBarVolumen.Maximum = 100;
            trackBarVolumen.Name = "trackBarVolumen";
            trackBarVolumen.Size = new Size(251, 69);
            trackBarVolumen.TabIndex = 19;
            trackBarVolumen.TickFrequency = 10;
            trackBarVolumen.Value = 100;
            // 
            // pictureBox1
            // 
            pictureBox1.Cursor = Cursors.Hand;
            pictureBox1.Image = Properties.Resources.full;
            pictureBox1.Location = new Point(1768, 49);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(64, 61);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 17;
            pictureBox1.TabStop = false;
            pictureBox1.Click += BtnPantallaCompleta_Click;
            // 
            // pbAvanzar
            // 
            pbAvanzar.Cursor = Cursors.Hand;
            pbAvanzar.Image = (Image)resources.GetObject("pbAvanzar.Image");
            pbAvanzar.Location = new Point(791, 5);
            pbAvanzar.Name = "pbAvanzar";
            pbAvanzar.Size = new Size(71, 60);
            pbAvanzar.SizeMode = PictureBoxSizeMode.Zoom;
            pbAvanzar.TabIndex = 16;
            pbAvanzar.TabStop = false;
            pbAvanzar.Click += BtnAvanzar_Click;
            // 
            // pbPlay
            // 
            pbPlay.Cursor = Cursors.Hand;
            pbPlay.Image = Properties.Resources.pause;
            pbPlay.Location = new Point(712, 5);
            pbPlay.Name = "pbPlay";
            pbPlay.Size = new Size(73, 59);
            pbPlay.SizeMode = PictureBoxSizeMode.Zoom;
            pbPlay.TabIndex = 15;
            pbPlay.TabStop = false;
            pbPlay.Click += BtnPlay_Click;
            // 
            // pbRetroceder
            // 
            pbRetroceder.Cursor = Cursors.Hand;
            pbRetroceder.Image = (Image)resources.GetObject("pbRetroceder.Image");
            pbRetroceder.Location = new Point(634, 5);
            pbRetroceder.Name = "pbRetroceder";
            pbRetroceder.Size = new Size(72, 58);
            pbRetroceder.SizeMode = PictureBoxSizeMode.Zoom;
            pbRetroceder.TabIndex = 14;
            pbRetroceder.TabStop = false;
            pbRetroceder.Click += BtnRetroceder_Click;
            // 
            // lblInfo
            // 
            lblInfo.AutoSize = true;
            lblInfo.ForeColor = Color.White;
            lblInfo.Location = new Point(18, 125);
            lblInfo.Margin = new Padding(4, 0, 4, 0);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(255, 25);
            lblInfo.TabIndex = 13;
            lblInfo.Text = "Seleccione un archivo de video";
            // 
            // lblTiempo
            // 
            lblTiempo.AutoSize = true;
            lblTiempo.ForeColor = Color.White;
            lblTiempo.Location = new Point(18, 85);
            lblTiempo.Margin = new Padding(4, 0, 4, 0);
            lblTiempo.Name = "lblTiempo";
            lblTiempo.Size = new Size(165, 25);
            lblTiempo.TabIndex = 6;
            lblTiempo.Text = "00:00:00 / 00:00:00";
            // 
            // trackBarProgress
            // 
            trackBarProgress.Location = new Point(209, 71);
            trackBarProgress.Margin = new Padding(4, 5, 4, 5);
            trackBarProgress.Maximum = 1000;
            trackBarProgress.Name = "trackBarProgress";
            trackBarProgress.Size = new Size(950, 69);
            trackBarProgress.TabIndex = 5;
            trackBarProgress.TickFrequency = 50;
            trackBarProgress.MouseDown += TrackBarProgress_MouseDown;
            trackBarProgress.MouseUp += TrackBarProgress_MouseUp;
            // 
            // btnAbrir
            // 
            btnAbrir.BackColor = Color.FromArgb(0, 122, 204);
            btnAbrir.FlatStyle = FlatStyle.Flat;
            btnAbrir.Font = new Font("Segoe UI", 10F);
            btnAbrir.ForeColor = Color.White;
            btnAbrir.Location = new Point(18, 10);
            btnAbrir.Margin = new Padding(4, 5, 4, 5);
            btnAbrir.Name = "btnAbrir";
            btnAbrir.Size = new Size(171, 59);
            btnAbrir.TabIndex = 0;
            btnAbrir.Text = "📂 Abrir Video";
            btnAbrir.UseVisualStyleBackColor = false;
            btnAbrir.Click += BtnAbrir_Click;
            // 
            // ReproductorVideoForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1844, 1050);
            Controls.Add(videoView);
            Controls.Add(panelControles);
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            Margin = new Padding(4, 5, 4, 5);
            Name = "ReproductorVideoForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Reproductor de Video";
            WindowState = FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)videoView).EndInit();
            panelControles.ResumeLayout(false);
            panelControles.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbMute).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarVolumen).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbAvanzar).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbPlay).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbRetroceder).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarProgress).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private LibVLCSharp.WinForms.VideoView videoView;
        private Panel panelControles;
        private Button btnAbrir;
        private Button btnAvanzar;
        private TrackBar trackBarProgress;
        private Label lblTiempo;
        private Label lblInfo;
        private PictureBox pbPlay;
        private PictureBox pbRetroceder;
        private PictureBox pbAvanzar;
        private PictureBox pictureBox1;
        private PictureBox pbMute;
        private ComboBox comboVelocidad;
        private Label lblVelocidad;
        private Label lblVolumen;
        private TrackBar trackBarVolumen;
    }
}

