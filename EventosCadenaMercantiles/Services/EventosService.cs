﻿using EventosCadenaMercantiles.Data;
using EventosCadenaMercantiles.Modelos;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventosCadenaMercantiles.Services
{
    public class EventosService
    {
        public static List<EventosModel> ObtenerEventos(DateTime desde, DateTime hasta)
        {
            var connection = Conexion.ObtenerConexion();
            List<EventosModel> eventos = new List<EventosModel>();

            connection.Open();
            var command = new OdbcCommand("SELECT even_evento, id_eventos, even_docum, even_receptor, even_identif, even_fecha, even_evento, even_xmlb64, even_codigo, even_response, even_qrcode, even_cufe " +
                                           "FROM eventos WHERE date(even_fecha) BETWEEN ? AND ?", connection);
            command.Parameters.AddWithValue("?", desde);
            command.Parameters.AddWithValue("?", hasta);

            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var evento = new EventosModel
                {
                    IdEventos = reader.GetInt32(1),
                    EvenDocum = reader.GetString(2),
                    EvenReceptor = reader.GetString(3),
                    EvenIdentif = reader.GetString(4),
                    EvenFecha = reader.GetString(5),
                    EvenEvento = reader.GetString(6),
                    EvenXmlb64 = reader.GetString(7),
                    EvenCodigo = reader.GetString(8),
                    EvenResponse = reader.GetString(9),
                    EvenQrcode = reader.GetString(10),
                    EvenCufe = reader.GetString(11)
                };

                // Validación: Si la factura o el evento es vacío, lo descartamos
                if (string.IsNullOrWhiteSpace(evento.EvenDocum) || string.IsNullOrWhiteSpace(evento.EvenEvento))
                {
                    continue; // Ignorar este registro
                }

                eventos.Add(evento);
            }
            connection.Close();

            return eventos;
        }


        // Método para guardar el evento en la base de datos
        public static void GuardarEvento(DocumentoAdjunto documento)
        {
            var connection = Conexion.ObtenerConexion();
            connection.Open();
            var command = new OdbcCommand("INSERT INTO eventos (`even_docum`, `even_receptor`, `even_identif`, `even_fecha`, `even_evento`, `even_xmlb64`, `even_codigo`, `even_response`, `even_qrcode`, `even_cufe`) " +
                                          "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)", connection);

            command.Parameters.AddWithValue("?", documento.PrefijoFactura);
            command.Parameters.AddWithValue("?", documento.Emisor);
            command.Parameters.AddWithValue("?", documento.Identificacion);
            command.Parameters.AddWithValue("?", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("?", "Validando la Factura");
            command.Parameters.AddWithValue("?", documento.XmlBase64);
            command.Parameters.AddWithValue("?", "COD_ESPERA");
            command.Parameters.AddWithValue("?", "ESPERANDO RESPUESTA");
            command.Parameters.AddWithValue("?", documento.QRCode ?? "");
            command.Parameters.AddWithValue("?", documento.Cufe ?? "");

            command.ExecuteNonQuery();
            connection.Close();
        }

        public static bool ExisteFactura(string prefijoFactura)
        {
            using (var connection = Conexion.ObtenerConexion())
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM eventos WHERE even_docum = ?";
                using (var command = new OdbcCommand(query, connection))
                {
                    command.Parameters.AddWithValue("?", prefijoFactura);

                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0; // Si existe al menos un registro con ese even_docum, ya está en la BD
                }
            }
        }


    }
}
