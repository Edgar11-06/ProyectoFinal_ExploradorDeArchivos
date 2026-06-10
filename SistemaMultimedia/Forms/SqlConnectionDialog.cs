using System;
using System.Drawing;
using System.Windows.Forms;

namespace SistemaMultimedia.Forms
{
    public partial class SqlConnectionDialog : Form
    {
        public string ConnectionString => txtConnection.Text.Trim();
        public string DatabaseName => txtDatabase.Text.Trim();
        public string TableName => txtTable.Text.Trim();

        public SqlConnectionDialog(string? defaultConnection = null, string? defaultDatabase = null, string? defaultTable = null)
        {
            InitializeComponent();

            txtConnection.Text = defaultConnection ?? string.Empty;
            txtDatabase.Text = defaultDatabase ?? "ImportedDataDb";
            txtTable.Text = defaultTable ?? "ImportedData";

            AcceptButton = btnOk;
            CancelButton = btnCancel;

            btnOk.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtConnection.Text))
                {
                    MessageBox.Show(this, "La cadena de conexión no puede estar vacía.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtTable.Text))
                {
                    MessageBox.Show(this, "El nombre de tabla no puede estar vacío.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DialogResult = DialogResult.OK;
                Close();
            };
        }

        private void btnOk_Click(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}