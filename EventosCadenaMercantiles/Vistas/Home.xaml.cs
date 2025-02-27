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
        // Método para manejar el clic en el botón de búsqueda
        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            // Aquí se puede agregar la lógica de búsqueda si es necesario en el futuro
        }

        // Método para manejar el cambio de selección del ComboBox "Filtro de evento"
        private void Filtroevento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Aquí se puede agregar la lógica para filtrar por evento si es necesario en el futuro
        }

        // Método para manejar el cambio de selección del ComboBox "Filtro de código"
        private void Filtrocodigo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Aquí se puede agregar la lógica para filtrar por código si es necesario en el futuro
        }

        private void EventosDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void EventosDataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Cbevento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedEvent = (ComboBoxItem)Cbevento.SelectedItem;
            // Lógica para manejar la selección del evento
            MessageBox.Show($"Evento seleccionado: {selectedEvent.Content}");
        }



    }

}
