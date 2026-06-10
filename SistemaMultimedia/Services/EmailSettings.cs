using System;
using System.IO;
using System.Text.Json;

namespace SistemaMultimedia.Services
{
    /// <summary>
    /// Representa la configuración SMTP necesaria para el envío de correos.
    /// Se carga desde un archivo JSON (emailsettings.json) ubicado en la carpeta de la aplicación.
    /// No incluya credenciales en el código fuente; coloque aquí la cuenta remitente y la contraseña de aplicación.
    /// </summary>
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;
        public string SenderEmail { get; set; } = "";
        public string SenderName { get; set; } = "";
        public string AppPassword { get; set; } = "";

        /// <summary>Ruta por defecto del archivo de configuración.</summary>
        public static string DefaultFileName => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "emailsettings.json");

        /// <summary>
        /// Intenta cargar la configuración desde el archivo JSON. Lanza excepción si no existe o es inválido.
        /// </summary>
        public static EmailSettings LoadFromFile(string? path = null)
        {
            // Construir lista de rutas candidatas para buscar el archivo de configuración.
            var candidates = new System.Collections.Generic.List<string>();

            if (!string.IsNullOrWhiteSpace(path)) candidates.Add(path);
            // Ruta de salida de la aplicación (bin) - ruta usada en tiempo de ejecución
            candidates.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "emailsettings.json"));
            // Posibles rutas relativas durante desarrollo (project\bin\Debug -> subir niveles)
            candidates.Add(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "emailsettings.json")));
            candidates.Add(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "emailsettings.json")));
            // Directorio actual de trabajo
            candidates.Add(Path.Combine(Directory.GetCurrentDirectory(), "emailsettings.json"));
            // Base directory de AppContext
            candidates.Add(Path.Combine(AppContext.BaseDirectory ?? AppDomain.CurrentDomain.BaseDirectory, "emailsettings.json"));

            string? found = null;
            foreach (var c in candidates)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(c)) continue;
                    if (File.Exists(c))
                    {
                        found = c;
                        break;
                    }
                }
                catch { }
            }

            if (found == null)
            {
                // Mensaje claro con las rutas revisadas para facilitar diagnóstico
                string list = string.Join('\n', candidates);
                throw new FileNotFoundException($"No se encontró el archivo de configuración de correo en ninguna de las rutas buscadas:\n{list}");
            }

            var txt = File.ReadAllText(found);
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var cfg = JsonSerializer.Deserialize<EmailSettings>(txt, opts);
            if (cfg == null) throw new InvalidOperationException("La configuración de correo es inválida.");
            if (string.IsNullOrWhiteSpace(cfg.SenderEmail) || string.IsNullOrWhiteSpace(cfg.AppPassword))
                throw new InvalidOperationException("La configuración de correo debe contener SenderEmail y AppPassword.");

            return cfg;
        }
    }
}
