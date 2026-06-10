using System;
using System.Drawing;
using System.Windows.Forms;
using VisorEditorDocumentos.Services;

namespace VisorEditorDocumentos.Controls
{
    /// <summary>
    /// Control personalizado que muestra las páginas de un PDF renderizadas
    /// por <see cref="PdfService"/>. Soporta scroll y redibujado automático.
    /// </summary>
    public class PdfViewerControl : ScrollableControl
    {
        // -----------------------------------------------------------------------
        // Campos privados
        // -----------------------------------------------------------------------

        private Image?   _currentPageImage;
        private PdfService? _pdfService;

        // -----------------------------------------------------------------------
        // Constructor
        // -----------------------------------------------------------------------

        public PdfViewerControl()
        {
            // Activar doble buffer para evitar parpadeo
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint           |
                ControlStyles.DoubleBuffer        |
                ControlStyles.ResizeRedraw,
                true
            );

            BackColor = Color.FromArgb(55, 55, 55); // Fondo gris oscuro estilo visor
            AutoScroll = true;
        }

        // -----------------------------------------------------------------------
        // API pública
        // -----------------------------------------------------------------------

        /// <summary>
        /// Asocia este control con un <see cref="PdfService"/> y muestra
        /// la página actual del documento.
        /// </summary>
        public void SetPdfService(PdfService service)
        {
            _pdfService = service;
            RefreshPage();
        }

        /// <summary>
        /// Vuelve a renderizar la página actual (útil después de cambiar
        /// de página o de zoom en el servicio).
        /// </summary>
        public void RefreshPage()
        {
            if (_pdfService == null || !_pdfService.IsLoaded)
            {
                ClearPage();
                return;
            }

            try
            {
                // Liberar imagen anterior
                _currentPageImage?.Dispose();
                _currentPageImage = null;

                // Renderizar en alta resolución para el zoom actual
                _currentPageImage = _pdfService.RenderCurrentPage(ClientSize.Width, ClientSize.Height);

                // Ajustar tamaño virtual del control para el scroll
                if (_currentPageImage != null)
                {
                    AutoScrollMinSize = _currentPageImage.Size;
                }

                Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al renderizar la página PDF:\n{ex.Message}",
                    "Error de renderizado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        /// <summary>Limpia el visor (sin documento cargado).</summary>
        public void ClearPage()
        {
            _currentPageImage?.Dispose();
            _currentPageImage = null;
            _pdfService = null;
            AutoScrollMinSize = Size.Empty;
            Invalidate();
        }

        // -----------------------------------------------------------------------
        // Pintura
        // -----------------------------------------------------------------------

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;

            if (_currentPageImage == null)
            {
                // Estado vacío: mostrar mensaje
                var msg = "Abra un archivo PDF para visualizarlo";
                using var font = new Font("Segoe UI", 12f, FontStyle.Regular);
                using var brush = new SolidBrush(Color.FromArgb(180, 180, 180));
                var size = g.MeasureString(msg, font);
                var x = (Width  - size.Width)  / 2f;
                var y = (Height - size.Height) / 2f;
                g.DrawString(msg, font, brush, x, y);
                return;
            }

            // Calcular posición centrada con scroll
            var imgW = _currentPageImage.Width;
            var imgH = _currentPageImage.Height;

            // Área visible teniendo en cuenta el scroll
            var scrollX = AutoScrollPosition.X;
            var scrollY = AutoScrollPosition.Y;

            // Centrar si la imagen es más pequeña que el control
            int drawX = scrollX + Math.Max(0, (ClientSize.Width  - imgW) / 2);
            int drawY = scrollY + Math.Max(0, (ClientSize.Height - imgH) / 2);

            // Sombra de la página
            using var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0));
            g.FillRectangle(shadowBrush,
                new Rectangle(drawX + 4, drawY + 4, imgW, imgH));

            // Dibujar la página
            g.DrawImage(_currentPageImage,
                new Rectangle(drawX, drawY, imgW, imgH));
        }

        // -----------------------------------------------------------------------
        // Limpieza
        // -----------------------------------------------------------------------

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _currentPageImage?.Dispose();
                _currentPageImage = null;
            }
            base.Dispose(disposing);
        }
    }
}
