using EventosCadenaMercantiles.Services;
using EventosCadenaMercantiles.Vistas;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
        private string _apellidos;
        private static readonly string archivoConexion = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "empresa.txt");
        public string Nombre
        {
            get => _nombre;
            set { _nombre = value; OnPropertyChanged(nameof(Nombre)); }
        }

        public string Apellidos
        {
            get => _apellidos;
            set { _apellidos = value; OnPropertyChanged(nameof(Apellidos)); }
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

            // Guardar en el archivo env.txt
            bool guardadoEnv = DatosEmpresaService.GuardarEnv(Ip, Code);
            DatabaseInitializer.CrearTablasSiNoExisten();

            if (!guardadoEnv)
            {
                MessageBox.Show("Hubo un problema al actualizar el archivo env.txt.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return; // No continuar si falla el guardado en env.txt
            }

            // Obtener NIT y DV
            string nit = Nit;
            string dv = "";

            if (Nit.Contains("-"))
            {
                var partes = Nit.Split('-');
                nit = partes[0].Trim();
                if (partes.Length > 1)
                    dv = partes[1].Trim();
            }

            // Guardar en el archivo empresa.txt
            try
            {
                string nitConDv = !string.IsNullOrWhiteSpace(dv) ? $"{nit}-{dv}" : nit;

                File.WriteAllLines(archivoConexion, new[]
                {
            Nombre.Trim(),   // Primera línea = Nombre de la empresa
            nitConDv.Trim()  // Segunda línea = NIT o NIT-DV
        });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar los datos de la empresa: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Guardar en la base de datos
            bool guardadoExitoso = DatosEmpresaService.GuardarEmpresa(nit: Nit, nombre: Nombre, nombre2: Apellidos);

            if (guardadoExitoso)
            {
                MessageBox.Show("Datos guardados correctamente y archivo actualizado.", "Guardar",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // Cerrar la vista actual y abrir la nueva
                CerrarVentanaYMostrarNueva();
            }
            else
            {
                MessageBox.Show("Datos guardados en el archivo env.txt, pero hubo un problema al guardar en la base de datos.", "Advertencia",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void CerrarVentanaYMostrarNueva()
        {
            // Obtener la ventana actual
            Window ventanaActual = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);

            // Abrir la nueva vista
            new ActivacionVista().Show();

            // Cerrar la ventana actual si existe
            ventanaActual?.Close();
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

