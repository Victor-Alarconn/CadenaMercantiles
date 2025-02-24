using EventosCadenaMercantiles.Data;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventosCadenaMercantiles.Services
{
    public static class DatabaseInitializer
    {
        public static void CrearTablasSiNoExisten()
        {
            try
            {
                using (var connection = Conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS datosempresa (
                        idDtEmp INT(11) NOT NULL AUTO_INCREMENT,
                        dt_token CHAR(128) NULL DEFAULT '',
                        dt_url CHAR(128) NULL DEFAULT '',
                        dt_nit CHAR(14) NULL DEFAULT '',
                        dt_nombre CHAR(128) NULL DEFAULT '',
                        dt_nombre2 CHAR(128) NULL DEFAULT '',
                        dt_clave CHAR(128) NULL DEFAULT '',
                        PRIMARY KEY (idDtEmp)
                    );";
                        command.ExecuteNonQuery();

                        command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS eventos (
                        id_eventos INT(11) NOT NULL AUTO_INCREMENT,
                        even_docum CHAR(128) NULL DEFAULT '',
                        even_receptor CHAR(128) NULL DEFAULT '',
                        even_identif CHAR(128) NULL DEFAULT '',
                        even_fecha CHAR(128) NULL DEFAULT '',
                        even_evento CHAR(45) NULL DEFAULT '',
                        even_cufe CHAR(254) NULL DEFAULT '',
                        even_codigo CHAR(45) NULL DEFAULT '',
                        even_response TEXT NULL DEFAULT NULL,
                        even_qrcode VARCHAR(254) NULL DEFAULT '',
                        even_xmlb64 MEDIUMTEXT NULL DEFAULT NULL,
                        PRIMARY KEY (id_eventos)
                    );";
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear las tablas: {ex.Message}");
            }
        }

    }
}
