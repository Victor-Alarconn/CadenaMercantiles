using MySqlConnector;
using System;
using System.Collections.Generic;
using System.IO;

namespace EventosCadenaMercantiles.Datos
{
    public class CadenaConexion
    {
        // Cadena de conexión para la base de datos
        private readonly string _connectionString;

        public CadenaConexion()
        {
            // Aquí puedes modificar la base de datos según sea necesario
            _connectionString = "Database=a059; Data Source=192.168.1.150; User Id=RmSoft20X; Password=*LiLo89*; ConvertZeroDateTime=True;";

            // Verificación si la cadena de conexión es válida
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("La cadena de conexión no está configurada correctamente.");
            }
        }

        // Método para crear una nueva conexión MySQL
        public MySqlConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
