using MySqlConnector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventosCadenaMercantiles.Services
{
    public class RecepcionService
    {
        private static readonly string _connectionString = ObtenerCadenaConexion();

        private static string ObtenerCadenaConexion()
        {
            string archivoConexion = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "datos.txt");
            var lineas = File.ReadAllLines(archivoConexion);
            return lineas.FirstOrDefault(l => l.StartsWith("MySqlConnectionRecepcion="))?
                          .Substring("MySqlConnectionRecepcion=".Length).Trim()
                   ?? throw new InvalidOperationException("No se encontró la conexión.");
        }

        private static MySqlConnection CrearConexion()
        {
            return new MySqlConnection(_connectionString);
        }

        public static bool ExisteDocumentoEnValidacion(string documentId)
        {
            using (var connection = CrearConexion())
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM doc_recepcion WHERE document_id = @document_id AND estado = 0";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@document_id", documentId);
                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }

        public static void MarcarComoConsultado(string documentId)
        {
            using (var connection = CrearConexion())
            {
                connection.Open();
                string query = "UPDATE doc_recepcion SET estado = 1 WHERE document_id = @document_id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@document_id", documentId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}


