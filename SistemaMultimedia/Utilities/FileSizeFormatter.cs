namespace SistemaMultimedia.Utilities;

public static class FileSizeFormatter
{
    public static string Format(long bytes)
    {
        string[] sufijos = { "B", "KB", "MB", "GB", "TB" };
        int indice = 0;
        double tamaño = bytes;

        while (tamaño >= 1024 && indice < sufijos.Length - 1)
        {
            tamaño /= 1024;
            indice++;
        }

        return $"{tamaño:0.##} {sufijos[indice]}";
    }
}
