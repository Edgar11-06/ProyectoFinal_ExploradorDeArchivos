using SistemaMultimedia.Forms.ImageEditor;
using SistemaMultimedia.Utilities;

namespace SistemaMultimedia.Forms;

public partial class GalleryForm : Form
{
    private readonly List<string> _mediaFiles;

    public GalleryForm(List<string> mediaFiles)
    {
        InitializeComponent();
        _mediaFiles = mediaFiles;
        ConfigurarListView();
        CargarElementosEnGaleria();
        listViewGallery.DoubleClick += ListViewGallery_DoubleClick;
    }

    private void ConfigurarListView()
    {
        listViewGallery.View = View.LargeIcon;
    }

    private void ListViewGallery_DoubleClick(object? sender, EventArgs e)
    {
        if (listViewGallery.SelectedItems.Count == 0)
        {
            return;
        }

        var path = listViewGallery.SelectedItems[0].Tag as string;
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            return;
        }

        var extension = Path.GetExtension(path);
        try
        {
            if (FileTypeHelper.IsImage(extension))
            {
                ImageEditorLauncher.Open(path, this);
                return;
            }

            if (FileTypeHelper.IsVideo(extension))
            {
                using var player = new ReproductorVideoForm();
                player.CargarVideo(path);
                player.ShowDialog(this);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"No se pudo abrir el archivo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void CargarElementosEnGaleria()
    {
        listViewGallery.Items.Clear();

        var thumbnails = new ImageList
        {
            ImageSize = new Size(128, 128),
            ColorDepth = ColorDepth.Depth32Bit
        };
        listViewGallery.LargeImageList = thumbnails;

        var imageIndex = 0;
        foreach (var path in _mediaFiles)
        {
            if (!File.Exists(path))
            {
                continue;
            }

            try
            {
                Bitmap? thumbnail = FileTypeHelper.IsVideo(Path.GetExtension(path))
                    ? MediaThumbnailHelper.TryCreateVideoThumbnail(path)
                    : LoadImageThumbnail(path);

                if (thumbnail == null)
                {
                    continue;
                }

                thumbnails.Images.Add(thumbnail);
                var item = new ListViewItem(Path.GetFileName(path))
                {
                    ImageIndex = imageIndex,
                    Tag = path
                };
                listViewGallery.Items.Add(item);
                imageIndex++;
            }
            catch
            {
            }
        }
    }

    private static Bitmap? LoadImageThumbnail(string path)
    {
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        return new Bitmap(stream);
    }
}
