namespace FotoGeolocalizada
{
    partial class GeolocalizacionForm
    {
        private System.ComponentModel.IContainer? components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GeolocalizacionForm));
            txtCoordenadas = new TextBox();
            btnMostrarUbicacion = new Button();
            webMapa = new Microsoft.Web.WebView2.WinForms.WebView2();
            txtLatManual = new TextBox();
            txtLonManual = new TextBox();
            btnGuardar = new Button();
            labelLat = new Label();
            labelLon = new Label();
            groupBox1 = new GroupBox();
            ((System.ComponentModel.ISupportInitialize)webMapa).BeginInit();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // txtCoordenadas
            // 
            txtCoordenadas.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtCoordenadas.Location = new Point(12, 403);
            txtCoordenadas.Margin = new Padding(4, 5, 4, 5);
            txtCoordenadas.Name = "txtCoordenadas";
            txtCoordenadas.ReadOnly = true;
            txtCoordenadas.Size = new Size(610, 39);
            txtCoordenadas.TabIndex = 2;
            // 
            // btnMostrarUbicacion
            // 
            btnMostrarUbicacion.Font = new Font("Segoe UI", 9F);
            btnMostrarUbicacion.Location = new Point(91, 452);
            btnMostrarUbicacion.Margin = new Padding(4, 5, 4, 5);
            btnMostrarUbicacion.Name = "btnMostrarUbicacion";
            btnMostrarUbicacion.Size = new Size(450, 45);
            btnMostrarUbicacion.TabIndex = 3;
            btnMostrarUbicacion.Text = "Mostrar Ubicacion en Google Maps";
            btnMostrarUbicacion.UseVisualStyleBackColor = true;
            btnMostrarUbicacion.Click += btnMostrarUbicacion_Click;
            // 
            // webMapa
            // 
            webMapa.AllowExternalDrop = true;
            webMapa.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            webMapa.CreationProperties = null;
            webMapa.DefaultBackgroundColor = Color.Violet;
            webMapa.Location = new Point(12, 14);
            webMapa.Margin = new Padding(4, 5, 4, 5);
            webMapa.Name = "webMapa";
            webMapa.Size = new Size(610, 379);
            webMapa.TabIndex = 4;
            webMapa.ZoomFactor = 1D;
            // 
            // txtLatManual
            // 
            txtLatManual.Font = new Font("Segoe UI", 9F);
            txtLatManual.Location = new Point(7, 75);
            txtLatManual.Margin = new Padding(4, 5, 4, 5);
            txtLatManual.Name = "txtLatManual";
            txtLatManual.PlaceholderText = "Latitud (dec.)";
            txtLatManual.Size = new Size(284, 31);
            txtLatManual.TabIndex = 5;
            // 
            // txtLonManual
            // 
            txtLonManual.Font = new Font("Segoe UI", 9F);
            txtLonManual.Location = new Point(316, 75);
            txtLonManual.Margin = new Padding(4, 5, 4, 5);
            txtLonManual.Name = "txtLonManual";
            txtLonManual.PlaceholderText = "Longitud (dec.)";
            txtLonManual.Size = new Size(284, 31);
            txtLonManual.TabIndex = 6;
            // 
            // btnGuardar
            // 
            btnGuardar.Font = new Font("Segoe UI", 9F);
            btnGuardar.ForeColor = Color.Black;
            btnGuardar.Location = new Point(215, 116);
            btnGuardar.Margin = new Padding(4, 5, 4, 5);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(179, 45);
            btnGuardar.TabIndex = 7;
            btnGuardar.Text = "Guardar con datos";
            btnGuardar.UseVisualStyleBackColor = true;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // labelLat
            // 
            labelLat.Font = new Font("Segoe UI", 10F);
            labelLat.ForeColor = Color.White;
            labelLat.Location = new Point(5, 48);
            labelLat.Margin = new Padding(4, 0, 4, 0);
            labelLat.Name = "labelLat";
            labelLat.Size = new Size(286, 33);
            labelLat.TabIndex = 1;
            labelLat.Text = "Latitud decimal:";
            // 
            // labelLon
            // 
            labelLon.Font = new Font("Segoe UI", 10F);
            labelLon.ForeColor = Color.White;
            labelLon.Location = new Point(316, 48);
            labelLon.Margin = new Padding(4, 0, 4, 0);
            labelLon.Name = "labelLon";
            labelLon.Size = new Size(286, 33);
            labelLon.TabIndex = 0;
            labelLon.Text = "Longitud decimal:";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(txtLatManual);
            groupBox1.Controls.Add(labelLon);
            groupBox1.Controls.Add(txtLonManual);
            groupBox1.Controls.Add(labelLat);
            groupBox1.Controls.Add(btnGuardar);
            groupBox1.ForeColor = Color.White;
            groupBox1.Location = new Point(12, 505);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(610, 171);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            groupBox1.Text = "Introducir ubicación manualmente";
            // 
            // GeolocalizacionForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 30);
            ClientSize = new Size(641, 688);
            Controls.Add(groupBox1);
            Controls.Add(webMapa);
            Controls.Add(btnMostrarUbicacion);
            Controls.Add(txtCoordenadas);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 5, 4, 5);
            Name = "GeolocalizacionForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Geolocalización";
            ((System.ComponentModel.ISupportInitialize)webMapa).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox txtCoordenadas;
        private Button btnMostrarUbicacion;
        private Microsoft.Web.WebView2.WinForms.WebView2 webMapa;
        private TextBox txtLatManual;
        private TextBox txtLonManual;
        private Button btnGuardar;
        private Label labelLat;
        private Label labelLon;
        private GroupBox groupBox1;
    }
}
