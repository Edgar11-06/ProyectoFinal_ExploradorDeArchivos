using SistemaMultimedia.Forms;

namespace SistemaMultimedia;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new ExploradorArchivosForm());
    }
}
