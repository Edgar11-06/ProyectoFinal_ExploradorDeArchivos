namespace DataValidationModule.Models
{
    /// <summary>
    /// Nombres legibles y estilos de reporte para <see cref="ErrorType"/>.
    /// Centraliza el mapeo usado en UI, TXT y HTML.
    /// </summary>
    public static class ErrorTypeInfo
    {
        public static string GetDisplayName(ErrorType type)
        {
            switch (type)
            {
                case ErrorType.NullOrEmpty:            return "Valor nulo/vacío";
                case ErrorType.Duplicate:              return "Registro duplicado";
                case ErrorType.InvalidEmail:           return "Correo inválido";
                case ErrorType.InvalidDate:            return "Fecha inválida";
                case ErrorType.NumericFieldWithText:   return "Texto en campo numérico";
                case ErrorType.LeadingTrailingSpaces:  return "Espacios innecesarios";
                case ErrorType.SuspiciousSpecialChars: return "Caracteres especiales";
                case ErrorType.RequiredFieldEmpty:     return "Campo obligatorio vacío";
                case ErrorType.MaxLengthExceeded:      return "Longitud máxima excedida";
                case ErrorType.DataTypeMismatch:       return "Tipo de dato incorrecto";
                default:                               return "Error desconocido";
            }
        }

        public static string GetHtmlBadgeClass(ErrorType type)
        {
            switch (type)
            {
                case ErrorType.NullOrEmpty:            return "badge-null";
                case ErrorType.Duplicate:              return "badge-dup";
                case ErrorType.InvalidEmail:           return "badge-email";
                case ErrorType.InvalidDate:            return "badge-date";
                case ErrorType.NumericFieldWithText:   return "badge-numeric";
                case ErrorType.LeadingTrailingSpaces:  return "badge-space";
                case ErrorType.SuspiciousSpecialChars: return "badge-char";
                case ErrorType.RequiredFieldEmpty:     return "badge-required";
                case ErrorType.MaxLengthExceeded:      return "badge-length";
                default:                               return "badge-type";
            }
        }
    }
}
