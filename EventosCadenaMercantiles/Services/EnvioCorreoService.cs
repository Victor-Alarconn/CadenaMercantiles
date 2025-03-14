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
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;

namespace EventosCadenaMercantiles.Services
{
    public static class EnvioCorreoService
    {
        private const string CorreoDestino = "rmsoft@efacturacadenapru.com";
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

        public static bool EnviarCorreoAlEmisor(string resultado, string tipoEvento, EventosModel documento)
        {
            try
            {
                var mensaje = new MailMessage
                {
                    From = new MailAddress("facturaelectronica@rmsoft.com.co", "Notificacion"),
                    Subject = $"Notificación de Evento {tipoEvento} - {documento}",
                    IsBodyHtml = true // Habilitar HTML
                };

                // Encabezado y contenido centrado
                string cuerpo = $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    color: #333;
                    display: flex;
                    justify-content: center;
                    align-items: center;
                    height: 100vh;
                    background-color: #f4f4f9;
                    margin: 0;
                    padding: 0;
                    box-sizing: border-box;
                }}
                .card {{
                    width: 500px;
                    border: 1px solid #ddd;
                    border-radius: 10px;
                    background-color: #ffffff;
                    box-shadow: 0 4px 8px rgba(0,0,0,0.1);
                    overflow: hidden;
                    text-align: center;
                    margin: auto;
                }}
                .encabezado {{
                    background-color: #D32F2F;
                    color: white;
                    padding: 15px;
                    font-size: 18px;
                    font-weight: bold;
                }}
                .empresa {{
                    font-size: 16px;
                    margin-top: 5px;
                }}
                .contenido {{
                    padding: 20px;
                    font-size: 14px;
                    text-align: center;
                }}
                .titulo {{
                    font-size: 16px;
                    font-weight: bold;
                    margin-bottom: 15px;
                }}
                .info {{
                    font-size: 14px;
                    line-height: 1.5;
                    text-align: center;
                    margin-bottom: 10px;
                }}
                .nota-confidencial {{
                    margin-top: 20px;
                    padding: 15px;
                    font-size: 12px;
                    color: #888;
                    background-color: #f9f9f9;
                    border-top: 1px solid #ddd;
                    text-align: justify;
                }}
            </style>
        </head>
        <body>
            <div class='card'>
                <div class='encabezado'>
                    <div>ESTIMADO/A CLIENTE</div>
                    <div class='empresa'>Rm Soft Casa de Software SAS.</div>
                </div>

                <div class='contenido'>
                    <p class='titulo'>Le informamos que la factura electrónica emitida tiene realizado un evento mercantil. A continuación, encontrará resumen de este documento:</p>

                    <div class='info'>
                        <p><strong>Evento:</strong> {tipoEvento}</p>
                        <p><strong>Emisor:</strong> Rm Soft Casa de Software SAS.</p>
                        <p><strong>Prefijo y número del documento:</strong> {documento}</p>
                        <p><strong>Tipo de documento:</strong> FACTURA ELECTRÓNICA DE VENTA</p>
                        <p><strong>Fecha de emisión:</strong> {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>
                    </div>

                    <p>En caso de tener alguna inquietud respecto a la información contenida en el documento, por favor comuníquese con <strong>PINTURAS PANELTON Y DISTRIBUCIONES S.A.S</strong>.</p>
                </div>

                <div class='nota-confidencial'>
                    <p><strong>NOTA CONFIDENCIAL:</strong> La información contenida en este correo electrónico y en todos sus archivos anexos es confidencial de <strong>PINTURAS PANELTON Y DISTRIBUCIONES S.A.S</strong>, solo para uso individual del destinatario o entidad a quienes está dirigido. Si usted no es destinatario, cualquier almacenamiento, distribución, difusión o copia de este mensaje está estrictamente prohibida y sancionada por la ley. Si por error recibe este mensaje, le ofrecemos disculpas. Por favor elimínelo inmediatamente y notifique de su error a la persona que la envió, absteniéndose de divulgar su contenido.</p>
                </div>
            </div>
        </body>
        </html>";

                // Asignar cuerpo al mensaje
                mensaje.Body = cuerpo;

                // Destinatario
                mensaje.To.Add("sistemas.rmsoft@gmail.com");

                // Configurar el cliente SMTP
                using (var clienteSmtp = new SmtpClient(SmtpServidor, SmtpPuerto))
                {
                    clienteSmtp.Credentials = new NetworkCredential(SmtpUsuario, SmtpClave);
                    clienteSmtp.EnableSsl = true;

                    // Enviar correo
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
