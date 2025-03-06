using MySqlConnector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EventosCadenaMercantiles.Services
{
    public class RecepcionService
    {
        private static readonly string _connectionString = ObtenerCadenaConexion();

        private static string ObtenerCadenaConexion()
        {
            string archivoConexion = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "datos.txt");

            if (!File.Exists(archivoConexion))
            {
                MessageBox.Show("El archivo 'datos.txt' no fue encontrado en el directorio de la aplicación.", "Error de configuración", MessageBoxButton.OK, MessageBoxImage.Error);
                
                return string.Empty;
            }

            var lineas = File.ReadAllLines(archivoConexion);
            string conexion = string.Empty;

            foreach (var linea in lineas)
            {
                if (linea.StartsWith("MySqlConnectionRecepcion="))
                {
                    conexion = linea.Substring("MySqlConnectionRecepcion=".Length).Trim();
                    break;
                }
            }

            if (string.IsNullOrWhiteSpace(conexion))
            {
                MessageBox.Show("No se encontró la cadena de conexión 'MySqlConnectionRecepcion' en el archivo 'datos.txt'.", "Error de configuración", MessageBoxButton.OK, MessageBoxImage.Error);
                return string.Empty;
            }

            return conexion;
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


