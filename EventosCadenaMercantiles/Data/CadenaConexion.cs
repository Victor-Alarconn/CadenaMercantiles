
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;


namespace EventosCadenaMercantiles.Datos
{
    public class CadenaConexion
    {

        private static readonly string archivoConexion = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "env.txt");

        // Método para leer la configuración de la conexión desde el archivo
        public static List<string> ReadConexion()
        {
            List<string> con = new List<string>();

            try
            {
                if (!File.Exists(archivoConexion))
                {
                    MessageBox.Show("El archivo de configuración inicial no existe. Se requiere configuración.",
                                    "Error de configuración", MessageBoxButton.OK, MessageBoxImage.Error);
                    return con; // Devuelve lista vacía
                }

                using (StreamReader sr = new StreamReader(archivoConexion))
                {
                    string linea;
                    while ((linea = sr.ReadLine()) != null)
                    {
                        con.Add(linea);
                    }
                }

                // Validar si el archivo tiene los datos completos
                if (con.Count < 4)
                {
                    MessageBox.Show("El archivo de configuración está incompleto. Verifique los datos.",
                                    "Error de configuración", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return con; 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al leer el archivo de configuración: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return con;
        }




        // Método para escribir la configuración de la conexión en el archivo
        public static void WriteConexion(string ip, string baseda, string usuario, string clave)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(archivoConexion))
                {
                    sw.WriteLine(usuario);
                    sw.WriteLine(clave);
                    sw.WriteLine(ip);
                    sw.WriteLine(baseda);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al escribir en el archivo de configuración: {ex.Message}");
            }
        }

    }
}
