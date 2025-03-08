using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using EventosCadenaMercantiles.Modelos;
using EventosCadenaMercantiles.Services;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Series;

namespace EventosCadenaMercantiles.ViewModels
{
    public class InformesEventosViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<EventosModel> _eventos;
        private PlotModel _plotModel;

        public ObservableCollection<EventosModel> Eventos
        {
            get => _eventos;
            set
            {
                if (_eventos != value)
                {
                    _eventos = value;
                    OnPropertyChanged(nameof(Eventos));
                    UpdatePlotModel();
                }
            }
        }

        public PlotModel PlotModel
        {
            get => _plotModel;
            private set
            {
                _plotModel = value;
                OnPropertyChanged(nameof(PlotModel));
            }
        }

        public InformesEventosViewModel()
        {
            Eventos = new ObservableCollection<EventosModel>();
            InitializePlotModel();
            // Cargar todos los eventos hasta la fecha actual
            CargarEventos(DateTime.MinValue, DateTime.Now);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void InitializePlotModel()
        {
            _plotModel = new PlotModel { Title = "Distribución de Eventos" };
            _plotModel.Series.Add(new PieSeries());
        }

        private void UpdatePlotModel()
        {
            if (_plotModel != null && _plotModel.Series[0] is PieSeries series)
            {
                series.Slices.Clear();

                // Verifica que Eventos no sea null antes de intentar operaciones sobre él
                if (Eventos != null)
                {
                    var groupedData = Eventos.GroupBy(e => e.EvenEvento)
                                             .Select(g => new { Evento = g.Key, Count = g.Count() });

                    foreach (var data in groupedData)
                    {
                        series.Slices.Add(new PieSlice(data.Evento, data.Count) { IsExploded = true });
                    }

                    // Solo actualiza el plot si hay datos para mostrar
                    if (groupedData.Any())
                    {
                        _plotModel.InvalidatePlot(true);
                    }
                }
            }
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
