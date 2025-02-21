using System;
using System.Windows;

namespace EventosCadenaMercantiles.Vistas
{
    public partial class VentanaClaveVista : Window
    {
        public VentanaClaveVista()
        {
            InitializeComponent();
        }

        private void CerrarAplicacion(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); // Cierra la aplicación
        }
    }
}
