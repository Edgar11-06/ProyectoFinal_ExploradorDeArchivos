using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SistemaMultimedia.Services
{
    /// <summary>
    /// Servicio responsable de enviar correos usando configuración proporcionada por EmailSettings.
    /// Implementa método asíncrono SendEmailAsync que retorna información de éxito/error.
    /// </summary>
    public class EmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(EmailSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        /// Envía un correo con un adjunto. No bloquea el hilo UI.
        /// Lanza excepción específica según el error ocurrido.
        /// </summary>
        public async Task SendEmailAsync(string toEmail, string subject, string body, string attachmentPath)
        {
            if (string.IsNullOrWhiteSpace(toEmail)) throw new ArgumentException("Correo destino inválido.", nameof(toEmail));
            if (string.IsNullOrWhiteSpace(subject)) throw new ArgumentException("Asunto vacío.", nameof(subject));
            if (string.IsNullOrWhiteSpace(body)) body = string.Empty;
            if (string.IsNullOrWhiteSpace(attachmentPath) || !File.Exists(attachmentPath)) throw new FileNotFoundException("Archivo adjunto no encontrado.", attachmentPath);

            using var message = new MailMessage();
            message.From = new MailAddress(_settings.SenderEmail, _settings.SenderName);
            message.To.Add(new MailAddress(toEmail));
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = false;

            var attachment = new Attachment(attachmentPath);
            message.Attachments.Add(attachment);

            using var client = new SmtpClient(_settings.SmtpServer, _settings.Port)
            {
                EnableSsl = _settings.EnableSsl,
                Credentials = new NetworkCredential(_settings.SenderEmail, _settings.AppPassword),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 30000
            };

            try
            {
                await client.SendMailAsync(message).ConfigureAwait(false);
            }
            catch (SmtpFailedRecipientException ex)
            {
                throw new InvalidOperationException($"Destinatario rechazado: {ex.FailedRecipient}", ex);
            }
            catch (SmtpException ex)
            {
                // Intentar proporcionar un mensaje más amigable según el contenido del error
                if (ex.StatusCode == SmtpStatusCode.GeneralFailure)
                    throw new InvalidOperationException("Error de conexión SMTP.", ex);

                // Si el mensaje contiene palabras clave típicas de autenticación, informar al usuario
                var msg = ex.Message ?? string.Empty;
                if (msg.IndexOf("auth", StringComparison.OrdinalIgnoreCase) >= 0 || msg.IndexOf("535", StringComparison.OrdinalIgnoreCase) >= 0)
                    throw new InvalidOperationException("Error de autenticación SMTP. Verifique las credenciales o la contraseña de la aplicación.", ex);

                throw new InvalidOperationException($"Error SMTP: {msg}", ex);
            }
        }
    }
}
