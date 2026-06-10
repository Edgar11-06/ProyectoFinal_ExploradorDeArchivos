using System;
using DirectShowLib;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using System.IO;
using System.Drawing.Imaging;

namespace SistemaMultimedia.Services.Camera
{
    public class CameraService : IDisposable
    {
        public event Action<Mat>? FrameArrived;
        private VideoCapture? _capture;
        private CancellationTokenSource? _cancellation;
        private Task? _captureTask;
        private Bitmap? _lastBitmap;
        private readonly object _bitmapLock = new object();

        public async Task StartCameraAsync(string deviceName, PictureBox target)
        {
            StopCamera();
            int index = 0;
            try
            {
                var devices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
                for (int i = 0; i < devices.Length; i++)
                {
                    if (string.Equals(devices[i].Name, deviceName, StringComparison.OrdinalIgnoreCase))
                    {
                        index = i;
                        break;
                    }
                }
            }
            catch
            {
                // if DirectShow fails for any reason, fallback to index 0
                index = 0;
            }

            _capture = new VideoCapture(index);
            if (!_capture.IsOpened())
            {
                throw new InvalidOperationException("No se pudo abrir la cámara");
            }

            _cancellation = new CancellationTokenSource();
            var token = _cancellation.Token;

            _captureTask = Task.Run(() =>
            {
                using var mat = new Mat();
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        if (!_capture.Read(mat))
                        {
                            continue;
                        }

                        try { FrameArrived?.Invoke(mat); } catch { }

                        var imgBytes = mat.ImEncode(".jpg");
                        using var ms = new MemoryStream(imgBytes);
                        var bmp = new Bitmap(ms);
                        lock (_bitmapLock)
                        {
                            var prev = _lastBitmap;
                            _lastBitmap = (Bitmap)bmp.Clone();
                            prev?.Dispose();
                        }
                        bmp.Dispose();
                    }
                    catch
                    {
                    }
                }
            }, token);

            await Task.CompletedTask;
        }

        public void StopCamera()
        {
            try
            {
                _cancellation?.Cancel();
                _captureTask?.Wait(500);
            }
            catch { }

            _captureTask = null;
            _cancellation?.Dispose();
            _cancellation = null;

            if (_capture != null)
            {
                _capture.Release();
                _capture.Dispose();
                _capture = null;
            }
        }

        public Bitmap? CapturePhoto()
        {
            lock (_bitmapLock)
            {
                if (_lastBitmap == null) return null;
                return (Bitmap)_lastBitmap.Clone();
            }
        }

        public void Dispose()
        {
            StopCamera();
            lock (_bitmapLock)
            {
                _lastBitmap?.Dispose();
                _lastBitmap = null;
            }
        }

        public void SaveImage(Image image, string path)
        {
            var ext = System.IO.Path.GetExtension(path)?.ToLowerInvariant();
            try
            {
                ImageFormat fmt = ext == ".png" ? ImageFormat.Png : ImageFormat.Jpeg;
                using var clone = new Bitmap(image);
                using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
                clone.Save(fs, fmt);
            }
            catch
            {
                throw;
            }
        }
    }
}
