namespace SistemaMultimedia.Utilities;

public static class FileTypeHelper
{
    private static readonly string[] AudioExtensions =
    {
        ".mp3", ".wav", ".wma", ".flac", ".aac", ".ogg"
    };

    private static readonly string[] VideoExtensions =
    {
        ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv", ".webm", ".m4v", ".mpg", ".mpeg",
        ".3gp", ".ts", ".m2ts", ".vob", ".ogv", ".divx", ".asf", ".m2v", ".f4v"
    };

    private static readonly string[] DataExtensions =
    {
        ".json", ".xml", ".csv", ".txt"
    };

    private static readonly string[] ImageExtensions =
    {
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".ico", ".tif", ".tiff", ".webp"
    };

    private static readonly string[] DocumentExtensions =
    {
        ".pdf", ".docx"
    };

    private static readonly Dictionary<string, string> FileTypeDescriptions = new(StringComparer.OrdinalIgnoreCase)
    {
        { ".jpg", "Imagen JPEG" }, { ".jpeg", "Imagen JPEG" }, { ".png", "Imagen PNG" },
        { ".gif", "Imagen GIF" }, { ".bmp", "Imagen BMP" }, { ".ico", "Icono" },
        { ".mp3", "Audio MP3" }, { ".wav", "Audio WAV" }, { ".wma", "Audio WMA" },
        { ".flac", "Audio FLAC" }, { ".aac", "Audio AAC" }, { ".ogg", "Audio OGG" },
        { ".mp4", "Video MP4" }, { ".avi", "Video AVI" }, { ".mkv", "Video MKV" },
        { ".mov", "Video MOV" }, { ".wmv", "Video WMV" }, { ".flv", "Video FLV" },
        { ".txt", "Archivo de texto" }, { ".json", "Archivo JSON" },
        { ".xml", "Archivo XML" }, { ".csv", "Archivo CSV" },
        { ".doc", "Documento Word" },
        { ".docx", "Documento Word" }, { ".pdf", "Documento PDF" },
        { ".xls", "Hoja de cálculo Excel" }, { ".xlsx", "Hoja de cálculo Excel" },
        { ".rtf", "Documento RTF" }
    };

    public static bool IsAudio(string extension) =>
        AudioExtensions.Contains(NormalizeExtension(extension));

    public static bool IsVideo(string extension) =>
        VideoExtensions.Contains(NormalizeExtension(extension));

    public static bool IsDataFile(string extension) =>
        DataExtensions.Contains(NormalizeExtension(extension));

    public static bool IsImage(string extension) =>
        ImageExtensions.Contains(NormalizeExtension(extension));

    public static bool IsDocument(string extension) =>
        DocumentExtensions.Contains(NormalizeExtension(extension));

    public static string GetIconKey(string extension)
    {
        extension = NormalizeExtension(extension);

        if (IsImage(extension))
            return "imagen";

        if (IsAudio(extension))
            return "audio";

        if (extension is ".mp4" or ".avi" or ".mkv" or ".mov" or ".wmv" or ".flv")
            return "video";

        if (extension is ".txt" or ".json" or ".xml" or ".csv")
            return "texto";

        if (extension is ".pdf")
            return "pdf";

        if (extension is ".doc" or ".docx")
            return "word";

        if (extension is ".xls" or ".xlsx")
            return "excel";

        if (extension is ".ppt" or ".pptx")
            return "powerpoint";

        return "archivo";
    }

    public static string GetFileTypeDescription(string extension)
    {
        extension = NormalizeExtension(extension);
        return FileTypeDescriptions.TryGetValue(extension, out var description)
            ? description
            : $"Archivo {extension}";
    }

    private static string NormalizeExtension(string extension) =>
        string.IsNullOrWhiteSpace(extension) ? string.Empty : extension.ToLowerInvariant();
}
