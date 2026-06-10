using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using VisorEditorDocumentos.Controls;
using VisorEditorDocumentos.Services;

namespace VisorEditorDocumentos
{
    public partial class MainForm : Form
    {
        private readonly WordService _wordService = new();

        private enum DocumentType { None, Pdf, Word, Excel }
        private DocumentType _currentDocType = DocumentType.None;
        private bool _wordReadOnly;
        private bool _isModified;
        private string? _currentFilePath;
        private string? _pendingFilePath;

        private BrowserDocumentViewerControl _documentViewer = null!;

        public MainForm()
        {
            InitializeComponent();
            AllowDrop = true;
            _documentViewer.ContentModified += (_, _) => MarkAsModified();
            ConfigureToolbarText();
            UpdateUiState();
        }

        public MainForm(string filePathToOpen) : this()
        {
            _pendingFilePath = filePathToOpen;
        }

        public void OpenDocument(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                MessageBox.Show("No se encontro el archivo indicado.", "Archivo no encontrado",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (IsHandleCreated && Visible)
                _ = OpenFileAsync(filePath);
            else
                _pendingFilePath = filePath;
        }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (string.IsNullOrEmpty(_pendingFilePath)) return;

            var path = _pendingFilePath;
            _pendingFilePath = null;
            await OpenFileAsync(path);
        }

        private void ConfigureToolbarText()
        {
            btnOpen.Text = "Abrir";
            btnSave.Text = "Guardar";
            btnSaveAs.Text = "Exportar";
            btnWordEdit.Text = "Editar";
            btnWordPreview.Text = "Vista previa";
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Title = "Abrir documento",
                Filter = "Documentos soportados|*.pdf;*.docx;*.xlsx;*.xls;*.csv|PDF (*.pdf)|*.pdf|Word (*.docx)|*.docx|Excel (*.xlsx;*.xls;*.csv)|*.xlsx;*.xls;*.csv",
                RestoreDirectory = true
            };

            if (dlg.ShowDialog(this) != DialogResult.OK) return;
            _ = OpenFileAsync(dlg.FileName);
        }

        private async Task OpenFileAsync(string filePath)
        {
            try
            {
                lblStatus.Text = "Cargando documento...";
                UseWaitCursor = true;

                await _documentViewer.EnsureInitializedAsync();

                var ext = Path.GetExtension(filePath).ToLowerInvariant();
                switch (ext)
                {
                    case ".pdf":
                        await OpenPdfAsync(filePath);
                        break;
                    case ".docx":
                        await OpenWordAsync(filePath);
                        break;
                    case ".xlsx":
                    case ".xls":
                    case ".csv":
                        await OpenExcelAsync(filePath);
                        break;
                    default:
                        MessageBox.Show(
                            "Formato no soportado. Se admiten PDF, DOCX y Excel (.xlsx/.xls/.csv).",
                            "Formato no soportado",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo abrir el archivo:\n{ex.Message}",
                    "Error al abrir", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (_currentDocType is not (DocumentType.Word or DocumentType.Excel)) return;

            try
            {
                var html = await _documentViewer.GetBodyHtmlAsync();
                if (_currentDocType == DocumentType.Word)
                {
                    _wordService.SaveHtml(html);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(_currentFilePath))
                        throw new InvalidOperationException("No hay ningun archivo Excel cargado.");

                    if (!Path.GetExtension(_currentFilePath).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("Los archivos .xls/.csv se pueden editar, pero para guardar cambios use Exportar y elija Excel .xlsx.",
                            "Guardar Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    ExcelHtmlConverter.SaveBodyHtmlAsXlsx(html, _currentFilePath);
                }

                lblStatus.Text = "Cambios guardados correctamente.";
                UpdateFileLabel(_currentFilePath ?? _wordService.CurrentFilePath!);
                ClearModifiedFlag();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnSaveAs_Click(object sender, EventArgs e)
        {
            if (_currentDocType == DocumentType.None) return;

            using var dlg = new SaveFileDialog
            {
                Title = "Exportar documento",
                Filter = "JSON (*.json)|*.json|TXT (*.txt)|*.txt|CSV (*.csv)|*.csv|XML (*.xml)|*.xml|PDF (*.pdf)|*.pdf|Word (*.docx)|*.docx|Excel (*.xlsx)|*.xlsx",
                DefaultExt = "pdf",
                FileName = Path.GetFileNameWithoutExtension(_currentFilePath ?? "documento") + "_exportado"
            };

            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                await ExportCurrentDocumentAsync(dlg.FileName);
                lblStatus.Text = "Documento exportado correctamente.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnWordPreview_Click(object sender, EventArgs e)
        {
            if (_currentDocType is not (DocumentType.Word or DocumentType.Excel)) return;
            _wordReadOnly = true;
            await _documentViewer.SetEditableAsync(false);
            btnWordEdit.Enabled = true;
            btnWordPreview.Enabled = false;
            btnSave.Enabled = false;
            lblStatus.Text = "Vista de solo lectura. Pulse Editar para modificar.";
        }

        private async void btnWordEdit_Click(object sender, EventArgs e)
        {
            if (_currentDocType is not (DocumentType.Word or DocumentType.Excel)) return;
            _wordReadOnly = false;
            await _documentViewer.SetEditableAsync(true);
            btnWordEdit.Enabled = false;
            btnWordPreview.Enabled = true;
            btnSave.Enabled = true;
            lblStatus.Text = "Modo edicion. Puede modificar texto y tablas directamente.";
        }

        private async Task OpenPdfAsync(string filePath)
        {
            _wordService.UnloadDocument();
            _currentDocType = DocumentType.Pdf;
            _currentFilePath = filePath;
            _wordReadOnly = true;
            panelWordTools.Visible = false;

            ShowViewer();
            await _documentViewer.NavigateToFileAsync(filePath);

            UpdateFileLabel(filePath);
            UpdateUiState();
            ClearModifiedFlag();
            lblStatus.Text = "PDF abierto en el visor integrado. Puede exportarlo a PDF, Word o Excel.";
        }

        private async Task OpenWordAsync(string filePath)
        {
            _currentDocType = DocumentType.Word;
            _currentFilePath = filePath;
            _wordReadOnly = false;

            var bodyHtml = _wordService.LoadAsHtml(filePath);
            panelWordTools.Visible = true;
            btnWordEdit.Enabled = false;
            btnWordPreview.Enabled = true;

            ShowViewer();
            await _documentViewer.LoadDocxEditorAsync(bodyHtml, editable: true);

            UpdateFileLabel(filePath);
            UpdateUiState();
            ClearModifiedFlag();
            lblStatus.Text = "Documento Word abierto. Edite el contenido y guarde o exporte los cambios.";
        }

        private async Task OpenExcelAsync(string filePath)
        {
            _wordService.UnloadDocument();
            _currentDocType = DocumentType.Excel;
            _currentFilePath = filePath;
            _wordReadOnly = false;

            var bodyHtml = ExcelHtmlConverter.ToBodyHtml(filePath);
            panelWordTools.Visible = true;
            btnWordEdit.Enabled = false;
            btnWordPreview.Enabled = true;

            ShowViewer();
            await _documentViewer.LoadDocxEditorAsync(bodyHtml, editable: true);

            UpdateFileLabel(filePath);
            UpdateUiState();
            ClearModifiedFlag();
            lblStatus.Text = "Documento Excel abierto. Edite las celdas y guarde como .xlsx o exporte.";
        }

        private async Task ExportCurrentDocumentAsync(string outputPath)
        {
            var ext = Path.GetExtension(outputPath).ToLowerInvariant();
            var html = await GetCurrentBodyHtmlAsync();

            switch (ext)
            {
                case ".json":
                    DocumentExportService.ExportHtmlAsJson(html, outputPath);
                    break;
                case ".txt":
                    DocumentExportService.ExportHtmlAsTxt(html, outputPath);
                    break;
                case ".csv":
                    DocumentExportService.ExportHtmlAsCsv(html, outputPath);
                    break;
                case ".xml":
                    DocumentExportService.ExportHtmlAsXml(html, outputPath);
                    break;
                case ".pdf":
                    if (_currentDocType == DocumentType.Pdf && !string.IsNullOrWhiteSpace(_currentFilePath))
                        File.Copy(_currentFilePath, outputPath, overwrite: true);
                    else
                        DocumentExportService.ExportHtmlAsPdf(html, outputPath, Path.GetFileName(_currentFilePath ?? "Documento"));
                    break;
                case ".docx":
                    DocumentExportService.ExportHtmlAsDocx(html, outputPath);
                    break;
                case ".xlsx":
                    DocumentExportService.ExportHtmlAsXlsx(html, outputPath);
                    break;
                default:
                    throw new InvalidOperationException("Formato de exportacion no soportado. Use JSON, TXT, CSV, XML, PDF, DOCX o XLSX.");
            }
        }

        private async Task<string> GetCurrentBodyHtmlAsync()
        {
            if (_currentDocType == DocumentType.Pdf)
                return DocumentExportService.PdfToBodyHtml(_currentFilePath ?? "documento.pdf");

            return await _documentViewer.GetBodyHtmlAsync();
        }

        private void ShowViewer()
        {
            _documentViewer.Visible = true;
            _documentViewer.BringToFront();
        }

        private void MarkAsModified()
        {
            if (_currentDocType == DocumentType.Pdf || _wordReadOnly || _isModified) return;
            _isModified = true;
            if (!Text.EndsWith(" *"))
                Text += " *";
            lblStatus.Text = "Documento modificado. Guarde o exporte los cambios.";
        }

        private void ClearModifiedFlag()
        {
            _isModified = false;
            if (Text.EndsWith(" *"))
                Text = Text[..^2];
        }

        private void UpdateFileLabel(string filePath)
        {
            lblFileName.Text = Path.GetFileName(filePath);
            Text = $"Visor y Editor - {Path.GetFileName(filePath)}";
        }

        private void UpdateUiState()
        {
            var isEditable = _currentDocType is DocumentType.Word or DocumentType.Excel;
            var hasDocument = _currentDocType != DocumentType.None;
            btnSave.Enabled = isEditable && !_wordReadOnly;
            btnSaveAs.Enabled = hasDocument;

            if (_currentDocType == DocumentType.None)
            {
                lblFileName.Text = "Ningun archivo abierto";
                lblStatus.Text = "Bienvenido. Haga clic en Abrir para comenzar.";
                Text = "Visor y Editor de Documentos";
            }
        }

        protected override async void OnFormClosing(FormClosingEventArgs e)
        {
            if (_isModified && _currentDocType is (DocumentType.Word or DocumentType.Excel))
            {
                var result = MessageBox.Show(
                    "Hay cambios sin guardar. Desea guardar antes de salir?",
                    "Cambios sin guardar",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        var html = await _documentViewer.GetBodyHtmlAsync();
                        if (_currentDocType == DocumentType.Word)
                        {
                            _wordService.SaveHtml(html);
                        }
                        else if (!string.IsNullOrWhiteSpace(_currentFilePath) &&
                                 Path.GetExtension(_currentFilePath).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                        {
                            ExcelHtmlConverter.SaveBodyHtmlAsXlsx(html, _currentFilePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al guardar: {ex.Message}",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                        return;
                    }
                }
            }

            _wordService.UnloadDocument();
            _documentViewer.Clear();
            base.OnFormClosing(e);
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            drgevent.Effect = drgevent.Data?.GetDataPresent(DataFormats.FileDrop) == true
                ? DragDropEffects.Copy
                : DragDropEffects.None;
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            var files = drgevent.Data?.GetData(DataFormats.FileDrop) as string[];
            if (files?.Length > 0)
                _ = OpenFileAsync(files[0]);
        }
    }
}
