using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HtmlAgilityPack;

namespace VisorEditorDocumentos.Services
{
    public static class DocumentExportService
    {
        public static void ExportHtmlAsDocx(string bodyHtml, string outputPath)
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);

            var tempTemplate = outputPath + ".template.docx";
            try
            {
                using (var template = WordprocessingDocument.Create(tempTemplate, WordprocessingDocumentType.Document))
                {
                    var templateMain = template.AddMainDocumentPart();
                    templateMain.Document = new Document(new Body(new Paragraph(new Run(new Text("")))));
                    templateMain.Document.Save();
                }

                File.Copy(tempTemplate, outputPath, overwrite: true);
                DocxHtmlConverter.SaveBodyHtml(bodyHtml, outputPath, outputPath);
            }
            finally
            {
                try { if (File.Exists(tempTemplate)) File.Delete(tempTemplate); } catch { }
            }
        }

        public static void ExportHtmlAsXlsx(string bodyHtml, string outputPath)
        {
            ExcelHtmlConverter.SaveBodyHtmlAsXlsx(bodyHtml, outputPath);
        }

        public static void ExportHtmlAsJson(string bodyHtml, string outputPath)
        {
            var rows = ExtractTableRows(bodyHtml).ToList();
            var data = RowsToDictionaries(rows);
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(outputPath, JsonSerializer.Serialize(data, options), Encoding.UTF8);
        }

        public static void ExportHtmlAsTxt(string bodyHtml, string outputPath)
        {
            File.WriteAllText(outputPath, HtmlToPlainText(bodyHtml), Encoding.UTF8);
        }

        public static void ExportHtmlAsCsv(string bodyHtml, string outputPath)
        {
            var rows = ExtractTableRows(bodyHtml).ToList();
            var csv = new StringBuilder();
            foreach (var row in rows)
                csv.AppendLine(string.Join(",", row.Select(EscapeCsv)));
            File.WriteAllText(outputPath, csv.ToString(), Encoding.UTF8);
        }

        public static void ExportHtmlAsXml(string bodyHtml, string outputPath)
        {
            var rows = RowsToDictionaries(ExtractTableRows(bodyHtml).ToList());
            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<rows>");
            foreach (var row in rows)
            {
                sb.AppendLine("  <row>");
                foreach (var kv in row)
                    sb.Append("    <").Append(XmlName(kv.Key)).Append(">")
                        .Append(WebUtility.HtmlEncode(kv.Value ?? string.Empty))
                        .Append("</").Append(XmlName(kv.Key)).AppendLine(">");
                sb.AppendLine("  </row>");
            }
            sb.AppendLine("</rows>");
            File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);
        }

        public static void ExportHtmlAsPdf(string bodyHtml, string outputPath, string title)
        {
            var lines = HtmlToPlainLines(bodyHtml).ToList();
            SimplePdfWriter.WriteTextPdf(outputPath, title, lines);
        }

        public static string PdfToBodyHtml(string pdfPath)
        {
            var title = WebUtility.HtmlEncode(Path.GetFileName(pdfPath));
            return "<h1>Documento PDF</h1>" +
                   $"<p><strong>Archivo:</strong> {title}</p>" +
                   "<p>El PDF original se visualiza en el visor integrado. Al exportarlo a Word o Excel se conserva esta referencia; para conservar el contenido visual completo use exportar a PDF.</p>";
        }

        public static string HtmlToPlainText(string bodyHtml)
        {
            return string.Join(Environment.NewLine, HtmlToPlainLines(bodyHtml));
        }

        public static IEnumerable<List<string>> ExtractTableRows(string bodyHtml)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml($"<div id=\"wrap\">{bodyHtml}</div>");
            var tableRows = doc.DocumentNode.SelectNodes("//tr");
            if (tableRows != null)
            {
                foreach (var tr in tableRows)
                {
                    var cells = tr.SelectNodes("./th|./td");
                    if (cells == null) continue;
                    yield return cells.Select(c => HtmlEntity.DeEntitize(c.InnerText).Trim()).ToList();
                }
                yield break;
            }

            foreach (var line in HtmlToPlainLines(bodyHtml))
                yield return new List<string> { line };
        }

        private static IEnumerable<string> HtmlToPlainLines(string bodyHtml)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml($"<div id=\"wrap\">{bodyHtml}</div>");

            var rows = doc.DocumentNode.SelectNodes("//tr");
            if (rows != null)
            {
                foreach (var row in rows)
                {
                    var cells = row.SelectNodes("./th|./td");
                    if (cells == null) continue;
                    var line = string.Join("    ", cells.Select(c => HtmlEntity.DeEntitize(c.InnerText).Trim()));
                    if (!string.IsNullOrWhiteSpace(line)) yield return line;
                }
                yield break;
            }

            foreach (var node in doc.DocumentNode.SelectNodes("//h1|//h2|//h3|//p|//li|//div") ?? Enumerable.Empty<HtmlNode>())
            {
                var text = HtmlEntity.DeEntitize(node.InnerText).Trim();
                if (!string.IsNullOrWhiteSpace(text))
                    yield return text;
            }
        }

        private static List<Dictionary<string, string>> RowsToDictionaries(IReadOnlyList<List<string>> rows)
        {
            var result = new List<Dictionary<string, string>>();
            if (rows.Count == 0) return result;

            var headers = rows[0].Select((h, i) => string.IsNullOrWhiteSpace(h) ? $"Col{i + 1}" : h.Trim()).ToList();
            foreach (var row in rows.Skip(1))
            {
                var item = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                for (var i = 0; i < headers.Count; i++)
                    item[headers[i]] = i < row.Count ? row[i] : string.Empty;
                result.Add(item);
            }

            if (result.Count == 0)
                result.Add(headers.Select((h, i) => new { h, i }).ToDictionary(x => x.h, x => string.Empty));

            return result;
        }

        private static string EscapeCsv(string value)
        {
            value ??= string.Empty;
            return value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r')
                ? "\"" + value.Replace("\"", "\"\"") + "\""
                : value;
        }

        private static string XmlName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "Campo";
            var cleaned = new string(name.Trim().Select(ch => char.IsLetterOrDigit(ch) || ch == '_' ? ch : '_').ToArray());
            if (cleaned.Length == 0) return "Campo";
            if (char.IsDigit(cleaned[0])) cleaned = "_" + cleaned;
            return cleaned;
        }
    }

    internal static class SimplePdfWriter
    {
        public static void WriteTextPdf(string outputPath, string title, IReadOnlyList<string> lines)
        {
            var objects = new List<string>();
            objects.Add("<< /Type /Catalog /Pages 2 0 R >>");
            objects.Add("<< /Type /Pages /Kids [3 0 R] /Count 1 >>");

            var content = BuildContent(title, lines);
            objects.Add("<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 4 0 R >> >> /Contents 5 0 R >>");
            objects.Add("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>");
            objects.Add($"<< /Length {Encoding.ASCII.GetByteCount(content)} >>\nstream\n{content}\nendstream");

            using var stream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
            using var writer = new StreamWriter(stream, Encoding.ASCII, leaveOpen: true) { NewLine = "\n" };
            writer.Write("%PDF-1.4\n");

            var offsets = new List<long> { 0 };
            for (var i = 0; i < objects.Count; i++)
            {
                writer.Flush();
                offsets.Add(stream.Position);
                writer.Write(string.Format(CultureInfo.InvariantCulture, "{0} 0 obj\n{1}\nendobj\n", i + 1, objects[i]));
            }

            writer.Flush();
            var xref = stream.Position;
            writer.Write(string.Format(CultureInfo.InvariantCulture, "xref\n0 {0}\n", objects.Count + 1));
            writer.Write("0000000000 65535 f \n");
            foreach (var offset in offsets.Skip(1))
                writer.Write(string.Format(CultureInfo.InvariantCulture, "{0:0000000000} 00000 n \n", offset));
            writer.Write(string.Format(CultureInfo.InvariantCulture,
                "trailer\n<< /Size {0} /Root 1 0 R >>\nstartxref\n{1}\n%%EOF",
                objects.Count + 1,
                xref));
        }

        private static string BuildContent(string title, IReadOnlyList<string> lines)
        {
            var sb = new StringBuilder();
            sb.Append("BT\n/F1 16 Tf\n50 750 Td\n(").Append(EscapePdf(title)).Append(") Tj\n");
            sb.Append("/F1 10 Tf\n0 -24 Td\n");

            var y = 0;
            foreach (var original in lines.DefaultIfEmpty(string.Empty))
            {
                foreach (var line in WrapLine(original ?? string.Empty, 95))
                {
                    if (y >= 60) break;
                    sb.Append('(').Append(EscapePdf(line)).Append(") Tj\n0 -13 Td\n");
                    y++;
                }
                if (y >= 60) break;
            }

            sb.Append("ET");
            return sb.ToString();
        }

        private static IEnumerable<string> WrapLine(string text, int width)
        {
            text = text.Replace("\r", " ").Replace("\n", " ").Trim();
            if (text.Length == 0) yield return string.Empty;
            while (text.Length > width)
            {
                var cut = text.LastIndexOf(' ', Math.Min(width, text.Length - 1));
                if (cut <= 0) cut = width;
                yield return text[..cut];
                text = text[cut..].TrimStart();
            }
            if (text.Length > 0) yield return text;
        }

        private static string EscapePdf(string text)
        {
            var normalized = RemoveDiacritics(text);
            return normalized.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
        }

        private static string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var ch in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (category != UnicodeCategory.NonSpacingMark && ch <= 255)
                    sb.Append(ch);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}

