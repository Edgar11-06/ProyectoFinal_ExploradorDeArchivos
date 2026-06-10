using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using Imagenes.Shared.Models;
using Imagenes.Shared.Services;

namespace FotoGeolocalizada;

public partial class GeolocalizacionForm : Form
{
    private double _latitude;
    private double _longitude;
    private Image? _currentImage;
    private string? _currentImagePath;
    private bool _ownsImage;

    public GeolocalizacionForm()
    {
        InitializeComponent();
    }

    public static DialogResult ShowForImage(IWin32Window? owner, Image sourceImage, string? sourcePath = null)
    {
        using var form = new GeolocalizacionForm();
        form.LoadImage(sourceImage, sourcePath, takeOwnership: false);
        return form.ShowDialog(owner);
    }

    public void LoadImage(Image sourceImage, string? sourcePath, bool takeOwnership = false)
    {
        ReleaseCurrentImage();

        _currentImage = takeOwnership
            ? sourceImage
            : ImageMetadataService.CloneWithMetadata(sourceImage);
        _ownsImage = !takeOwnership;
        _currentImagePath = sourcePath;

        ApplyCoordinatesFromImage();
    }

    private void ReleaseCurrentImage()
    {
        if (_currentImage == null)
        {
            return;
        }

        if (_ownsImage)
        {
            _currentImage.Dispose();
        }

        _currentImage = null;
        _ownsImage = false;
    }

    private void ApplyCoordinatesFromImage()
    {
        if (_currentImage == null)
        {
            return;
        }

        var coordinates = GpsExifService.TryReadCoordinates(_currentImage, _currentImagePath);
        if (coordinates.HasValue)
        {
            SetCoordinates(coordinates.Value);
            txtCoordenadas.Text =
                $"{FormatDecimal(coordinates.Value.Latitude)}, {FormatDecimal(coordinates.Value.Longitude)}";
            _ = MostrarMapaAsync(coordinates.Value.Latitude, coordinates.Value.Longitude);
            return;
        }

        txtCoordenadas.Text = "No se encontraron datos GPS. Ingrese manualmente y presione \"Guardar con GPS\".";
        _latitude = 0;
        _longitude = 0;
    }

    private void SetCoordinates(GpsCoordinates coordinates)
    {
        _latitude = coordinates.Latitude;
        _longitude = coordinates.Longitude;
        txtLatManual.Text = FormatDecimal(coordinates.Latitude);
        txtLonManual.Text = FormatDecimal(coordinates.Longitude);
    }

    private static string FormatDecimal(double value) =>
        value.ToString("F6", CultureInfo.InvariantCulture);

    private async Task MostrarMapaAsync(double latitude, double longitude)
    {
        await webMapa.EnsureCoreWebView2Async();

        var html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <style>
      html, body, iframe {{ height:100%; width:100%; margin:0; padding:0; }}
      body {{ background:#fff; }}
    </style>
</head>
<body>
    <iframe
        src='https://www.google.com/maps?q={latitude},{longitude}&z=15&output=embed&iwloc=near'
        frameborder='0'
        style='border:0'
        allowfullscreen>
    </iframe>
</body>
</html>";

        webMapa.NavigateToString(html);
    }

    private void btnMostrarUbicacion_Click(object? sender, EventArgs e)
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = $"https://www.google.com/maps?q={_latitude},{_longitude}",
            UseShellExecute = true
        });
    }

    private void btnGuardar_Click(object? sender, EventArgs e)
    {
        if (_currentImage == null)
        {
            MessageBox.Show("Cargue primero una imagen.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (!TryParseCoordinate(txtLatManual.Text, out var latManual) ||
            !TryParseCoordinate(txtLonManual.Text, out var lonManual))
        {
            MessageBox.Show(
                "Latitud o longitud no válidas. Use formato decimal (ej. 19.123456).",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }

        try
        {
            GpsExifService.WriteCoordinates(_currentImage, latManual, lonManual);

            using var sfd = new SaveFileDialog
            {
                Filter = "JPEG Image|*.jpg;*.jpeg",
                FileName = Path.GetFileNameWithoutExtension(_currentImagePath ?? "imagen") + "_gps.jpg"
            };

            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            _currentImage.Save(sfd.FileName, ImageFormat.Jpeg);
            MessageBox.Show("Imagen guardada con GPS.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ReleaseCurrentImage();
            txtLatManual.Clear();
            txtLonManual.Clear();
            txtCoordenadas.Clear();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al guardar EXIF GPS: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static bool TryParseCoordinate(string? text, out double value)
    {
        return double.TryParse(
            text?.Replace(',', '.'),
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out value);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        ReleaseCurrentImage();
        base.OnFormClosed(e);
    }
}
