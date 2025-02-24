using EventosCadenaMercantiles.Data;
using EventosCadenaMercantiles.Modelos;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EventosCadenaMercantiles.Services
{
    public class EmpresaService
    {
        // Obtener empresa por clave
        public static DatosEmpresaModel ObtenerEmpresaPorClave(string clave)
        {
            using (var connection = Conexion.ObtenerConexion())
            {
                connection.Open();
                string query = "SELECT dt_nit, dt_nombre FROM datosempresa WHERE dt_clave = ?";

                using (var command = new OdbcCommand(query, connection))
                {
                    command.Parameters.AddWithValue("?", clave);

                    using (var reader = command.ExecuteReader())
                    {
                        return reader.Read()
                            ? new DatosEmpresaModel
                            {
                                DtNit = reader.GetString(0),
                                DtNombre = reader.GetString(1)
                            }
                            : null; // Si no se encuentra la empresa
                    }
                }
            }
        }

        // Guardar clave en la base de datos
        //public static void GuardarClave(string nit, string clave, string empresa)
        //{
        //    using (var conexionPrincipal = Conexion.ObtenerConexion())
        //    {
        //        GuardarClaveEnServidor(conexionPrincipal, nit, clave);
        //    }

        //    using (var conexionSecundaria = Conexion.ObtenerConexionServidorSecundario(empresa))
        //    {
        //        if (conexionSecundaria != null)
        //        {
        //            GuardarClaveEnServidor(conexionSecundaria, nit, clave);
        //        }
        //        else
        //        {
        //            Console.WriteLine($"No se pudo conectar al servidor secundario para la empresa: {empresa}");
        //        }
        //    }
        //}

        // Método reutilizable para actualizar o insertar la clave
        private static void GuardarClaveEnServidor(OdbcConnection connection, string nit, string clave)
        {
            try
            {
                connection.Open();
                DatabaseInitializer.CrearTablasSiNoExisten(connection);

                string query = "UPDATE datosempresa SET dt_clave = ? WHERE dt_nit = ?";
                using (var command = new OdbcCommand(query, connection))
                {
                    command.Parameters.AddWithValue("?", clave);
                    command.Parameters.AddWithValue("?", nit);

                    if (command.ExecuteNonQuery() == 0)
                    {
                        // Si no se actualizó, significa que la empresa no existe, entonces insertamos
                        using (var insertarCommand = new OdbcCommand(
                            "INSERT INTO datosempresa (dt_nit, dt_clave) VALUES (?, ?)", connection))
                        {
                            insertarCommand.Parameters.AddWithValue("?", nit);
                            insertarCommand.Parameters.AddWithValue("?", clave);
                            insertarCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar la clave: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }


        public enum EstadoEmpresa
        {
            NoRegistrada,
            Suspendida,
            Activa
        }


        public static EstadoEmpresa VerificarEmpresa(string mac, string codigo)
        {
            try
            {
                using (var connection = Conexion.ObtenerConexion())
                {
                    connection.Open();

                    string query = "SELECT activar, fsuspende FROM empresas.llequipo WHERE empresa = ? AND nro_mac = ? AND modulos LIKE '%M14%'";

                    using (var command = new OdbcCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("?", codigo);
                        command.Parameters.AddWithValue("?", mac);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string activar = reader.GetString(0); // "SI" o "NO"
                                DateTime fechaSuspension = reader.GetDateTime(1);
                                DateTime hoy = DateTime.Now;
                                TimeSpan diferencia = fechaSuspension - hoy;

                                // Si la empresa ya está suspendida
                                if (fechaSuspension < hoy)
                                {
                                    return EstadoEmpresa.Suspendida;
                                }

                                // Mostrar advertencia si faltan 3 días o menos para la suspensión
                                if (diferencia.TotalDays <= 3)
                                {
                                    MessageBox.Show("Su cuenta está cerca de quedar suspendida. Realice el pago a tiempo.",
                                                    "Aviso de suspensión", MessageBoxButton.OK, MessageBoxImage.Warning);
                                }

                                return activar == "SI" ? EstadoEmpresa.Activa : EstadoEmpresa.NoRegistrada;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar la empresa: {ex.Message}");
            }

            return EstadoEmpresa.NoRegistrada; // Si no se encuentra o hay error, asumir que no está registrada
        }

    }
}
