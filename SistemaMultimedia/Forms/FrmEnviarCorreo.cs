using System;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Mail;
using SistemaMultimedia.Services;

namespace SistemaMultimedia.Forms
{
    /// <summary>
    /// Formulario para enviar un archivo por correo utilizando una cuenta configurada por el desarrollador.
    /// El usuario sólo introduce destinatario, asunto y mensaje.
    /// </summary>
    public partial class FrmEnviarCorreo : Form
    {
        private readonly string _attachmentPath;
        private readonly EmailService _emailService;
        private readonly EmailSettings _settings;

        public FrmEnviarCorreo(string attachmentPath)
        {
            InitializeComponent();

            _attachmentPath = attachmentPath;
            try
            {
                _settings = EmailSettings.LoadFromFile();
                _emailService = new EmailService(_settings);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cargando configuración de correo: {ex.Message}", "Configuración de correo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            lblAttachment.Text = _attachmentPath ?? "(sin archivo)";
            progressBar.Visible = false;

            // Establecer iconos y estilo visual en tiempo de ejecución
            try
            {
                // Intentar cargar iconos embebidos o recursos del ensamblado
                var asm = System.Reflection.Assembly.GetExecutingAssembly();
                var stream = asm.GetManifestResourceStream("SistemaMultimedia.Resources.email.png");
                if (stream != null)
                {
                    using var bmp = System.Drawing.Image.FromStream(stream);
                    pbStatus.Image = new System.Drawing.Bitmap(bmp);
                }
            }
            catch { }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtTo.Text))
            {
                MessageBox.Show("Ingrese el correo destino.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!IsValidEmail(txtTo.Text))
            {
                MessageBox.Show("El correo destino no tiene un formato válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtSubject.Text))
            {
                MessageBox.Show("Ingrese el asunto.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(_attachmentPath) || !File.Exists(_attachmentPath))
            {
                MessageBox.Show("El archivo adjunto no existe.", "Archivo no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                // Simple regex para validación básica
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase);
            }
            catch {
                return false;
            }
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            // Bloquear controles
            SetControlsEnabled(false);
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.Visible = true;

            try
            {
                await _emailService.SendEmailAsync(txtTo.Text.Trim(), txtSubject.Text.Trim(), txtBody.Text, _attachmentPath);

                progressBar.Visible = false;
                pbStatus.Image = SystemIcons.Shield.ToBitmap();
                MessageBox.Show("Correo enviado correctamente.", "Envío completado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"Archivo no encontrado: {ex.FileName}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Error en envío de correo: {ex.Message}", "Error SMTP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (SmtpFailedRecipientException ex)
            {
                MessageBox.Show($"Destinatario rechazado: {ex.FailedRecipient}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetControlsEnabled(true);
                progressBar.Visible = false;
            }
        }

        private void SetControlsEnabled(bool enabled)
        {
            txtTo.Enabled = enabled;
            txtSubject.Enabled = enabled;
            txtBody.Enabled = enabled;
            btnSend.Enabled = enabled;
            btnCancel.Enabled = enabled;
        }
    }
}
