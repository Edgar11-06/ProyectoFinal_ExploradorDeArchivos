using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HtmlAgilityPack;
using A = DocumentFormat.OpenXml.Drawing;

namespace VisorEditorDocumentos.Services
{
    /// <summary>
    /// Convierte .docx ↔ HTML para edición independiente (Open XML + editor WebView2).
    /// </summary>
    public static class DocxHtmlConverter
    {
        private const string EditorStyles = """
            body { margin: 0; background: #525252; font-family: 'Segoe UI', Calibri, sans-serif; }
            .page {
                max-width: 816px; margin: 24px auto; padding: 72px 56px;
                background: #fff; min-height: 900px;
                box-shadow: 0 2px 12px rgba(0,0,0,.35);
            }
            #doc-root { outline: none; font-size: 11pt; line-height: 1.5; color: #222; }
            #doc-root h1 { font-size: 16pt; font-weight: bold; margin: 14pt 0 6pt; }
            #doc-root h2 { font-size: 14pt; font-weight: bold; margin: 12pt 0 6pt; }
            #doc-root h3 { font-size: 12pt; font-weight: bold; margin: 10pt 0 4pt; }
            #doc-root p { margin: 0 0 8pt; }
            #doc-root table { border-collapse: collapse; width: 100%; margin: 8pt 0; }
            #doc-root td, #doc-root th { border: 1px solid #bbb; padding: 4pt 6pt; vertical-align: top; }
            #doc-root th { background: #f0f0f0; font-weight: bold; }
            #doc-root .sheet-wrap { overflow: auto; max-width: 100%; border: 1px solid #c9d3df; margin: 8pt 0; background: #fff; }
            #doc-root .excel-grid { width: max-content; min-width: 100%; table-layout: fixed; margin: 0; font-size: 10pt; }
            #doc-root .excel-grid th {
                position: sticky; top: 0; z-index: 1;
                background: #eaf2ff; color: #1f2937; border-color: #9fb7d7;
            }
            #doc-root .excel-grid td { min-width: 120px; max-width: 260px; white-space: pre-wrap; border-color: #d5dde8; }
            #doc-root .excel-grid tr:nth-child(even) td { background: #fafcff; }
            #doc-root .excel-grid td:focus, #doc-root .excel-grid th:focus { outline: 2px solid #2563eb; outline-offset: -2px; }
            #doc-root ul, #doc-root ol { margin: 0 0 8pt 24pt; }
            #doc-root img { max-width: 100%; height: auto; }
            """;

        public static string WrapForEditor(string bodyHtml, bool editable)
        {
            var editAttr = editable ? "true" : "false";
            var script = """
                <script>
                document.getElementById('doc-root').addEventListener('input', function() {
                  if (window.chrome && window.chrome.webview)
                    window.chrome.webview.postMessage('modified');
                });
                </script>
                """;

            return "<!DOCTYPE html><html lang=\"es\"><head><meta charset=\"utf-8\"/>" +
                   "<style>" + EditorStyles + "</style></head><body>" +
                   "<div class=\"page\"><div id=\"doc-root\" contenteditable=\"" + editAttr + "\">" +
                   bodyHtml + "</div></div>" + script + "</body></html>";
        }

        public static string ToBodyHtml(string docxPath)
        {
            var sb = new StringBuilder();
            using var doc = WordprocessingDocument.Open(docxPath, false);
            var mainPart = doc.MainDocumentPart;
            var body = mainPart?.Document?.Body;
            if (body == null) return "<p></p>";

            foreach (var element in body.Elements())
            {
                if (element is Paragraph para)
                    sb.Append(ParagraphToHtml(para, mainPart!));
                else if (element is Table table)
                    sb.Append(TableToHtml(table, mainPart!));
            }

            var html = sb.ToString();
            return string.IsNullOrWhiteSpace(html) ? "<p></p>" : html;
        }

        public static void SaveBodyHtml(string bodyHtml, string docxPath, string templateDocxPath)
        {
            if (!string.Equals(templateDocxPath, docxPath, StringComparison.OrdinalIgnoreCase))
                File.Copy(templateDocxPath, docxPath, overwrite: true);

            using var wordDoc = WordprocessingDocument.Open(docxPath, true);
            var mainPart = wordDoc.MainDocumentPart!;
            var body = mainPart.Document!.Body!;
            body.RemoveAllChildren();

            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml($"<div id=\"wrap\">{bodyHtml}</div>");
            var wrap = htmlDoc.GetElementbyId("wrap");
            if (wrap == null) return;

            foreach (var child in wrap.ChildNodes.Where(n => n.NodeType == HtmlNodeType.Element))
            {
                var ox = HtmlNodeToOpenXml(child);
                if (ox != null)
                    body.AppendChild(ox);
            }

            mainPart.Document.Save();
        }

        private static string ParagraphToHtml(Paragraph para, MainDocumentPart mainPart)
        {
            var style = para.ParagraphProperties?.ParagraphStyleId?.Val?.Value ?? "";
            var tag = style switch
            {
                "Heading1" or "heading 1" or "Título1" => "h1",
                "Heading2" or "heading 2" or "Título2" => "h2",
                "Heading3" or "heading 3" or "Título3" => "h3",
                _ => "p"
            };

            var inner = BuildRunHtml(para, mainPart);
            if (string.IsNullOrEmpty(inner))
                inner = "<br/>";

            return $"<{tag}>{inner}</{tag}>";
        }

        private static string BuildRunHtml(OpenXmlElement container, MainDocumentPart mainPart)
        {
            var sb = new StringBuilder();

            foreach (var run in container.Descendants<Run>())
            {
                var img = TryGetImageHtml(run, mainPart);
                if (!string.IsNullOrEmpty(img))
                {
                    sb.Append(img);
                    continue;
                }

                var text = string.Concat(run.Descendants<Text>().Select(t => t.Text));
                if (string.IsNullOrEmpty(text))
                {
                    if (run.Descendants<Break>().Any())
                        sb.Append("<br/>");
                    continue;
                }

                var encoded = WebUtility.HtmlEncode(text);
                var props = run.RunProperties;
                if (props?.Bold != null) encoded = $"<strong>{encoded}</strong>";
                if (props?.Italic != null) encoded = $"<em>{encoded}</em>";
                if (props?.Underline != null) encoded = $"<u>{encoded}</u>";
                sb.Append(encoded);
            }

            return sb.ToString();
        }

        private static string? TryGetImageHtml(Run run, MainDocumentPart mainPart)
        {
            var embed = run.Descendants<A.Blip>().FirstOrDefault()?.Embed?.Value;
            if (embed == null) return null;

            try
            {
                var part = (ImagePart)mainPart.GetPartById(embed);
                using var stream = part.GetStream();
                using var ms = new MemoryStream();
                stream.CopyTo(ms);
                var b64 = Convert.ToBase64String(ms.ToArray());
                return $"<img src=\"data:{part.ContentType};base64,{b64}\" alt=\"\"/>";
            }
            catch
            {
                return null;
            }
        }

        private static string TableToHtml(Table table, MainDocumentPart mainPart)
        {
            var sb = new StringBuilder("<table>");
            var rows = table.Elements<TableRow>().ToList();
            for (var i = 0; i < rows.Count; i++)
            {
                sb.Append("<tr>");
                foreach (var cell in rows[i].Elements<TableCell>())
                {
                    var tag = i == 0 ? "th" : "td";
                    sb.Append($"<{tag}>");
                    foreach (var para in cell.Elements<Paragraph>())
                        sb.Append(ParagraphToHtml(para, mainPart));
                    sb.Append($"</{tag}>");
                }
                sb.Append("</tr>");
            }
            sb.Append("</table>");
            return sb.ToString();
        }

        private static OpenXmlElement? HtmlNodeToOpenXml(HtmlNode node)
        {
            switch (node.Name.ToLowerInvariant())
            {
                case "h1": return HeadingParagraph(node, "Heading1");
                case "h2": return HeadingParagraph(node, "Heading2");
                case "h3": return HeadingParagraph(node, "Heading3");
                case "p":
                case "div":
                case "li":
                    return TextParagraph(node);
                case "table":
                    return HtmlTableToOpenXml(node);
                case "br":
                    return new Paragraph(new Run(new Break()));
                case "ul":
                case "ol":
                    var first = node.ChildNodes.FirstOrDefault(n => n.Name.Equals("li", StringComparison.OrdinalIgnoreCase));
                    return first != null ? TextParagraph(first) : null;
                default:
                    if (!string.IsNullOrWhiteSpace(node.InnerText))
                        return TextParagraph(node);
                    return null;
            }
        }

        private static Paragraph HeadingParagraph(HtmlNode node, string styleId)
        {
            var para = new Paragraph();
            para.AppendChild(new ParagraphProperties(new ParagraphStyleId { Val = styleId }));
            AppendTextRuns(para, node);
            return para;
        }

        private static Paragraph TextParagraph(HtmlNode node)
        {
            var para = new Paragraph();
            AppendTextRuns(para, node);
            if (!para.Descendants<Text>().Any())
                para.AppendChild(new Run(new Text("") { Space = SpaceProcessingModeValues.Preserve }));
            return para;
        }

        private static void AppendTextRuns(Paragraph para, HtmlNode node)
        {
            if (node.NodeType == HtmlNodeType.Text)
            {
                var t = HtmlEntity.DeEntitize(node.InnerText);
                if (!string.IsNullOrEmpty(t))
                    para.AppendChild(MakeRun(t, node));
                return;
            }

            foreach (var child in node.ChildNodes)
            {
                if (child.NodeType == HtmlNodeType.Text)
                {
                    var t = HtmlEntity.DeEntitize(child.InnerText);
                    if (!string.IsNullOrEmpty(t))
                        para.AppendChild(MakeRun(t, node));
                }
                else if (child.Name.Equals("br", StringComparison.OrdinalIgnoreCase))
                {
                    para.AppendChild(new Run(new Break()));
                }
                else if (child.Name.Equals("img", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                else if (child.ChildNodes.Count > 0)
                {
                    AppendTextRuns(para, child);
                }
                else
                {
                    var t = HtmlEntity.DeEntitize(child.InnerText);
                    if (!string.IsNullOrEmpty(t))
                        para.AppendChild(MakeRun(t, child));
                }
            }
        }

        private static Run MakeRun(string text, HtmlNode styleNode)
        {
            var run = new Run();
            var props = new RunProperties();
            var tag = styleNode.Name.ToLowerInvariant();
            if (tag is "strong" or "b") props.AppendChild(new Bold());
            if (tag is "em" or "i") props.AppendChild(new Italic());
            if (tag is "u") props.AppendChild(new Underline());
            if (props.HasChildren) run.AppendChild(props);
            run.AppendChild(new Text(text) { Space = SpaceProcessingModeValues.Preserve });
            return run;
        }

        private static Table HtmlTableToOpenXml(HtmlNode tableNode)
        {
            var table = new Table();
            table.AppendChild(new TableProperties(
                new TableBorders(
                    new TopBorder { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder { Val = BorderValues.Single, Size = 4 },
                    new RightBorder { Val = BorderValues.Single, Size = 4 },
                    new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                    new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 })));

            var rows = tableNode.SelectNodes("tr");
            if (rows == null) return table;

            foreach (var rowNode in rows)
            {
                var tr = new TableRow();
                var cells = rowNode.SelectNodes("th|td");
                if (cells == null) continue;

                foreach (var cellNode in cells)
                {
                    var tc = new TableCell();
                    foreach (var child in cellNode.ChildNodes.Where(n => n.NodeType == HtmlNodeType.Element))
                    {
                        var ox = HtmlNodeToOpenXml(child);
                        if (ox is Paragraph p)
                            tc.AppendChild(p);
                    }
                    if (!tc.HasChildren)
                    {
                        var p = new Paragraph();
                        p.AppendChild(new Run(new Text(cellNode.InnerText) { Space = SpaceProcessingModeValues.Preserve }));
                        tc.AppendChild(p);
                    }
                    tr.AppendChild(tc);
                }
                table.AppendChild(tr);
            }
            return table;
        }
    }
}
