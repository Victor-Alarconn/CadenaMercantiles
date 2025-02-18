//using System;
//using System.Data.Odbc;
//using EventosCadenaMercantiles.Modelos;

//namespace EventosCadenaMercantiles.Datos
//{
//    public class DatosDB
//    {
//        // Método para obtener una empresa por clave
//        public static DatosEmpresaModel ObtenerEmpresaPorClave(string clave)
//        {
//            using (var connection = Conexion.ObtenerConexion())
//            {
//                connection.Open();
//                string query = "SELECT dt_nit, dt_nombre FROM datosempresa WHERE dt_clave = ?";
//                using (var command = new OdbcCommand(query, connection))
//                {
//                    command.Parameters.AddWithValue("?", clave);
//                    using (var reader = command.ExecuteReader())
//                    {
//                        if (reader.Read())
//                        {
//                            return new DatosEmpresaModel
//                            {
//                                DtNit = reader.GetString(0),
//                                DtNombre = reader.GetString(1)
//                            };
//                        }
//                    }
//                }
//            }
//            return null; // Si no se encuentra una empresa con esa clave
//        }

//        // Crear tablas si no existen
//        public static void CreateTablas()
//        {
//            try
//            {
//                var connection = Conexion.ObtenerConexion();
//                connection.Open();

//                // Crear tabla 'datosempresa' si no existe
//                new OdbcCommand("CREATE TABLE IF NOT EXISTS `datosempresa` (" +
//                    "  `idDtEmp` INT(11) NOT NULL AUTO_INCREMENT," +
//                    "  `dt_token` CHAR(128) NULL DEFAULT ''," +
//                    "  `dt_url` CHAR(128) NULL DEFAULT ''," +
//                    "  `dt_nit` CHAR(14) NULL DEFAULT ''," +
//                    "  `dt_nombre` CHAR(128) NULL DEFAULT ''," +
//                    "  `dt_nombre2` CHAR(128) NULL DEFAULT ''," +
//                    "  `dt_clave` CHAR(128) NULL DEFAULT ''," +
//                    "  PRIMARY KEY(`idDtEmp`));", connection).ExecuteNonQuery();

//                // Crear tabla 'eventos' si no existe
//                new OdbcCommand("CREATE TABLE IF NOT EXISTS `eventos` (" +
//                    "  `id_eventos` INT(11) NOT NULL AUTO_INCREMENT," +
//                    "  `even_docum` CHAR(128) NULL DEFAULT ''," +
//                    "  `even_receptor` CHAR(128) NULL DEFAULT ''," +
//                    "  `even_identif` CHAR(128) NULL DEFAULT ''," +
//                    "  `even_fecha` CHAR(128) NULL DEFAULT ''," +
//                    "  `even_evento` CHAR(45) NULL DEFAULT ''," +
//                    "  `even_cufe` CHAR(254) NULL DEFAULT ''," +
//                    "  `even_codigo` CHAR(45) NULL DEFAULT ''," +
//                    "  `even_response` TEXT NULL DEFAULT NULL," +
//                    "  `even_qrcode` VARCHAR(254) NULL DEFAULT ''," +
//                    "  `even_xmlb64` MEDIUMTEXT NULL DEFAULT NULL," +
//                    "  PRIMARY KEY(`id_eventos`));", connection).ExecuteNonQuery();

//                connection.Close();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Ocurrió un error: {ex.Message}");
//            }
//        }

//        // Método para guardar la clave
//        public static void GuardarClave(string nit, string clave, string empresa)
//        {
//            GuardarClaveEnServidor(Conexion.ObtenerConexion(), nit, clave);

//            using (var conexionSecundaria = Conexion.ObtenerConexionServidorSecundario(empresa))
//            {
//                if (conexionSecundaria != null)
//                {
//                    GuardarClaveEnServidor(conexionSecundaria, nit, clave);
//                }
//                else
//                {
//                    Console.WriteLine($"No se pudo conectar al servidor secundario para la empresa: {empresa}");
//                }
//            }
//        }

//        // Método reutilizable para guardar la clave en una conexión dada
//        private static void GuardarClaveEnServidor(OdbcConnection connection, string nit, string clave)
//        {
//            try
//            {
//                connection.Open();
//                CreateTablas();

//                string query = "UPDATE datosempresa SET dt_clave = ? WHERE dt_nit = ?";
//                using (var command = new OdbcCommand(query, connection))
//                {
//                    command.Parameters.AddWithValue("?", clave);
//                    command.Parameters.AddWithValue("?", nit);
//                    int filasAfectadas = command.ExecuteNonQuery();

//                    if (filasAfectadas == 0)
//                    {
//                        string insertarQuery = "INSERT INTO datosempresa (dt_nit, dt_clave) VALUES (?, ?)";
//                        using (var insertarCommand = new OdbcCommand(insertarQuery, connection))
//                        {
//                            insertarCommand.Parameters.AddWithValue("?", nit);
//                            insertarCommand.Parameters.AddWithValue("?", clave);
//                            insertarCommand.ExecuteNonQuery();
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error al guardar la clave en el servidor: {ex.Message}");
//            }
//            finally
//            {
//                connection.Close();
//            }
//        }
//    }
//}
