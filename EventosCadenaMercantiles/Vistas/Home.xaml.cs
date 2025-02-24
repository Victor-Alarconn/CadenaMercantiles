using System;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using EventosCadenaMercantiles.ViewModels;


namespace EventosCadenaMercantiles.Vistas
{
    public partial class Home : Window
    {
        public HomeViewModel ViewModel { get; }
        public Home()
        {
            InitializeComponent();
            ViewModel = new HomeViewModel();
            DataContext = ViewModel;
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
