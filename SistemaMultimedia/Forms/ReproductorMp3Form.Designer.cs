namespace SistemaMultimedia.Forms
{
    partial class ReproductorMp3Form
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnLyrics;
        private System.Windows.Forms.Button btnOpenWmp;

        private System.Windows.Forms.Button pbStop;
        private System.Windows.Forms.Button pbSkipBack100;
        private System.Windows.Forms.TrackBar trackBarPosition;
        private System.Windows.Forms.TrackBar trackBarVolume;
        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Timer tmrPosition;
        private System.Windows.Forms.DataGridView dgvPlaylist;
        private System.Windows.Forms.PictureBox pbArtwork;
        private System.Windows.Forms.RichTextBox rtbLyrics;

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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReproductorMp3Form));
            btnOpen = new Button();
            btnLyrics = new Button();
            btnOpenWmp = new Button();
            pbStop = new Button();
            trackBarPosition = new TrackBar();
            trackBarVolume = new TrackBar();
            lblFile = new Label();
            lblTime = new Label();
            tmrPosition = new System.Windows.Forms.Timer(components);
            dgvPlaylist = new DataGridView();
            colArtwork = new DataGridViewImageColumn();
            colTitle = new DataGridViewTextBoxColumn();
            colArtist = new DataGridViewTextBoxColumn();
            colAlbum = new DataGridViewTextBoxColumn();
            colSize = new DataGridViewTextBoxColumn();
            colSampleRate = new DataGridViewTextBoxColumn();
            colDuration = new DataGridViewTextBoxColumn();
            colFilePath = new DataGridViewTextBoxColumn();
            pbArtwork = new PictureBox();
            rtbLyrics = new RichTextBox();
            pbNext = new PictureBox();
            pbPrev = new PictureBox();
            pbPlay = new PictureBox();
            pbSkipBack10 = new PictureBox();
            pbSkipForward10 = new PictureBox();
            pbSavePlaylist = new PictureBox();
            pbLoadPlaylist = new PictureBox();
            pbClearPlaylist = new PictureBox();
            lblCargar = new Label();
            lblDescargar = new Label();
            lblEliminar = new Label();
            pbShuffle = new PictureBox();
            pbRepeat = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)trackBarPosition).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarVolume).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvPlaylist).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbArtwork).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbNext).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbPrev).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbPlay).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbSkipBack10).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbSkipForward10).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbSavePlaylist).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbLoadPlaylist).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbClearPlaylist).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbShuffle).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbRepeat).BeginInit();
            SuspendLayout();
            // 
            // btnOpen
            // 
            btnOpen.Anchor = AnchorStyles.Bottom;
            btnOpen.ForeColor = Color.Black;
            btnOpen.Location = new Point(12, 337);
            btnOpen.Name = "btnOpen";
            btnOpen.Size = new Size(84, 30);
            btnOpen.TabIndex = 0;
            btnOpen.Text = "Añadir";
            btnOpen.UseVisualStyleBackColor = true;
            btnOpen.Click += btnOpen_Click;
            // 
            // btnLyrics
            // 
            btnLyrics.Anchor = AnchorStyles.Bottom;
            btnLyrics.ForeColor = Color.Black;
            btnLyrics.Location = new Point(12, 12);
            btnLyrics.Name = "btnLyrics";
            btnLyrics.Size = new Size(159, 30);
            btnLyrics.TabIndex = 1;
            btnLyrics.Text = "Mostrar letra";
            btnLyrics.UseVisualStyleBackColor = true;
            btnLyrics.Click += btnLyrics_Click;
            // 
            // btnOpenWmp
            // 
            btnOpenWmp.Anchor = AnchorStyles.Bottom;
            btnOpenWmp.ForeColor = Color.Black;
            btnOpenWmp.Location = new Point(968, 337);
            btnOpenWmp.Name = "btnOpenWmp";
            btnOpenWmp.Size = new Size(120, 30);
            btnOpenWmp.TabIndex = 14;
            btnOpenWmp.Text = "Abrir WMP";
            btnOpenWmp.UseVisualStyleBackColor = true;
            btnOpenWmp.Click += btnOpenWmp_Click;
            // 
            // pbStop
            // 
            pbStop.BackColor = Color.FromArgb(40, 40, 40);
            pbStop.BackgroundImage = (Image)resources.GetObject("pbStop.BackgroundImage");
            pbStop.BackgroundImageLayout = ImageLayout.Zoom;
            pbStop.Cursor = Cursors.Hand;
            pbStop.FlatAppearance.BorderSize = 0;
            pbStop.FlatStyle = FlatStyle.Flat;
            pbStop.Location = new Point(428, 103);
            pbStop.Name = "pbStop";
            pbStop.Size = new Size(50, 50);
            pbStop.TabIndex = 7;
            pbStop.TabStop = false;
            pbStop.UseVisualStyleBackColor = true;
            pbStop.Click += pbStop_Click;
            // 
            // trackBarPosition
            // 
            trackBarPosition.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarPosition.Location = new Point(199, 263);
            trackBarPosition.Name = "trackBarPosition";
            trackBarPosition.Size = new Size(663, 69);
            trackBarPosition.TabIndex = 9;
            trackBarPosition.Scroll += trackBarPosition_Scroll;
            trackBarPosition.MouseDown += trackBarPosition_MouseDown;
            trackBarPosition.MouseUp += trackBarPosition_MouseUp;
            // 
            // trackBarVolume
            // 
            trackBarVolume.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            trackBarVolume.Location = new Point(934, 12);
            trackBarVolume.Name = "trackBarVolume";
            trackBarVolume.Orientation = Orientation.Vertical;
            trackBarVolume.Size = new Size(69, 269);
            trackBarVolume.TabIndex = 10;
            trackBarVolume.Value = 10;
            trackBarVolume.Scroll += trackBarVolume_Scroll;
            // 
            // lblFile
            // 
            lblFile.AutoEllipsis = true;
            lblFile.Location = new Point(481, 237);
            lblFile.Name = "lblFile";
            lblFile.Size = new Size(297, 23);
            lblFile.TabIndex = 11;
            // 
            // lblTime
            // 
            lblTime.ForeColor = Color.White;
            lblTime.Location = new Point(736, 237);
            lblTime.Name = "lblTime";
            lblTime.Size = new Size(126, 23);
            lblTime.TabIndex = 12;
            lblTime.Text = "00:00 / 00:00";
            lblTime.TextAlign = ContentAlignment.TopRight;
            // 
            // tmrPosition
            // 
            tmrPosition.Interval = 500;
            tmrPosition.Tick += tmrPosition_Tick;
            // 
            // dgvPlaylist
            // 
            dgvPlaylist.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvPlaylist.BackgroundColor = Color.FromArgb(30, 30, 30);
            dgvPlaylist.BorderStyle = BorderStyle.None;
            dgvPlaylist.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPlaylist.Columns.AddRange(new DataGridViewColumn[] { colArtwork, colTitle, colArtist, colAlbum, colSize, colSampleRate, colDuration, colFilePath });
            dgvPlaylist.Location = new Point(-1, 386);
            dgvPlaylist.MultiSelect = false;
            dgvPlaylist.Name = "dgvPlaylist";
            dgvPlaylist.ReadOnly = true;
            dgvPlaylist.RowHeadersVisible = false;
            dgvPlaylist.RowHeadersWidth = 51;
            dgvPlaylist.RowTemplate.Height = 60;
            dgvPlaylist.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPlaylist.Size = new Size(1100, 224);
            dgvPlaylist.TabIndex = 13;
            dgvPlaylist.CellDoubleClick += dgvPlaylist_CellDoubleClick;
            // 
            // colArtwork
            // 
            colArtwork.DataPropertyName = "Artwork";
            colArtwork.HeaderText = "";
            colArtwork.ImageLayout = DataGridViewImageCellLayout.Zoom;
            colArtwork.MinimumWidth = 6;
            colArtwork.Name = "colArtwork";
            colArtwork.ReadOnly = true;
            colArtwork.Width = 60;
            // 
            // colTitle
            // 
            colTitle.DataPropertyName = "Title";
            colTitle.HeaderText = "Canción";
            colTitle.MinimumWidth = 6;
            colTitle.Name = "colTitle";
            colTitle.ReadOnly = true;
            colTitle.Width = 220;
            // 
            // colArtist
            // 
            colArtist.DataPropertyName = "Artist";
            colArtist.HeaderText = "Autor";
            colArtist.MinimumWidth = 6;
            colArtist.Name = "colArtist";
            colArtist.ReadOnly = true;
            colArtist.Width = 160;
            // 
            // colAlbum
            // 
            colAlbum.DataPropertyName = "Album";
            colAlbum.HeaderText = "Álbum";
            colAlbum.MinimumWidth = 6;
            colAlbum.Name = "colAlbum";
            colAlbum.ReadOnly = true;
            colAlbum.Width = 160;
            // 
            // colSize
            // 
            colSize.DataPropertyName = "FileSizeText";
            colSize.HeaderText = "Peso";
            colSize.MinimumWidth = 6;
            colSize.Name = "colSize";
            colSize.ReadOnly = true;
            colSize.Width = 90;
            // 
            // colSampleRate
            // 
            colSampleRate.DataPropertyName = "SampleRateText";
            colSampleRate.HeaderText = "Frecuencia";
            colSampleRate.MinimumWidth = 6;
            colSampleRate.Name = "colSampleRate";
            colSampleRate.ReadOnly = true;
            colSampleRate.Width = 125;
            // 
            // colDuration
            // 
            colDuration.DataPropertyName = "DurationText";
            colDuration.HeaderText = "Duración";
            colDuration.MinimumWidth = 6;
            colDuration.Name = "colDuration";
            colDuration.ReadOnly = true;
            colDuration.Width = 80;
            // 
            // colFilePath
            // 
            colFilePath.DataPropertyName = "FilePath";
            colFilePath.HeaderText = "Ruta";
            colFilePath.MinimumWidth = 6;
            colFilePath.Name = "colFilePath";
            colFilePath.ReadOnly = true;
            colFilePath.Width = 300;
            // 
            // pbArtwork
            // 
            pbArtwork.BackColor = Color.FromArgb(30, 30, 30);
            pbArtwork.Location = new Point(416, 12);
            pbArtwork.Name = "pbArtwork";
            pbArtwork.Size = new Size(284, 220);
            pbArtwork.SizeMode = PictureBoxSizeMode.Zoom;
            pbArtwork.TabIndex = 14;
            pbArtwork.TabStop = false;
            pbArtwork.Click += pbArtwork_Click;
            // 
            // rtbLyrics
            // 
            rtbLyrics.BackColor = Color.FromArgb(30, 30, 30);
            rtbLyrics.BorderStyle = BorderStyle.None;
            rtbLyrics.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            rtbLyrics.ForeColor = Color.White;
            rtbLyrics.Location = new Point(12, 48);
            rtbLyrics.Name = "rtbLyrics";
            rtbLyrics.ReadOnly = true;
            rtbLyrics.ScrollBars = RichTextBoxScrollBars.Vertical;
            rtbLyrics.Size = new Size(378, 184);
            rtbLyrics.TabIndex = 15;
            rtbLyrics.Text = "";
            rtbLyrics.Visible = false;
            rtbLyrics.TextChanged += rtbLyrics_TextChanged;
            // 
            // pbNext
            // 
            pbNext.Cursor = Cursors.Hand;
            pbNext.Image = Properties.Resources.next;
            pbNext.Location = new Point(659, 315);
            pbNext.Name = "pbNext";
            pbNext.Size = new Size(50, 51);
            pbNext.SizeMode = PictureBoxSizeMode.Zoom;
            pbNext.TabIndex = 20;
            pbNext.TabStop = false;
            pbNext.Click += pbNext_Click;
            // 
            // pbPrev
            // 
            pbPrev.Cursor = Cursors.Hand;
            pbPrev.Image = Properties.Resources.back;
            pbPrev.Location = new Point(397, 315);
            pbPrev.Name = "pbPrev";
            pbPrev.Size = new Size(50, 51);
            pbPrev.SizeMode = PictureBoxSizeMode.Zoom;
            pbPrev.TabIndex = 21;
            pbPrev.TabStop = false;
            pbPrev.Click += pbPrev_Click;
            // 
            // pbPlay
            // 
            pbPlay.Cursor = Cursors.Hand;
            pbPlay.Image = Properties.Resources.play;
            pbPlay.Location = new Point(531, 316);
            pbPlay.Name = "pbPlay";
            pbPlay.Size = new Size(50, 51);
            pbPlay.SizeMode = PictureBoxSizeMode.Zoom;
            pbPlay.TabIndex = 22;
            pbPlay.TabStop = false;
            pbPlay.Click += pbPlay_Click;
            // 
            // pbSkipBack10
            // 
            pbSkipBack10.Cursor = Cursors.Hand;
            pbSkipBack10.Image = (Image)resources.GetObject("pbSkipBack10.Image");
            pbSkipBack10.Location = new Point(467, 316);
            pbSkipBack10.Name = "pbSkipBack10";
            pbSkipBack10.Size = new Size(50, 51);
            pbSkipBack10.SizeMode = PictureBoxSizeMode.Zoom;
            pbSkipBack10.TabIndex = 23;
            pbSkipBack10.TabStop = false;
            pbSkipBack10.Click += pbSkipBack10_Click;
            // 
            // pbSkipForward10
            // 
            pbSkipForward10.Cursor = Cursors.Hand;
            pbSkipForward10.Image = Properties.Resources.adelantar;
            pbSkipForward10.Location = new Point(595, 316);
            pbSkipForward10.Name = "pbSkipForward10";
            pbSkipForward10.Size = new Size(50, 51);
            pbSkipForward10.SizeMode = PictureBoxSizeMode.Zoom;
            pbSkipForward10.TabIndex = 24;
            pbSkipForward10.TabStop = false;
            pbSkipForward10.Click += pbSkipForward10_Click;
            // 
            // pbSavePlaylist
            // 
            pbSavePlaylist.Cursor = Cursors.Hand;
            pbSavePlaylist.Image = Properties.Resources.save;
            pbSavePlaylist.Location = new Point(523, 650);
            pbSavePlaylist.Name = "pbSavePlaylist";
            pbSavePlaylist.Size = new Size(58, 56);
            pbSavePlaylist.SizeMode = PictureBoxSizeMode.Zoom;
            pbSavePlaylist.TabIndex = 25;
            pbSavePlaylist.TabStop = false;
            pbSavePlaylist.Click += pbSavePlaylist_Click;
            // 
            // pbLoadPlaylist
            // 
            pbLoadPlaylist.Cursor = Cursors.Hand;
            pbLoadPlaylist.Image = Properties.Resources.upload;
            pbLoadPlaylist.Location = new Point(428, 650);
            pbLoadPlaylist.Name = "pbLoadPlaylist";
            pbLoadPlaylist.Size = new Size(57, 56);
            pbLoadPlaylist.SizeMode = PictureBoxSizeMode.Zoom;
            pbLoadPlaylist.TabIndex = 26;
            pbLoadPlaylist.TabStop = false;
            pbLoadPlaylist.Click += pbLoadPlaylist_Click;
            // 
            // pbClearPlaylist
            // 
            pbClearPlaylist.Cursor = Cursors.Hand;
            pbClearPlaylist.Image = Properties.Resources.delete;
            pbClearPlaylist.Location = new Point(623, 650);
            pbClearPlaylist.Name = "pbClearPlaylist";
            pbClearPlaylist.Size = new Size(58, 56);
            pbClearPlaylist.SizeMode = PictureBoxSizeMode.Zoom;
            pbClearPlaylist.TabIndex = 27;
            pbClearPlaylist.TabStop = false;
            pbClearPlaylist.Click += btnClearPlaylist_Click;
            // 
            // lblCargar
            // 
            lblCargar.AutoEllipsis = true;
            lblCargar.Location = new Point(418, 612);
            lblCargar.Name = "lblCargar";
            lblCargar.Size = new Size(76, 35);
            lblCargar.TabIndex = 28;
            lblCargar.Text = "Cargar";
            // 
            // lblDescargar
            // 
            lblDescargar.AutoEllipsis = true;
            lblDescargar.Location = new Point(509, 612);
            lblDescargar.Name = "lblDescargar";
            lblDescargar.Size = new Size(113, 35);
            lblDescargar.TabIndex = 29;
            lblDescargar.Text = "Descargar";
            // 
            // lblEliminar
            // 
            lblEliminar.AutoEllipsis = true;
            lblEliminar.Location = new Point(612, 613);
            lblEliminar.Name = "lblEliminar";
            lblEliminar.Size = new Size(87, 35);
            lblEliminar.TabIndex = 30;
            lblEliminar.Text = "Eliminar";
            // 
            // pbShuffle
            // 
            pbShuffle.Cursor = Cursors.Hand;
            pbShuffle.Image = Properties.Resources.shuffle;
            pbShuffle.Location = new Point(331, 315);
            pbShuffle.Name = "pbShuffle";
            pbShuffle.Size = new Size(50, 51);
            pbShuffle.SizeMode = PictureBoxSizeMode.Zoom;
            pbShuffle.TabIndex = 31;
            pbShuffle.TabStop = false;
            pbShuffle.Click += pbShuffle_Click;
            // pbRepeat
            // 
            pbRepeat.Cursor = Cursors.Hand;
            pbRepeat.Image = Properties.Resources.gg1;
            pbRepeat.Location = new Point(728, 315);
            pbRepeat.Name = "pbRepeat";
            pbRepeat.Size = new Size(50, 51);
            pbRepeat.SizeMode = PictureBoxSizeMode.Zoom;
            pbRepeat.TabIndex = 32;
            pbRepeat.TabStop = false;
            // 
            // ReproductorMp3Form
            // 
            AutoScaleDimensions = new SizeF(11F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 30);
            ClientSize = new Size(1100, 718);
            Controls.Add(pbRepeat);
            Controls.Add(pbShuffle);
            Controls.Add(lblEliminar);
            Controls.Add(lblDescargar);
            Controls.Add(lblCargar);
            Controls.Add(pbClearPlaylist);
            Controls.Add(pbLoadPlaylist);
            Controls.Add(pbSavePlaylist);
            Controls.Add(pbSkipForward10);
            Controls.Add(pbSkipBack10);
            Controls.Add(pbPlay);
            Controls.Add(pbPrev);
            Controls.Add(pbNext);
            Controls.Add(btnOpenWmp);
            Controls.Add(btnLyrics);
            Controls.Add(rtbLyrics);
            Controls.Add(pbArtwork);
            Controls.Add(lblTime);
            Controls.Add(lblFile);
            Controls.Add(trackBarVolume);
            Controls.Add(dgvPlaylist);
            Controls.Add(pbStop);
            Controls.Add(btnOpen);
            Controls.Add(trackBarPosition);
            Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ForeColor = Color.White;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "ReproductorMp3Form";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Reproductor Audio";
            FormClosing += ReproductorMp3Form_FormClosing;
            Load += ReproductorMp3Form_Load;
            Resize += ReproductorMp3Form_Resize;
            ((System.ComponentModel.ISupportInitialize)trackBarPosition).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarVolume).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvPlaylist).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbArtwork).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbNext).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbPrev).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbPlay).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbSkipBack10).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbSkipForward10).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbSavePlaylist).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbLoadPlaylist).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbClearPlaylist).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbShuffle).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbRepeat).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pbSavePlaylist;
        private PictureBox pbLoadPlaylist;
        private PictureBox pbNext;
        private PictureBox pbPrev;
        private PictureBox pbPlay;
        private PictureBox pbSkipBack10;
        private PictureBox pbSkipForward10;
        private PictureBox pbClearPlaylist;
        private DataGridViewImageColumn colArtwork;
        private DataGridViewTextBoxColumn colTitle;
        private DataGridViewTextBoxColumn colArtist;
        private DataGridViewTextBoxColumn colAlbum;
        private DataGridViewTextBoxColumn colSize;
        private DataGridViewTextBoxColumn colSampleRate;
        private DataGridViewTextBoxColumn colDuration;
        private DataGridViewTextBoxColumn colFilePath;
        private Label lblCargar;
        private Label lblDescargar;
        private Label lblEliminar;
        private PictureBox pbShuffle;
        private PictureBox pbRepeat;
    }
}

