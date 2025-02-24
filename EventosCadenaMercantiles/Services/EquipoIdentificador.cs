using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

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

    }
}
