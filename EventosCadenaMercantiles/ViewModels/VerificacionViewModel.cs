using EventosCadenaMercantiles.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EventosCadenaMercantiles.ViewModels
{
    public class VerificacionViewModel : INotifyPropertyChanged
    {
        private string _nombre;
        private string _nit;
        private string _mac;
        private string _ip;
        private string _code;

        public string Nombre
        {
            get => _nombre;
            set { _nombre = value; OnPropertyChanged(nameof(Nombre)); }
        }

        public string Nit
        {
            get => _nit;
            set { _nit = value; OnPropertyChanged(nameof(Nit)); }
        }

        public string Mac
        {
            get => _mac;
            set { _mac = value; OnPropertyChanged(nameof(Mac)); }
        }

        public string Ip
        {
            get => _ip;
            set { _ip = value; OnPropertyChanged(nameof(Ip)); }
        }

        public string Code
        {
            get => _code;
            set { _code = value; OnPropertyChanged(nameof(Code)); }
        }

        public ICommand GuardarCommand { get; }

        public VerificacionViewModel()
        {
            GuardarCommand = new RelayCommand(Guardar);
            Mac = EquipoIdentificador.GetUniqueIdentifier(); // Se asigna automáticamente
        }

        private void Guardar(object parameter)
        {
            if (string.IsNullOrWhiteSpace(Nombre) ||
                string.IsNullOrWhiteSpace(Nit) ||
                string.IsNullOrWhiteSpace(Mac) ||
                string.IsNullOrWhiteSpace(Ip) ||
                string.IsNullOrWhiteSpace(Code))
            {
                MessageBox.Show("Todos los campos deben estar llenos.", "Advertencia",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show("Datos guardados correctamente", "Guardar",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;

        public RelayCommand(Action<object> execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object parameter) => true; // Siempre habilitado
        public void Execute(object parameter) => _execute(parameter);
        public event EventHandler CanExecuteChanged;
    }
}

