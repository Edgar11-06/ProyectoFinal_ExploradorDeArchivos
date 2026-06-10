namespace SistemaMultimedia.Forms.ImageEditor;

public static class ImageEditorLauncher
{
    public static void Open(string imagePath, IWin32Window? owner = null)
    {
        if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
        {
            MessageBox.Show(
                owner,
                "No se encontró la imagen.",
                "Editor de imágenes",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        using var editor = new ImageEditorForm(imagePath);
        editor.ShowDialog(owner);
    }
}
