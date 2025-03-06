using EventosCadenaMercantiles.Datos;
using EventosCadenaMercantiles.Services;
using EventosCadenaMercantiles.Vistas;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
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

            var splash = new SplashScreenWindow();
            splash.Show();

            Task.Run(() =>
            {
                Thread.Sleep(1000); // Pequeño delay visual

                List<string> datosConexion = CadenaConexion.ReadConexion();

                Dispatcher.Invoke(() =>
                {
                    if (datosConexion.Count == 0 || datosConexion.Count < 4 || string.IsNullOrWhiteSpace(datosConexion[0]) || string.IsNullOrWhiteSpace(datosConexion[1]))
                    {
                        splash.Close();
                        MessageBox.Show("El archivo de configuración inicial no está configurado. Por favor verifique con su proveedor.",
                                        "Error de configuración", MessageBoxButton.OK, MessageBoxImage.Error);
                        new DatosEmpresaVista().Show();
                        MainWindow = new DatosEmpresaVista(); // IMPORTANTE: esto hace que la app siga viva
                        return;
                    }

                    string Mac = EquipoIdentificador.GetUniqueIdentifier();
                    EstadoEmpresa estado = EmpresaService.VerificarEmpresa(Mac, datosConexion[3]);

                    Window ventanaPrincipal;

                    if (estado == EstadoEmpresa.Activa)
                    {
                        ventanaPrincipal = new Home();
                    }
                    else if (estado == EstadoEmpresa.Suspendida)
                    {
                        ventanaPrincipal = new VentanaClaveVista();
                    }
                    else
                    {
                        ventanaPrincipal = new ActivacionVista();
                    }

                    splash.Close();
                    ventanaPrincipal.Show();

                    // IMPORTANTE: definir la MainWindow evita que la app cierre
                    MainWindow = ventanaPrincipal;
                });
            });
        }


    }
}
