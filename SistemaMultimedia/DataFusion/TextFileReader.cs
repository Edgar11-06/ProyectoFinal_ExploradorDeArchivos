using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SistemaMultimedia.DataFusion
{
    /// <summary>
    /// Utilidades para leer archivos de texto plano.
    /// </summary>
    public static class TextFileReader
    {
        /// <summary>
        /// Lee todas las líneas de un archivo con la codificación indicada (UTF8 por defecto).
        /// </summary>
        public static List<string> ReadLines(string path, Encoding? encoding = null, bool skipEmpty = true)
        {
            encoding ??= Encoding.UTF8;
            var lines = new List<string>();

            using var sr = new StreamReader(path, encoding);
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                if (skipEmpty && string.IsNullOrWhiteSpace(line))
                    continue;
                lines.Add(line);
            }

            return lines;
        }

        /// <summary>
        /// Lee el archivo y aplica <paramref name="lineParser"/> a cada línea.
        /// El parser debe devolver null para indicar que la línea se ignora.
        /// Devuelve una lista de diccionarios (clave->valor) por línea parseada.
        /// </summary>
        public static List<Dictionary<string, string>> ReadAsDictionaries(string path, Func<string, Dictionary<string, string>?> lineParser, Encoding? encoding = null, bool skipEmpty = true)
        {
            if (lineParser == null) throw new ArgumentNullException(nameof(lineParser));

            var result = new List<Dictionary<string, string>>();
            var lines = ReadLines(path, encoding, skipEmpty);

            foreach (var line in lines)
            {
                try
                {
                    var item = lineParser(line);
                    if (item != null)
                        result.Add(item);
                }
                catch
                {
                    // ignorar líneas que provocan errores en el parser
                    continue;
                }
            }

            return result;
        }
    }
}