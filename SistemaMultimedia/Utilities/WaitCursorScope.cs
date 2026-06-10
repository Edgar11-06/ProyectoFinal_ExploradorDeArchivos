namespace SistemaMultimedia.Utilities;

public sealed class WaitCursorScope : IDisposable
{
    private readonly bool _previousUseWaitCursor;

    public WaitCursorScope()
    {
        try
        {
            _previousUseWaitCursor = Application.UseWaitCursor;
            Application.UseWaitCursor = true;
            Cursor.Current = Cursors.WaitCursor;
            foreach (Form form in Application.OpenForms)
            {
                try { form.UseWaitCursor = true; } catch { }
            }
        }
        catch { }
    }

    public void Dispose()
    {
        try
        {
            Application.UseWaitCursor = _previousUseWaitCursor;
            Cursor.Current = Cursors.Default;
            foreach (Form form in Application.OpenForms)
            {
                try { form.UseWaitCursor = _previousUseWaitCursor; } catch { }
            }
        }
        catch { }
    }
}
