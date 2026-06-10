using System;
using System.Linq;
using System.Windows.Forms;

namespace SistemaMultimedia.Forms
{
    public partial class GroupByDialog : Form
    {
        public string[] SelectedColumns { get; private set; } = Array.Empty<string>();

        public GroupByDialog(System.Data.DataTable table)
        {
            InitializeComponent();

            if (table != null)
            {
                foreach (System.Data.DataColumn c in table.Columns)
                {
                    // ocultar columna interna de índice si existe
                    if (c.ColumnName == "__original_index") continue;
                    clbColumns.Items.Add(c.ColumnName);
                }
            }

            btnOk.Click += (s, e) =>
            {
                SelectedColumns = clbColumns.CheckedItems.Cast<string>().ToArray();
                if (SelectedColumns.Length == 0)
                {
                    // confirmar agrupación sin columnas
                    if (MessageBox.Show(this, "No has seleccionado columnas. ¿Mostrar todos los grupos (sin agrupar)?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        DialogResult = DialogResult.None;
                        return;
                    }
                }
                DialogResult = DialogResult.OK;
                Close();
            };
        }

        public static string[]? ShowDialogAndGetColumns(IWin32Window owner, System.Data.DataTable table)
        {
            using var dlg = new GroupByDialog(table);
            var res = dlg.ShowDialog(owner);
            if (res != DialogResult.OK) return null;
            return dlg.SelectedColumns;
        }
    }
}