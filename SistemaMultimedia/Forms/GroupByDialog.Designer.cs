using System;
using System.Windows.Forms;
using System.Drawing;

namespace SistemaMultimedia.Forms
{
    partial class GroupByDialog
    {
        private CheckedListBox clbColumns;
        private Button btnOk;
        private Button btnCancel;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GroupByDialog));
            clbColumns = new CheckedListBox();
            btnOk = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // clbColumns
            // 
            clbColumns.BackColor = Color.FromArgb(30, 30, 30);
            clbColumns.ForeColor = Color.White;
            clbColumns.Location = new Point(3, 3);
            clbColumns.Name = "clbColumns";
            clbColumns.Size = new Size(390, 256);
            clbColumns.TabIndex = 0;
            // 
            // btnOk
            // 
            btnOk.BackColor = Color.White;
            btnOk.Cursor = Cursors.Hand;
            btnOk.ForeColor = Color.Black;
            btnOk.Location = new Point(104, 265);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(88, 29);
            btnOk.TabIndex = 1;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = false;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.White;
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.ForeColor = Color.Black;
            btnCancel.Location = new Point(198, 265);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 29);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Cancelar";
            btnCancel.UseVisualStyleBackColor = false;
            // 
            // GroupByDialog
            // 
            AcceptButton = btnOk;
            BackColor = Color.FromArgb(30, 30, 30);
            CancelButton = btnCancel;
            ClientSize = new Size(398, 304);
            Controls.Add(clbColumns);
            Controls.Add(btnOk);
            Controls.Add(btnCancel);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "GroupByDialog";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Seleccionar columnas para agrupar";
            ResumeLayout(false);
        }
    }
}
