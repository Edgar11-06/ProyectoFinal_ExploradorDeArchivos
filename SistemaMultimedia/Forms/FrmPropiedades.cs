using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using SistemaMultimedia.Utilities;

namespace SistemaMultimedia.Forms
{
    /// <summary>
    /// Formulario para mostrar propiedades detalladas de un archivo o carpeta.
    /// Se muestran pestañas General y Detalles. Utiliza FileInfo / DirectoryInfo.
    /// </summary>
    public partial class FrmPropiedades : Form
    {
        private readonly string _path;
        private readonly bool _isDirectory;

        public FrmPropiedades(string path)
        {
            InitializeComponent();
            _path = path ?? throw new ArgumentNullException(nameof(path));
            _isDirectory = Directory.Exists(path);

            LoadBasicInfo();
            // Cargas pesadas (tamaño recursivo) en background
            Task.Run(() => LoadExtendedInfoAsync());
        }

        private void LoadBasicInfo()
        {
            try
            {
                if (_isDirectory)
                {
                    var di = new DirectoryInfo(_path);
                    lblName.Text = di.Name;
                    lblFullPath.Text = di.FullName;
                    lblType.Text = "Carpeta";
                    lblCreated.Text = di.CreationTime.ToString();
                    lblModified.Text = di.LastWriteTime.ToString();
                    lblAttributes.Text = di.Attributes.ToString();

                    var ico = FileHelpers.GetSystemIcon(_path, small: false, treatAsDirectory: true);
                    if (ico != null) picIcon.Image = ico.ToBitmap();
                }
                else
                {
                    var fi = new FileInfo(_path);
                    lblName.Text = fi.Name;
                    lblFullPath.Text = fi.FullName;
                    lblType.Text = FileHelpers.DetectFileCategory(_path) + " (" + fi.Extension + ")";
                    lblCreated.Text = fi.CreationTime.ToString();
                    lblModified.Text = fi.LastWriteTime.ToString();
                    lblAttributes.Text = fi.Attributes.ToString();
                    lblSize.Text = FileHelpers.GetReadableSize(fi.Length);

                    var ico = FileHelpers.GetSystemIcon(_path, small: false, treatAsDirectory: false);
                    if (ico != null) picIcon.Image = ico.ToBitmap();

                    // Si es imagen intentar mostrar resolución y formato
                    try
                    {
                        var ext = fi.Extension.ToLowerInvariant();
                        if (new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff", ".webp" }.Contains(ext))
                        {
                            using var img = Image.FromFile(_path);
                            lblImageResolution.Text = $"{img.Width} x {img.Height}";
                            lblImageFormat.Text = img.RawFormat.ToString();
                            lblImageDepth.Text = Image.GetPixelFormatSize(img.PixelFormat) + " bits";
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error leyendo propiedades: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void LoadExtendedInfoAsync()
        {
            try
            {
                if (_isDirectory)
                {
                    var di = new DirectoryInfo(_path);

                    // Usar método robusto para contar archivos y carpetas (maneja accesos denegados)
                    var counts = FileHelpers.GetDirectoryCounts(_path);
                    int fileCount = counts.files;
                    int dirCount = counts.directories;

                    long totalSize = FileHelpers.GetDirectorySize(_path);

                    this.BeginInvoke(() =>
                    {
                        lblFilesCount.Text = $"Archivos: {fileCount}";
                        lblFoldersCount.Text = $"Subcarpetas: {dirCount}";
                        lblSize.Text = FileHelpers.GetReadableSize(totalSize);
                    });
                }
                else
                {
                    var fi = new FileInfo(_path);
                    this.BeginInvoke(() =>
                    {
                        lblFullPath.Text = fi.FullName;
                        lblSize.Text = FileHelpers.GetReadableSize(fi.Length);
                    });

                    // Intentar metadatos de audio/video con MediaPlayer si está disponible
                    try
                    {
                        var category = FileHelpers.DetectFileCategory(_path);
                        if (category == "Video" || category == "Audio")
                        {
                            // Intento simple usando Windows Media Player COM no incluido. Omitir si no disponible.
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
