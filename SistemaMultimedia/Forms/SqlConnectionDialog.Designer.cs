using System;
using System.Windows.Forms;
using System.Drawing;

namespace SistemaMultimedia.Forms
{
    partial class SqlConnectionDialog
    {
        private TextBox txtConnection;
        private TextBox txtDatabase;
        private TextBox txtTable;
        private Button btnOk;
        private Button btnCancel;

        private void InitializeComponent()
        {
            lblConn = new Label();
            txtConnection = new TextBox();
            lblDb = new Label();
            txtDatabase = new TextBox();
            lblTable = new Label();
            txtTable = new TextBox();
            btnOk = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // lblConn
            // 
            lblConn.BackColor = Color.FromArgb(30, 30, 30);
            lblConn.ForeColor = Color.White;
            lblConn.Location = new Point(12, 2);
            lblConn.Name = "lblConn";
            lblConn.Size = new Size(167, 23);
            lblConn.TabIndex = 0;
            lblConn.Text = "Connection String";
            // 
            // txtConnection
            // 
            txtConnection.BackColor = Color.White;
            txtConnection.Cursor = Cursors.IBeam;
            txtConnection.ForeColor = Color.Black;
            txtConnection.Location = new Point(12, 28);
            txtConnection.Name = "txtConnection";
            txtConnection.Size = new Size(654, 31);
            txtConnection.TabIndex = 1;
            // 
            // lblDb
            // 
            lblDb.BackColor = Color.FromArgb(30, 30, 30);
            lblDb.ForeColor = Color.White;
            lblDb.Location = new Point(12, 79);
            lblDb.Name = "lblDb";
            lblDb.Size = new Size(319, 23);
            lblDb.TabIndex = 2;
            lblDb.Text = "Base de datos (se creára si no existe)";
            // 
            // txtDatabase
            // 
            txtDatabase.BackColor = Color.White;
            txtDatabase.Cursor = Cursors.IBeam;
            txtDatabase.ForeColor = Color.Black;
            txtDatabase.Location = new Point(12, 105);
            txtDatabase.Name = "txtDatabase";
            txtDatabase.Size = new Size(319, 31);
            txtDatabase.TabIndex = 3;
            // 
            // lblTable
            // 
            lblTable.BackColor = Color.FromArgb(30, 30, 30);
            lblTable.ForeColor = Color.White;
            lblTable.Location = new Point(394, 79);
            lblTable.Name = "lblTable";
            lblTable.Size = new Size(272, 23);
            lblTable.TabIndex = 4;
            lblTable.Text = "Nombre de la tabla destino";
            // 
            // txtTable
            // 
            txtTable.BackColor = Color.White;
            txtTable.Cursor = Cursors.IBeam;
            txtTable.ForeColor = Color.Black;
            txtTable.Location = new Point(394, 105);
            txtTable.Name = "txtTable";
            txtTable.Size = new Size(272, 31);
            txtTable.TabIndex = 5;
            // 
            // btnOk
            // 
            btnOk.BackColor = Color.White;
            btnOk.Cursor = Cursors.Hand;
            btnOk.ForeColor = Color.Black;
            btnOk.Location = new Point(483, 142);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(90, 31);
            btnOk.TabIndex = 6;
            btnOk.Text = "Aceptar";
            btnOk.UseVisualStyleBackColor = false;
            btnOk.Click += btnOk_Click;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.White;
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.ForeColor = Color.Black;
            btnCancel.Location = new Point(579, 142);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(90, 31);
            btnCancel.TabIndex = 7;
            btnCancel.Text = "Cancelar";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += btnCancel_Click;
            // 
            // SqlConnectionDialog
            // 
            BackColor = Color.FromArgb(30, 30, 30);
            ClientSize = new Size(678, 184);
            Controls.Add(lblConn);
            Controls.Add(txtConnection);
            Controls.Add(lblDb);
            Controls.Add(txtDatabase);
            Controls.Add(lblTable);
            Controls.Add(txtTable);
            Controls.Add(btnOk);
            Controls.Add(btnCancel);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SqlConnectionDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Datos conexión SQL Server";
            ResumeLayout(false);
            PerformLayout();
        }

        private Label lblConn;
        private Label lblDb;
        private Label lblTable;
    }
}
