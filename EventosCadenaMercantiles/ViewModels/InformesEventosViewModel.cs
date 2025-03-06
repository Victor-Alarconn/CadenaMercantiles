using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using EventosCadenaMercantiles.Modelos;
using EventosCadenaMercantiles.Services;
using Microsoft.Win32;

namespace EventosCadenaMercantiles.ViewModels
{
    public class InformesEventosViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<EventosModel> _eventos;

        public ObservableCollection<EventosModel> Eventos
        {
            get => _eventos;
            set
            {
                if (_eventos != value)
                {
                    _eventos = value;
                    OnPropertyChanged(nameof(Eventos));
                }
            }
        }

        public InformesEventosViewModel()
        {
            Eventos = new ObservableCollection<EventosModel>();
            // Cargar todos los eventos hasta la fecha actual
            CargarEventos(DateTime.MinValue, DateTime.Now);  // Ajusta las fechas como consideres necesario
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void CargarEventos(DateTime desde, DateTime hasta)
        {
            var eventosCargados = EventosService.ObtenerEventos(desde, hasta);
            Eventos = new ObservableCollection<EventosModel>(eventosCargados);
        }

        public void FiltrarEventos(DateTime startDate, DateTime endDate, string tipoEvento)
        {
            var eventosFiltrados = EventosService.ObtenerEventos(startDate, endDate)
                                                 .Where(e => e.EvenEvento == tipoEvento)
                                                 .ToList();
            Eventos = new ObservableCollection<EventosModel>(eventosFiltrados);
        }

        public void ExportarEventos()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                Title = "Guardar archivo de Excel"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                EventosService.ExportarEventosAExcel(Eventos, saveFileDialog.FileName);
            }
        }

    }
}
