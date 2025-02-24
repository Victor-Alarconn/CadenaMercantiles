using EventosCadenaMercantiles.Data;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EventosCadenaMercantiles.Services
{
    public class DatosEmpresaService
    {

        public static bool GuardarEmpresa(string nit, string nombre, string nombre2)
        {
            try
            {
                using (var connection = Conexion.ObtenerConexion())
                {
                    connection.Open();

                    string query = "INSERT INTO `datosempresa` (`dt_nit`, `dt_nombre`, `dt_nombre2`) " +
                                   "VALUES (?, ?, ?)";

                    using (var command = new OdbcCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("?", nit);
                        command.Parameters.AddWithValue("?", nombre);
                        command.Parameters.AddWithValue("?", string.IsNullOrEmpty(nombre2) ? "" : nombre2);

                        int filasAfectadas = command.ExecuteNonQuery();

                        return filasAfectadas > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la empresa: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }


        private static readonly string archivoConexion = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "env.txt");

        public static bool GuardarEnv(string ip, string code)
        {
            try
            {
                // Verificar si el archivo existe
                if (!File.Exists(archivoConexion))
                {
                    MessageBox.Show("El archivo env.txt no existe.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                // Leer el contenido actual del archivo
                string[] lineas = File.ReadAllLines(archivoConexion, Encoding.UTF8);

                // Agregar IP y Code como nuevas líneas
                using (StreamWriter writer = new StreamWriter(archivoConexion, false, Encoding.UTF8))
                {
                    foreach (string linea in lineas)
                    {
                        writer.WriteLine(linea);
                    }
                    writer.WriteLine(ip);
                    writer.WriteLine(code);
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar en env.txt: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
