using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using OpenCvSharp;


namespace SistemaMultimedia.Services.Camera
{
    public class RecordingService : IDisposable
    {
        private readonly CameraService _cameraService;
        private DateTime? _startTime;
        private Process? _ffmpegProcess;
        private System.IO.Stream? _ffmpegInput;
        private readonly object _ffmpegLock = new object();
        private string? _outputPath;
        private string? _microphoneName;
        private VideoWriter? _writer;
        private bool _useFfmpeg;
        private bool _writerInitialized;
        private System.Collections.Generic.List<Mat>? _bufferedFrames;

        private readonly object _lock = new object();

        public RecordingService(CameraService cameraService)
        {
            _cameraService = cameraService;
        }

        public async Task<string> StartRecordingAsync(string outputPath, string cameraName, string microphoneName)
        {
            _outputPath = outputPath;
            _microphoneName = microphoneName;
            _startTime = DateTime.UtcNow;

            _useFfmpeg = true; 

            try
            {
                var dir = Path.GetDirectoryName(_outputPath ?? string.Empty);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
            }
            catch { }

            _writerInitialized = false;
            _ffmpegProcess = null;
            _ffmpegInput = null;
            _bufferedFrames = new System.Collections.Generic.List<Mat>();

            _cameraService.FrameArrived += OnFrameArrivedForWrite;

            await Task.CompletedTask;
            return _outputPath ?? outputPath;
        }

        public async Task StopRecordingAsync()
        {
            try
            {
                try { _cameraService.FrameArrived -= OnFrameArrivedForWrite; } catch { }

                if (_useFfmpeg && _ffmpegProcess != null && !_ffmpegProcess.HasExited)
                {
                    _ffmpegProcess.StandardInput?.Close();

                    _ffmpegProcess.WaitForExit(5000);

                    if (!_ffmpegProcess.HasExited) _ffmpegProcess.Kill(true);
                }

                if (_writer != null)
                {
                    lock (_lock)
                    {
                        _writer.Release();
                        _writer.Dispose();
                        _writer = null;
                    }
                }
            }
            catch { }
            finally
            {
                _ffmpegProcess?.Dispose();
                _ffmpegProcess = null;
                _ffmpegInput = null;
                _startTime = null;

                try
                {
                    if (_bufferedFrames != null)
                    {
                        foreach (var f in _bufferedFrames) try { f.Dispose(); } catch { }
                        _bufferedFrames.Clear();
                    }
                }
                catch { }
            }

            await Task.CompletedTask;
        }

        private void OnFrameArrivedForWrite(Mat mat)
        {
            try
            {
                if (mat == null || mat.Empty()) return;

                lock (_lock)
                {
                    if (!_writerInitialized)
                    {
                        try
                        {
                            if (_useFfmpeg)
                            {
                                string micParam = string.IsNullOrEmpty(_microphoneName) ? "" : $"-f dshow -i audio=\"{_microphoneName}\"";
                                string audioCodec = string.IsNullOrEmpty(_microphoneName) ? "" : "-c:a aac -shortest";

                                string args = $"-y -f rawvideo -vcodec rawvideo -s {mat.Width}x{mat.Height} -pix_fmt bgr24 -framerate 25 -i - {micParam} -c:v libx264 -preset ultrafast -pix_fmt yuv420p {audioCodec} \"{_outputPath}\"";

                                var psi = new ProcessStartInfo
                                {
                                    FileName = "ffmpeg",
                                    Arguments = args,
                                    UseShellExecute = false,
                                    RedirectStandardInput = true,
                                    CreateNoWindow = true
                                };

                                _ffmpegProcess = Process.Start(psi);
                                if (_ffmpegProcess != null)
                                {
                                    _ffmpegInput = _ffmpegProcess.StandardInput.BaseStream;
                                    _writerInitialized = true;
                                }
                            }
                        }
                        catch
                        {
                            _useFfmpeg = false;
                        }

                        if (!_useFfmpeg)
                        {
                            _writer = new VideoWriter(_outputPath, VideoWriter.FourCC('m', 'p', '4', 'v'), 25, new OpenCvSharp.Size(mat.Width, mat.Height));
                            _writerInitialized = true;
                        }
                    }

                    if (_writerInitialized)
                    {
                        if (_useFfmpeg && _ffmpegInput != null && _ffmpegProcess != null)
                        {
                            try
                            {
                                if (!_ffmpegProcess.HasExited)
                                {
                                    int size = (int)(mat.Total() * mat.Channels());
                                    byte[] buffer = new byte[size];
                                    System.Runtime.InteropServices.Marshal.Copy(mat.Data, buffer, 0, size);
                                    _ffmpegInput.Write(buffer, 0, size);
                                }
                            }
                            catch { } 
                        }
                        else if (!_useFfmpeg && _writer != null)
                        {
                            _writer.Write(mat);
                        }
                    }
                }
            }
            catch { }
        }

        public TimeSpan GetRecordingDuration()
        {
            if (!_startTime.HasValue) return TimeSpan.Zero;
            return DateTime.UtcNow - _startTime.Value;
        }

        public void Dispose()
        {
            try
            {
                StopRecordingAsync().Wait(500);
            }
            catch { }
        }
    }
}
