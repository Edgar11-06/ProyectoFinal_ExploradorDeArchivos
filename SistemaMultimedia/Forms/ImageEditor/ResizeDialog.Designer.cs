namespace SistemaMultimedia.Forms.ImageEditor;

partial class ResizeDialog
{
    private System.ComponentModel.IContainer? components = null;
    private NumericUpDown nudWidth = null!;
    private NumericUpDown nudHeight = null!;
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
        nudWidth = new NumericUpDown();
        nudHeight = new NumericUpDown();
        btnOk = new Button();
        btnCancel = new Button();
        ((System.ComponentModel.ISupportInitialize)nudWidth).BeginInit();
        ((System.ComponentModel.ISupportInitialize)nudHeight).BeginInit();
        SuspendLayout();
        nudWidth.Location = new Point(12, 36);
        nudWidth.Maximum = new decimal(new int[] { 20000, 0, 0, 0 });
        nudWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        nudWidth.Name = "nudWidth";
        nudWidth.Size = new Size(320, 31);
        nudWidth.Value = new decimal(new int[] { 1, 0, 0, 0 });
        nudHeight.Location = new Point(12, 96);
        nudHeight.Maximum = new decimal(new int[] { 20000, 0, 0, 0 });
        nudHeight.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        nudHeight.Name = "nudHeight";
        nudHeight.Size = new Size(320, 31);
        nudHeight.Value = new decimal(new int[] { 1, 0, 0, 0 });
        btnOk.Location = new Point(152, 136);
        btnOk.Name = "btnOk";
        btnOk.Size = new Size(80, 30);
        btnOk.Text = "Ok";
        btnOk.Click += btnOk_Click;
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.Location = new Point(240, 136);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(92, 30);
        btnCancel.Text = "Cancelar";
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(30, 30, 30);
        ClientSize = new Size(360, 180);
        Controls.Add(btnCancel);
        Controls.Add(btnOk);
        Controls.Add(nudHeight);
        Controls.Add(nudWidth);
        ForeColor = Color.White;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ResizeDialog";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Redimensionar";
        ((System.ComponentModel.ISupportInitialize)nudWidth).EndInit();
        ((System.ComponentModel.ISupportInitialize)nudHeight).EndInit();
        ResumeLayout(false);
    }
}
