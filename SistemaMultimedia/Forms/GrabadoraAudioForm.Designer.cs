namespace SistemaMultimedia.Forms
{
    partial class GrabadoraAudioForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel panelSuperior;
        private System.Windows.Forms.Label lblMicrofono;
        private System.Windows.Forms.ComboBox cboMicrofono;
        private System.Windows.Forms.Label lblTiempoTitulo;
        private System.Windows.Forms.Label lblTiempo;
        private System.Windows.Forms.Label lblNivelTitulo;
        private System.Windows.Forms.ProgressBar progressNivel;
        private System.Windows.Forms.Panel panelControles;
        private System.Windows.Forms.Button btnGrabar;
        private System.Windows.Forms.Button btnPausar;
        private System.Windows.Forms.Button btnDetener;
        private System.Windows.Forms.Button btnReproducir;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.Timer tmrGrabacion = null!;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GrabadoraAudioForm));
            panelSuperior = new Panel();
            lblMicrofono = new Label();
            cboMicrofono = new ComboBox();
            lblTiempoTitulo = new Label();
            lblTiempo = new Label();
            lblNivelTitulo = new Label();
            progressNivel = new ProgressBar();
            panelControles = new Panel();
            lblEstado = new Label();
            btnGrabar = new Button();
            btnPausar = new Button();
            btnDetener = new Button();
            btnReproducir = new Button();
            btnGuardar = new Button();
            tmrGrabacion = new System.Windows.Forms.Timer(components);
            saveFileDialog = new SaveFileDialog();
            panelSuperior.SuspendLayout();
            panelControles.SuspendLayout();
            SuspendLayout();
            // 
            // panelSuperior
            // 
            panelSuperior.BackColor = Color.FromArgb(45, 45, 45);
            panelSuperior.Controls.Add(lblMicrofono);
            panelSuperior.Controls.Add(cboMicrofono);
            panelSuperior.Controls.Add(lblTiempoTitulo);
            panelSuperior.Controls.Add(lblTiempo);
            panelSuperior.Controls.Add(lblNivelTitulo);
            panelSuperior.Controls.Add(progressNivel);
            panelSuperior.Dock = DockStyle.Top;
            panelSuperior.Location = new Point(0, 0);
            panelSuperior.Name = "panelSuperior";
            panelSuperior.Padding = new Padding(16, 12, 16, 12);
            panelSuperior.Size = new Size(584, 120);
            panelSuperior.TabIndex = 0;
            // 
            // lblMicrofono
            // 
            lblMicrofono.AutoSize = true;
            lblMicrofono.ForeColor = Color.White;
            lblMicrofono.Location = new Point(16, 16);
            lblMicrofono.Name = "lblMicrofono";
            lblMicrofono.Size = new Size(99, 25);
            lblMicrofono.TabIndex = 0;
            lblMicrofono.Text = "Micrófono:";
            // 
            // cboMicrofono
            // 
            cboMicrofono.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cboMicrofono.BackColor = Color.White;
            cboMicrofono.DropDownStyle = ComboBoxStyle.DropDownList;
            cboMicrofono.FormattingEnabled = true;
            cboMicrofono.Location = new Point(120, 12);
            cboMicrofono.Name = "cboMicrofono";
            cboMicrofono.Size = new Size(448, 33);
            cboMicrofono.TabIndex = 1;
            // 
            // lblTiempoTitulo
            // 
            lblTiempoTitulo.AutoSize = true;
            lblTiempoTitulo.ForeColor = Color.White;
            lblTiempoTitulo.Location = new Point(16, 56);
            lblTiempoTitulo.Name = "lblTiempoTitulo";
            lblTiempoTitulo.Size = new Size(76, 25);
            lblTiempoTitulo.TabIndex = 2;
            lblTiempoTitulo.Text = "Tiempo:";
            // 
            // lblTiempo
            // 
            lblTiempo.AutoSize = true;
            lblTiempo.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTiempo.ForeColor = Color.White;
            lblTiempo.Location = new Point(120, 50);
            lblTiempo.Name = "lblTiempo";
            lblTiempo.Size = new Size(129, 38);
            lblTiempo.TabIndex = 3;
            lblTiempo.Text = "00:00:00";
            // 
            // lblNivelTitulo
            // 
            lblNivelTitulo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblNivelTitulo.AutoSize = true;
            lblNivelTitulo.ForeColor = Color.White;
            lblNivelTitulo.Location = new Point(280, 56);
            lblNivelTitulo.Name = "lblNivelTitulo";
            lblNivelTitulo.Size = new Size(55, 25);
            lblNivelTitulo.TabIndex = 4;
            lblNivelTitulo.Text = "Nivel:";
            // 
            // progressNivel
            // 
            progressNivel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progressNivel.Location = new Point(336, 54);
            progressNivel.Name = "progressNivel";
            progressNivel.Size = new Size(232, 28);
            progressNivel.TabIndex = 5;
            // 
            // panelControles
            // 
            panelControles.BackColor = Color.FromArgb(38, 38, 38);
            panelControles.Controls.Add(lblEstado);
            panelControles.Controls.Add(btnGrabar);
            panelControles.Controls.Add(btnPausar);
            panelControles.Controls.Add(btnDetener);
            panelControles.Controls.Add(btnReproducir);
            panelControles.Controls.Add(btnGuardar);
            panelControles.Dock = DockStyle.Fill;
            panelControles.Location = new Point(0, 120);
            panelControles.Name = "panelControles";
            panelControles.Padding = new Padding(24);
            panelControles.Size = new Size(584, 181);
            panelControles.TabIndex = 1;
            // 
            // lblEstado
            // 
            lblEstado.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblEstado.ForeColor = Color.White;
            lblEstado.Location = new Point(24, 140);
            lblEstado.Name = "lblEstado";
            lblEstado.Size = new Size(536, 25);
            lblEstado.TabIndex = 5;
            lblEstado.Text = "Listo para grabar.";
            // 
            // btnGrabar
            // 
            btnGrabar.BackColor = Color.White;
            btnGrabar.FlatStyle = FlatStyle.Flat;
            btnGrabar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnGrabar.ForeColor = Color.Black;
            btnGrabar.Location = new Point(24, 24);
            btnGrabar.Name = "btnGrabar";
            btnGrabar.Size = new Size(120, 44);
            btnGrabar.TabIndex = 0;
            btnGrabar.Text = "●  Grabar";
            btnGrabar.UseVisualStyleBackColor = false;
            btnGrabar.Click += btnGrabar_Click;
            // 
            // btnPausar
            // 
            btnPausar.BackColor = Color.White;
            btnPausar.Enabled = false;
            btnPausar.FlatStyle = FlatStyle.Flat;
            btnPausar.ForeColor = Color.Black;
            btnPausar.Location = new Point(156, 24);
            btnPausar.Name = "btnPausar";
            btnPausar.Size = new Size(120, 44);
            btnPausar.TabIndex = 1;
            btnPausar.Text = "⏸  Pausar";
            btnPausar.UseVisualStyleBackColor = false;
            btnPausar.Click += btnPausar_Click;
            // 
            // btnDetener
            // 
            btnDetener.BackColor = Color.White;
            btnDetener.Enabled = false;
            btnDetener.FlatStyle = FlatStyle.Flat;
            btnDetener.ForeColor = Color.Black;
            btnDetener.Location = new Point(288, 24);
            btnDetener.Name = "btnDetener";
            btnDetener.Size = new Size(120, 44);
            btnDetener.TabIndex = 2;
            btnDetener.Text = "■  Detener";
            btnDetener.UseVisualStyleBackColor = false;
            btnDetener.Click += btnDetener_Click;
            // 
            // btnReproducir
            // 
            btnReproducir.BackColor = Color.White;
            btnReproducir.Enabled = false;
            btnReproducir.FlatStyle = FlatStyle.Flat;
            btnReproducir.ForeColor = Color.Black;
            btnReproducir.Location = new Point(24, 84);
            btnReproducir.Name = "btnReproducir";
            btnReproducir.Size = new Size(160, 44);
            btnReproducir.TabIndex = 3;
            btnReproducir.Text = "▶  Reproducir";
            btnReproducir.UseVisualStyleBackColor = false;
            btnReproducir.Click += btnReproducir_Click;
            // 
            // btnGuardar
            // 
            btnGuardar.BackColor = Color.White;
            btnGuardar.Enabled = false;
            btnGuardar.FlatStyle = FlatStyle.Flat;
            btnGuardar.ForeColor = Color.Black;
            btnGuardar.Location = new Point(200, 84);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(160, 44);
            btnGuardar.TabIndex = 4;
            btnGuardar.Text = "💾  Guardar";
            btnGuardar.UseVisualStyleBackColor = false;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // tmrGrabacion
            // 
            tmrGrabacion.Tick += tmrGrabacion_Tick;
            // 
            // saveFileDialog
            // 
            saveFileDialog.DefaultExt = "wav";
            saveFileDialog.Filter = "Audio WAV (*.wav)|*.wav|Audio MP3 (*.mp3)|*.mp3";
            saveFileDialog.Title = "Guardar grabación";
            // 
            // GrabadoraAudioForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 30);
            ClientSize = new Size(584, 301);
            Controls.Add(panelControles);
            Controls.Add(panelSuperior);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimumSize = new Size(600, 340);
            Name = "GrabadoraAudioForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Grabadora de audio";
            panelSuperior.ResumeLayout(false);
            panelSuperior.PerformLayout();
            panelControles.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
