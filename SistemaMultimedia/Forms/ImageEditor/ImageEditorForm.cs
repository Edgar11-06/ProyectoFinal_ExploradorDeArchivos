using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using FotoGeolocalizada;
using Imagenes.Shared.Services;

namespace SistemaMultimedia.Forms.ImageEditor;

public partial class ImageEditorForm : Form
{
    private readonly string? _initialPath;
    private Bitmap? _currentImage;
    private string? _currentImagePath;
    private bool _isDrawingLine;
    private bool _isPlacingText;
    private bool _isMouseDownForLine;
    private Point _lastPoint;
    private Bitmap? _previewImage;
    private Bitmap? _adjustmentPreview;
    private bool _previewIsAdjustment;
    private Color _currentLineColor = Color.Red;
    private float _currentLineThickness = 2f;
    private string _pendingText = string.Empty;
    private Font _pendingFont = new(FontFamily.GenericSansSerif, 16);
    private Color _pendingTextColor = Color.Black;
    private readonly Stack<Bitmap> _undoStack = new();
    private readonly Stack<Bitmap> _redoStack = new();

    public ImageEditorForm(string? imagePath = null)
    {
        _initialPath = imagePath;
        InitializeComponent();
        // runtime fix: asegurar que el botón Abrir sea visible y esté al frente
        try { buttonOpen.Visible = true; } catch { }
        try { buttonOpen.BringToFront(); } catch { }
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (!string.IsNullOrWhiteSpace(_initialPath))
        {
            LoadImageFromPath(_initialPath);
        }
    }

    private void buttonOpen_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "Imágenes|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tif;*.tiff;*.webp|Todos los archivos|*.*",
            Title = "Seleccione una imagen"
        };

        if (dialog.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        LoadImageFromPath(dialog.FileName);
    }

    private void LoadImageFromPath(string path)
    {
        try
        {
            using var image = Image.FromFile(path);
            _currentImage?.Dispose();
            _currentImage = ImageMetadataService.CloneWithMetadata(image);
            _currentImagePath = path;
            txtPath.Text = path;
            pictureBoxImage.Image = _currentImage;
            ClearHistory();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar imagen: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnGeolocalizacion_Click(object? sender, EventArgs e)
    {
        if (!EnsureImageLoaded())
        {
            return;
        }

        GeolocalizacionForm.ShowForImage(this, _currentImage!, _currentImagePath);
    }

    private void ClearHistory()
    {
        while (_undoStack.Count > 0)
        {
            _undoStack.Pop().Dispose();
        }

        while (_redoStack.Count > 0)
        {
            _redoStack.Pop().Dispose();
        }
    }

    private void pictureBoxImage_Click(object? sender, EventArgs e)
    {
        if (!_isPlacingText || _currentImage == null)
        {
            return;
        }

        if (e is MouseEventArgs mouse && mouse.Button == MouseButtons.Left)
        {
            var point = MapToImageCoordinates(mouse.Location);
            var bitmap = new Bitmap(_currentImage);
            using var graphics = Graphics.FromImage(bitmap);
            using var brush = new SolidBrush(_pendingTextColor);
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            graphics.DrawString(_pendingText, _pendingFont, brush, point);
            _isPlacingText = false;
            _pendingText = string.Empty;
            ApplyNewImage(bitmap, "_text");
        }
    }

    private void MenuGrayscale_Click(object? sender, EventArgs e)
    {
        if (!EnsureImageLoaded())
        {
            return;
        }

        ClearAdjustmentPreview();
        ApplyNewImage(ImageProcessingHelper.ApplyGrayscale(_currentImage!), "_grayscale");
    }

    private void MenuSepia_Click(object? sender, EventArgs e)
    {
        if (!EnsureImageLoaded())
        {
            return;
        }

        ClearAdjustmentPreview();
        ApplyNewImage(ImageProcessingHelper.ApplySepia(_currentImage!), "_sepia");
    }

    private void btnRotate90_Click(object? sender, EventArgs e) => RotateImage(90);
    private void btnRotate180_Click(object? sender, EventArgs e) => RotateImage(180);
    private void btnRotate270_Click(object? sender, EventArgs e) => RotateImage(270);

    private void RotateImage(int degrees)
    {
        if (!EnsureImageLoaded())
        {
            return;
        }

        var bitmap = new Bitmap(_currentImage!);
        switch (degrees)
        {
            case 90: bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone); break;
            case 180: bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone); break;
            case 270: bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone); break;
        }

        ApplyNewImage(bitmap, $"_rot{degrees}");
    }

    private void MenuResize_Click(object? sender, EventArgs e)
    {
        if (!EnsureImageLoaded())
        {
            return;
        }

        ClearAdjustmentPreview();
        if (!ResizeDialog.TryGetSize(_currentImage!.Width, _currentImage.Height, out var width, out var height))
        {
            return;
        }

        var bitmap = new Bitmap(width, height);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.DrawImage(_currentImage, 0, 0, width, height);
        ApplyNewImage(bitmap, $"_resized_{width}x{height}");
    }

    private void trackBarBrightness_Scroll(object? sender, EventArgs e)
    {
        lblBrightness.Text = $"Brillo: {trackBarBrightness.Value}";
        ShowAdjustmentPreview();
    }

    private void trackBarContrast_Scroll(object? sender, EventArgs e)
    {
        lblContrast.Text = $"Contraste: {trackBarContrast.Value}";
        ShowAdjustmentPreview();
    }

    private void ClearAdjustmentPreview()
    {
        _adjustmentPreview?.Dispose();
        _adjustmentPreview = null;

        if (_previewIsAdjustment)
        {
            pictureBoxImage.Image = _currentImage;
            _previewIsAdjustment = false;
        }
    }

    private void ShowAdjustmentPreview()
    {
        if (_currentImage == null)
        {
            return;
        }

        var brightness = trackBarBrightness.Value;
        var contrast = trackBarContrast.Value;
        _adjustmentPreview?.Dispose();

        if (brightness == 0 && contrast == 0)
        {
            ClearAdjustmentPreview();
            return;
        }

        _adjustmentPreview = ImageProcessingHelper.AdjustBrightnessContrast(_currentImage, brightness, contrast);
        pictureBoxImage.Image = _adjustmentPreview;
        _previewIsAdjustment = true;
    }

    private void MenuSave_Click(object? sender, EventArgs e)
    {
        if (!EnsureImageLoaded())
        {
            return;
        }

        ClearAdjustmentPreview();
        using var dialog = new SaveFileDialog
        {
            InitialDirectory = string.IsNullOrEmpty(_currentImagePath)
                ? Environment.CurrentDirectory
                : Path.GetDirectoryName(_currentImagePath),
            FileName = (string.IsNullOrEmpty(_currentImagePath) ? "image" : Path.GetFileNameWithoutExtension(_currentImagePath))
                + "_saved_" + DateTime.Now.ToString("yyyyMMddHHmmss"),
            Filter = "PNG (*.png)|*.png|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg",
            Title = "Guardar imagen como"
        };

        if (dialog.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        SaveToPath(dialog.FileName);
    }

    private void MenuAddText_Click(object? sender, EventArgs e)
    {
        if (!EnsureImageLoaded())
        {
            return;
        }

        ClearAdjustmentPreview();
        var text = TextInputDialog.Prompt("Texto a añadir", "Añadir texto");
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        using var fontDialog = new FontDialog { Font = _pendingFont };
        if (fontDialog.ShowDialog() == DialogResult.OK)
        {
            _pendingFont = fontDialog.Font;
        }

        using var colorDialog = new ColorDialog { Color = _pendingTextColor };
        if (colorDialog.ShowDialog() == DialogResult.OK)
        {
            _pendingTextColor = colorDialog.Color;
        }

        _pendingText = text;
        _isPlacingText = true;
        MessageBox.Show(
            "Haga clic en la imagen para colocar el texto.",
            "Añadir texto",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void MenuDrawLine_Click(object? sender, EventArgs e)
    {
        if (!EnsureImageLoaded())
        {
            return;
        }

        ClearAdjustmentPreview();
        using var colorDialog = new ColorDialog { Color = _currentLineColor };
        if (colorDialog.ShowDialog() == DialogResult.OK)
        {
            _currentLineColor = colorDialog.Color;
        }

        var thicknessText = TextInputDialog.Prompt("Grosor (px)", "Dibujar línea", _currentLineThickness.ToString());
        if (float.TryParse(thicknessText, out var thickness) && thickness > 0)
        {
            _currentLineThickness = thickness;
        }

        _isDrawingLine = true;
        MessageBox.Show(
            "Mantenga pulsado y arrastre sobre la imagen para dibujar.",
            "Dibujar libre",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void pictureBoxImage_MouseDown(object? sender, MouseEventArgs e)
    {
        if (!_isDrawingLine || e.Button != MouseButtons.Left || _currentImage == null)
        {
            return;
        }

        ClearAdjustmentPreview();
        _isMouseDownForLine = true;
        _lastPoint = MapToImageCoordinates(e.Location);
        _previewImage?.Dispose();
        _previewImage = new Bitmap(_currentImage);
    }

    private void pictureBoxImage_MouseMove(object? sender, MouseEventArgs e)
    {
        if (!_isDrawingLine || !_isMouseDownForLine || _currentImage == null || _previewImage == null)
        {
            return;
        }

        var point = MapToImageCoordinates(e.Location);
        using var graphics = Graphics.FromImage(_previewImage);
        using var pen = new Pen(_currentLineColor, _currentLineThickness)
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round
        };
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.DrawLine(pen, _lastPoint, point);
        _lastPoint = point;
        pictureBoxImage.Image = _previewImage;
    }

    private void pictureBoxImage_MouseUp(object? sender, MouseEventArgs e)
    {
        if (!_isDrawingLine || !_isMouseDownForLine || e.Button != MouseButtons.Left || _currentImage == null)
        {
            return;
        }

        _isMouseDownForLine = false;
        if (_previewImage == null)
        {
            return;
        }

        var bitmap = new Bitmap(_previewImage);
        _previewImage.Dispose();
        _previewImage = null;
        ApplyNewImage(bitmap, "_draw");
    }

    private Point MapToImageCoordinates(Point mousePoint)
    {
        if (pictureBoxImage.Image == null)
        {
            return mousePoint;
        }

        var image = pictureBoxImage.Image;
        var imageWidth = image.Width;
        var imageHeight = image.Height;
        var controlWidth = pictureBoxImage.Width;
        var controlHeight = pictureBoxImage.Height;
        var imageAspect = (float)imageWidth / imageHeight;
        var controlAspect = (float)controlWidth / controlHeight;
        int drawWidth;
        int drawHeight;
        int offsetX;
        int offsetY;

        if (imageAspect > controlAspect)
        {
            drawWidth = controlWidth;
            drawHeight = (int)(controlWidth / imageAspect);
            offsetX = 0;
            offsetY = (controlHeight - drawHeight) / 2;
        }
        else
        {
            drawHeight = controlHeight;
            drawWidth = (int)(controlHeight * imageAspect);
            offsetY = 0;
            offsetX = (controlWidth - drawWidth) / 2;
        }

        var x = Math.Clamp(mousePoint.X - offsetX, 0, drawWidth);
        var y = Math.Clamp(mousePoint.Y - offsetY, 0, drawHeight);
        var imageX = (int)(x * (imageWidth / (float)drawWidth));
        var imageY = (int)(y * (imageHeight / (float)drawHeight));
        return new Point(imageX, imageY);
    }

    private bool EnsureImageLoaded()
    {
        if (_currentImage != null)
        {
            return true;
        }

        MessageBox.Show("Cargue primero una imagen.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return false;
    }

    private void ApplyNewImage(Bitmap bitmap, string suffix)
    {
        if (_currentImage != null)
        {
            ImageMetadataService.CopyMetadata(_currentImage, bitmap);
            _undoStack.Push(new Bitmap(_currentImage));
            ClearRedoStack();
        }

        _currentImage = bitmap;
        pictureBoxImage.Image = _currentImage;

        if (string.IsNullOrEmpty(_currentImagePath))
        {
            return;
        }

        try
        {
            var directory = Path.GetDirectoryName(_currentImagePath)!;
            var name = Path.GetFileNameWithoutExtension(_currentImagePath);
            var extension = Path.GetExtension(_currentImagePath);
            var outputPath = Path.Combine(directory, $"{name}{suffix}{extension}");
            bitmap.Save(outputPath);
        }
        catch
        {
        }
    }

    private void ClearRedoStack()
    {
        while (_redoStack.Count > 0)
        {
            _redoStack.Pop().Dispose();
        }
    }

    private void Undo()
    {
        if (_undoStack.Count == 0)
        {
            return;
        }

        if (_currentImage != null)
        {
            _redoStack.Push(new Bitmap(_currentImage));
        }

        var bitmap = _undoStack.Pop();
        _currentImage?.Dispose();
        _currentImage = bitmap;
        pictureBoxImage.Image = _currentImage;
    }

    private void Redo()
    {
        if (_redoStack.Count == 0)
        {
            return;
        }

        if (_currentImage != null)
        {
            _undoStack.Push(new Bitmap(_currentImage));
        }

        var bitmap = _redoStack.Pop();
        _currentImage?.Dispose();
        _currentImage = bitmap;
        pictureBoxImage.Image = _currentImage;
    }

    private void UndoMenu_Click(object? sender, EventArgs e) => Undo();
    private void RedoMenu_Click(object? sender, EventArgs e) => Redo();

    private void SaveToPath(string path)
    {
        try
        {
            var extension = Path.GetExtension(path).ToLowerInvariant();
            if (extension is ".jpg" or ".jpeg")
            {
                var codec = ImageCodecInfo.GetImageDecoders().FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);
                using var parameters = new EncoderParameters(1);
                parameters.Param[0] = new EncoderParameter(Encoder.Quality, 90L);
                _currentImage!.Save(path, codec, parameters);
            }
            else
            {
                _currentImage!.Save(path, ImageFormat.Png);
            }

            MessageBox.Show($"Imagen guardada:\n{path}", "Guardado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al guardar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        ClearAdjustmentPreview();
        _previewImage?.Dispose();
        _currentImage?.Dispose();
        ClearHistory();
        base.OnFormClosed(e);
    }
}
