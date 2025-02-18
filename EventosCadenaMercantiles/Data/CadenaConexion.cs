//using System;
//using System.Collections.Generic;
//using System.IO;

//namespace EventosCadenaMercantiles.Datos
//{
//    public class CadenaConexion
//    {
//        private static string archivoConexion = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "env.txt");

//        // Método para leer la configuración de la conexión desde el archivo
//        public static List<string> ReadConexion()
//        {
//            List<string> con = new();

//            try
//            {
//                // Verificar si el archivo existe
//                if (File.Exists(archivoConexion))
//                {
//                    using (StreamReader sr = new(archivoConexion))
//                    {
//                        con.Add(sr.ReadLine());
//                        con.Add(sr.ReadLine());
//                        con.Add(sr.ReadLine());
//                    }
//                }
//                else
//                {
//                    throw new FileNotFoundException("El archivo de configuración no se encuentra.");
//                }
//            }
//            catch (Exception ex)
//            {
//                // Manejo de errores (archivo no encontrado o problemas al leerlo)
//                Console.WriteLine($"Error al leer el archivo de configuración: {ex.Message}");
//            }

//            return con;
//        }

//        // Método para escribir la configuración de la conexión en el archivo
//        public static void WriteConexion(string ip, string baseda, string codigo)
//        {
//            try
//            {
//                using (StreamWriter sw = new(archivoConexion))
//                {
//                    // Escribir las líneas de configuración en el archivo
//                    sw.WriteLine(ip);
//                    sw.WriteLine(baseda);
//                    sw.WriteLine(codigo);
//                }
//            }
//            catch (Exception ex)
//            {
//                // Manejo de errores (problemas al escribir el archivo)
//                Console.WriteLine($"Error al escribir en el archivo de configuración: {ex.Message}");
//            }
//        }
//    }
//}
