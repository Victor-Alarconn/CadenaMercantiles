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
using System.Windows.Input;
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
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            this.Exit += App_Exit;
            this.SessionEnding += App_SessionEnding;

            EventManager.RegisterClassHandler(typeof(Window), Window.PreviewKeyDownEvent, new KeyEventHandler(GlobalPreviewKeyDown));

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

        private void GlobalPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                ClaveLocal claveLocal = new ClaveLocal();
                claveLocal.Owner = Current.MainWindow;
                claveLocal.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                claveLocal.ShowDialog();
            }
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // Manejo de excepciones no controladas
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            // Limpieza al salir de la aplicación
        }

        private void App_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            // Acciones al cerrar sesión en el sistema operativo
        }
    }
}
