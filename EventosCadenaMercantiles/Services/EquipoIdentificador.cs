using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EventosCadenaMercantiles.Services
{
    public class EquipoIdentificador
    {
        public static string GetMacAddress()
        {
            try
            {
                string mac = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    .Select(nic => nic.GetPhysicalAddress().ToString())
                    .FirstOrDefault();

                return !string.IsNullOrEmpty(mac) ? mac : "No disponible";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la MAC: {ex.Message}");
                return "Error";
            }
        }

        public static string GetSerialNumber()
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_DiskDrive"))
                {
                    foreach (ManagementObject wmi_HD in searcher.Get())
                    {
                        string serial = wmi_HD["SerialNumber"]?.ToString().Trim();
                        if (!string.IsNullOrEmpty(serial))
                        {
                            return serial;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el número de serie: {ex.Message}");
            }

            return "Desconocido";
        }

        public static string GetUniqueIdentifier()
        {
            string mac = GetMacAddress();
            return mac != "No disponible" && mac != "Error" ? mac : GetSerialNumber();
        }

        private static readonly string archivoConexion = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "env.txt");

        public static string ObtenerEmpresa()
        {
            try
            {
                if (File.Exists(archivoConexion))
                {
                    string[] lineas = File.ReadAllLines(archivoConexion);

                    // Verificamos que existan al menos 4 líneas antes de acceder a la cuarta
                    if (lineas.Length >= 4)
                    {
                        return lineas[3]; // La cuarta línea (índice 3 porque comienza en 0)
                    }
                    else
                    {
                        MessageBox.Show("El archivo env.txt no contiene suficientes datos.", "Advertencia",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("El archivo env.txt no existe.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al leer el archivo env.txt: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return string.Empty; // Retorna una cadena vacía en caso de error
        }

    }
}
