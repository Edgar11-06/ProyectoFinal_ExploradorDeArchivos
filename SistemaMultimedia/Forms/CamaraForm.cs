using System.Drawing;
using OpenCvSharp;
using SistemaMultimedia.Forms.ImageEditor;
using SistemaMultimedia.Services.Camera;
using SistemaMultimedia.Utilities;

namespace SistemaMultimedia.Forms
{
    public partial class CamaraForm : Form
    {
        private readonly DeviceService _deviceService;
        private readonly CameraService _cameraService;
        private readonly RecordingService _recordingService;
        private readonly List<string> _sessionPhotos = new();
        private readonly List<string> _sessionVideos = new();
        private string? _lastRecordingPath;
        private string? _lastPreviewPath;

        public CamaraForm()
        {
            InitializeComponent();

            _deviceService = new DeviceService();
            _cameraService = new CameraService();
            _recordingService = new RecordingService(_cameraService);
            Load += CamaraForm_Load;
            FormClosing += MainForm_FormClosing;
            _cameraService.FrameArrived += CameraService_FrameArrived;
            try { comboBoxCameras.SelectedIndexChanged += ComboBoxCameras_SelectedIndexChanged; } catch { } 
            try
            {
                pbStartRecording.Enabled = true;
                pbStopRecording.Enabled = false;
            }
            catch { }
        }

        private async void ComboBoxCameras_SelectedIndexChanged(object? sender, EventArgs e)
        {
            try
            {
                var device = comboBoxCameras.SelectedItem as string;
                if (string.IsNullOrEmpty(device)) return;
               
                await _cameraService.StartCameraAsync(device, pbCamera);
                labelStatus.Text = "Cámara cambiada";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo cambiar la cámara: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CameraService_FrameArrived(OpenCvSharp.Mat mat)
        {
            try
            {
                var bytes = mat.ImEncode(".jpg");
                using var ms = new MemoryStream(bytes);
                var bmp = new Bitmap(ms);

                if (pbCamera.IsHandleCreated && !pbCamera.IsDisposed)
                {
                    pbCamera.InvokeIfRequired(() =>
                    {
                        var prev = pbCamera.Image;
                        pbCamera.Image = (Bitmap)bmp.Clone();
                        prev?.Dispose();
                    });
                }
                bmp.Dispose();
            }
            catch { }
        }

        private void pbTakePhoto_Click(object sender, EventArgs e)
        {
            try
            {
                var bmp = _cameraService.CapturePhoto();
                if (bmp == null)
                {
                    MessageBox.Show("No se pudo capturar la imagen.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var targetDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                    "Álbum de cámara");
                if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

                var fileName = $"imagen{DateTime.Now:yyyyMMddHHmmss}.jpg";
                var fullPath = Path.Combine(targetDir, fileName);

                _cameraService.SaveImage(bmp, fullPath);

                _sessionPhotos.Add(fullPath);
                _lastPreviewPath = fullPath;

                var previous = pbPreview.Image;
                try
                {
                    var img = new Bitmap(fullPath);
                    pbPreview.Image = img;
                }
                finally
                {
                    previous?.Dispose();
                    bmp.Dispose();
                }

                labelStatus.Text = $"Foto guardada: {fileName}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void CamaraForm_Load(object sender, EventArgs e)
        {
            using (new WaitCursorScope())
            {
                await InitializeDevicesAsync();

                try
                {
                    var device = comboBoxCameras.SelectedItem as string;
                    if (string.IsNullOrEmpty(device) == false)
                    {
                        await _cameraService.StartCameraAsync(device, pbCamera);
                        labelStatus.Text = "Cámara activa";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async Task InitializeDevicesAsync()
        {
            try
            {
                comboBoxCameras.Items.Clear();
                comboBoxMicrophones.Items.Clear();

                var cameras = _deviceService.GetVideoDevices();
                var mics = _deviceService.GetAudioDevices();

                comboBoxCameras.Items.AddRange(cameras);
                comboBoxMicrophones.Items.AddRange(mics);

                if (comboBoxCameras.Items.Count > 0) comboBoxCameras.SelectedIndex = 0;
                if (comboBoxMicrophones.Items.Count > 0) comboBoxMicrophones.SelectedIndex = 0;

                try { comboBoxCameras.SelectedIndexChanged += ComboBoxCameras_SelectedIndexChanged; } catch { }

                labelStatus.Text = "Listo";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inicializando dispositivos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void pbStartRecording_Click(object sender, EventArgs e)
        {
            try
            {
                try { pbStartRecording.Enabled = false; pbStopRecording.Enabled = true; } catch { }
                var cam = comboBoxCameras.SelectedItem as string;
                var mic = comboBoxMicrophones.SelectedItem as string;
                if (string.IsNullOrEmpty(cam)) return;

                var targetDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                    "Grabaciones");
                if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

                var fileName = $"video{DateTime.Now:yyyyMMdd-HHmmss}.mp4";
                var fullPath = Path.Combine(targetDir, fileName);

                var savedPath = await _recordingService.StartRecordingAsync(fullPath, cam, mic);
                _lastRecordingPath = savedPath;
                if (!_sessionVideos.Contains(savedPath)) _sessionVideos.Add(savedPath);

                labelStatus.Text = "Grabando...";
                timerRecording.Start();
            }
            catch (Exception ex)
            {
                try { pbStartRecording.Enabled = true; pbStopRecording.Enabled = false; } catch { }
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void timerRecording_Tick(object sender, EventArgs e)
        {
            labelRecordingTime.Text = _recordingService.GetRecordingDuration().ToString("hh\\:mm\\:ss");
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            timerRecording.Stop();
            _recordingService.Dispose();
            try { _cameraService.FrameArrived -= CameraService_FrameArrived; } catch { }
            _cameraService.Dispose();
        }

        private void pbPreview_Click(object sender, EventArgs e)
        {
            try
            {
                var allMedia = new System.Collections.Generic.List<string>();
                allMedia.AddRange(_sessionPhotos);
                allMedia.AddRange(_sessionVideos);

                if (allMedia.Count == 0)
                {
                    MessageBox.Show("No hay fotos ni videos en la sesión.", "Galería", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var gallery = new GalleryForm(allMedia);
                gallery.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pbPreview_DoubleClick(object? sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(_lastPreviewPath) &&
                    File.Exists(_lastPreviewPath) &&
                    FileTypeHelper.IsImage(Path.GetExtension(_lastPreviewPath)))
                {
                    ImageEditorLauncher.Open(_lastPreviewPath, this);
                    return;
                }

                if (!string.IsNullOrEmpty(_lastRecordingPath) && File.Exists(_lastRecordingPath))
                {
                    using var player = new ReproductorVideoForm();
                    player.CargarVideo(_lastRecordingPath);
                    player.ShowDialog(this);
                    return;
                }

                pbPreview_Click(sender, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void pbStopRecording_Click(object sender, EventArgs e)
        {
            try
            {
                await _recordingService.StopRecordingAsync();
                try { pbStartRecording.Enabled = true; pbStopRecording.Enabled = false; } catch { }
                labelStatus.Text = "Grabación detenida";
                timerRecording.Stop();
                labelRecordingTime.Text = "00:00:00";

                if (!string.IsNullOrEmpty(_lastRecordingPath))
                {
                    bool found = false;
                    Bitmap? thumb = null;
                    for (int i = 0; i < 10; i++)
                    {
                        try
                        {
                            if (File.Exists(_lastRecordingPath))
                            {
                                found = true;
                                thumb = MediaThumbnailHelper.TryCreateVideoThumbnail(_lastRecordingPath);
                                _lastPreviewPath = _lastRecordingPath;
                                if (thumb != null)
                                {
                                    break;
                                }
                            }
                        }
                        catch { }

                        await Task.Delay(200);
                    }

                    if (found && thumb != null)
                    {
                        var previous = pbPreview.Image;
                        try
                        {
                            pbPreview.InvokeIfRequired(() => { pbPreview.Image = (Bitmap)thumb.Clone(); });
                        }
                        finally
                        {
                            previous?.Dispose();
                            thumb.Dispose();
                        }

                        if (!_sessionVideos.Contains(_lastRecordingPath)) _sessionVideos.Add(_lastRecordingPath);
                    }
                    else
                    {
                        if (!found) MessageBox.Show("No se encontró el archivo de vídeo generado.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else MessageBox.Show("No se pudo generar la miniatura del vídeo.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
