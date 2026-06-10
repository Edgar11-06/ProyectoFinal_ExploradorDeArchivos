namespace SistemaMultimedia.Forms.ImageEditor;

public partial class TextInputDialog : Form
{
    public string ResultText { get; private set; } = string.Empty;

    public TextInputDialog(string label, string title, string initial = "")
    {
        InitializeComponent();
        lblPrompt.Text = label;
        Text = title;
        txtValue.Text = initial;
    }

    private void btnOk_Click(object? sender, EventArgs e)
    {
        ResultText = txtValue.Text;
        DialogResult = DialogResult.OK;
        Close();
    }

    public static string Prompt(string label, string title, string initial = "")
    {
        using var dialog = new TextInputDialog(label, title, initial);
        return dialog.ShowDialog() == DialogResult.OK ? dialog.ResultText : string.Empty;
    }
}
