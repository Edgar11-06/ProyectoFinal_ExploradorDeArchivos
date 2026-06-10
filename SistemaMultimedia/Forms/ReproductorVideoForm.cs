using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using System.Diagnostics;
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace SistemaMultimedia.Forms
{
    public partial class ReproductorVideoForm : Form
    {
        private LibVLC? _libVLC;
        private MediaPlayer? _mediaPlayer;
        private Media? _media;
        private bool _isDragging = false;
        private bool _isFullScreen = false;
        private FormWindowState _previousWindowState;
        private FormBorderStyle _previousBorderStyle;
        private System.Windows.Forms.Timer? _updateTimer;
        private System.Windows.Forms.Timer? _hideControlsTimer;
        private Point _lastMousePosition;

        public ReproductorVideoForm()
        {
            InitializeComponent();
            InitializeVLC();
            InitializeTimer();
            InitializeHideControlsTimer();
        }

        private void InitializeVLC()
        {
            Core.Initialize();
            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);
            videoView.MediaPlayer = _mediaPlayer;

            // Configurar volumen inicial
            _mediaPlayer.Volume = 100;
            // Asegurar que no inicie en mute
            _mediaPlayer.Mute = false;

            // Eventos del MediaPlayer
            _mediaPlayer.TimeChanged += MediaPlayer_TimeChanged;
            _mediaPlayer.LengthChanged += MediaPlayer_LengthChanged;
            _mediaPlayer.EndReached += MediaPlayer_EndReached;
        }

        private void InitializeTimer()
        {
            _updateTimer = new System.Windows.Forms.Timer();
            _updateTimer.Interval = 100;
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();
        }

        private void InitializeHideControlsTimer()
        {
            _hideControlsTimer = new System.Windows.Forms.Timer();
            _hideControlsTimer.Interval = 3000; // 3 segundos
            _hideControlsTimer.Tick += HideControlsTimer_Tick;

            // Eventos de movimiento del mouse
            videoView.MouseMove += VideoView_MouseMove;
            panelControles.MouseMove += PanelControles_MouseMove;
        }

        private void HideControlsTimer_Tick(object? sender, EventArgs e)
        {
            if (_isFullScreen && !panelControles.ClientRectangle.Contains(panelControles.PointToClient(Cursor.Position)))
            {
                panelControles.Visible = false;
                Cursor.Hide();
            }
            _hideControlsTimer?.Stop();
        }

        private void VideoView_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isFullScreen)
            {
                Point currentPosition = Cursor.Position;
                if (_lastMousePosition != currentPosition)
                {
                    ShowControlsInFullScreen();
                    _lastMousePosition = currentPosition;
                }
            }
        }

        private void PanelControles_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isFullScreen)
            {
                _hideControlsTimer?.Stop();
                _hideControlsTimer?.Start();
            }
        }

        private void ShowControlsInFullScreen()
        {
            if (!panelControles.Visible)
            {
                panelControles.Visible = true;
                Cursor.Show();
            }
            _hideControlsTimer?.Stop();
            _hideControlsTimer?.Start();
        }

        private void UpdateTimer_Tick(object? sender, EventArgs e)
        {
            if (_mediaPlayer != null && _mediaPlayer.IsPlaying && !_isDragging)
            {
                UpdateTimeDisplay();
            }
        }

        private void BtnAbrir_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Archivos de Video|*.mp4;*.avi;*.mkv;*.mov;*.wmv;*.flv;*.webm;*.m4v;*.mpg;*.mpeg;*.3gp;*.ts;*.m2ts;*.vob;*.ogv;*.divx;*.asf;*.m2v;*.f4v|Todos los archivos|*.*",
                Title = "Seleccionar archivo de video"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                CargarVideo(openFileDialog.FileName);
            }
        }

        public void CargarVideo(string rutaArchivo)
        {
            try
            {
                _media?.Dispose();
                _media = new Media(_libVLC, new Uri(rutaArchivo));
                _mediaPlayer!.Play(_media);

                // Asegurar que el volumen se aplica correctamente
                _mediaPlayer.Volume = trackBarVolumen.Value;
                // Quitar mute al cargar un nuevo video
                _mediaPlayer.Mute = false;
                lblVolumen.Text = $"{trackBarVolumen.Value}%";

                lblInfo.Text = $"Reproduciendo: {Path.GetFileName(rutaArchivo)}";
                trackBarProgress.Value = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el video: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnPlay_Click(object sender, EventArgs e)
        {
            if (_mediaPlayer == null || _media == null) return;

            if (_mediaPlayer.IsPlaying)
            {
                _mediaPlayer.Pause();
                pbPlay.Image = Properties.Resources.play;
            }
            else
            {
                _mediaPlayer.Play();
                pbPlay.Image = Properties.Resources.pause;
            }
        }


        private void BtnRetroceder_Click(object sender, EventArgs e)
        {
            if (_mediaPlayer != null && _mediaPlayer.Length > 0)
            {
                long newTime = Math.Max(0, _mediaPlayer.Time - 10000); // 10 segundos atrás
                _mediaPlayer.Time = newTime;
            }
        }

        private void BtnAvanzar_Click(object sender, EventArgs e)
        {
            if (_mediaPlayer != null && _mediaPlayer.Length > 0)
            {
                long newTime = Math.Min(_mediaPlayer.Length, _mediaPlayer.Time + 10000); // 10 segundos adelante
                _mediaPlayer.Time = newTime;
            }
        }

        private void BtnMute_Click(object sender, EventArgs e)
        {
            if (_mediaPlayer == null) return;

            _mediaPlayer.Mute = !_mediaPlayer.Mute;
            pbMute.Image = _mediaPlayer.Mute ? Properties.Resources.volume : Properties.Resources.mute;
        }

        private void BtnPantallaCompleta_Click(object sender, EventArgs e)
        {
            ToggleFullScreen();
        }

        private void ToggleFullScreen()
        {
            if (!_isFullScreen)
            {
                _previousWindowState = WindowState;
                _previousBorderStyle = FormBorderStyle;

                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
                _isFullScreen = true;

                // Iniciar timer para ocultar controles
                _hideControlsTimer?.Start();
            }
            else
            {
                FormBorderStyle = _previousBorderStyle;
                WindowState = _previousWindowState;
                _isFullScreen = false;

                // Detener timer y asegurar que los controles estén visibles
                _hideControlsTimer?.Stop();
                panelControles.Visible = true;
                Cursor.Show();
            }
        }

        private void TrackBarVolumen_Scroll(object sender, EventArgs e)
        {
            if (_mediaPlayer != null)
            {
                _mediaPlayer.Volume = trackBarVolumen.Value;
                lblVolumen.Text = $"{trackBarVolumen.Value}%";

                // Si estaba en mute, quitarlo al cambiar volumen
                if (_mediaPlayer.Mute && trackBarVolumen.Value > 0)
                {
                    _mediaPlayer.Mute = false;
                }
            }
        }

        private void TrackBarProgress_MouseDown(object sender, MouseEventArgs e)
        {
            _isDragging = true;
        }

        private void TrackBarProgress_MouseUp(object sender, MouseEventArgs e)
        {
            if (_mediaPlayer != null && _mediaPlayer.Length > 0)
            {
                long newTime = (long)((_mediaPlayer.Length * trackBarProgress.Value) / 1000.0);
                _mediaPlayer.Time = newTime;
            }
            _isDragging = false;
        }

        private void ComboVelocidad_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_mediaPlayer != null && comboVelocidad.SelectedItem != null)
            {
                string velocidad = comboVelocidad.SelectedItem.ToString()!.Replace("x", "");
                if (float.TryParse(velocidad, System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out float rate))
                {
                    _mediaPlayer.SetRate(rate);
                }
            }
        }

        private void MediaPlayer_TimeChanged(object? sender, MediaPlayerTimeChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => MediaPlayer_TimeChanged(sender, e)));
                return;
            }

            if (!_isDragging && _mediaPlayer != null && _mediaPlayer.Length > 0)
            {
                int progressValue = (int)((e.Time * 1000) / _mediaPlayer.Length);
                trackBarProgress.Value = Math.Min(1000, Math.Max(0, progressValue));
            }
        }

        private void MediaPlayer_LengthChanged(object? sender, MediaPlayerLengthChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => MediaPlayer_LengthChanged(sender, e)));
                return;
            }

            UpdateTimeDisplay();
        }

        private void MediaPlayer_EndReached(object? sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => MediaPlayer_EndReached(sender, e)));
                return;
            }

            // Detener y posicionar al inicio para poder reproducir de nuevo
            _mediaPlayer?.Stop();
            trackBarProgress.Value = 0;
            lblTiempo.Text = "00:00:00 / 00:00:00";
        }

        private void UpdateTimeDisplay()
        {
            if (_mediaPlayer == null) return;

            TimeSpan currentTime = TimeSpan.FromMilliseconds(_mediaPlayer.Time);
            TimeSpan totalTime = TimeSpan.FromMilliseconds(_mediaPlayer.Length);

            lblTiempo.Text = $"{currentTime:hh\\:mm\\:ss} / {totalTime:hh\\:mm\\:ss}";
        }

        private void VideoView_DoubleClick(object sender, EventArgs e)
        {
            ToggleFullScreen();
        }

        private void VideoView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && _isFullScreen)
            {
                ToggleFullScreen();
            }
            else if (e.KeyCode == Keys.Space)
            {
                BtnPlay_Click(sender, e);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _updateTimer?.Stop();
            _updateTimer?.Dispose();
            _hideControlsTimer?.Stop();
            _hideControlsTimer?.Dispose();
            _media?.Dispose();
            _mediaPlayer?.Dispose();
            _libVLC?.Dispose();
            base.OnFormClosing(e);
        }
    }
}

