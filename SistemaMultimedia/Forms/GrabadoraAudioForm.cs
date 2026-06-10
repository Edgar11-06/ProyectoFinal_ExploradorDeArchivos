using System;
using System.IO;
using System.Windows.Forms;
using NAudio.Lame;
using NAudio.Wave;

namespace SistemaMultimedia.Forms
{
    public partial class GrabadoraAudioForm : Form
    {
        private WaveInEvent? _waveIn;
        private WaveFileWriter? _writer;
        private WaveOutEvent? _waveOut;
        private AudioFileReader? _playbackReader;

        private string? _recordedWavPath;
        private bool _isRecording;
        private bool _isPaused;
        private DateTime _recordingStartedUtc;
        private TimeSpan _elapsedBeforePause;

        public GrabadoraAudioForm()
        {
            InitializeComponent();
            LoadInputDevices();
            UpdateUi();
        }

        private void LoadInputDevices()
        {
            cboMicrofono.Items.Clear();
            for (int i = 0; i < WaveInEvent.DeviceCount; i++)
            {
                var caps = WaveInEvent.GetCapabilities(i);
                cboMicrofono.Items.Add(caps.ProductName);
            }

            if (cboMicrofono.Items.Count > 0)
                cboMicrofono.SelectedIndex = 0;
            else
                lblEstado.Text = "No se detectó ningún micrófono.";
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            if (_isRecording)
                return;

            try
            {
                StopPlayback();

                _recordedWavPath = Path.Combine(
                    Path.GetTempPath(),
                    $"Grabadora_{DateTime.Now:yyyyMMdd_HHmmss}.wav");

                var deviceIndex = cboMicrofono.SelectedIndex >= 0 ? cboMicrofono.SelectedIndex : 0;
                _waveIn = new WaveInEvent
                {
                    DeviceNumber = deviceIndex,
                    WaveFormat = new WaveFormat(44100, 16, 1),
                    BufferMilliseconds = 50
                };

                _writer = new WaveFileWriter(_recordedWavPath, _waveIn.WaveFormat);
                _waveIn.DataAvailable += WaveIn_DataAvailable;
                _waveIn.RecordingStopped += WaveIn_RecordingStopped;

                _isRecording = true;
                _isPaused = false;
                _recordingStartedUtc = DateTime.UtcNow;
                _elapsedBeforePause = TimeSpan.Zero;

                _waveIn.StartRecording();
                tmrGrabacion.Start();

                lblEstado.Text = "Grabando...";
                UpdateUi();
            }
            catch (Exception ex)
            {
                CleanupRecording();
                MessageBox.Show($"No se pudo iniciar la grabación:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPausar_Click(object sender, EventArgs e)
        {
            if (!_isRecording) return;

            _isPaused = !_isPaused;

            if (_isPaused)
            {
                _elapsedBeforePause += DateTime.UtcNow - _recordingStartedUtc;
                lblEstado.Text = "Grabación en pausa.";
            }
            else
            {
                _recordingStartedUtc = DateTime.UtcNow;
                lblEstado.Text = "Grabando...";
            }

            UpdateUi();
        }

        private void btnDetener_Click(object sender, EventArgs e)
        {
            if (_isRecording)
            {
                _waveIn?.StopRecording();
                return;
            }

            StopPlayback();
        }

        private void WaveIn_RecordingStopped(object? sender, StoppedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => WaveIn_RecordingStopped(sender, e));
                return;
            }

            tmrGrabacion.Stop();
            CleanupRecording();

            if (e.Exception != null)
            {
                lblEstado.Text = "Error al grabar.";
                MessageBox.Show(e.Exception.Message, "Error de grabación",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!string.IsNullOrEmpty(_recordedWavPath) && File.Exists(_recordedWavPath))
            {
                lblEstado.Text = "Grabación finalizada. Puede reproducir o guardar.";
                lblTiempo.Text = FormatTime(GetRecordedDuration());
            }

            UpdateUi();
        }

        private void WaveIn_DataAvailable(object? sender, WaveInEventArgs e)
        {
            if (_isPaused || _writer == null || e.BytesRecorded == 0)
                return;

            _writer.Write(e.Buffer, 0, e.BytesRecorded);

            float peak = 0;
            for (int i = 0; i < e.BytesRecorded; i += 2)
            {
                short sample = (short)(e.Buffer[i] | (e.Buffer[i + 1] << 8));
                float sample32 = sample / 32768f;
                if (sample32 > peak) peak = sample32;
                if (-sample32 > peak) peak = -sample32;
            }

            var level = (int)(peak * progressNivel.Maximum);
            if (level > progressNivel.Value)
            {
                if (InvokeRequired)
                    BeginInvoke(() => progressNivel.Value = Math.Min(level, progressNivel.Maximum));
                else
                    progressNivel.Value = Math.Min(level, progressNivel.Maximum);
            }
        }

        private void tmrGrabacion_Tick(object sender, EventArgs e)
        {
            lblTiempo.Text = FormatTime(GetCurrentRecordingDuration());

            if (!_isPaused && progressNivel.Value > 0)
                progressNivel.Value = Math.Max(0, progressNivel.Value - 4);
        }

        private TimeSpan GetCurrentRecordingDuration()
        {
            if (!_isRecording) return GetRecordedDuration();

            var active = _isPaused ? TimeSpan.Zero : DateTime.UtcNow - _recordingStartedUtc;
            return _elapsedBeforePause + active;
        }

        private TimeSpan GetRecordedDuration()
        {
            if (string.IsNullOrEmpty(_recordedWavPath) || !File.Exists(_recordedWavPath))
                return TimeSpan.Zero;

            try
            {
                using var reader = new AudioFileReader(_recordedWavPath);
                return reader.TotalTime;
            }
            catch
            {
                return TimeSpan.Zero;
            }
        }

        private static string FormatTime(TimeSpan t) =>
            $"{(int)t.TotalHours:00}:{t.Minutes:00}:{t.Seconds:00}";

        private void btnReproducir_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_recordedWavPath) || !File.Exists(_recordedWavPath))
            {
                MessageBox.Show("No hay ninguna grabación para reproducir.",
                    "Grabadora", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                StopPlayback();

                _playbackReader = new AudioFileReader(_recordedWavPath);
                _waveOut = new WaveOutEvent();
                _waveOut.Init(_playbackReader);
                _waveOut.PlaybackStopped += (_, _) =>
                {
                    if (InvokeRequired)
                        BeginInvoke(StopPlayback);
                    else
                        StopPlayback();
                };
                _waveOut.Play();

                lblEstado.Text = "Reproduciendo grabación...";
                btnReproducir.Enabled = false;
                btnDetener.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo reproducir:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_recordedWavPath) || !File.Exists(_recordedWavPath))
            {
                MessageBox.Show("Primero grabe un audio.",
                    "Grabadora", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            saveFileDialog.FileName = $"Grabacion_{DateTime.Now:yyyyMMdd_HHmmss}";
            if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
                return;

            try
            {
                var ext = Path.GetExtension(saveFileDialog.FileName).ToLowerInvariant();
                if (ext == ".mp3")
                    SaveAsMp3(_recordedWavPath, saveFileDialog.FileName);
                else
                    File.Copy(_recordedWavPath, saveFileDialog.FileName, overwrite: true);

                lblEstado.Text = $"Guardado: {Path.GetFileName(saveFileDialog.FileName)}";
                MessageBox.Show("Archivo guardado correctamente.",
                    "Grabadora", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void SaveAsMp3(string wavPath, string mp3Path)
        {
            using var reader = new AudioFileReader(wavPath);
            using var writer = new LameMP3FileWriter(mp3Path, reader.WaveFormat, LAMEPreset.STANDARD);
            reader.CopyTo(writer);
        }

        private void StopPlayback()
        {
            _waveOut?.Stop();
            _waveOut?.Dispose();
            _waveOut = null;
            _playbackReader?.Dispose();
            _playbackReader = null;

            if (!_isRecording)
            {
                btnReproducir.Enabled = HasRecording();
                btnDetener.Enabled = false;
            }
        }

        private void CleanupRecording()
        {
            if (_waveIn != null)
            {
                _waveIn.DataAvailable -= WaveIn_DataAvailable;
                _waveIn.RecordingStopped -= WaveIn_RecordingStopped;
                _waveIn.Dispose();
                _waveIn = null;
            }

            _writer?.Dispose();
            _writer = null;

            _isRecording = false;
            _isPaused = false;
        }

        private bool HasRecording() =>
            !string.IsNullOrEmpty(_recordedWavPath) && File.Exists(_recordedWavPath);

        private void UpdateUi()
        {
            btnGrabar.Enabled = !_isRecording;
            btnPausar.Enabled = _isRecording;
            btnPausar.Text = _isPaused ? "▶  Reanudar" : "⏸  Pausar";
            btnDetener.Enabled = _isRecording || _waveOut != null;
            btnReproducir.Enabled = !_isRecording && _waveOut == null && HasRecording();
            btnGuardar.Enabled = !_isRecording && HasRecording();
            cboMicrofono.Enabled = !_isRecording;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_isRecording)
            {
                var r = MessageBox.Show(
                    "Hay una grabación en curso. ¿Desea detenerla y salir?",
                    "Grabadora",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (r == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }

                _waveIn?.StopRecording();
            }

            StopPlayback();
            CleanupRecording();

            try
            {
                if (!string.IsNullOrEmpty(_recordedWavPath) && File.Exists(_recordedWavPath))
                    File.Delete(_recordedWavPath);
            }
            catch { /* temp file */ }

            base.OnFormClosing(e);
        }
    }
}
