using DocumentFormat.OpenXml.Presentation;
using SistemaMultimedia.Forms.ImageEditor;
using SistemaMultimedia.Integration;
using SistemaMultimedia.Utilities;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using VisorEditorDocumentos;

namespace SistemaMultimedia.Forms
{
    public partial class ExploradorArchivosForm : Form
    {
        private Stack<string> historialRutas = new Stack<string>();
        private string rutaActual = "";

        public ExploradorArchivosForm()
        {
            InitializeComponent();
            ConfigurarListView();
            ConfigurarImageList();
            CargarUnidadesDelSistema();

            comboVisualizacion.SelectedIndex = 5;

            // Añadir opción de menú contextual para enviar archivo por correo
            var ctx = new ContextMenuStrip();
            var openItem = new ToolStripMenuItem("Abrir");
            openItem.Click += (s, e) => { if (listViewArchivos.SelectedItems.Count > 0) { var it = listViewArchivos.SelectedItems[0]; var path = it.Tag?.ToString(); if (!string.IsNullOrEmpty(path)) AbrirArchivo(path); } };
            var renameItem = new ToolStripMenuItem("Renombrar");
            renameItem.Click += (s, e) => { if (listViewArchivos.SelectedItems.Count > 0) listViewArchivos.SelectedItems[0].BeginEdit(); };
            var deleteItem = new ToolStripMenuItem("Eliminar");
            deleteItem.Click += (s, e) => { if (listViewArchivos.SelectedItems.Count > 0) { var it = listViewArchivos.SelectedItems[0]; var path = it.Tag?.ToString(); if (!string.IsNullOrEmpty(path)) { try { if (Directory.Exists(path)) Directory.Delete(path, true); else if (File.Exists(path)) File.Delete(path); CargarDirectorio(rutaActual, false); } catch (Exception ex) { MessageBox.Show($"Error al eliminar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); } } } };
            var enviarItem = new ToolStripMenuItem("Enviar por correo");
            enviarItem.Click += EnviarPorCorreoMenuItem_Click;
            var propsItem = new ToolStripMenuItem("Propiedades");
            propsItem.Click += PropiedadesMenuItem_Click;

            ctx.Items.Add(openItem);
            ctx.Items.Add(renameItem);
            ctx.Items.Add(deleteItem);
            ctx.Items.Add(new ToolStripSeparator());
            ctx.Items.Add(enviarItem);
            ctx.Items.Add(propsItem);

            listViewArchivos.ContextMenuStrip = ctx;
        }

        private void PropiedadesMenuItem_Click(object? sender, EventArgs e)
        {
            if (listViewArchivos.SelectedItems.Count == 0) return;
            var item = listViewArchivos.SelectedItems[0];
            string ruta = item.Tag?.ToString() ?? "";
            if (string.IsNullOrEmpty(ruta)) return;

            try
            {
                var frm = new FrmPropiedades(ruta);
                frm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error abriendo propiedades: {ex.Message}", "Propiedades", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarListView()
        {
            listViewArchivos.View = View.Details;
            listViewArchivos.FullRowSelect = true;
            listViewArchivos.GridLines = true;
            listViewArchivos.SmallImageList = imageListIconos;
            listViewArchivos.LabelEdit = true;

            listViewArchivos.Columns.Add("Nombre", 250);
            listViewArchivos.Columns.Add("Tipo", 120);
            listViewArchivos.Columns.Add("Tamaño", 100);
            listViewArchivos.Columns.Add("Fecha Modificación", 150);
            listViewArchivos.Columns.Add("Atributos", 100);
            listViewArchivos.AfterLabelEdit += ListViewArchivos_AfterLabelEdit;
        }

        private void ListViewArchivos_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            // e.Label == nuevo nombre (null si cancelado)
            if (e.Label == null) return; // usuario canceló

            try
            {
                var item = listViewArchivos.Items[e.Item];
                string oldPath = item.Tag?.ToString() ?? "";
                if (string.IsNullOrEmpty(oldPath)) return;

                string newName = e.Label.Trim();
                if (string.IsNullOrEmpty(newName))
                {
                    MessageBox.Show("Nombre inválido.", "Renombrar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.CancelEdit = true;
                    return;
                }

                string newPath = Path.Combine(Path.GetDirectoryName(oldPath) ?? "", newName);
                if (string.Equals(oldPath, newPath, StringComparison.OrdinalIgnoreCase)) return;

                if (File.Exists(oldPath))
                {
                    if (File.Exists(newPath)) { MessageBox.Show("Ya existe un archivo con ese nombre.", "Renombrar", MessageBoxButtons.OK, MessageBoxIcon.Warning); e.CancelEdit = true; return; }
                    File.Move(oldPath, newPath);
                }
                else if (Directory.Exists(oldPath))
                {
                    if (Directory.Exists(newPath)) { MessageBox.Show("Ya existe una carpeta con ese nombre.", "Renombrar", MessageBoxButtons.OK, MessageBoxIcon.Warning); e.CancelEdit = true; return; }
                    Directory.Move(oldPath, newPath);
                }
                else
                {
                    MessageBox.Show("El elemento original no existe.", "Renombrar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.CancelEdit = true;
                    return;
                }

                // Actualizar tag y texto
                item.Tag = newPath;
                item.Text = newName;
                CargarDirectorio(rutaActual, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al renombrar: {ex.Message}", "Renombrar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.CancelEdit = true;
            }
        }

        private void EnviarPorCorreoMenuItem_Click(object? sender, EventArgs e)
        {
            if (listViewArchivos.SelectedItems.Count == 0) return;
            var item = listViewArchivos.SelectedItems[0];
            string ruta = item.Tag?.ToString() ?? "";
            if (string.IsNullOrEmpty(ruta) || !File.Exists(ruta))
            {
                MessageBox.Show("Seleccione un archivo válido para enviar.", "Enviar por correo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var frm = new FrmEnviarCorreo(ruta);
                frm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo abrir el formulario de envío: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarImageList()
        {
            imageListIconos.ImageSize = new Size(16, 16);
            imageListIconos.ColorDepth = ColorDepth.Depth32Bit;

            imageListIconosMedianos.ImageSize = new Size(32, 32);
            imageListIconosMedianos.ColorDepth = ColorDepth.Depth32Bit;

            imageListIconosGrandes.ImageSize = new Size(48, 48);
            imageListIconosGrandes.ColorDepth = ColorDepth.Depth32Bit;

            imageListIconosMuyGrandes.ImageSize = new Size(96, 96);
            imageListIconosMuyGrandes.ColorDepth = ColorDepth.Depth32Bit;

            AgregarIconosATodosLosTamaños();
        }

        private void AgregarIconosATodosLosTamaños()
        {
            var imageLists = new[] { imageListIconos, imageListIconosMedianos,
                                     imageListIconosGrandes, imageListIconosMuyGrandes };

            foreach (var imgList in imageLists)
            {
                int size = imgList.ImageSize.Width;
                imgList.Images.Add("carpeta", TryGetResourceBitmap(new[] { "folder" }, size) ?? CrearIconoArchivo(size));
                imgList.Images.Add("archivo", TryGetResourceBitmap(new[] { "file" }, size) ?? CrearIconoArchivo(size));
                imgList.Images.Add("imagen", TryGetResourceBitmap(new[] { "file-image" }, size) ?? CrearIconoArchivo(size));
                imgList.Images.Add("audio", TryGetResourceBitmap(new[] { "file-audio" }, size) ?? CrearIconoArchivo(size));
                imgList.Images.Add("video", TryGetResourceBitmap(new[] { "file-video" }, size) ?? CrearIconoArchivo(size));
                imgList.Images.Add("texto", TryGetResourceBitmap(new[] { "txt" }, size) ?? CrearIconoTexto(size));
                imgList.Images.Add("pdf", TryGetResourceBitmap(new[] { "pdf", "file-pdf", "file-acrobat", "adobe_pdf" }, size) ?? CrearIconoArchivo(size));
                imgList.Images.Add("word", TryGetResourceBitmap(new[] { "word", "file-word", "doc", "docx" }, size) ?? CrearIconoArchivo(size));
                imgList.Images.Add("excel", TryGetResourceBitmap(new[] { "excel", "file-excel", "xls", "xlsx" }, size) ?? CrearIconoArchivo(size));
                imgList.Images.Add("powerpoint", TryGetResourceBitmap(new[] { "powerpoint", "ppt", "pptx", "file-powerpoint" }, size) ?? CrearIconoArchivo(size));
                imgList.Images.Add("unidad", TryGetResourceBitmap(new[] { "drive" }, size) ?? CrearIconoUnidad(size));
                imgList.Images.Add("inicio", TryGetResourceBitmap(new[] { "folder", }, size) ?? CrearIconoArchivo(size));
                imgList.Images.Add("escritorio", TryGetResourceBitmap(new[] { "folder" }, size) ?? CrearIconoArchivo(size));
                imgList.Images.Add("documentos", TryGetResourceBitmap(new[] { "folder-document" }, size) ?? CrearIconoArchivo(size));
                imgList.Images.Add("descargas", TryGetResourceBitmap(new[] { "folder-downloads" }, size) ?? CrearIconoArchivo(size));
                imgList.Images.Add("imagenes", TryGetResourceBitmap(new[] { "folder-imagen" }, size) ?? CrearIconoArchivo(size));
                imgList.Images.Add("musica", TryGetResourceBitmap(new[] { "folder-audio" }, size) ?? CrearIconoArchivo(size));
                imgList.Images.Add("videos", TryGetResourceBitmap(new[] { "folder-video" }, size) ?? CrearIconoArchivo(size));
            }
        }

        private Bitmap? TryGetResourceBitmap(string[] candidateNames, int size)
        {
            try
            {
                var rm = Properties.Resources.ResourceManager;
                foreach (var name in candidateNames)
                {
                    try
                    {
                        var obj = rm.GetObject(name);
                        if (obj is Bitmap bmpExact) return ResizeBitmap(bmpExact, size, size);
                        if (obj is Image imgExact) { using var tmp = new Bitmap(imgExact); return ResizeBitmap(tmp, size, size); }
                    }
                    catch { }
                }

                var set = rm.GetResourceSet(System.Globalization.CultureInfo.CurrentUICulture, true, true);
                if (set != null)
                {
                    var keys = new List<string>();
                    foreach (System.Collections.DictionaryEntry entry in set)
                    {
                        if (entry.Key is string k) keys.Add(k);
                    }

                    foreach (var candidate in candidateNames)
                    {
                        var cand = candidate.Replace('-', '_').Replace(" ", "").ToLowerInvariant();
                        var found = keys.FirstOrDefault(k => k.Replace('-', '_').Replace(" ", "").ToLowerInvariant().Contains(cand));
                        if (found != null)
                        {
                            try
                            {
                                var obj = rm.GetObject(found);
                                if (obj is Bitmap bmp) return ResizeBitmap(bmp, size, size);
                                if (obj is Image img) { using var tmp = new Bitmap(img); return ResizeBitmap(tmp, size, size); }
                            }
                            catch { }
                        }
                    }
                }
            }
            catch { }
            try
            {
                var disk = TryLoadFromResourcesFolder(candidateNames, size);
                if (disk != null) return disk;
            }
            catch { }

            return null;
        }

        private Bitmap? TryLoadFromResourcesFolder(string[] candidateNames, int size)
        {
            var tried = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var exts = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".ico", ".gif" };

            var baseCandidates = new List<string?> {
                AppDomain.CurrentDomain.BaseDirectory,
                Environment.CurrentDirectory,
                Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            };

            foreach (var basePath in baseCandidates)
            {
                if (string.IsNullOrEmpty(basePath)) continue;
                var dir = new DirectoryInfo(basePath);
                for (int depth = 0; dir != null && depth < 8; depth++)
                {
                    var resDir = Path.Combine(dir.FullName, "Resources");
                    if (Directory.Exists(resDir))
                    {
                        foreach (var candidate in candidateNames)
                        {
                            var variants = new[] { candidate, candidate.Replace('-', '_'), candidate.Replace('_', '-'), candidate.Replace(" ", ""), candidate.ToLowerInvariant() };
                            foreach (var v in variants)
                            {
                                if (string.IsNullOrWhiteSpace(v)) continue;
                                foreach (var ext in exts)
                                {
                                    var file = Path.Combine(resDir, v + ext);
                                    if (tried.Contains(file)) continue;
                                    tried.Add(file);
                                    if (File.Exists(file))
                                    {
                                        try
                                        {
                                            using var img = Image.FromFile(file);
                                            using var bmp = new Bitmap(img);
                                            return ResizeBitmap(bmp, size, size);
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                    }
                    dir = dir.Parent;
                }
            }

            return null;
        }

        private Bitmap ResizeBitmap(Bitmap source, int width, int height)
        {
            var dest = new Bitmap(width, height);
            using (var g = Graphics.FromImage(dest))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(source, 0, 0, width, height);
            }
            return dest;
        }
        private Bitmap CrearIconoArchivo(int size)
        {
            var bmp = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                int pad = Math.Max(1, size / 12);
                var fileRect = new Rectangle(pad, pad, size - pad * 2, size - pad * 2);
                g.FillRectangle(Brushes.White, fileRect);
                g.DrawRectangle(Pens.Gray, fileRect);

                int corner = Math.Max(1, size / 5);
                var pts = new Point[] {
                    new Point(fileRect.Right - corner, fileRect.Top),
                    new Point(fileRect.Right, fileRect.Top + corner),
                    new Point(fileRect.Right - corner, fileRect.Top + corner)
                };
                g.FillPolygon(Brushes.LightGray, pts);
                g.DrawPolygon(Pens.Gray, pts);
            }
            return bmp;
        }

        private Bitmap CrearIconoTexto(int size)
        {
            var bmp = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.Clear(Color.Transparent);

                int pad = Math.Max(1, size / 10);
                var paper = new Rectangle(pad, pad, size - pad * 2, size - pad * 2);
                g.FillRectangle(Brushes.WhiteSmoke, paper);
                g.DrawRectangle(Pens.Gray, paper);

                int y = paper.Top + pad;
                int gap = Math.Max(1, size / 10);
                for (int i = 0; i < 3; i++)
                {
                    g.FillRectangle(Brushes.LightGray, paper.Left + pad, y, paper.Width - pad * 2, Math.Max(1, size / 20));
                    y += gap + Math.Max(1, size / 20);
                }
            }
            return bmp;
        }

        private Bitmap CrearIconoUnidad(int size)
        {
            var bmp = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                int r = Math.Max(1, size / 3);
                var center = new Point(size / 2, size / 2);
                g.FillEllipse(Brushes.Silver, center.X - r, center.Y - r, r * 2, r * 2);
                g.FillEllipse(Brushes.DarkSlateGray, center.X - r / 2, center.Y - r / 2, r, r);
                g.DrawEllipse(Pens.Gray, center.X - r, center.Y - r, r * 2, r * 2);
            }
            return bmp;
        }

        private void comboVisualizacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboVisualizacion.SelectedIndex)
            {
                case 0:
                    listViewArchivos.View = View.LargeIcon;
                    listViewArchivos.LargeImageList = imageListIconosMuyGrandes;
                    break;
                case 1:
                    listViewArchivos.View = View.LargeIcon;
                    listViewArchivos.LargeImageList = imageListIconosGrandes;
                    break;
                case 2:
                    listViewArchivos.View = View.LargeIcon;
                    listViewArchivos.LargeImageList = imageListIconosMedianos;
                    break;
                case 3:
                    listViewArchivos.View = View.SmallIcon;
                    listViewArchivos.SmallImageList = imageListIconos;
                    break;
                case 4:
                    listViewArchivos.View = View.List;
                    listViewArchivos.SmallImageList = imageListIconos;
                    break;
                case 5:
                    listViewArchivos.View = View.Details;
                    listViewArchivos.SmallImageList = imageListIconos;
                    break;
            }

            if (!string.IsNullOrEmpty(rutaActual))
            {
                CargarDirectorio(rutaActual, false);
            }
        }

        private void CargarUnidadesDelSistema()
        {
            treeViewDirectorios.Nodes.Clear();

            AgregarCarpetasEspeciales();

            TreeNode separador = new TreeNode("Unidades")
            {
                ForeColor = Color.Gray,
                Tag = "SEPARADOR"
            };
            treeViewDirectorios.Nodes.Add(separador);

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                try
                {
                    if (drive.IsReady)
                    {
                        string etiqueta = $"{drive.Name} [{drive.DriveType}]";
                        if (drive.VolumeLabel != "")
                        {
                            etiqueta = $"{drive.VolumeLabel} ({drive.Name})";
                        }

                        TreeNode nodoUnidad = new TreeNode(etiqueta)
                        {
                            Tag = drive.Name,
                            ImageKey = "unidad",
                            SelectedImageKey = "unidad"
                        };
                        nodoUnidad.Nodes.Add(new TreeNode("Cargando...") { Tag = "dummy" });
                        treeViewDirectorios.Nodes.Add(nodoUnidad);
                    }
                }
                catch { }
            }
        }

        private void AgregarCarpetasEspeciales()
        {
            var carpetasEspeciales = new List<(string Nombre, string Ruta, string Icono)>
            {
                ("Inicio", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ""),
                ("Escritorio", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "escritorio"),
                ("Documentos", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "documentos"),
                ("Descargas", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"), "descargas"),
                ("Imágenes", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "imagenes"),
                ("Música", Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "musica"),
                ("Videos", Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "videos")
            };

            foreach (var carpeta in carpetasEspeciales)
            {
                try
                {
                    if (Directory.Exists(carpeta.Ruta))
                    {
                        TreeNode nodo = new TreeNode(carpeta.Nombre)
                        {
                            Tag = carpeta.Ruta,
                            ImageKey = carpeta.Icono,
                            SelectedImageKey = carpeta.Icono
                        };

                        try
                        {
                            DirectoryInfo dirInfo = new DirectoryInfo(carpeta.Ruta);
                            if (dirInfo.GetDirectories().Length > 0)
                            {
                                nodo.Nodes.Add(new TreeNode("Cargando...") { Tag = "dummy" });
                            }
                        }
                        catch
                        {
                            nodo.Nodes.Add(new TreeNode("Cargando...") { Tag = "dummy" });
                        }

                        treeViewDirectorios.Nodes.Add(nodo);
                    }
                }
                catch { }
            }
        }

        private void treeViewDirectorios_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Tag?.ToString() == "SEPARADOR")
            {
                e.Cancel = true;
                return;
            }

            if (e.Node.Nodes.Count > 0 && e.Node.Nodes[0].Tag?.ToString() == "dummy")
            {
                e.Node.Nodes.Clear();
                CargarSubdirectorios(e.Node);
            }
        }

        private void CargarSubdirectorios(TreeNode nodoPadre)
        {
            string ruta = nodoPadre.Tag?.ToString();
            if (string.IsNullOrEmpty(ruta) || ruta == "SEPARADOR") return;

            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(ruta);
                DirectoryInfo[] directorios;

                try
                {
                    directorios = dirInfo.GetDirectories();
                }
                catch (UnauthorizedAccessException)
                {
                    nodoPadre.Nodes.Add(new TreeNode("? Acceso denegado") { ForeColor = Color.Red });
                    return;
                }
                catch (Exception ex)
                {
                    nodoPadre.Nodes.Add(new TreeNode($"? Error: {ex.Message}") { ForeColor = Color.Red });
                    return;
                }

                if (directorios.Length == 0)
                {
                    return;
                }

                foreach (DirectoryInfo dir in directorios)
                {
                    try
                    {
                        TreeNode nodoHijo = new TreeNode(dir.Name)
                        {
                            Tag = dir.FullName,
                            ImageKey = "carpeta",
                            SelectedImageKey = "carpeta"
                        };

                        try
                        {
                            if (dir.GetDirectories().Length > 0)
                            {
                                nodoHijo.Nodes.Add(new TreeNode("Cargando...") { Tag = "dummy" });
                            }
                        }
                        catch
                        {
                            nodoHijo.Nodes.Add(new TreeNode("Cargando...") { Tag = "dummy" });
                        }

                        nodoPadre.Nodes.Add(nodoHijo);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                nodoPadre.Nodes.Add(new TreeNode($"Error: {ex.Message}") { ForeColor = Color.Red });
            }
        }

        private void treeViewDirectorios_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag?.ToString() == "SEPARADOR")
            {
                return;
            }

            if (e.Node?.Tag?.ToString() == "dummy" || e.Node?.Text.Contains("?") == true)
            {
                return;
            }

            if (e.Node != null && e.Node.Tag != null)
            {
                string ruta = e.Node.Tag.ToString();
                CargarDirectorio(ruta, false);
            }
        }

        private void btnAbrir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Seleccione un directorio";
                folderDialog.ShowNewFolderButton = false;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    historialRutas.Clear();
                    CargarDirectorio(folderDialog.SelectedPath);
                }
            }
        }

        private void btnRegresar_Click(object sender, EventArgs e)
        {
            if (historialRutas.Count > 0)
            {
                string rutaAnterior = historialRutas.Pop();
                CargarDirectorio(rutaAnterior, false);
            }
        }

        private void CargarDirectorio(string ruta, bool agregarHistorial = true)
        {
            try
            {
                if (agregarHistorial && !string.IsNullOrEmpty(rutaActual))
                {
                    historialRutas.Push(rutaActual);
                }

                rutaActual = ruta;
                listViewArchivos.Items.Clear();
                txtRutaActual.Text = ruta;
                pbRegresar.Enabled = historialRutas.Count > 0;

                DirectoryInfo dirInfo = new DirectoryInfo(ruta);

                foreach (var directorio in dirInfo.GetDirectories())
                {
                    try
                    {
                        var item = new ListViewItem(directorio.Name);
                        item.ImageKey = "carpeta";
                        item.SubItems.Add("Carpeta");
                        item.SubItems.Add("");
                        item.SubItems.Add(directorio.LastWriteTime.ToString("dd/MM/yyyy HH:mm"));
                        item.SubItems.Add(directorio.Attributes.ToString());
                        item.Tag = directorio.FullName;
                        listViewArchivos.Items.Add(item);
                    }
                    catch { }
                }

                foreach (var archivo in dirInfo.GetFiles())
                {
                    try
                    {
                        var item = new ListViewItem(archivo.Name);
                        item.ImageKey = FileTypeHelper.GetIconKey(archivo.Extension);
                        item.SubItems.Add(FileTypeHelper.GetFileTypeDescription(archivo.Extension));
                        item.SubItems.Add(FileSizeFormatter.Format(archivo.Length));
                        item.SubItems.Add(archivo.LastWriteTime.ToString("dd/MM/yyyy HH:mm"));
                        item.SubItems.Add(archivo.Attributes.ToString());
                        item.Tag = archivo.FullName;
                        listViewArchivos.Items.Add(item);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el directorio: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void listViewArchivos_DoubleClick(object sender, EventArgs e)
        {
            if (listViewArchivos.SelectedItems.Count > 0)
            {
                var item = listViewArchivos.SelectedItems[0];
                string ruta = item.Tag?.ToString();

                if (string.IsNullOrEmpty(ruta)) return;

                if (Directory.Exists(ruta))
                {
                    CargarDirectorio(ruta);
                }
                else if (File.Exists(ruta))
                {
                    AbrirArchivo(ruta);
                }
            }
        }

        private void AbrirArchivo(string ruta)
        {
            var extension = Path.GetExtension(ruta);

            try
            {
                if (FileTypeHelper.IsAudio(extension))
                {
                    var reproductor = new ReproductorMp3Form();
                    reproductor.AbrirArchivos(new[] { ruta });
                    reproductor.Show();
                    return;
                }

                if (FileTypeHelper.IsVideo(extension))
                {
                    var reproductor = new ReproductorVideoForm();
                    reproductor.CargarVideo(ruta);
                    reproductor.Show();
                    return;
                }

                if (FileTypeHelper.IsDataFile(extension))
                {
                    var analizador = new DataFusionForm(ruta);
                    analizador.Show();
                    return;
                }

                if (FileTypeHelper.IsDocument(extension))
                {
                    DocumentViewerLauncher.Open(ruta, this);
                    return;
                }

                if (FileTypeHelper.IsImage(extension))
                {
                    ImageEditorLauncher.Open(ruta, this);
                    return;
                }

                Process.Start(new ProcessStartInfo
                {
                    FileName = ruta,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo abrir el archivo: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pbCamara_Click(object sender, EventArgs e)
        {
            var camara = new CamaraForm();
            camara.Show();
        }

        private void pbEditorFoto_Click(object sender, EventArgs e)
        {
            var editor = new ImageEditorForm();
            editor.Show();
        }

        private void pbReproductorVideo_Click(object sender, EventArgs e)
        {
            var video = new ReproductorVideoForm();
            video.Show();
        }

        private void pbReproductorMP3_Click(object sender, EventArgs e)
        {
            var audio = new ReproductorMp3Form();
            audio.Show();
        }

        private void pbDataFusion_Click(object sender, EventArgs e)
        {
            var data = new DataFusionForm();
            data.Show();
        }

        private void pbVisorDocumentos_Click(object sender, EventArgs e)
        {
            var visor = new MainForm();
            visor.Show();
        }

        private void pbGrabadora_Click(object sender, EventArgs e)
        {
            var grabadora = new GrabadoraAudioForm();    
            grabadora.Show();
        }
    }
}

