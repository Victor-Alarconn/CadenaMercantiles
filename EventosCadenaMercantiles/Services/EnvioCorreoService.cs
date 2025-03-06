using EventosCadenaMercantiles.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

namespace EventosCadenaMercantiles.Services
{
    public static class EnvioCorreoService
    {
        private const string CorreoDestino = "sistemas.rmsoft@gmail.com";
        private const string SmtpServidor = "smtp.resend.com";
        private const int SmtpPuerto = 587;
        private const string SmtpUsuario = "resend";
        private const string SmtpClave = "re_BAYQ9XHb_hx3tudPJJEixBJS2B3gV2UgZ";

        public static bool EnviarCorreo(DocumentoAdjunto documento, string rutaZip)
        {
            try
            {
                var mensaje = new MailMessage
                {
                    From = new MailAddress("facturaelectronica@rmsoft.com.co", documento.Receptor),
                    Subject = $"{documento.Identificacion}; {documento.Emisor}; {documento.PrefijoFactura}; 01; {documento.Emisor}",
                    Body = "Se adjunta archivo ZIP con el documento electrónico.",
                    IsBodyHtml = false
                };

                mensaje.To.Add(CorreoDestino);

                if (File.Exists(rutaZip))
                {
                    mensaje.Attachments.Add(new Attachment(rutaZip));
                }
                else
                {
                    MessageBox.Show("No se encontró el archivo ZIP para adjuntar al correo.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                using (var clienteSmtp = new SmtpClient(SmtpServidor, SmtpPuerto))
                {
                    clienteSmtp.Credentials = new NetworkCredential(SmtpUsuario, SmtpClave);
                    clienteSmtp.EnableSsl = true;

                    clienteSmtp.Send(mensaje);
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al enviar el correo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }

}
