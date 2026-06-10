using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DataValidationModule.Models;

namespace DataValidationModule
{
    /// <summary>
    /// Genera y exporta reportes de validación en formato TXT y HTML.
    /// </summary>
    public static class ReportExporter
    {
        // -------------------------------------------------------------------
        // Exportar como TXT plano
        // -------------------------------------------------------------------

        /// <summary>
        /// Genera un reporte de texto plano con el resumen de estadísticas y lista de errores.
        /// </summary>
        public static void ExportTxt(string filePath,
                                     DatasetStatistics stats,
                                     List<ValidationResult> errors,
                                     List<string> changeLog = null)
        {
            var sb = new StringBuilder();
            string separator = new string('=', 70);
            string separator2 = new string('-', 70);

            sb.AppendLine(separator);
            sb.AppendLine("  REPORTE DE VALIDACIÓN Y CORRECCIÓN DE DATASET");
            sb.AppendLine($"  Generado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine(separator);
            sb.AppendLine();

            // --- Estadísticas ---
            sb.AppendLine("ESTADÍSTICAS DEL DATASET");
            sb.AppendLine(separator2);
            sb.AppendLine($"  Total de registros     : {stats.TotalRecords}");
            sb.AppendLine($"  Total de columnas      : {stats.TotalColumns}");
            sb.AppendLine($"  Errores encontrados    : {stats.ErrorsFound}");
            sb.AppendLine($"  Duplicados encontrados : {stats.DuplicatesFound}");
            sb.AppendLine($"  Valores nulos          : {stats.NullValues}");
            sb.AppendLine($"  Correcciones aplicadas : {stats.CorrectionsApplied}");
            sb.AppendLine($"  Calidad del dataset    : {stats.QualityPercent:F1}%");
            sb.AppendLine();

            // --- Errores ---
            sb.AppendLine($"ERRORES DETECTADOS ({errors.Count})");
            sb.AppendLine(separator2);

            if (errors.Count == 0)
            {
                sb.AppendLine("  ✓ No se encontraron errores.");
            }
            else
            {
                foreach (var e in errors)
                {
                    sb.AppendLine($"  Fila     : {e.RowIndex + 1}");
                    sb.AppendLine($"  Columna  : {e.ColumnName}");
                    sb.AppendLine($"  Valor    : {e.CurrentValue}");
                    sb.AppendLine($"  Error    : {e.ErrorTypeName}");
                    sb.AppendLine($"  Detalle  : {e.Description}");
                    if (!string.IsNullOrEmpty(e.SuggestedValue))
                        sb.AppendLine($"  Sugerencia: {e.SuggestedValue}");
                    sb.AppendLine(separator2);
                }
            }

            // --- Log de cambios ---
            if (changeLog != null && changeLog.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine($"LOG DE CORRECCIONES APLICADAS ({changeLog.Count})");
                sb.AppendLine(separator2);
                foreach (var entry in changeLog)
                    sb.AppendLine("  " + entry);
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        // -------------------------------------------------------------------
        // Exportar como HTML
        // -------------------------------------------------------------------

        /// <summary>
        /// Genera un reporte HTML con estilos visuales, estadísticas y tabla de errores.
        /// </summary>
        public static void ExportHtml(string filePath,
                                      DatasetStatistics stats,
                                      List<ValidationResult> errors)
        {
            var sb = new StringBuilder();

            // Calcular color del indicador de calidad
            string qualityColor = stats.QualityPercent >= 80 ? "#27ae60"
                                : stats.QualityPercent >= 50 ? "#f39c12"
                                : "#c0392b";

            sb.AppendLine(@"<!DOCTYPE html>
<html lang='es'>
<head>
  <meta charset='UTF-8'>
  <meta name='viewport' content='width=device-width, initial-scale=1.0'>
  <title>Reporte de Validación de Dataset</title>
  <style>
    * { box-sizing: border-box; margin: 0; padding: 0; }
    body { font-family: 'Segoe UI', Arial, sans-serif; background: #f0f2f5; color: #333; }
    .container { max-width: 1100px; margin: 30px auto; padding: 0 20px; }
    header { background: linear-gradient(135deg, #2c3e50, #3498db); color: white;
             padding: 30px; border-radius: 10px 10px 0 0; }
    header h1 { font-size: 24px; margin-bottom: 6px; }
    header p  { opacity: .8; font-size: 13px; }
    .stats-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(160px, 1fr));
                  gap: 16px; background: white; padding: 24px; border-bottom: 1px solid #e0e0e0; }
    .stat-card  { text-align: center; padding: 16px; background: #f8f9fa;
                  border-radius: 8px; border-left: 4px solid #3498db; }
    .stat-card .value { font-size: 28px; font-weight: 700; color: #2c3e50; }
    .stat-card .label { font-size: 11px; color: #666; text-transform: uppercase; margin-top: 4px; }
    .quality-bar { padding: 20px 24px; background: white; border-bottom: 1px solid #e0e0e0; }
    .quality-bar p { font-size: 13px; color: #555; margin-bottom: 8px; }
    .bar-outer { height: 22px; background: #ecf0f1; border-radius: 11px; overflow: hidden; }
    .bar-inner  { height: 100%; border-radius: 11px; display: flex; align-items: center;
                  justify-content: center; font-size: 12px; font-weight: 600; color: white; }
    .errors-section { background: white; padding: 24px; border-radius: 0 0 10px 10px; }
    .errors-section h2 { font-size: 16px; margin-bottom: 16px; color: #2c3e50; }
    table { width: 100%; border-collapse: collapse; font-size: 13px; }
    thead th { background: #2c3e50; color: white; padding: 10px 12px; text-align: left;
               font-weight: 600; }
    tbody tr:nth-child(even) { background: #f8f9fa; }
    tbody td { padding: 9px 12px; border-bottom: 1px solid #ecf0f1; vertical-align: top; }
    .badge { display: inline-block; padding: 2px 8px; border-radius: 12px;
             font-size: 11px; font-weight: 600; }
    .badge-null    { background: #fadbd8; color: #c0392b; }
    .badge-dup     { background: #fdebd0; color: #d35400; }
    .badge-email   { background: #d5f5e3; color: #1e8449; }
    .badge-date    { background: #d6eaf8; color: #1a5276; }
    .badge-numeric { background: #e8daef; color: #6c3483; }
    .badge-space   { background: #d5d8dc; color: #424949; }
    .badge-char    { background: #fef9e7; color: #9a7d0a; }
    .badge-required{ background: #fadbd8; color: #922b21; }
    .badge-length  { background: #d0ece7; color: #1b6a4a; }
    .badge-type    { background: #e9f7ef; color: #1d6a39; }
    .no-errors { text-align: center; padding: 40px; color: #27ae60; font-size: 18px; }
    footer { text-align: center; margin-top: 16px; font-size: 12px; color: #aaa; }
  </style>
</head>
<body>
<div class='container'>
  <header>
    <h1>📊 Reporte de Validación de Dataset</h1>
    <p>Generado el " + DateTime.Now.ToString("dd/MM/yyyy 'a las' HH:mm:ss") + @"</p>
  </header>

  <div class='stats-grid'>
    <div class='stat-card'>
      <div class='value'>" + stats.TotalRecords + @"</div>
      <div class='label'>Registros</div>
    </div>
    <div class='stat-card'>
      <div class='value'>" + stats.TotalColumns + @"</div>
      <div class='label'>Columnas</div>
    </div>
    <div class='stat-card'>
      <div class='value' style='color:#c0392b'>" + stats.ErrorsFound + @"</div>
      <div class='label'>Errores</div>
    </div>
    <div class='stat-card'>
      <div class='value' style='color:#d35400'>" + stats.DuplicatesFound + @"</div>
      <div class='label'>Duplicados</div>
    </div>
    <div class='stat-card'>
      <div class='value' style='color:#7d6608'>" + stats.NullValues + @"</div>
      <div class='label'>Nulos</div>
    </div>
    <div class='stat-card'>
      <div class='value' style='color:#1e8449'>" + stats.CorrectionsApplied + @"</div>
      <div class='label'>Correcciones</div>
    </div>
  </div>

  <div class='quality-bar'>
    <p>Calidad del dataset: <strong>" + stats.QualityPercent.ToString("F1") + @"%</strong></p>
    <div class='bar-outer'>
      <div class='bar-inner' style='width:" + stats.QualityPercent.ToString("F0") + @"%;background:" + qualityColor + @"'>
        " + stats.QualityPercent.ToString("F1") + @"%
      </div>
    </div>
  </div>

  <div class='errors-section'>
    <h2>Errores detectados (" + errors.Count + @")</h2>");

            if (errors.Count == 0)
            {
                sb.AppendLine("<div class='no-errors'>✅ ¡El dataset no tiene errores detectados!</div>");
            }
            else
            {
                sb.AppendLine(@"<table>
      <thead>
        <tr>
          <th>#</th><th>Fila</th><th>Columna</th><th>Valor encontrado</th>
          <th>Tipo de error</th><th>Descripción</th><th>Sugerencia</th>
        </tr>
      </thead>
      <tbody>");

                int i = 1;
                foreach (var e in errors)
                {
                    string badgeClass = ErrorTypeInfo.GetHtmlBadgeClass(e.ErrorType);
                    sb.AppendLine($@"        <tr>
          <td>{i++}</td>
          <td>{e.RowIndex + 1}</td>
          <td><strong>{Encode(e.ColumnName)}</strong></td>
          <td><code>{Encode(e.CurrentValue)}</code></td>
          <td><span class='badge {badgeClass}'>{Encode(e.ErrorTypeName)}</span></td>
          <td>{Encode(e.Description)}</td>
          <td>{Encode(e.SuggestedValue ?? "—")}</td>
        </tr>");
                }
                sb.AppendLine("      </tbody></table>");
            }

            sb.AppendLine(@"  </div>
  <footer>Generado con DataValidationModule • " + DateTime.Now.Year + @"</footer>
</div>
</body>
</html>");

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        // -------------------------------------------------------------------
        // Helpers
        // -------------------------------------------------------------------

        private static string Encode(string s)
            => System.Net.WebUtility.HtmlEncode(s ?? "");

    }
}
