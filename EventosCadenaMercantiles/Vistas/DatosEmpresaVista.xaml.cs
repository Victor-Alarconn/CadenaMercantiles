using EventosCadenaMercantiles.Services;
using EventosCadenaMercantiles.ViewModels;
using System;
using System.Windows;

namespace EventosCadenaMercantiles.Vistas
{
    public partial class DatosEmpresaVista : Window
    {
        public VerificacionViewModel ViewModel { get; }

        public DatosEmpresaVista()
        {
            InitializeComponent();
            txtmac.Text = EquipoIdentificador.GetUniqueIdentifier();
            ViewModel = new VerificacionViewModel();
            DataContext = ViewModel;
        }
    }
}
