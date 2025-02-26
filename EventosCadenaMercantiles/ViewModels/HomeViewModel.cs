using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.IO;

namespace EventosCadenaMercantiles.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        private string _nombreArchivo;
        private bool _popupEventoAbierto;
        private bool _popupCoRechazoAbierto;
        private string _textoEvento;
        private string _textoCoRechazo;

        public string NombreArchivo
        {
            get => _nombreArchivo;
            set => SetProperty(ref _nombreArchivo, value);
        }

        public bool PopupEventoAbierto
        {
            get => _popupEventoAbierto;
            set => SetProperty(ref _popupEventoAbierto, value);
        }

        public bool PopupCoRechazoAbierto
        {
            get => _popupCoRechazoAbierto;
            set => SetProperty(ref _popupCoRechazoAbierto, value);
        }

        public string TextoEvento
        {
            get => _textoEvento;
            set => SetProperty(ref _textoEvento, value);
        }

        public string TextoCoRechazo
        {
            get => _textoCoRechazo;
            set => SetProperty(ref _textoCoRechazo, value);
        }

        public ICommand AbrirArchivoCommand { get; }
        public ICommand RefrescarVistaCommand { get; }
        public ICommand CerrarVentanaCommand { get; }
        public ICommand EnviarXmlCommand { get; }
        public ICommand TogglePopupEventoCommand { get; }
        public ICommand TogglePopupCoRechazoCommand { get; }
        public ICommand SeleccionarEventoCommand { get; }
        public ICommand SeleccionarCoRechazoCommand { get; }
        public ICommand CerrarPopupsCommand { get; }

        public class RelayCommand<T> : ICommand
        {
            private readonly Action<T> _execute;
            private readonly Predicate<T> _canExecute;

            public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter) => _canExecute == null || _canExecute((T)parameter);

            public void Execute(object parameter) => _execute((T)parameter);

            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }
        }



        public HomeViewModel()
        {
            AbrirArchivoCommand = new RelayCommand(param => AbrirArchivo());
            RefrescarVistaCommand = new RelayCommand(param => RefrescarVista());
            CerrarVentanaCommand = new RelayCommand(param => CerrarVentana());
            EnviarXmlCommand = new RelayCommand(param => EnviarXml());
            TogglePopupEventoCommand = new RelayCommand(param => PopupEventoAbierto = !PopupEventoAbierto);
            TogglePopupCoRechazoCommand = new RelayCommand(param => PopupCoRechazoAbierto = !PopupCoRechazoAbierto);
            SeleccionarEventoCommand = new RelayCommand<string>(param => SeleccionarEvento(param));
            SeleccionarCoRechazoCommand = new RelayCommand<string>(param => SeleccionarCoRechazo(param));
            CerrarPopupsCommand = new RelayCommand(param => CerrarPopups());
        }



        private void AbrirArchivo()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Archivos XML (*.xml)|*.xml"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                NombreArchivo = Path.GetFileName(openFileDialog.FileName);
                ProcesarXml(openFileDialog.FileName);
            }
        }
         
        private void ProcesarXml(string filePath)
        {
            try
            {
                MessageBox.Show($"Archivo seleccionado: {filePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al procesar el archivo: {ex.Message}");
            }
        }

        private void RefrescarVista()
        {
            MessageBox.Show("Vista refrescada.");
        }

        private void CerrarVentana()
        {
            Application.Current.MainWindow.Close();
        }

        private void EnviarXml()
        {
            MessageBox.Show("Enviando el documento XML...");
        }

        private void SeleccionarEvento(string evento)
        {
            TextoEvento = evento;
            PopupEventoAbierto = false;
        }

        private void SeleccionarCoRechazo(string corechazo)
        {
            TextoCoRechazo = corechazo;
            PopupCoRechazoAbierto = false;
        }

        private void CerrarPopups()
        {
            PopupEventoAbierto = false;
            PopupCoRechazoAbierto = false;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

    }
}
