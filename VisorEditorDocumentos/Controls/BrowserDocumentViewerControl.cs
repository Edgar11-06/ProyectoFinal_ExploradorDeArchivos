using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using VisorEditorDocumentos.Services;

namespace VisorEditorDocumentos.Controls
{
    /// <summary>
    /// Visor PDF y editor Word (HTML) con WebView2.
    /// </summary>
    public class BrowserDocumentViewerControl : UserControl
    {
        private const string LocalPdfHost = "visor.local";

        private readonly WebView2 _webView = new();
        private readonly SemaphoreSlim _initLock = new(1, 1);

        private bool _initialized;
        private string? _pendingPath;
        private string? _pendingHtml;
        private string? _mappedFolder;

        public BrowserDocumentViewerControl()
        {
            Dock = DockStyle.Fill;
            BackColor = System.Drawing.Color.FromArgb(55, 55, 55);

            _webView.Dock = DockStyle.Fill;
            _webView.DefaultBackgroundColor = System.Drawing.Color.FromArgb(55, 55, 55);
            Controls.Add(_webView);
        }

        public bool IsReady => _initialized && _webView.CoreWebView2 != null;

        public event EventHandler? ViewerReady;
        public event EventHandler? ContentModified;

        /// <summary>Inicializa WebView2 (debe llamarse cuando el control ya tiene handle).</summary>
        public async Task EnsureInitializedAsync()
        {
            if (_initialized) return;

            await _initLock.WaitAsync();
            try
            {
                if (_initialized) return;

                if (!IsHandleCreated)
                    CreateControl();

                await _webView.EnsureCoreWebView2Async();

                var core = _webView.CoreWebView2!;
                var settings = core.Settings;
                settings.AreDefaultContextMenusEnabled = true;
                settings.AreDevToolsEnabled = false;
                settings.IsStatusBarEnabled = false;
                settings.IsZoomControlEnabled = true;

                core.WebMessageReceived += OnWebMessageReceived;

                _initialized = true;
                ViewerReady?.Invoke(this, EventArgs.Empty);

                if (!string.IsNullOrEmpty(_pendingPath))
                {
                    var path = _pendingPath;
                    _pendingPath = null;
                    await NavigateToFileAsync(path);
                }
                else if (!string.IsNullOrEmpty(_pendingHtml))
                {
                    var html = _pendingHtml;
                    _pendingHtml = null;
                    NavigateToString(html);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "No se pudo inicializar WebView2. Instale el runtime de Microsoft Edge WebView2.",
                    ex);
            }
            finally
            {
                _initLock.Release();
            }
        }

        private void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            if (e.TryGetWebMessageAsString() == "modified")
                ContentModified?.Invoke(this, EventArgs.Empty);
        }

        public async Task NavigateToFileAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                throw new FileNotFoundException("No se encontró el archivo.", filePath);

            Visible = true;
            _webView.Visible = true;

            if (!_initialized)
            {
                _pendingPath = filePath;
                _pendingHtml = null;
                await EnsureInitializedAsync();
                return;
            }

            var fullPath = Path.GetFullPath(filePath);
            var ext = Path.GetExtension(fullPath).ToLowerInvariant();

            if (ext == ".pdf")
                NavigateToLocalPdf(fullPath);
            else
                _webView.CoreWebView2!.Navigate(new Uri(fullPath).AbsoluteUri);
        }

        /// <summary>
        /// WebView2 bloquea muchos file://; mapeamos la carpeta del PDF a un host virtual HTTPS.
        /// </summary>
        private void NavigateToLocalPdf(string fullPath)
        {
            var core = _webView.CoreWebView2!;
            var folder = Path.GetDirectoryName(fullPath)!;
            var fileName = Path.GetFileName(fullPath);

            if (!string.Equals(_mappedFolder, folder, StringComparison.OrdinalIgnoreCase))
            {
                try { core.ClearVirtualHostNameToFolderMapping(LocalPdfHost); } catch { /* primera vez */ }

                core.SetVirtualHostNameToFolderMapping(
                    LocalPdfHost,
                    folder,
                    CoreWebView2HostResourceAccessKind.Deny);

                _mappedFolder = folder;
            }

            var encodedName = Uri.EscapeDataString(fileName);
            core.Navigate($"https://{LocalPdfHost}/{encodedName}");
        }

        public async Task LoadDocxEditorAsync(string bodyHtml, bool editable)
        {
            var fullHtml = DocxHtmlConverter.WrapForEditor(bodyHtml, editable);
            await LoadEditorHtmlAsync(fullHtml);
        }

        private async Task LoadEditorHtmlAsync(string fullHtml)
        {
            Visible = true;
            _webView.Visible = true;

            if (!_initialized)
            {
                _pendingHtml = fullHtml;
                _pendingPath = null;
                await EnsureInitializedAsync();
                return;
            }

            NavigateToString(fullHtml);
        }

        private void NavigateToString(string html)
        {
            _webView.CoreWebView2!.NavigateToString(html);
        }

        public async Task SetEditableAsync(bool editable)
        {
            if (_webView.CoreWebView2 == null) return;
            var flag = editable ? "true" : "false";
            await _webView.CoreWebView2.ExecuteScriptAsync(
                $"var r=document.getElementById('doc-root');if(r)r.contentEditable={flag};");
        }

        public async Task<string> GetBodyHtmlAsync()
        {
            if (_webView.CoreWebView2 == null)
                return string.Empty;

            var json = await _webView.CoreWebView2.ExecuteScriptAsync(
                "(function(){var r=document.getElementById('doc-root');return r?r.innerHTML:'';})()");

            return JsonSerializer.Deserialize<string>(json) ?? string.Empty;
        }

        public void Clear()
        {
            _pendingPath = null;
            _pendingHtml = null;
            if (_webView.CoreWebView2 != null)
                _webView.CoreWebView2.Navigate("about:blank");
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            _ = EnsureInitializedAsync();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _webView.CoreWebView2 != null)
                _webView.CoreWebView2.WebMessageReceived -= OnWebMessageReceived;
            if (disposing)
            {
                _initLock.Dispose();
                _webView?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
