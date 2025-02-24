using EventosCadenaMercantiles.Datos;
using EventosCadenaMercantiles.Services;
using EventosCadenaMercantiles.Vistas;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using static EventosCadenaMercantiles.Services.EmpresaService;

namespace EventosCadenaMercantiles
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            List<string> datosConexion = CadenaConexion.ReadConexion();

            if (datosConexion.Count == 0)
            {
                MessageBox.Show("El archivo de configuración inicial no esta configurado. Por favor verifique con su proveedor.",
                                    "Error de configuración", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validar que los datos esenciales estén completos
            if (datosConexion.Count < 4 || string.IsNullOrWhiteSpace(datosConexion[0]) || string.IsNullOrWhiteSpace(datosConexion[1]))
            {
                // Si faltan datos, abrir la vista de activación
                new DatosEmpresaVista().Show();
                return;
            }

           string Mac = EquipoIdentificador.GetUniqueIdentifier();
            // Verificar el estado de la empresa
            EstadoEmpresa estado = EmpresaService.VerificarEmpresa(Mac, datosConexion[3]);

            if (estado == EstadoEmpresa.Activa)
            {
                new Home().Show(); // Empresa activa → Mostrar ventana principal
            }
            else if (estado == EstadoEmpresa.Suspendida)
            {
               // MessageBox.Show("El servicio se encuentra inactivo", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                new VentanaClaveVista().Show();
            }
            else
            {
                new ActivacionVista().Show(); // Empresa no registrada → Mostrar activación
            }
        }

    }
}
