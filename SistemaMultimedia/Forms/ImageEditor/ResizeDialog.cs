namespace SistemaMultimedia.Forms.ImageEditor;

public partial class ResizeDialog : Form
{
    public int ResultWidth { get; private set; }
    public int ResultHeight { get; private set; }

    public ResizeDialog(int currentWidth, int currentHeight)
    {
        InitializeComponent();
        nudWidth.Value = Math.Max(1, currentWidth);
        nudHeight.Value = Math.Max(1, currentHeight);
    }

    private void btnOk_Click(object? sender, EventArgs e)
    {
        ResultWidth = (int)nudWidth.Value;
        ResultHeight = (int)nudHeight.Value;
        DialogResult = DialogResult.OK;
        Close();
    }

    public static bool TryGetSize(int currentWidth, int currentHeight, out int width, out int height)
    {
        using var dialog = new ResizeDialog(currentWidth, currentHeight);
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            width = dialog.ResultWidth;
            height = dialog.ResultHeight;
            return true;
        }

        width = 0;
        height = 0;
        return false;
    }
}
