using EventosCadenaMercantiles.Data;
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
        public static List<EventosModel> ObtenerEventos(DateTime desde, DateTime hasta) // Método para obtener los eventos de la base de datos
        {
            var connection = Conexion.ObtenerConexion();
            List<EventosModel> eventos = new List<EventosModel>();

            connection.Open();
            var command = new OdbcCommand("SELECT even_evento, id_eventos, even_docum, even_receptor, even_identif, even_fecha, even_evento, even_xmlb64, even_codigo, even_response, even_qrcode, even_cufe " +
                                         "FROM eventos WHERE date(even_fecha) BETWEEN ? AND ?", connection);
            command.Parameters.AddWithValue("?", desde);
            command.Parameters.AddWithValue("?", hasta);

            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    eventos.Add(new EventosModel
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
                    });
                }
            }
            connection.Close();

            return eventos;
        }

        // Método para guardar el evento en la base de datos
        public static void PostEvento(string prefijoFactura, string codigo, string fecha, string xMLBase64, string evento, List<string> datos, string emisor, string identificacion, string qRCode, string cufe)
        {
            var connection = Conexion.ObtenerConexion();
            connection.Open();
            var command = new OdbcCommand("INSERT INTO eventos (`even_docum`, `even_receptor`, `even_identif`, `even_fecha`, `even_evento`, `even_xmlb64`, `even_codigo`, `even_response`, `even_qrcode`, `even_cufe`) " +
                                         "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)", connection);
            command.Parameters.AddWithValue("?", prefijoFactura);
            command.Parameters.AddWithValue("?", emisor);
            command.Parameters.AddWithValue("?", identificacion);
            command.Parameters.AddWithValue("?", fecha);
            command.Parameters.AddWithValue("?", evento);
            command.Parameters.AddWithValue("?", xMLBase64);
            command.Parameters.AddWithValue("?", codigo);
            command.Parameters.AddWithValue("?", datos[0]);
            command.Parameters.AddWithValue("?", qRCode);
            command.Parameters.AddWithValue("?", cufe);

            command.ExecuteNonQuery();
            connection.Close();
        }
    }
}
