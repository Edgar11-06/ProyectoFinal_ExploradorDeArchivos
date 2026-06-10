namespace DataValidationModule.Common
{
    /// <summary>
    /// Formatos de fecha compartidos entre validación y limpieza de datos.
    /// </summary>
    public static class DataFormats
    {
        public static readonly string[] AcceptedDateFormats =
        {
            "dd/MM/yyyy", "MM/dd/yyyy", "yyyy-MM-dd", "dd-MM-yyyy",
            "d/M/yyyy",   "M/d/yyyy",   "yyyy/MM/dd", "dd.MM.yyyy",
            "dd/MM/yy",   "M/d/yy",     "d/M/yy"
        };
    }
}
