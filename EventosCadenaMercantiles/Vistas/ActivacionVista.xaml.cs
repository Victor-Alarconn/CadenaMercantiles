using System.Net.NetworkInformation;
using System;
using System.Windows;
using EventosCadenaMercantiles.Services;
using EventosCadenaMercantiles.ViewModels;

namespace EventosCadenaMercantiles.Vistas
{
    public partial class ActivacionVista : Window
    {
        public ActivacionViewModel ViewModel { get; }
        public ActivacionVista()
        {
            InitializeComponent();
            Txtmac.Text = EquipoIdentificador.GetUniqueIdentifier();
            Txtempresa.Text = EquipoIdentificador.ObtenerEmpresa();

            ViewModel = new ActivacionViewModel();
            DataContext = ViewModel;
        }

    }
}
