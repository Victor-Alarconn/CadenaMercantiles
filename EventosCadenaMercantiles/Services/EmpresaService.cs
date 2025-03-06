using EventosCadenaMercantiles.Data;
using EventosCadenaMercantiles.Modelos;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EventosCadenaMercantiles.Services
{
    public class EmpresaService
    {

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

                    string query = "SELECT fsuspende FROM empresas.llequipo WHERE empresa = ? AND nro_mac = ? AND modulos LIKE '%M14%'";

                    using (var command = new OdbcCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("?", codigo);
                        command.Parameters.AddWithValue("?", mac);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string fechaStr = reader.GetString(0); // Fecha en formato "2025-03-24"

                                // Convertir la fecha desde string a DateTime en formato correcto
                                DateTime fechaSuspension = DateTime.ParseExact(fechaStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);
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

                                return EstadoEmpresa.Activa; // Si la fecha es válida y aún no ha vencido
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

        public static bool GuardarEquipo(string empresa, string nroMac, DateTime factivar, string modulos, string usuario)
        {
            try
            {
                using (var connection = Conexion.ObtenerConexion())
                {
                    connection.Open();

                    string query = @"INSERT INTO empresas.llequipo 
                            (empresa, nro_mac, maquina, factivar, fsuspende, modulos, usuario) 
                            VALUES (?, ?, ?, ?, ?, ?, ?)";

                    using (var command = new OdbcCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("?", empresa);
                        command.Parameters.AddWithValue("?", nroMac);
                        command.Parameters.AddWithValue("?", Environment.MachineName); // Nombre del equipo
                        command.Parameters.AddWithValue("?", factivar.ToString("yyyy-MM-dd")); // Formato de fecha
                        command.Parameters.AddWithValue("?", factivar.AddMonths(1).ToString("yyyy-MM-dd")); // fsuspende = fecha + 1 mes
                        command.Parameters.AddWithValue("?", modulos);
                        command.Parameters.AddWithValue("?", usuario);

                        int filasAfectadas = command.ExecuteNonQuery();
                        return filasAfectadas > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el equipo: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }


    }
}
