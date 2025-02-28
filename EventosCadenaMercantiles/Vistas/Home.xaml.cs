using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

    }

}
