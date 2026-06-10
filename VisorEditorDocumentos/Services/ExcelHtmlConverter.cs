using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using HtmlAgilityPack;

namespace VisorEditorDocumentos.Services
{
    public static class ExcelHtmlConverter
    {
        public static string ToBodyHtml(string filePath)
        {
            var ext = Path.GetExtension(filePath).ToLowerInvariant();
            return ext switch
            {
                ".xlsx" => XlsxToBodyHtml(filePath),
                ".csv" => DataTableToBodyHtml(ReadCsv(filePath)),
                ".xls" => DataTableToBodyHtml(ReadXlsWithOleDb(filePath)),
                _ => throw new InvalidOperationException("El archivo no es un documento Excel compatible.")
            };
        }

        public static void SaveBodyHtmlAsXlsx(string bodyHtml, string outputPath)
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);

            using var document = SpreadsheetDocument.Create(outputPath, SpreadsheetDocumentType.Workbook);
            var workbookPart = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            var sheetData = new SheetData();
            worksheetPart.Worksheet = new Worksheet(sheetData);

            var rows = ExtractRows(bodyHtml).ToList();
            uint rowIndex = 1;
            foreach (var rowValues in rows)
            {
                var row = new Row { RowIndex = rowIndex };
                for (var i = 0; i < rowValues.Count; i++)
                {
                    var cell = new Cell
                    {
                        CellReference = GetColumnName(i + 1) + rowIndex.ToString(CultureInfo.InvariantCulture),
                        DataType = CellValues.String,
                        CellValue = new CellValue(rowValues[i] ?? string.Empty)
                    };
                    row.Append(cell);
                }

                sheetData.Append(row);
                rowIndex++;
            }

            if (rows.Count == 0)
                sheetData.Append(new Row(new Cell { DataType = CellValues.String, CellValue = new CellValue(string.Empty) }));

            var sheets = workbookPart.Workbook.AppendChild(new Sheets());
            sheets.Append(new Sheet
            {
                Id = workbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "Hoja1"
            });

            workbookPart.Workbook.Save();
        }

        public static string PlainTextToExcelHtml(string text)
        {
            var rows = (text ?? string.Empty)
                .Replace("\r\n", "\n")
                .Split('\n')
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => new[] { line.Trim() });

            return RowsToHtml(rows);
        }

        public static string DataTableToBodyHtml(DataTable table)
        {
            var rows = table.Rows.Cast<DataRow>()
                .Select(row => table.Columns.Cast<DataColumn>()
                    .Select(col => Convert.ToString(row[col], CultureInfo.CurrentCulture) ?? string.Empty));

            var header = table.Columns.Cast<DataColumn>().Select(c => c.ColumnName);
            return RowsToHtml(new[] { header }.Concat(rows));
        }

        private static string XlsxToBodyHtml(string filePath)
        {
            using var document = SpreadsheetDocument.Open(filePath, false);
            var workbookPart = document.WorkbookPart ?? throw new InvalidOperationException("El libro Excel no tiene contenido.");
            var firstSheet = workbookPart.Workbook.Sheets?.Elements<Sheet>().FirstOrDefault()
                ?? throw new InvalidOperationException("El libro Excel no contiene hojas.");

            var worksheetPart = (WorksheetPart)workbookPart.GetPartById(firstSheet.Id!);
            var rows = worksheetPart.Worksheet.Descendants<Row>()
                .Select(row => RowToValues(row, workbookPart).ToList())
                .Where(values => values.Any(v => !string.IsNullOrWhiteSpace(v)))
                .ToList();

            return RowsToHtml(rows);
        }

        private static List<string> RowToValues(Row row, WorkbookPart workbookPart)
        {
            var result = new List<string>();
            foreach (var cell in row.Elements<Cell>())
            {
                var colIndex = GetColumnIndex(cell.CellReference?.Value);
                while (result.Count < colIndex - 1)
                    result.Add(string.Empty);

                result.Add(GetCellText(cell, workbookPart));
            }

            return result;
        }

        private static string GetCellText(Cell cell, WorkbookPart workbookPart)
        {
            var raw = cell.CellValue?.InnerText ?? string.Empty;
            if (cell.DataType?.Value == CellValues.SharedString &&
                int.TryParse(raw, out var sharedIndex))
            {
                return workbookPart.SharedStringTablePart?.SharedStringTable
                    .Elements<SharedStringItem>()
                    .ElementAtOrDefault(sharedIndex)
                    ?.InnerText ?? string.Empty;
            }

            return raw;
        }

        private static IEnumerable<List<string>> ExtractRows(string bodyHtml)
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

            foreach (var p in doc.DocumentNode.SelectNodes("//p|//div") ?? Enumerable.Empty<HtmlNode>())
            {
                var text = HtmlEntity.DeEntitize(p.InnerText).Trim();
                if (!string.IsNullOrWhiteSpace(text))
                    yield return new List<string> { text };
            }
        }

        private static string RowsToHtml(IEnumerable<IEnumerable<string>> rows)
        {
            var materialized = rows.Select(r => r.ToList()).ToList();
            var maxColumns = Math.Max(1, materialized.Select(r => r.Count).DefaultIfEmpty(1).Max());
            var sb = new StringBuilder();
            sb.Append("<div class=\"sheet-wrap\"><table class=\"excel-grid\"><colgroup>");
            for (var i = 0; i < maxColumns; i++)
                sb.Append("<col style=\"min-width:120px\"/>");
            sb.Append("</colgroup>");

            var rowIndex = 0;
            foreach (var row in materialized)
            {
                sb.Append("<tr>");
                var tag = rowIndex == 0 ? "th" : "td";
                for (var i = 0; i < maxColumns; i++)
                {
                    var cell = i < row.Count ? row[i] : string.Empty;
                    sb.Append('<').Append(tag).Append('>')
                        .Append(WebUtility.HtmlEncode(cell ?? string.Empty))
                        .Append("</").Append(tag).Append('>');
                }
                sb.Append("</tr>");
                rowIndex++;
            }
            sb.Append("</table></div>");
            return rowIndex == 0 ? "<div class=\"sheet-wrap\"><table class=\"excel-grid\"><tr><td></td></tr></table></div>" : sb.ToString();
        }

        private static DataTable ReadCsv(string filePath)
        {
            var table = new DataTable("Hoja1");
            var lines = File.ReadAllLines(filePath, Encoding.UTF8);
            if (lines.Length == 0) return table;

            var headers = SplitDelimitedLine(lines[0]);
            foreach (var header in headers)
                table.Columns.Add(string.IsNullOrWhiteSpace(header) ? $"Col{table.Columns.Count + 1}" : header);

            foreach (var line in lines.Skip(1))
            {
                var values = SplitDelimitedLine(line);
                var row = table.NewRow();
                for (var i = 0; i < table.Columns.Count; i++)
                    row[i] = i < values.Length ? values[i] : string.Empty;
                table.Rows.Add(row);
            }

            return table;
        }

        private static string[] SplitDelimitedLine(string line)
        {
            var separator = line.Count(c => c == ';') > line.Count(c => c == ',') ? ';' : ',';
            return line.Split(separator).Select(v => v.Trim().Trim('"')).ToArray();
        }

        private static DataTable ReadXlsWithOleDb(string filePath)
        {
            var table = new DataTable("Hoja1");
            try
            {
                var factory = System.Data.Common.DbProviderFactories.GetFactory("System.Data.OleDb");
                using var connection = factory.CreateConnection();
                if (connection == null) throw new InvalidOperationException();

                connection.ConnectionString =
                    $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\";";
                connection.Open();

                var schema = connection.GetSchema("Tables");
                var sheetName = schema.Rows.Cast<DataRow>()
                    .Select(r => Convert.ToString(r["TABLE_NAME"]))
                    .FirstOrDefault(n => !string.IsNullOrWhiteSpace(n));
                if (string.IsNullOrWhiteSpace(sheetName)) return table;

                using var command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM [{sheetName}]";
                using var reader = command.ExecuteReader();
                table.Load(reader!);
                return table;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "No se pudo abrir el archivo .xls. Instale Microsoft Access Database Engine o guarde el archivo como .xlsx.",
                    ex);
            }
        }

        private static int GetColumnIndex(string? cellReference)
        {
            if (string.IsNullOrWhiteSpace(cellReference)) return 1;
            var letters = new string(cellReference.TakeWhile(char.IsLetter).ToArray()).ToUpperInvariant();
            var sum = 0;
            foreach (var ch in letters)
                sum = (sum * 26) + ch - 'A' + 1;
            return Math.Max(1, sum);
        }

        private static string GetColumnName(int index)
        {
            var name = string.Empty;
            while (index > 0)
            {
                var modulo = (index - 1) % 26;
                name = Convert.ToChar('A' + modulo) + name;
                index = (index - modulo) / 26;
            }
            return name;
        }
    }
}
