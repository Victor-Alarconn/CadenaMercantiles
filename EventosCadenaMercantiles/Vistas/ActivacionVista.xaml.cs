using System.Windows;

namespace EventosCadenaMercantiles.Vistas
{
    public partial class ActivacionVista : Window
    {
        public ActivacionVista()
        {
            InitializeComponent();
        }

        private void BtnActivar_Click(object sender, RoutedEventArgs e)
        {
            // Lógica para el botón de Activar
            MessageBox.Show("Activación exitosa!");
        }

        private void Txtmodulos_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Lógica para lo que debe hacer cuando el texto cambia en el TextBox de módulos
            // Por ejemplo, habilitar/deshabilitar botones o realizar validaciones
        }

        private void Txtmac_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
