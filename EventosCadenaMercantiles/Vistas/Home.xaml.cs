using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using EventosCadenaMercantiles.ViewModels;

namespace EventosCadenaMercantiles.Vistas
{
    public partial class Home : Window
    {
        public HomeViewModel ViewModel { get; }  // Definir el HomeViewModel

        public Home()
        {
            InitializeComponent();
            ViewModel = new HomeViewModel();  // Inicializar el ViewModel de Home
            DataContext = ViewModel;  // Asignar el DataContext al ViewModel de Home

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove(); // Permite mover la ventana al hacer clic y arrastrar
            }
        }

        private void OpenInformesEventosView(object sender, RoutedEventArgs e)
        {
            InformesEventos informesEventosWindow = new InformesEventos();
            informesEventosWindow.Show();
        }

        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            // Establecer el texto del placeholder solo si no hay texto ya presente
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Foreground = new SolidColorBrush(Colors.Gray);
                textBox.Text = "Buscar por Nombre, identificación, o documento";
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            // Borrar el texto del placeholder al enfocar si el texto es el del placeholder
            if (textBox.Text == "Buscar por Nombre, identificación, o documento")
            {
                textBox.Text = "";
                textBox.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            // Restablecer el texto del placeholder solo si el TextBox está vacío
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Foreground = new SolidColorBrush(Colors.Gray);
                textBox.Text = "Buscar por Nombre, identificación, o documento";
            }
        }




    }

}
