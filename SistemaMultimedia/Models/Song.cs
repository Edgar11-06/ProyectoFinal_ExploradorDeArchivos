namespace SistemaMultimedia.Models;

using System.ComponentModel;

public class Song : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private bool _isPlaying;

    public string FilePath { get; set; } = "";
    public string Title { get; set; } = "";
    public string Artist { get; set; } = "";
    public string Album { get; set; } = "";
    public TimeSpan Duration { get; set; } = TimeSpan.Zero;
    public Image? Artwork { get; set; }
    public string Lyrics { get; set; } = "";
    public string FileSizeText { get; set; } = "";
    public int SampleRate { get; set; }
    public string SampleRateText { get; set; } = "";
    public string DurationText => $"{(int)Duration.TotalMinutes:00}:{Duration.Seconds:00}";

    public bool IsPlaying
    {
        get => _isPlaying;
        set
        {
            if (_isPlaying != value)
            {
                _isPlaying = value;
                OnPropertyChanged(nameof(IsPlaying));
                OnPropertyChanged(nameof(PlayingText));
            }
        }
    }

    public string PlayingText => IsPlaying ? "▶" : "";

    public override string ToString() => Title;

    private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
