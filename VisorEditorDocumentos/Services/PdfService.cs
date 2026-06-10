using System;
using System.Drawing;
using System.IO;
using PdfiumViewer;

namespace VisorEditorDocumentos.Services
{
    /// <summary>
    /// Servicio encargado de gestionar documentos PDF.
    /// Utiliza PdfiumViewer para renderizar las páginas.
    /// </summary>
    public class PdfService : IDisposable
    {
        // -----------------------------------------------------------------------
        // Campos privados
        // -----------------------------------------------------------------------

        private PdfDocument? _pdfDocument;
        private bool _disposed = false;

        // -----------------------------------------------------------------------
        // Propiedades públicas
        // -----------------------------------------------------------------------

        /// <summary>Ruta del archivo PDF actualmente cargado.</summary>
        public string? CurrentFilePath { get; private set; }

        /// <summary>Indica si hay un documento PDF cargado.</summary>
        public bool IsLoaded => _pdfDocument != null;

        /// <summary>Número total de páginas del documento.</summary>
        public int PageCount => _pdfDocument?.PageCount ?? 0;

        /// <summary>Índice (base 0) de la página actual.</summary>
        public int CurrentPage { get; private set; } = 0;

        /// <summary>Nivel de zoom actual (1.0 = 100%).</summary>
        public float Zoom { get; private set; } = 1.0f;

        // -----------------------------------------------------------------------
        // Zoom mínimo y máximo permitidos
        // -----------------------------------------------------------------------

        public const float MinZoom = 0.25f;
        public const float MaxZoom = 5.0f;

        // -----------------------------------------------------------------------
        // Carga y descarga del documento
        // -----------------------------------------------------------------------

        /// <summary>
        /// Carga un archivo PDF desde disco.
        /// </summary>
        /// <param name="filePath">Ruta completa del archivo .pdf.</param>
        public void LoadDocument(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("El archivo PDF no existe.", filePath);

            if (Path.GetExtension(filePath).ToLower() != ".pdf")
                throw new InvalidOperationException("El archivo no es un PDF.");

            // Liberar documento anterior
            _pdfDocument?.Dispose();
            _pdfDocument = null;

            _pdfDocument = PdfDocument.Load(filePath);
            CurrentFilePath = filePath;
            CurrentPage = 0;
            Zoom = 1.0f;
        }

        /// <summary>
        /// Libera el documento PDF actual sin cerrar el servicio.
        /// </summary>
        public void UnloadDocument()
        {
            _pdfDocument?.Dispose();
            _pdfDocument = null;
            CurrentFilePath = null;
            CurrentPage = 0;
            Zoom = 1.0f;
        }

        // -----------------------------------------------------------------------
        // Renderizado de páginas
        // -----------------------------------------------------------------------

        /// <summary>
        /// Renderiza la página actual a un objeto <see cref="Image"/> con el
        /// zoom aplicado. El llamador es responsable de eliminar la imagen cuando
        /// ya no la necesite.
        /// </summary>
        /// <param name="targetWidth">Ancho del área de visualización en píxeles.</param>
        /// <param name="targetHeight">Alto del área de visualización en píxeles.</param>
        /// <returns>Imagen renderizada de la página.</returns>
        public Image RenderCurrentPage(int targetWidth, int targetHeight)
        {
            EnsureLoaded();

            // Dimensiones de la página en puntos (1 punto = 1/72 pulgada)
            var pageSize = _pdfDocument!.PageSizes[CurrentPage];

            // Calcular resolución en DPI aplicando zoom
            // Resolución base: 96 DPI (estándar de pantalla)
            const float baseDpi = 96f;
            int dpi = (int)(baseDpi * Zoom);
            dpi = Math.Max(dpi, 24);   // mínimo para no obtener imagen nula
            dpi = Math.Min(dpi, 1200); // máximo razonable

            // Dimensiones en píxeles a esa resolución
            int width  = (int)(pageSize.Width  / 72f * dpi);
            int height = (int)(pageSize.Height / 72f * dpi);

            if (width < 1)  width  = 1;
            if (height < 1) height = 1;

            return _pdfDocument.Render(CurrentPage, width, height, dpi, dpi, false);
        }

        // -----------------------------------------------------------------------
        // Navegación entre páginas
        // -----------------------------------------------------------------------

        /// <summary>Navega a la primera página.</summary>
        public void GoToFirstPage()
        {
            EnsureLoaded();
            CurrentPage = 0;
        }

        /// <summary>Navega a la última página.</summary>
        public void GoToLastPage()
        {
            EnsureLoaded();
            CurrentPage = PageCount - 1;
        }

        /// <summary>Avanza a la siguiente página si existe.</summary>
        /// <returns>True si avanzó, false si ya estaba en la última.</returns>
        public bool NextPage()
        {
            EnsureLoaded();
            if (CurrentPage < PageCount - 1)
            {
                CurrentPage++;
                return true;
            }
            return false;
        }

        /// <summary>Retrocede a la página anterior si existe.</summary>
        /// <returns>True si retrocedió, false si ya estaba en la primera.</returns>
        public bool PreviousPage()
        {
            EnsureLoaded();
            if (CurrentPage > 0)
            {
                CurrentPage--;
                return true;
            }
            return false;
        }

        /// <summary>Navega a una página específica (base 0).</summary>
        public void GoToPage(int pageIndex)
        {
            EnsureLoaded();
            if (pageIndex < 0 || pageIndex >= PageCount)
                throw new ArgumentOutOfRangeException(nameof(pageIndex), "Índice de página fuera de rango.");
            CurrentPage = pageIndex;
        }

        // -----------------------------------------------------------------------
        // Control de zoom
        // -----------------------------------------------------------------------

        /// <summary>Aumenta el zoom un 25%.</summary>
        public void ZoomIn()
        {
            float newZoom = Zoom + 0.25f;
            Zoom = Math.Min(newZoom, MaxZoom);
        }

        /// <summary>Reduce el zoom un 25%.</summary>
        public void ZoomOut()
        {
            float newZoom = Zoom - 0.25f;
            Zoom = Math.Max(newZoom, MinZoom);
        }

        /// <summary>Establece un nivel de zoom específico.</summary>
        public void SetZoom(float zoom)
        {
            if (zoom < MinZoom || zoom > MaxZoom)
                throw new ArgumentOutOfRangeException(nameof(zoom),
                    $"El zoom debe estar entre {MinZoom} y {MaxZoom}.");
            Zoom = zoom;
        }

        /// <summary>Devuelve el zoom actual como texto legible (ej. "100%").</summary>
        public string ZoomText => $"{(int)(Zoom * 100)}%";

        // -----------------------------------------------------------------------
        // Validación
        // -----------------------------------------------------------------------

        /// <summary>Valida que un archivo sea un PDF legible.</summary>
        public static bool IsValidPdf(string filePath)
        {
            try
            {
                if (!File.Exists(filePath)) return false;
                if (Path.GetExtension(filePath).ToLower() != ".pdf") return false;
                using var doc = PdfDocument.Load(filePath);
                return doc.PageCount > 0;
            }
            catch
            {
                return false;
            }
        }

        // -----------------------------------------------------------------------
        // IDisposable
        // -----------------------------------------------------------------------

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _pdfDocument?.Dispose();
                    _pdfDocument = null;
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // -----------------------------------------------------------------------
        // Helpers privados
        // -----------------------------------------------------------------------

        private void EnsureLoaded()
        {
            if (_pdfDocument == null)
                throw new InvalidOperationException("No hay ningún documento PDF cargado.");
        }
    }
}
