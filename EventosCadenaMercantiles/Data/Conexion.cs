using EventosCadenaMercantiles.Datos;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventosCadenaMercantiles.Data
{
    public class Conexion
    {
        private static List<string> datosconexion;

        public static OdbcConnection ObtenerConexion()
        {
            datosconexion = CadenaConexion.ReadConexion();

            // Si el archivo `env.txt` solo tiene usuario y contraseña pero no el servidor ni la base de datos
            if (string.IsNullOrWhiteSpace(datosconexion[2]) || string.IsNullOrWhiteSpace(datosconexion[3]))
            {
                throw new Exception("Empresa nueva detectada. Se requiere configuración de conexión.");
                // Aquí podrías lanzar un evento o redirigir a una vista de configuración.
            }

            string MyConString = $"DRIVER={{MySQL ODBC 5.1 Driver}};" +
                                 $"SERVER={datosconexion[2]};" +
                                 $"DATABASE={datosconexion[3]};" +
                                 $"UID={datosconexion[0]};" +
                                 $"PASSWORD={datosconexion[1]};" +
                                 $"Option=3;";

            return new OdbcConnection(MyConString);
        }

    }
}
