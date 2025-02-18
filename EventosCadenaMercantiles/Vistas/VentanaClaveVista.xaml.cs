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

        // Aquí debes agregar el manejador de eventos para Btnclave_Click
        private void Btnclave_Click(object sender, RoutedEventArgs e)
        {
            // Lógica para manejar el evento de hacer clic en el botón
            string clave = txtclave.Password.Trim();
            MessageBox.Show("Clave ingresada: " + clave);
        }
    }
}
