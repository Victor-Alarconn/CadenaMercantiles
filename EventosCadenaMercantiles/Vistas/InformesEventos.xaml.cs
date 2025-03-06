using System;
using System.Windows;
using System.Windows.Controls;
using EventosCadenaMercantiles.ViewModels;

namespace EventosCadenaMercantiles.Vistas
{
    public partial class InformesEventos : Window
    {
        public InformesEventosViewModel ViewModel { get; }

        public InformesEventos()
        {
            InitializeComponent();
            ViewModel = new InformesEventosViewModel();
            DataContext = ViewModel;
        }

        // Método para manejar el evento click del botón de filtro
        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = dpStartDate.SelectedDate ?? DateTime.MinValue;
            DateTime endDate = dpEndDate.SelectedDate ?? DateTime.MaxValue;
            string tipoEvento = (cbTipoEvento.SelectedItem as ComboBoxItem)?.Content.ToString();

            ViewModel.FiltrarEventos(startDate, endDate, tipoEvento);
        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ExportarEventos();
        }

    }
}
