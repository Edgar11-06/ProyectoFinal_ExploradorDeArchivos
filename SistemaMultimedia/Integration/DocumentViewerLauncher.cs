using VisorEditorDocumentos;

namespace SistemaMultimedia.Integration;

public static class DocumentViewerLauncher
{
    public static void Open(string filePath, IWin32Window? owner = null)
    {
        var form = new MainForm(filePath);

        if (owner != null)
            form.Show(owner);
        else
            form.Show();
    }
}
