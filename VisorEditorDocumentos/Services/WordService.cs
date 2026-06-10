using System;
using System.IO;

namespace VisorEditorDocumentos.Services
{
    /// <summary>
    /// Gestión de documentos .docx sin Microsoft Word: conversión HTML ↔ DOCX.
    /// </summary>
    public class WordService
    {
        public string? CurrentFilePath { get; private set; }
        public bool IsLoaded => !string.IsNullOrEmpty(CurrentFilePath);

        /// <summary>Carga el .docx y devuelve el HTML del cuerpo para el editor.</summary>
        public string LoadAsHtml(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("El archivo no existe.", filePath);

            if (!Path.GetExtension(filePath).Equals(".docx", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("El archivo no es un documento Word (.docx).");

            CurrentFilePath = filePath;
            return DocxHtmlConverter.ToBodyHtml(filePath);
        }

        public void SaveHtml(string bodyHtml)
        {
            if (string.IsNullOrEmpty(CurrentFilePath))
                throw new InvalidOperationException("No hay ningún documento Word cargado.");

            SaveHtmlAs(bodyHtml, CurrentFilePath, updateCurrentFilePath: false);
        }

        public void SaveHtmlAs(string bodyHtml, string newFilePath, bool updateCurrentFilePath = true)
        {
            if (string.IsNullOrEmpty(CurrentFilePath))
                throw new InvalidOperationException("No hay ningún documento Word cargado como base.");

            DocxHtmlConverter.SaveBodyHtml(bodyHtml, newFilePath, CurrentFilePath);

            if (updateCurrentFilePath)
                CurrentFilePath = newFilePath;
        }

        public static bool IsValidDocx(string filePath)
        {
            try
            {
                if (!File.Exists(filePath)) return false;
                if (!Path.GetExtension(filePath).Equals(".docx", StringComparison.OrdinalIgnoreCase))
                    return false;

                using var doc = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(filePath, false);
                return doc.MainDocumentPart?.Document?.Body != null;
            }
            catch
            {
                return false;
            }
        }

        public void UnloadDocument() => CurrentFilePath = null;
    }
}
