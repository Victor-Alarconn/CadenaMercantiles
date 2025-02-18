using System;
using System.Windows;

namespace EventosCadenaMercantiles.Vistas
{
    public partial class DatosEmpresaVista : Window
    {
        public DatosEmpresaVista()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Aquí puedes agregar la lógica para guardar los datos
            MessageBox.Show("Datos guardados correctamente", "Guardar", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Actualizardb_Click(object sender, RoutedEventArgs e)
        {
            // Aquí puedes agregar la lógica para actualizar la base de datos
            MessageBox.Show("Base de datos actualizada", "Actualizar", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
