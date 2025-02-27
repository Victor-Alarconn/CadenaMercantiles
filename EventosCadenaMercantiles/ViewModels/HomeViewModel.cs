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
using System.Windows.Controls;

using EventosCadenaMercantiles.Modelos;
using System.Collections.ObjectModel;
using EventosCadenaMercantiles.Services;
using System.Diagnostics;
using System.IO.Compression;
using System.Xml;
using System.Web.UI.WebControls;

namespace EventosCadenaMercantiles.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        private string _nombreArchivo;
        private bool _popupEventoAbierto;
        private bool _popupCoRechazoAbierto;
        private string _textoEvento;
        private string _textoCoRechazo;
        private ObservableCollection<EventosModel> _eventos;
        public ObservableCollection<EventosModel> Eventos
        {
            get => _eventos;
            set
            {
                _eventos = value;
                OnPropertyChanged(nameof(Eventos));
            }
        }

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
        public ICommand CargarEventosCommand { get; }
        public ICommand  Even_documCommand { get; }

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
            Even_documCommand = new RelayCommand<EventosModel>(AbrirQR);

            Eventos = new ObservableCollection<EventosModel>();
            CargarEventosCommand = new RelayCommand<object>(param => LoadEventos());

            // Llamar directamente a LoadEventos aquí
            LoadEventos();

        }


        private void LoadEventos()
        {
            DateTime fechaInicio = DateTime.Now.AddDays(-7); // Ajusta las fechas según necesidad
            DateTime fechaFin = DateTime.Now;

            var eventosLista = EventosService.ObtenerEventos(fechaInicio, fechaFin);

            Eventos.Clear();
            foreach (var evento in eventosLista)
            {
                Eventos.Add(evento);
            }
        }

        private void AbrirQR(EventosModel evento)
        {
            if (evento == null || string.IsNullOrWhiteSpace(evento.EvenQrcode))
                return;

            try
            {
                Process myProcess = new Process();
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.StartInfo.FileName = evento.EvenQrcode;
                myProcess.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al abrir QR: {ex.Message}");
            }
        }



        private void AbrirArchivo()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Archivos ZIP (*.zip)|*.zip"
            };

            if(openFileDialog.ShowDialog() == true)
    {
                NombreArchivo = Path.GetFileName(openFileDialog.FileName);
                ExtraerYProcesarZip(openFileDialog.FileName);
            }
        }

        private void ExtraerYProcesarZip(string rutaZip)

        {
            using (ZipArchive archive = ZipFile.OpenRead(rutaZip))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                    {
                        using (Stream xmlStream = entry.Open())
                        {
                            ProcesarXmlDesdeStream(xmlStream);
                        }
                        return; // Solo procesa el primer XML encontrado
                    }
                }
            }

            MessageBox.Show("No se encontró un archivo XML en el ZIP.");
        }

        private DocumentoAdjunto ProcesarXmlDesdeStream(Stream xmlStream)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlStream);

            if (!EsAttachedDocument(doc))
                throw new InvalidDataException("El XML no es un documento válido.");

            // Crear el XmlNamespaceManager
            var ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            ns.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");

            var documento = new DocumentoAdjunto();

            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                if (node.LocalName == "SenderParty")
                {
                    ProcesarSenderParty(node, documento, ns); // ahora pasa el namespace manager
                }
                else if (node.LocalName == "ReceiverParty")
                {
                    ValidarReceiverParty(node, documento, ns); // pasamos el namespace manager
                }
                else if (node.LocalName == "Attachment")
                {
                    ProcesarAttachment(node, documento, ns); // pasamos el namespace manager
                }
                else if (node.LocalName == "ParentDocumentLineReference")
                {
                    ProcesarReferenciaDocumento(node, documento, ns);  // Aquí se pasa el namespace
                }
                else if (node.LocalName == "ParentDocumentID")
                {
                    documento.PrefijoFactura = node.InnerText;
                }
            }

            documento.XmlBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(doc.InnerXml));
            return documento;
        }


        private bool EsAttachedDocument(XmlDocument doc)
        {
            return doc.DocumentElement?.FirstChild?.ParentNode?.LocalName == "AttachedDocument";
        }

        private void ProcesarSenderParty(XmlNode node, DocumentoAdjunto doc, XmlNamespaceManager ns)
        {
            var taxScheme = node.SelectSingleNode("cac:PartyTaxScheme", ns);
            if (taxScheme != null)
            {
                var companyID = taxScheme.SelectSingleNode("cbc:CompanyID", ns);
                if (companyID != null)
                {
                    doc.Emisor = companyID.InnerText;
                    doc.Identificacion = companyID.NextSibling?.InnerText;  // Esto puede necesitar ajuste dependiendo de la estructura
                }
            }
        }


        private void ValidarReceiverParty(XmlNode node, DocumentoAdjunto doc, XmlNamespaceManager ns)
        {
            var taxScheme = node.SelectSingleNode("cac:PartyTaxScheme", ns);
            if (taxScheme != null)
            {
                var companyIdNode = taxScheme.SelectSingleNode("cbc:CompanyID", ns);
                var companyId = companyIdNode?.InnerText;

                if (companyId != doc.Idnit)
                {
                    throw new InvalidOperationException("El identificador de la factura no corresponde al identificador del cliente.");
                }

                var schemeID = companyIdNode?.Attributes["schemeID"];
                if (schemeID != null)
                {
                    schemeID.InnerText = doc.Dv;
                }
            }
        }


        private void ProcesarAttachment(XmlNode attachmentNode, DocumentoAdjunto documento, XmlNamespaceManager ns)
        {
            var externalRef = attachmentNode.SelectSingleNode("cac:ExternalReference", ns);
            if (externalRef != null)
            {
                var descriptionNode = externalRef.SelectSingleNode("cbc:Description", ns);
                if (descriptionNode != null)
                {
                    var innerXml = descriptionNode.InnerText;
                    XmlDocument embeddedDoc = new XmlDocument();
                    embeddedDoc.LoadXml(innerXml);

                    var embeddedNs = new XmlNamespaceManager(embeddedDoc.NameTable);
                    embeddedNs.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
                    embeddedNs.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");

                    var customerParty = embeddedDoc.SelectSingleNode("//cac:AccountingCustomerParty", embeddedNs);
                    if (customerParty != null)
                    {
                        var partyTaxScheme = customerParty.SelectSingleNode("cac:Party/cac:PartyTaxScheme", embeddedNs);
                        if (partyTaxScheme != null)
                        {
                            var companyID = partyTaxScheme.SelectSingleNode("cbc:CompanyID", embeddedNs);
                            if (companyID != null)
                            {
                                var schemeID = companyID.Attributes["schemeID"];
                                if (schemeID != null)
                                {
                                    schemeID.InnerText = documento.Dv;
                                }
                            }
                        }
                    }

                    descriptionNode.InnerText = embeddedDoc.OuterXml;
                }
            }
        }


        private void ProcesarReferenciaDocumento(XmlNode referenceNode, DocumentoAdjunto doc, XmlNamespaceManager ns)
        {
            var documentReference = referenceNode.SelectSingleNode("cac:DocumentReference", ns);
            if (documentReference != null)
            {
                var uuidNode = documentReference.SelectSingleNode("cbc:UUID", ns);
                if (uuidNode != null)
                {
                    doc.Cufe = uuidNode.InnerText;
                    doc.QRCode = $"https://catalogo-vpfe.dian.gov.co/document/searchqr?documentkey={doc.Cufe}";
                }
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


    }
}
