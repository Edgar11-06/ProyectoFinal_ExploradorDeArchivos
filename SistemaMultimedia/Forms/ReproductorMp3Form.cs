using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using NAudio.Wave;
using TagLib;
using SistemaMultimedia.Models;
using SistemaMultimedia.Utilities;

namespace SistemaMultimedia.Forms
{
    public partial class ReproductorMp3Form : Form
    {
        private AudioFileReader? audioFile;
        private IWavePlayer? outputDevice;
        private bool isSeeking = false;
        private bool userIsDraggingPosition = false;

        // Playlist
        private BindingList<Song> playlist = new();
        private int currentIndex = -1;
        // Shuffle / Repeat
        private bool shuffleEnabled = false;
        private List<Song>? originalOrder = null; // guarda el orden original cuando activamos shuffle

        private bool repeatEnabled = false; // repetir 1 vez la pista actual al terminar
        private bool hasRepeatedCurrent = false; // si la pista ya se repitió una vez
        private bool internalStopRequested = false; // para distinguir paradas internas vs fin natural

        // artwork currently shown in PictureBox (we dispose when replaced)
        private Image? displayedArtwork = null;

        // Http client compartido para peticiones de letra
        private static readonly HttpClient _httpClient = new();

        // Icono play/pause (pauseIcon cargado desde ruta de usuario)
        private Image? playIcon;
        private Image? pauseIcon;

        public ReproductorMp3Form()
        {
            InitializeComponent();

            // Bind the playlist to the DataGridView so it updates automatically
            try
            {
                dgvPlaylist.AutoGenerateColumns = false;
                dgvPlaylist.DataSource = playlist;
            }

            catch { }

            // Capturar icono de play en memoria (desde diseñador)
            try { playIcon = pbPlay.BackgroundImage; } catch { playIcon = null; }

            // Cargar icono de pausa desde la ruta proporcionada por el usuario.
            // Se crea una copia en memoria para evitar que el archivo quede bloqueado.
            try
            {
                var pausePath = @"C:\Users\GARO\OneDrive - Instituto Tecnológico Superior de Monclova\Documentos\Trabajos tec\cuarto semestre\Administracion\pause3.png";
                if (System.IO.File.Exists(pausePath))
                {
                    var bytes = System.IO.File.ReadAllBytes(pausePath);
                    using var ms = new MemoryStream(bytes);
                    using var img = Image.FromStream(ms);
                    pauseIcon = new Bitmap(img);
                }
                else
                {
                    pauseIcon = null;
                }
            }
            catch
            {
                pauseIcon = null;
            }

        }

        // Compatibilidad: versión síncrona para llamadas existentes en el proyecto
        public void AbrirArchivos(System.Collections.Generic.IEnumerable<string> archivos)
        {
            _ = AbrirArchivosAsync(archivos);
        }

        // Crea región circular en un control (ideal para botones cuadrados)
        private void MakeCircular(Control c)
        {
            if (c == null) return;
            var w = Math.Max(2, c.Width);
            var h = Math.Max(2, c.Height);
            var size = Math.Min(w, h);
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse((w - size) / 2, (h - size) / 2, size, size);
            try { c.Region?.Dispose(); } catch { }
            c.Region = new Region(path);
            path.Dispose();
        }

        private void ReproductorMp3Form_Load(object sender, EventArgs e) 
        {
            dgvPlaylist.DefaultCellStyle.ForeColor = Color.Black;
        }

        private void ReproductorMp3Form_Resize(object? sender, EventArgs e)
        {
            try { MakeCircular(pbPrev); } catch { }
            try { MakeCircular(pbPlay); } catch { }
            try { MakeCircular(pbNext); } catch { }
            try { MakeCircular(pbStop); } catch { }
            try { MakeCircular(pbSkipBack10); } catch { }
            try { MakeCircular(pbSkipForward10); } catch { }
        }

        private async void btnOpen_Click(object? sender, EventArgs e) => await AbrirArchivoDesdeDialogoAsync();

        private void pbPlay_Click(object? sender, EventArgs e)
        {
            try
            {
                if (audioFile == null && (currentIndex >= 0 && currentIndex < playlist.Count))
                {
                    PlayAt(currentIndex);
                    return;
                }


                if (audioFile == null && outputDevice == null) return;

                if (outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing)
                    Pausar();
                else
                    Reproducir();
            }
            finally
            {
                UpdatePlayPauseIcon();
            }
        }

        // Nuevo: toggle de visibilidad de la letra
        private void btnLyrics_Click(object? sender, EventArgs e)
        {
            try
            {
                rtbLyrics.Visible = !rtbLyrics.Visible;
                btnLyrics.Text = rtbLyrics.Visible ? "Ocultar letra" : "Mostrar letra";
                if (rtbLyrics.Visible && currentIndex >= 0 && currentIndex < playlist.Count)
                    _ = UpdateLyricsForIndexAsync(currentIndex);
            }
            catch { }
        }

        private async Task AbrirArchivoDesdeDialogoAsync()
        {
            using var ofd = new OpenFileDialog { Filter = "MP3 files|*.mp3|All files|*.*", Multiselect = true };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            await AbrirArchivosAsync(ofd.FileNames);
        }

        public async Task AbrirArchivosAsync(IEnumerable<string> archivos)
        {
            var files = archivos?.Where(f => !string.IsNullOrWhiteSpace(f)).ToArray() ?? Array.Empty<string>();
            if (files.Length == 0) return;

            using (new WaitCursorScope())
            {
                var loadTasks = files.Select(f => LoadSongFromFileAsync(f)).ToList();
                var results = await Task.WhenAll(loadTasks).ConfigureAwait(false);

                // Añadir en hilo UI
                if (dgvPlaylist.InvokeRequired)
                {
                    dgvPlaylist.Invoke(new Action(() =>
                    {
                        int firstAddedIndex = -1;
                        foreach (var song in results)
                        {
                            if (song == null) continue;
                            playlist.Add(song);
                            if (firstAddedIndex == -1) firstAddedIndex = playlist.Count - 1;
                        }
                        if (currentIndex == -1 && firstAddedIndex != -1) PlayAt(firstAddedIndex);
                        try { pbPrev.Enabled = (currentIndex - 1) >= 0; } catch { }
                        try { pbNext.Enabled = (currentIndex + 1) < playlist.Count; } catch { }
                    }));
                }
                else
                {
                    int firstAddedIndex = -1;
                    foreach (var song in results)
                    {
                        if (song == null) continue;
                        playlist.Add(song);
                        if (firstAddedIndex == -1) firstAddedIndex = playlist.Count - 1;
                    }
                    if (currentIndex == -1 && firstAddedIndex != -1) PlayAt(firstAddedIndex);
                    try { pbPrev.Enabled = (currentIndex - 1) >= 0; } catch { }
                    try { pbNext.Enabled = (currentIndex + 1) < playlist.Count; } catch { }
                }
            }
        }

        private void PlayAt(int index)
        {
            if (index < 0 || index >= playlist.Count) return;
            // actualizar flags IsPlaying
            for (int i = 0; i < playlist.Count; i++) playlist[i].IsPlaying = (i == index);
            currentIndex = index;
            hasRepeatedCurrent = false;
            var path = playlist[index].FilePath;
            LoadFile(path);

            if (index >= 0 && index < dgvPlaylist.Rows.Count)
            {
                dgvPlaylist.ClearSelection();
                dgvPlaylist.Rows[index].Selected = true;
                dgvPlaylist.FirstDisplayedScrollingRowIndex = index;
            }

            UpdateArtworkForIndex(index);
            _ = UpdateLyricsForIndexAsync(index);

            try { pbPrev.Enabled = (currentIndex - 1) >= 0; } catch { }
            try { pbNext.Enabled = (currentIndex + 1) < playlist.Count; } catch { }
            try { pbPlay.Enabled = true; } catch { }
            try { pbStop.Enabled = true; } catch { }

            PlayAt_EnableSkipButtons(true);
            UpdatePlayPauseIcon();
        }

        private void dgvPlaylist_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < playlist.Count) PlayAt(e.RowIndex);
        }

        private void LoadFile(string path)
        {
            StopAndDispose();

            try
            {
                audioFile = new AudioFileReader(path);
                outputDevice = new WaveOutEvent();
                outputDevice.Init(audioFile);
                outputDevice.PlaybackStopped += OutputDevice_PlaybackStopped;

                audioFile.Volume = trackBarVolume.Value / 100f;

                trackBarPosition.Maximum = Math.Max(1, (int)audioFile.TotalTime.TotalSeconds);
                trackBarPosition.Value = 0;
                trackBarPosition.Enabled = true;

                lblFile.Text = $"Archivo: {Path.GetFileName(path)}";
                lblTime.Text = FormatTime(TimeSpan.Zero) + " / " + FormatTime(audioFile.TotalTime);

                outputDevice.Play();
                tmrPosition.Start();

                try { pbPlay.Enabled = true; } catch { }
                try { pbStop.Enabled = true; } catch { }

                PlayAt_EnableSkipButtons(true);
                UpdatePlayPauseIcon();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo abrir el archivo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                StopAndDispose();
            }
        }

        private void pbNext_Click(object? sender, EventArgs e)
        {
            if (currentIndex + 1 < playlist.Count) PlayAt(currentIndex + 1);
        }

        private void pbPrev_Click(object? sender, EventArgs e)
        {
            if (currentIndex - 1 >= 0) PlayAt(currentIndex - 1);
        }

        private void pbStop_Click(object? sender, EventArgs e)
        {
            Stop();
            UpdatePlayPauseIcon();
        }

        private void Reproducir()
        {
            if (outputDevice == null && audioFile != null)
            {
                outputDevice = new WaveOutEvent();
                outputDevice.Init(audioFile);
                outputDevice.PlaybackStopped += OutputDevice_PlaybackStopped;
            }

            if (outputDevice != null)
            {
                outputDevice.Play();
                tmrPosition.Start();
                try { pbPlay.Enabled = true; } catch { }
                try { pbStop.Enabled = true; } catch { }
            }

            UpdatePlayPauseIcon();
        }

        private void Pausar()
        {
            if (outputDevice != null)
            {
                outputDevice.Pause();
                tmrPosition.Stop();
                try { pbPlay.Enabled = true; } catch { }
            }

            UpdatePlayPauseIcon();
        }

        private void Stop()
        {
            if (outputDevice != null && audioFile != null)
            {
                try { internalStopRequested = true; } catch { }
                try { outputDevice.Stop(); } catch { }
                audioFile.Position = 0;
                trackBarPosition.Value = 0;
                lblTime.Text = FormatTime(TimeSpan.Zero) + " / " + FormatTime(audioFile.TotalTime);
                tmrPosition.Stop();
                try { pbPlay.Enabled = true; } catch { }
            }

            UpdatePlayPauseIcon();
        }

        private void trackBarVolume_Scroll(object? sender, EventArgs e)
        {
            if (audioFile != null) audioFile.Volume = trackBarVolume.Value / 100f;
        }

        private void trackBarPosition_Scroll(object? sender, EventArgs e)
        {
            if (audioFile == null) return;
            if (userIsDraggingPosition)
            {
                var seconds = Math.Min(trackBarPosition.Maximum, Math.Max(0, trackBarPosition.Value));
                lblTime.Text = FormatTime(TimeSpan.FromSeconds(seconds)) + " / " + FormatTime(audioFile.TotalTime);
            }
            else
            {
                SeekTo(trackBarPosition.Value);
            }
        }

        private void trackBarPosition_MouseDown(object? sender, MouseEventArgs e)
        {
            userIsDraggingPosition = true;
            tmrPosition.Stop();
        }

        private void trackBarPosition_MouseUp(object? sender, MouseEventArgs e)
        {
            userIsDraggingPosition = false;
            SeekTo(trackBarPosition.Value);
            tmrPosition.Start();
        }

        private void SeekTo(int seconds)
        {
            if (audioFile == null) return;
            isSeeking = true;
            seconds = Math.Min(trackBarPosition.Maximum, Math.Max(0, seconds));
            audioFile.CurrentTime = TimeSpan.FromSeconds(seconds);
            lblTime.Text = FormatTime(audioFile.CurrentTime) + " / " + FormatTime(audioFile.TotalTime);
            isSeeking = false;
        }

        private void tmrPosition_Tick(object? sender, EventArgs e)
        {
            if (audioFile == null || outputDevice == null) return;
            if (isSeeking || userIsDraggingPosition) return;

            try
            {
                var current = audioFile.CurrentTime;
                var total = audioFile.TotalTime;
                var seconds = (int)current.TotalSeconds;
                if (seconds >= 0 && seconds <= trackBarPosition.Maximum)
                    trackBarPosition.Value = seconds;

                lblTime.Text = FormatTime(current) + " / " + FormatTime(total);

                if (outputDevice.PlaybackState == PlaybackState.Stopped && current >= total)
                {
                    trackBarPosition.Value = trackBarPosition.Maximum;
                    try { pbPlay.Enabled = true; } catch { }
                    try { pbPrev.Enabled = (currentIndex - 1) >= 0; } catch { }
                    try { pbNext.Enabled = (currentIndex + 1) < playlist.Count; } catch { }
                    try { pbStop.Enabled = false; } catch { }
                    tmrPosition.Stop();

                    // Si repeat está activado y no se ha repetido aún la pista actual, repetirla una vez
                    if (repeatEnabled && !hasRepeatedCurrent)
                    {
                        hasRepeatedCurrent = true;
                        // reiniciar y reproducir la misma pista
                        if (audioFile != null && outputDevice != null)
                        {
                            SeekTo(0);
                            Reproducir();
                            return;
                        }
                    }

                    // Si shuffle está activado, avanzar a la siguiente pista en orden aleatorio
                    if (shuffleEnabled)
                    {
                        // si no hay más canciones, no hacer nada
                        if (playlist.Count == 0) return;

                        // elegir índice aleatorio distinto al actual si es posible
                        var rnd = new Random();
                        int next = currentIndex;
                        if (playlist.Count == 1)
                        {
                            // solo una pista -> si ya se repitió, detener
                            return;
                        }
                        for (int i = 0; i < 10 && next == currentIndex; i++) next = rnd.Next(0, playlist.Count);

                        PlayAt(next);
                        return;
                    }

                    // comportamiento por defecto: reproducir siguiente si existe
                    if (currentIndex + 1 < playlist.Count) PlayAt(currentIndex + 1);
                }
            }
            catch { }
        }

        // Click en shuffle: alterna modo y reordena la playlist si se activa
        private void pbShuffle_Click(object? sender, EventArgs e)
        {
            try
            {
                shuffleEnabled = !shuffleEnabled;
                // visual simple
                pbShuffle.BackColor = shuffleEnabled ? Color.FromArgb(60, 60, 60) : Color.Transparent;

                if (shuffleEnabled)
                {
                    // guardar orden original (objetos Song)
                    originalOrder = playlist.ToList();

                    var rnd = new Random();
                    // Si hay una pista actual válida, la dejamos en su posición y barajamos solo las restantes
                    if (currentIndex >= 0 && currentIndex < originalOrder.Count)
                    {
                        var currentSong = originalOrder[currentIndex];
                        var others = originalOrder.Where((s, i) => i != currentIndex).OrderBy(_ => rnd.Next()).ToList();
                        var count = originalOrder.Count;
                        var reordered = new List<Song>(count);

                        // Construir lista manteniendo la canción actual en su índice
                        int otherPos = 0;
                        for (int i = 0; i < count; i++)
                        {
                            if (i == currentIndex)
                            {
                                reordered.Add(currentSong);
                            }
                            else
                            {
                                reordered.Add(others[otherPos++]);
                            }
                        }

                        playlist.RaiseListChangedEvents = false;
                        try
                        {
                            playlist.Clear();
                            foreach (var s in reordered) playlist.Add(s);
                            // currentIndex permanece igual (la canción se quedó en la misma posición)
                        }
                        finally { playlist.RaiseListChangedEvents = true; playlist.ResetBindings(); }
                    }
                    else
                    {
                        // no hay pista activa: barajar todo
                        var shuffled = originalOrder.OrderBy(_ => rnd.Next()).ToList();
                        playlist.RaiseListChangedEvents = false;
                        try
                        {
                            playlist.Clear();
                            foreach (var s in shuffled) playlist.Add(s);
                            currentIndex = -1;
                        }
                        finally { playlist.RaiseListChangedEvents = true; playlist.ResetBindings(); }
                    }
                }
                else
                {
                    // restaurar orden original si la tenemos
                    if (originalOrder != null)
                    {
                        var currentPath = currentIndex >= 0 && currentIndex < playlist.Count ? playlist[currentIndex].FilePath : null;
                        playlist.RaiseListChangedEvents = false;
                        try
                        {
                            playlist.Clear();
                            foreach (var s in originalOrder) playlist.Add(s);
                            if (currentPath != null)
                            {
                                var newIndex = playlist.ToList().FindIndex(x => x.FilePath == currentPath);
                                if (newIndex >= 0) currentIndex = newIndex;
                            }
                        }
                        finally { playlist.RaiseListChangedEvents = true; playlist.ResetBindings(); }
                        originalOrder = null;
                    }
                }
            }
            catch { }
        }

        // Click en repeat: alterna repeat
        private void pbRepeat_Click(object? sender, EventArgs e)
        {
            try
            {
                repeatEnabled = !repeatEnabled;
                hasRepeatedCurrent = false;
                // visual: cambiar opacity o imagen si tienes otra resource, aquí solo alternamos border
                pbRepeat.BackColor = repeatEnabled ? Color.FromArgb(60, 60, 60) : Color.Transparent;
            }
            catch { }
        }

        private static string FormatTime(TimeSpan t) => $"{(int)t.TotalMinutes:00}:{t.Seconds:00}";

        
        private void UpdatePlayPauseIcon()
        {
            try
            {
                var isPlaying = outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing;
                if (isPlaying)
                {
                    try { pbPlay.Image = Properties.Resources.pause; } catch { if (pauseIcon != null) pbPlay.BackgroundImage = pauseIcon; }
                }
                else
                {
                    try { pbPlay.Image = Properties.Resources.play; } catch { if (playIcon != null) pbPlay.BackgroundImage = playIcon; }
                }
            }
            catch { }
        }

        private void PlayAt_EnableSkipButtons(bool enabled)
        {
            try { pbSkipBack10.Enabled = enabled; } catch { }
            try { pbSkipForward10.Enabled = enabled; } catch { }
        }

        // Skip handlers
        private void pbSkipBack10_Click(object? sender, EventArgs e)
        {
            if (audioFile == null) return;
            var seconds = Math.Max(0, (int)audioFile.CurrentTime.TotalSeconds - 10);
            SeekTo(seconds);
        }

        private void pbSkipForward10_Click(object? sender, EventArgs e)
        {
            if (audioFile == null) return;
            var seconds = Math.Min((int)audioFile.TotalTime.TotalSeconds, (int)audioFile.CurrentTime.TotalSeconds + 10);
            SeekTo(seconds);
        }

        private void StopAndDispose()
        {
            try
            {
                tmrPosition.Stop();
                if (outputDevice != null)
                {
                    try { internalStopRequested = true; } catch { }
                    try { outputDevice.Stop(); } catch { }
                    try { outputDevice.PlaybackStopped -= OutputDevice_PlaybackStopped; } catch { }
                    try { outputDevice.Dispose(); } catch { }
                    outputDevice = null;
                }
                if (audioFile != null)
                {
                    audioFile.Dispose();
                    audioFile = null;
                }
            }
            catch { }

            try { pbPlay.Enabled = false; } catch { }
            try { pbPrev.Enabled = false; } catch { }
            try { pbNext.Enabled = false; } catch { }
            try { pbStop.Enabled = false; } catch { }
            try { pbSkipBack10.Enabled = false; } catch { }
            try { pbSkipForward10.Enabled = false; } catch { }
            trackBarPosition.Enabled = false;
            lblFile.Text = "Archivo: (ninguno)";
            lblTime.Text = "00:00 / 00:00";

            try { rtbLyrics.Text = ""; } catch { }

            try { if (playIcon != null) pbPlay.BackgroundImage = playIcon; } catch { }
        }

        private void OutputDevice_PlaybackStopped(object? sender, NAudio.Wave.StoppedEventArgs e)
        {
            try
            {
                // ignorar paradas solicitadas internamente
                if (internalStopRequested)
                {
                    internalStopRequested = false;
                    return;
                }

                if (audioFile == null) return;

                var current = audioFile.CurrentTime;
                var total = audioFile.TotalTime;

                // si no alcanzó el final razonablemente, no considerarlo fin de pista
                if (current + TimeSpan.FromMilliseconds(200) < total) return;

                // repetir una vez si está activado
                if (repeatEnabled && !hasRepeatedCurrent)
                {
                    hasRepeatedCurrent = true;
                    SeekTo(0);
                    Reproducir();
                    return;
                }

                // comportamiento shuffle por defecto
                if (shuffleEnabled)
                {
                    if (playlist.Count <= 1) return;
                    var rnd = new Random();
                    int next = currentIndex;
                    for (int i = 0; i < 10 && next == currentIndex; i++) next = rnd.Next(0, playlist.Count);
                    PlayAt(next);
                    return;
                }

                // avanzar a la siguiente pista si existe
                if (currentIndex + 1 < playlist.Count) PlayAt(currentIndex + 1);
            }
            catch { }
        }

        private void ReproductorMp3Form_FormClosing(object? sender, FormClosingEventArgs e)
        {
            StopAndDispose();
            displayedArtwork?.Dispose();
            displayedArtwork = null;
            try { pauseIcon?.Dispose(); } catch { }
        }

        private void UpdateArtworkForIndex(int index)
        {
            Image? toShow = null;
            if (index >= 0 && index < playlist.Count) toShow = playlist[index].Artwork;

            if (displayedArtwork != null)
            {
                pbArtwork.Image = null;
                displayedArtwork.Dispose();
                displayedArtwork = null;
            }

            if (toShow != null)
            {
                displayedArtwork = new Bitmap(toShow);
                pbArtwork.Image = displayedArtwork;
            }
            else
            {
                pbArtwork.Image = null;
            }
        }

        // Actualiza la letra mostrada en el RichTextBox; si no existe intenta obtenerla desde una API
        private async Task UpdateLyricsForIndexAsync(int index)
        {
            try
            {
                if (index < 0 || index >= playlist.Count) return;

                var song = playlist[index];

                // Si ya tenemos letra en tags, muéstrala y no llamamos a la API
                if (!string.IsNullOrWhiteSpace(song.Lyrics))
                {
                    rtbLyrics.Text = song.Lyrics;
                    return;
                }

                // Mostrar texto provisional si el control está visible
                if (rtbLyrics.Visible) rtbLyrics.Text = "Buscando letra...";

                // Intentar varias variantes y proveedores
                var fetched = await TryFetchLyricsVariantsAsync(song.Title, song.Artist).ConfigureAwait(false);

                if (!string.IsNullOrWhiteSpace(fetched))
                {
                    // guardar en la playlist para futuras referencias
                    song.Lyrics = fetched.Trim();
                    // si aún es la misma pista, actualizar UI en hilo de UI
                    if (index == currentIndex)
                    {
                        if (rtbLyrics.InvokeRequired)
                        {
                            rtbLyrics.Invoke(new Action(() => rtbLyrics.Text = song.Lyrics));
                        }
                        else
                        {
                            rtbLyrics.Text = song.Lyrics;
                        }
                    }
                }
                else
                {
                    if (rtbLyrics.InvokeRequired)
                    {
                        rtbLyrics.Invoke(new Action(() => rtbLyrics.Text = "Letra no disponible."));
                    }
                    else
                    {
                        rtbLyrics.Text = "Letra no disponible.";
                    }
                }
            }
            catch
            {
                // en caso de error, mostrar mensaje de no disponible si visible
                try
                {
                    if (rtbLyrics.InvokeRequired)
                    {
                        rtbLyrics.Invoke(new Action(() => rtbLyrics.Text = "Letra no disponible."));
                    }
                    else
                    {
                        if (rtbLyrics.Visible) rtbLyrics.Text = "Letra no disponible.";
                    }
                }
                catch { }
            }
        }

        // Prueba múltiples variantes normalizadas del artista/título y varios proveedores
        private async Task<string?> TryFetchLyricsVariantsAsync(string title, string artist)
        {
            // Generar variantes razonables
            var variants = new (string artist, string title)[]
            {
                (artist, title),
                (NormalizeForSearch(artist), NormalizeForSearch(title)),
                (NormalizeForSearch(RemoveFeat(artist)), NormalizeForSearch(RemoveParentheses(title))),
                (NormalizeForSearch(artist), NormalizeForSearch(RemoveParentheses(title))),
                ("", NormalizeForSearch(title)), // solo título
                (NormalizeForSearch(artist), "") // solo artista (raro)
            };

            foreach (var v in variants)
            {
                // ignorar exactamente iguales repetidos
                var a = v.artist ?? "";
                var t = v.title ?? "";
                if (string.IsNullOrWhiteSpace(a) && string.IsNullOrWhiteSpace(t)) continue;

                try
                {
                    var lyrics = await FetchLyricsFromProvidersAsync(t, a).ConfigureAwait(false);
                    if (!string.IsNullOrWhiteSpace(lyrics))
                        return lyrics;
                }
                catch
                {
                    // ignorar y probar siguiente variante
                }
            }

            return null;
        }

        // Intenta varios proveedores populares en orden
        private async Task<string?> FetchLyricsFromProvidersAsync(string titleNormalized, string artistNormalized)
        {
            // lyrics.ovh expects original artist/title, but normalized may help in combinations.
            // Primero intentar lyrics.ovh
            var ovh = await FetchFromLyricsOvhAsync(artistNormalized, titleNormalized).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(ovh)) return ovh;

            // Fallback a some-random-api.ml (acepta título)
            var randomApi = await FetchFromSomeRandomApiAsync(titleNormalized).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(randomApi)) return randomApi;

            // Aquí puedes añadir más proveedores (Genius/Musixmatch) que requieran API keys.

            return null;
        }

        // lyrics.ovh (intenta ambos, artist/title y title-only)
        private async Task<string?> FetchFromLyricsOvhAsync(string artistPart, string titlePart)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(artistPart) && !string.IsNullOrWhiteSpace(titlePart))
                {
                    var url = $"https://api.lyrics.ovh/v1/{Uri.EscapeDataString(artistPart)}/{Uri.EscapeDataString(titlePart)}";
                    using var req = new HttpRequestMessage(HttpMethod.Get, url);
                    using var resp = await _httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                    if (resp.IsSuccessStatusCode)
                    {
                        await using var stream = await resp.Content.ReadAsStreamAsync().ConfigureAwait(false);
                        var doc = await JsonDocument.ParseAsync(stream).ConfigureAwait(false);
                        if (doc.RootElement.TryGetProperty("lyrics", out var lyricsProp))
                        {
                            var lyrics = lyricsProp.GetString();
                            if (!string.IsNullOrWhiteSpace(lyrics)) return lyrics;
                        }
                    }
                }

                // intentar solo con título (algunos proveedores funcionan mejor así)
                if (!string.IsNullOrWhiteSpace(titlePart))
                {
                    var url2 = $"https://api.lyrics.ovh/v1/{Uri.EscapeDataString("")}/{Uri.EscapeDataString(titlePart)}";
                    // lyrics.ovh normalmente necesita artista, pero probamos de forma conservadora
                    using var req2 = new HttpRequestMessage(HttpMethod.Get, url2);
                    using var resp2 = await _httpClient.SendAsync(req2, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                    if (resp2.IsSuccessStatusCode)
                    {
                        await using var stream2 = await resp2.Content.ReadAsStreamAsync().ConfigureAwait(false);
                        var doc2 = await JsonDocument.ParseAsync(stream2).ConfigureAwait(false);
                        if (doc2.RootElement.TryGetProperty("lyrics", out var lyricsProp2))
                        {
                            var lyrics2 = lyricsProp2.GetString();
                            if (!string.IsNullOrWhiteSpace(lyrics2)) return lyrics2;
                        }
                    }
                }
            }
            catch
            {
                // ignorar errores del proveedor
            }

            return null;
        }

        // some-random-api.ml fallback (simple, acepta title)
        private async Task<string?> FetchFromSomeRandomApiAsync(string titlePart)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(titlePart)) return null;
                var url = $"https://some-random-api.ml/lyrics?title={Uri.EscapeDataString(titlePart)}";
                using var req = new HttpRequestMessage(HttpMethod.Get, url);
                using var resp = await _httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                if (!resp.IsSuccessStatusCode) return null;

                await using var stream = await resp.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var doc = await JsonDocument.ParseAsync(stream).ConfigureAwait(false);
                if (doc.RootElement.TryGetProperty("lyrics", out var lyricsProp))
                {
                    var lyrics = lyricsProp.GetString();
                    return lyrics;
                }
            }
            catch
            {
                // ignorar
            }

            return null;
        }

        // Helpers para normalizar / limpiar cadenas
        private static string NormalizeForSearch(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            s = s.Trim();
            s = RemoveParentheses(s);
            s = RemoveFeat(s);
            s = RemoveDiacritics(s);
            s = s.ToLowerInvariant();
            s = Regex.Replace(s, @"[^\w\s]", " "); // quitar signos
            s = Regex.Replace(s, @"\s+", " ").Trim();
            return s;
        }

        private static string RemoveParentheses(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return s ?? "";
            return Regex.Replace(s, @"\s*\(.*?\)\s*", " ").Trim();
        }

        private static string RemoveFeat(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return s ?? "";
            // elimina "feat.", "ft.", "featuring" y lo que venga después
            return Regex.Replace(s, @"\b(feat\.?|ft\.?|featuring)\b.*", "", RegexOptions.IgnoreCase).Trim();
        }

        private static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text ?? "";
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var ch in normalized)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        // DTO para serializar la lista (no incluye artwork)
        private record PlaylistEntry(string FilePath, string Title, string Artist, string Album, string Lyrics);



        // Cargar lista de reproducción desde JSON y reproducir
        private async void pbLoadPlaylist_Click(object? sender, EventArgs e)
        {
            try
            {
                using var _wc = new WaitCursorScope();
                using var ofd = new OpenFileDialog();
                ofd.Filter = "Playlist JSON|*.json";
                if (ofd.ShowDialog() != DialogResult.OK) return;

                // Leer en forma asíncrona, pero dejar la continuación en el hilo de UI (no ConfigureAwait(false))
                var text = await System.IO.File.ReadAllTextAsync(ofd.FileName, Encoding.UTF8);

                var entries = JsonSerializer.Deserialize<PlaylistEntry[]?>(text);
                if (entries == null || entries.Length == 0)
                {
                    MessageBox.Show("Archivo de lista vacío o inválido.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var missing = new List<string>();
                // Limpiar lista actual (esto se hace en UI thread)
                playlist.Clear();
                currentIndex = -1;

                // Cargar metadata en background pero añadir al BindingList en el hilo UI
                foreach (var eEntry in entries)
                {
                    if (string.IsNullOrWhiteSpace(eEntry.FilePath) || !System.IO.File.Exists(eEntry.FilePath))
                    {
                        missing.Add(eEntry.FilePath ?? "(vacío)");
                        continue;
                    }

                    // LoadSongFromFileAsync ya realiza I/O y TagLib en un hilo de fondo
                    var song = await LoadSongFromFileAsync(eEntry.FilePath);

                    if (song != null)
                    {
                        if (string.IsNullOrWhiteSpace(song.Title) && !string.IsNullOrWhiteSpace(eEntry.Title)) song.Title = eEntry.Title;
                        if (string.IsNullOrWhiteSpace(song.Artist) && !string.IsNullOrWhiteSpace(eEntry.Artist)) song.Artist = eEntry.Artist;
                        if (string.IsNullOrWhiteSpace(song.Album) && !string.IsNullOrWhiteSpace(eEntry.Album)) song.Album = eEntry.Album;
                        if (string.IsNullOrWhiteSpace(song.Lyrics) && !string.IsNullOrWhiteSpace(eEntry.Lyrics)) song.Lyrics = eEntry.Lyrics;

                        // Asegurar modificación de BindingList en el hilo UI
                        if (dgvPlaylist.InvokeRequired)
                            dgvPlaylist.Invoke(new Action(() => playlist.Add(song)));
                        else
                            playlist.Add(song);
                    }
                }

                if (missing.Count > 0)
                {
                    MessageBox.Show($"Se omitieron {missing.Count} archivos no encontrados.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                if (playlist.Count > 0)
                {
                    // Ejecutar PlayAt en hilo UI (ya estamos en UI thread porque no usamos ConfigureAwait(false))
                    PlayAt(0);
                }
                else
                {
                    MessageBox.Show("No se cargó ninguna canción válida.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar la lista: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Carga metadata y artwork desde archivo e instancia Song. Si falla, devuelve un Song básico con FilePath.
        private static Task<Song?> LoadSongFromFileAsync(string file)
        {
            return Task.Run(() =>
            {
                try
                {
                    using var tfile = TagLib.File.Create(file);
                    var title = string.IsNullOrWhiteSpace(tfile.Tag.Title) ? Path.GetFileNameWithoutExtension(file) : tfile.Tag.Title;
                    var artist = tfile.Tag.Performers != null && tfile.Tag.Performers.Length > 0 ? string.Join(", ", tfile.Tag.Performers) : "";
                    var album = tfile.Tag.Album ?? "";
                    var duration = tfile.Properties.Duration;

                    Image? artwork = null;
                    var pictures = tfile.Tag.Pictures;
                    if (pictures != null && pictures.Length > 0)
                    {
                        try
                        {
                            var pic = pictures[0];
                            using var ms = new MemoryStream(pic.Data.Data);
                            artwork = Image.FromStream(ms);
                        }
                        catch
                        {
                            artwork = null;
                        }
                    }

                    var lyrics = "";
                    try
                    {
                        lyrics = tfile.Tag.Lyrics ?? "";
                        lyrics = lyrics.Trim();
                    }
                    catch { lyrics = ""; }

                    // nuevas propiedades
                    var fileInfo = new FileInfo(file);
                    var fileSizeText = FileSizeFormatter.Format(fileInfo.Length);
                    var sampleRate = tfile.Properties.AudioSampleRate;
                    var sampleRateText = sampleRate > 0 ? $"{sampleRate / 1000.0:0.##} kHz" : "";

                    return new Song
                    {
                        FilePath = file,
                        Title = title,
                        Artist = artist,
                        Album = album,
                        Duration = duration,
                        Artwork = artwork,
                        Lyrics = lyrics,
                        FileSizeText = fileSizeText,
                        SampleRate = sampleRate,
                        SampleRateText = sampleRateText
                    };
                }
                catch
                {
                    // fallback: devolver objeto mínimo si el taglib falla
                    var fi = new FileInfo(file);
                    return new Song
                    {
                        FilePath = file,
                        Title = Path.GetFileNameWithoutExtension(file),
                        Artist = "",
                        Album = "",
                        Duration = TimeSpan.Zero,
                        Artwork = null,
                        Lyrics = "",
                        FileSizeText = FileSizeFormatter.Format(fi.Exists ? fi.Length : 0),
                        SampleRate = 0,
                        SampleRateText = ""
                    };
                }
            });
        }

        // Manejador del nuevo botón: abrir la canción actual en Windows Media Player
        private void btnOpenWmp_Click(object? sender, EventArgs e)
        {
            try
            {
                if (currentIndex < 0 || currentIndex >= playlist.Count)
                {
                    MessageBox.Show("No hay ninguna canción seleccionada.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var path = playlist[currentIndex].FilePath;


                var psi = new ProcessStartInfo
                {
                    FileName = "wmplayer.exe",
                    Arguments = $"\"{path}\"",
                    UseShellExecute = true
                };

                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo abrir en Windows Media Player: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Botón para limpiar (eliminar) todo el contenido de la lista de reproducción (DataGridView)
        private void btnClearPlaylist_Click(object? sender, EventArgs e)
        {
            try
            {
                if (playlist.Count == 0)
                {
                    MessageBox.Show("La lista ya está vacía.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var dr = MessageBox.Show("¿Deseas eliminar todas las canciones de la lista de reproducción?", "Confirmar borrado", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr != DialogResult.Yes) return;

                // Detener y liberar recursos de audio
                StopAndDispose();

                // Liberar imágenes de las canciones para evitar fugas
                foreach (var s in playlist)
                {
                    try
                    {
                        s.Artwork?.Dispose();
                        s.Artwork = null;
                    }
                    catch { }
                }

                // Vaciar la lista ligada que alimenta el DataGridView
                playlist.Clear();
                currentIndex = -1;
                // reset shuffle/repeat state
                shuffleEnabled = false;
                originalOrder = null;
                repeatEnabled = false;
                hasRepeatedCurrent = false;

                // Limpiar UI relacionada
                if (displayedArtwork != null)
                {
                    pbArtwork.Image = null;
                    displayedArtwork.Dispose();
                    displayedArtwork = null;
                }

                dgvPlaylist.ClearSelection();
                lblFile.Text = "Archivo: (ninguno)";
                lblTime.Text = "00:00 / 00:00";
                rtbLyrics.Text = "";

                try { pbPlay.Enabled = false; } catch { }
                try { pbStop.Enabled = false; } catch { }
                try { pbPrev.Enabled = false; } catch { }
                try { pbNext.Enabled = false; } catch { }
                trackBarPosition.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo limpiar la lista: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pbArtwork_Click(object sender, EventArgs e) { }

        private void rtbLyrics_TextChanged(object sender, EventArgs e)
        {

        }

        private void pbSavePlaylist_Click(object sender, EventArgs e)
        {
            try
            {
                if (playlist.Count == 0)
                {
                    MessageBox.Show("La lista de reproducción está vacía.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using var sfd = new SaveFileDialog();
                sfd.Filter = "Playlist JSON|*.json";
                sfd.DefaultExt = "json";
                sfd.FileName = "playlist.json";
                if (sfd.ShowDialog() != DialogResult.OK) return;

                var entries = playlist.Select(s => new PlaylistEntry(s.FilePath, s.Title, s.Artist, s.Album, s.Lyrics)).ToList();
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(entries, options);
                using (new WaitCursorScope())
                {
                    System.IO.File.WriteAllText(sfd.FileName, json, Encoding.UTF8);
                }

                MessageBox.Show("Lista guardada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la lista: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

