using System;

namespace DataValidationModule.Models
{
    /// <summary>
    /// Enumeración de tipos de error detectados durante la validación del dataset.
    /// </summary>
    public enum ErrorType
    {
        NullOrEmpty,
        Duplicate,
        InvalidEmail,
        InvalidDate,
        NumericFieldWithText,
        LeadingTrailingSpaces,
        SuspiciousSpecialChars,
        RequiredFieldEmpty,
        MaxLengthExceeded,
        DataTypeMismatch
    }

    /// <summary>
    /// Representa un resultado de validación para una celda específica del dataset.
    /// Contiene información completa sobre el error detectado y la corrección sugerida.
    /// </summary>
    public class ValidationResult
    {
        /// <summary>Índice de la fila donde se encontró el error (base 0).</summary>
        public int RowIndex { get; set; }

        /// <summary>Nombre de la columna donde se encontró el error.</summary>
        public string ColumnName { get; set; }

        /// <summary>Valor actual almacenado en la celda.</summary>
        public string CurrentValue { get; set; }

        /// <summary>Tipo de error detectado.</summary>
        public ErrorType ErrorType { get; set; }

        /// <summary>Descripción legible del error encontrado.</summary>
        public string Description { get; set; }

        /// <summary>Valor sugerido para corregir el error (puede ser null si no hay sugerencia).</summary>
        public string SuggestedValue { get; set; }

        /// <summary>
        /// Retorna el nombre legible del tipo de error para reportes.
        /// </summary>
        public string ErrorTypeName => ErrorTypeInfo.GetDisplayName(ErrorType);

        /// <summary>
        /// Constructor por defecto.
        /// </summary>
        public ValidationResult() { }

        /// <summary>
        /// Constructor completo para crear un resultado de validación con todos sus campos.
        /// </summary>
        public ValidationResult(int rowIndex, string columnName, string currentValue,
                                 ErrorType errorType, string description, string suggestedValue = null)
        {
            RowIndex       = rowIndex;
            ColumnName     = columnName;
            CurrentValue   = currentValue;
            ErrorType      = errorType;
            Description    = description;
            SuggestedValue = suggestedValue;
        }

        /// <summary>
        /// Representación en texto del resultado para reportes y logs.
        /// </summary>
        public override string ToString()
        {
            return $"[Fila {RowIndex + 1}] [{ColumnName}] {ErrorTypeName}: \"{CurrentValue}\" → {Description}" +
                   (SuggestedValue != null ? $" | Sugerencia: \"{SuggestedValue}\"" : "");
        }
    }

    /// <summary>
    /// Estadísticas globales del dataset analizado.
    /// </summary>
    public class DatasetStatistics
    {
        public int TotalRecords        { get; set; }
        public int TotalColumns        { get; set; }
        public int ErrorsFound         { get; set; }
        public int DuplicatesFound     { get; set; }
        public int NullValues          { get; set; }
        public int CorrectionsApplied  { get; set; }
        public double QualityPercent   { get; set; }

        /// <summary>
        /// Calcula el porcentaje de calidad del dataset.
        /// Fórmula: 100 - (errores / (filas * columnas) * 100)
        /// </summary>
        public void CalculateQuality()
        {
            int totalCells = TotalRecords * TotalColumns;
            if (totalCells <= 0)
            {
                QualityPercent = 0;
                return;
            }
            double errorRate = (double)ErrorsFound / totalCells;
            QualityPercent   = Math.Max(0, Math.Round((1 - errorRate) * 100, 2));
        }
    }
}
