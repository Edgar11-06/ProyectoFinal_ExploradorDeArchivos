namespace SistemaMultimedia.Forms.ImageEditor;

partial class TextInputDialog
{
    private System.ComponentModel.IContainer? components = null;
    private Label lblPrompt = null!;
    private TextBox txtValue = null!;
    private Button btnOk = null!;
    private Button btnCancel = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        lblPrompt = new Label();
        txtValue = new TextBox();
        btnOk = new Button();
        btnCancel = new Button();
        SuspendLayout();
        lblPrompt.AutoSize = true;
        lblPrompt.ForeColor = Color.White;
        lblPrompt.Location = new Point(12, 8);
        lblPrompt.Name = "lblPrompt";
        lblPrompt.Size = new Size(121, 25);
        lblPrompt.Text = "Ingresar texto";
        txtValue.Location = new Point(12, 36);
        txtValue.Name = "txtValue";
        txtValue.Size = new Size(480, 31);
        btnOk.Location = new Point(308, 72);
        btnOk.Name = "btnOk";
        btnOk.Size = new Size(80, 30);
        btnOk.Text = "Ok";
        btnOk.Click += btnOk_Click;
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.Location = new Point(394, 72);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(98, 30);
        btnCancel.Text = "Cancelar";
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(30, 30, 30);
        ClientSize = new Size(504, 114);
        Controls.Add(btnCancel);
        Controls.Add(btnOk);
        Controls.Add(txtValue);
        Controls.Add(lblPrompt);
        ForeColor = Color.White;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "TextInputDialog";
        StartPosition = FormStartPosition.CenterParent;
        ResumeLayout(false);
        PerformLayout();
    }
}
