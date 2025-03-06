using EventosCadenaMercantiles.Services;
using EventosCadenaMercantiles.Vistas;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Input;

namespace EventosCadenaMercantiles.ViewModels
{
    public class ActivacionViewModel : INotifyPropertyChanged
    {
        private string _empresa;
        private string _mac;
        private string _modulos;
        private DateTime _fecha;

        public string Empresa
        {
            get => _empresa;
            set { _empresa = value; OnPropertyChanged(nameof(Empresa)); }
        }

        public string Mac
        {
            get => _mac;
            set { _mac = value; OnPropertyChanged(nameof(Mac)); }
        }

        public string Modulos
        {
            get => _modulos;
            set { _modulos = value; OnPropertyChanged(nameof(Modulos)); }
        }

        public DateTime Fecha
        {
            get => _fecha;
            set { _fecha = value; OnPropertyChanged(nameof(Fecha)); }
        }

        public ICommand ActivarCommand { get; }

        public ActivacionViewModel()
        {
            ActivarCommand = new RelayCommand(Activar);
            Mac = EquipoIdentificador.GetUniqueIdentifier(); // Se asigna automáticamente
            Empresa = EquipoIdentificador.ObtenerEmpresa(); // Se asigna automáticamente
            Fecha = DateTime.Today;
        }

        private void Activar(object parameter)
        {
            if (string.IsNullOrWhiteSpace(Empresa) ||
                string.IsNullOrWhiteSpace(Mac) ||
                string.IsNullOrWhiteSpace(Modulos) ||
                Fecha == null || Fecha == DateTime.MinValue)
            {
                MessageBox.Show("Todos los campos deben estar llenos.", "Advertencia",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Llamar al servicio para guardar el equipo
            bool guardadoExitoso = EmpresaService.GuardarEquipo(empresa: Empresa, nroMac: Mac, factivar: Fecha, modulos: Modulos, usuario: "RmSoft20X");

            if (guardadoExitoso)
            {
                MessageBox.Show("Equipo registrado correctamente.", "Éxito",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // Cerrar la vista actual y abrir la nueva
                CerrarVentanaYMostrarNueva();
            }
            else
            {
                MessageBox.Show("Hubo un problema al registrar el equipo.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CerrarVentanaYMostrarNueva()
        {
            // Obtener la ventana actual
            Window ventanaActual = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);

            // Abrir la nueva vista
            new Home().Show();

            // Cerrar la ventana actual si existe
            ventanaActual?.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
