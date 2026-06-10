namespace SistemaMultimedia.Forms
{
    partial class ExploradorArchivosForm
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExploradorArchivosForm));
            listViewArchivos = new ListView();
            imageListIconos = new ImageList(components);
            txtRutaActual = new TextBox();
            splitContainer1 = new SplitContainer();
            treeViewDirectorios = new TreeView();
            imageListIconosGrandes = new ImageList(components);
            imageListIconosMuyGrandes = new ImageList(components);
            imageListIconosMedianos = new ImageList(components);
            comboVisualizacion = new ComboBox();
            label1 = new Label();
            pbRegresar = new PictureBox();
            pbAbrir = new PictureBox();
            pbCamara = new PictureBox();
            groupBox1 = new GroupBox();
            pbGrabadora = new PictureBox();
            pbVisorDocumentos = new PictureBox();
            pbDataFusion = new PictureBox();
            pbReproductorMP3 = new PictureBox();
            pbReproductorVideo = new PictureBox();
            pbEditorFoto = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbRegresar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbAbrir).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbCamara).BeginInit();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbGrabadora).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbVisorDocumentos).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbDataFusion).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbReproductorMP3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbReproductorVideo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbEditorFoto).BeginInit();
            SuspendLayout();
            // 
            // listViewArchivos
            // 
            listViewArchivos.BackColor = Color.FromArgb(30, 30, 30);
            listViewArchivos.Dock = DockStyle.Fill;
            listViewArchivos.ForeColor = Color.White;
            listViewArchivos.Location = new Point(0, 0);
            listViewArchivos.Margin = new Padding(4, 5, 4, 5);
            listViewArchivos.Name = "listViewArchivos";
            listViewArchivos.Size = new Size(755, 582);
            listViewArchivos.TabIndex = 2;
            listViewArchivos.UseCompatibleStateImageBehavior = false;
            listViewArchivos.View = View.Tile;
            listViewArchivos.DoubleClick += listViewArchivos_DoubleClick;
            // 
            // imageListIconos
            // 
            imageListIconos.ColorDepth = ColorDepth.Depth32Bit;
            imageListIconos.ImageSize = new Size(16, 16);
            imageListIconos.TransparentColor = Color.Transparent;
            // 
            // txtRutaActual
            // 
            txtRutaActual.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtRutaActual.BackColor = Color.FromArgb(30, 30, 30);
            txtRutaActual.ForeColor = Color.White;
            txtRutaActual.Location = new Point(17, 88);
            txtRutaActual.Margin = new Padding(4, 5, 4, 5);
            txtRutaActual.Name = "txtRutaActual";
            txtRutaActual.ReadOnly = true;
            txtRutaActual.Size = new Size(748, 31);
            txtRutaActual.TabIndex = 3;
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.BackColor = Color.FromArgb(30, 30, 30);
            splitContainer1.Location = new Point(17, 148);
            splitContainer1.Margin = new Padding(4, 5, 4, 5);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(treeViewDirectorios);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(listViewArchivos);
            splitContainer1.Size = new Size(1109, 582);
            splitContainer1.SplitterDistance = 348;
            splitContainer1.SplitterWidth = 6;
            splitContainer1.TabIndex = 4;
            // 
            // treeViewDirectorios
            // 
            treeViewDirectorios.BackColor = Color.FromArgb(30, 30, 30);
            treeViewDirectorios.Dock = DockStyle.Fill;
            treeViewDirectorios.ForeColor = Color.White;
            treeViewDirectorios.ImageIndex = 0;
            treeViewDirectorios.ImageList = imageListIconosGrandes;
            treeViewDirectorios.Location = new Point(0, 0);
            treeViewDirectorios.Margin = new Padding(4, 5, 4, 5);
            treeViewDirectorios.Name = "treeViewDirectorios";
            treeViewDirectorios.SelectedImageIndex = 0;
            treeViewDirectorios.Size = new Size(348, 582);
            treeViewDirectorios.TabIndex = 0;
            treeViewDirectorios.BeforeExpand += treeViewDirectorios_BeforeExpand;
            treeViewDirectorios.AfterSelect += treeViewDirectorios_AfterSelect;
            // 
            // imageListIconosGrandes
            // 
            imageListIconosGrandes.ColorDepth = ColorDepth.Depth32Bit;
            imageListIconosGrandes.ImageSize = new Size(48, 48);
            imageListIconosGrandes.TransparentColor = Color.Transparent;
            // 
            // imageListIconosMuyGrandes
            // 
            imageListIconosMuyGrandes.ColorDepth = ColorDepth.Depth32Bit;
            imageListIconosMuyGrandes.ImageSize = new Size(96, 96);
            imageListIconosMuyGrandes.TransparentColor = Color.Transparent;
            // 
            // imageListIconosMedianos
            // 
            imageListIconosMedianos.ColorDepth = ColorDepth.Depth32Bit;
            imageListIconosMedianos.ImageSize = new Size(32, 32);
            imageListIconosMedianos.TransparentColor = Color.Transparent;
            // 
            // comboVisualizacion
            // 
            comboVisualizacion.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            comboVisualizacion.BackColor = Color.FromArgb(30, 30, 30);
            comboVisualizacion.DropDownStyle = ComboBoxStyle.DropDownList;
            comboVisualizacion.ForeColor = Color.White;
            comboVisualizacion.FormattingEnabled = true;
            comboVisualizacion.Items.AddRange(new object[] { "📦 Iconos muy grandes", "📦 Iconos grandes", "📦 Iconos medianos", "📦 Iconos pequeños", "📋 Lista", "📊 Detalles" });
            comboVisualizacion.Location = new Point(874, 88);
            comboVisualizacion.Margin = new Padding(4, 5, 4, 5);
            comboVisualizacion.Name = "comboVisualizacion";
            comboVisualizacion.Size = new Size(250, 33);
            comboVisualizacion.TabIndex = 5;
            comboVisualizacion.SelectedIndexChanged += comboVisualizacion_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.BackColor = Color.FromArgb(30, 30, 30);
            label1.ForeColor = Color.White;
            label1.Location = new Point(776, 93);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(54, 25);
            label1.TabIndex = 6;
            label1.Text = "Vista:";
            // 
            // pbRegresar
            // 
            pbRegresar.Cursor = Cursors.Hand;
            pbRegresar.Image = Properties.Resources.regresar;
            pbRegresar.Location = new Point(615, 53);
            pbRegresar.Name = "pbRegresar";
            pbRegresar.Size = new Size(47, 27);
            pbRegresar.SizeMode = PictureBoxSizeMode.Zoom;
            pbRegresar.TabIndex = 9;
            pbRegresar.TabStop = false;
            pbRegresar.Click += btnRegresar_Click;
            // 
            // pbAbrir
            // 
            pbAbrir.Cursor = Cursors.Hand;
            pbAbrir.Image = Properties.Resources.directory1;
            pbAbrir.Location = new Point(543, 37);
            pbAbrir.Name = "pbAbrir";
            pbAbrir.Size = new Size(66, 43);
            pbAbrir.SizeMode = PictureBoxSizeMode.Zoom;
            pbAbrir.TabIndex = 10;
            pbAbrir.TabStop = false;
            pbAbrir.Click += btnAbrir_Click;
            // 
            // pbCamara
            // 
            pbCamara.Cursor = Cursors.Hand;
            pbCamara.Image = Properties.Resources.camara;
            pbCamara.Location = new Point(6, 26);
            pbCamara.Name = "pbCamara";
            pbCamara.Size = new Size(66, 43);
            pbCamara.SizeMode = PictureBoxSizeMode.Zoom;
            pbCamara.TabIndex = 11;
            pbCamara.TabStop = false;
            pbCamara.Click += pbCamara_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(pbGrabadora);
            groupBox1.Controls.Add(pbVisorDocumentos);
            groupBox1.Controls.Add(pbDataFusion);
            groupBox1.Controls.Add(pbReproductorMP3);
            groupBox1.Controls.Add(pbReproductorVideo);
            groupBox1.Controls.Add(pbEditorFoto);
            groupBox1.Controls.Add(pbCamara);
            groupBox1.ForeColor = Color.White;
            groupBox1.Location = new Point(17, 5);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(520, 75);
            groupBox1.TabIndex = 12;
            groupBox1.TabStop = false;
            groupBox1.Text = "Aplicaciones";
            // 
            // pbGrabadora
            // 
            pbGrabadora.Cursor = Cursors.Hand;
            pbGrabadora.Image = Properties.Resources.sound_recorder;
            pbGrabadora.Location = new Point(443, 26);
            pbGrabadora.Name = "pbGrabadora";
            pbGrabadora.Size = new Size(66, 43);
            pbGrabadora.SizeMode = PictureBoxSizeMode.Zoom;
            pbGrabadora.TabIndex = 17;
            pbGrabadora.TabStop = false;
            pbGrabadora.Click += pbGrabadora_Click;
            // 
            // pbVisorDocumentos
            // 
            pbVisorDocumentos.Cursor = Cursors.Hand;
            pbVisorDocumentos.Image = Properties.Resources.visor;
            pbVisorDocumentos.Location = new Point(371, 26);
            pbVisorDocumentos.Name = "pbVisorDocumentos";
            pbVisorDocumentos.Size = new Size(66, 43);
            pbVisorDocumentos.SizeMode = PictureBoxSizeMode.Zoom;
            pbVisorDocumentos.TabIndex = 16;
            pbVisorDocumentos.TabStop = false;
            pbVisorDocumentos.Click += pbVisorDocumentos_Click;
            // 
            // pbDataFusion
            // 
            pbDataFusion.Cursor = Cursors.Hand;
            pbDataFusion.Image = Properties.Resources.database;
            pbDataFusion.Location = new Point(294, 26);
            pbDataFusion.Name = "pbDataFusion";
            pbDataFusion.Size = new Size(66, 43);
            pbDataFusion.SizeMode = PictureBoxSizeMode.Zoom;
            pbDataFusion.TabIndex = 15;
            pbDataFusion.TabStop = false;
            pbDataFusion.Click += pbDataFusion_Click;
            // 
            // pbReproductorMP3
            // 
            pbReproductorMP3.Cursor = Cursors.Hand;
            pbReproductorMP3.Image = Properties.Resources.music;
            pbReproductorMP3.Location = new Point(78, 26);
            pbReproductorMP3.Name = "pbReproductorMP3";
            pbReproductorMP3.Size = new Size(66, 43);
            pbReproductorMP3.SizeMode = PictureBoxSizeMode.Zoom;
            pbReproductorMP3.TabIndex = 14;
            pbReproductorMP3.TabStop = false;
            pbReproductorMP3.Click += pbReproductorMP3_Click;
            // 
            // pbReproductorVideo
            // 
            pbReproductorVideo.Cursor = Cursors.Hand;
            pbReproductorVideo.Image = Properties.Resources.video;
            pbReproductorVideo.Location = new Point(150, 26);
            pbReproductorVideo.Name = "pbReproductorVideo";
            pbReproductorVideo.Size = new Size(66, 43);
            pbReproductorVideo.SizeMode = PictureBoxSizeMode.Zoom;
            pbReproductorVideo.TabIndex = 13;
            pbReproductorVideo.TabStop = false;
            pbReproductorVideo.Click += pbReproductorVideo_Click;
            // 
            // pbEditorFoto
            // 
            pbEditorFoto.Cursor = Cursors.Hand;
            pbEditorFoto.Image = Properties.Resources.file_image;
            pbEditorFoto.Location = new Point(222, 26);
            pbEditorFoto.Name = "pbEditorFoto";
            pbEditorFoto.Size = new Size(66, 43);
            pbEditorFoto.SizeMode = PictureBoxSizeMode.Zoom;
            pbEditorFoto.TabIndex = 12;
            pbEditorFoto.TabStop = false;
            pbEditorFoto.Click += pbEditorFoto_Click;
            // 
            // ExploradorArchivosForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 30);
            ClientSize = new Size(1143, 750);
            Controls.Add(groupBox1);
            Controls.Add(pbAbrir);
            Controls.Add(pbRegresar);
            Controls.Add(label1);
            Controls.Add(comboVisualizacion);
            Controls.Add(splitContainer1);
            Controls.Add(txtRutaActual);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 5, 4, 5);
            Name = "ExploradorArchivosForm";
            Text = "Explorador de Archivos";
            WindowState = FormWindowState.Maximized;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbRegresar).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbAbrir).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbCamara).EndInit();
            groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbGrabadora).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbVisorDocumentos).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbDataFusion).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbReproductorMP3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbReproductorVideo).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbEditorFoto).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ListView listViewArchivos;
        private TextBox txtRutaActual;
        private ImageList imageListIconos;
        private SplitContainer splitContainer1;
        private TreeView treeViewDirectorios;
        private ComboBox comboVisualizacion;
        private Label label1;
        private ImageList imageListIconosGrandes;
        private ImageList imageListIconosMuyGrandes;
        private ImageList imageListIconosMedianos;
        private PictureBox pbRegresar;
        private PictureBox pbAbrir;
        private PictureBox pbCamara;
        private GroupBox groupBox1;
        private PictureBox pbEditorFoto;
        private PictureBox pbReproductorMP3;
        private PictureBox pictureBox1;
        private PictureBox pbReproductorVideo;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private PictureBox pbVisorDocumentos;
        private PictureBox pbDataFusion;
        private PictureBox pbGrabadora;
    }
}

