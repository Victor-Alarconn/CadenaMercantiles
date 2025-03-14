using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EventosCadenaMercantiles.Vistas
{
    /// <summary>
    /// Lógica de interacción para ClaveLocal.xaml
    /// </summary>
    public partial class ClaveLocal : Window
    {
        public ClaveLocal()
        {
            InitializeComponent();
        }

        private void Btnclave_Click(object sender, RoutedEventArgs e)
        {
            if (txtclave.Password == "*M0D4L0S*") // Verifica que la clave sea correcta
            {
                // Oculta los controles de la clave y muestra el DatePicker
                txtclave.Visibility = Visibility.Collapsed;
                (sender as Button).Visibility = Visibility.Collapsed; // Oculta el botón

                // Muestra el DatePicker
                datePickerFecha.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Clave incorrecta. Intente nuevamente.", "Acceso Denegado", MessageBoxButton.OK, MessageBoxImage.Error);
                txtclave.Clear(); // Limpia el campo de contraseña para un nuevo intento
            }
        }

    }
}
