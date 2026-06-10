using DataValidationModule.UI;

namespace SistemaMultimedia.Integration;

public static class DataValidationLauncher
{
    public static void Open(string? filePath, IWin32Window? owner = null)
    {
        var form = new FrmDataValidation();
        if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
            form.LoadFromFile(filePath);

        if (owner != null)
            form.Show(owner);
        else
            form.Show();
    }
}
