﻿using Microsoft.Win32;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EventosCadenaMercantiles.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        private string _nombreArchivo;
        private bool _popupEventoAbierto;
        private bool _popupCoRechazoAbierto;
        private string _textoEvento;
        private string _textoCoRechazo;
        private ImageSource _logoEmpresa;
        private string _tipoEventoSeleccionado;
        private string _codigoEventoSeleccionado;



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

        public ImageSource LogoEmpresa
        {
            get => _logoEmpresa;
            set => SetProperty(ref _logoEmpresa, value);
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

        public string TipoEventoSeleccionado
        {
            get => _tipoEventoSeleccionado;
            set
            {
                if (_tipoEventoSeleccionado != value)
                {
                    _tipoEventoSeleccionado = value;
                    OnPropertyChanged(nameof(TipoEventoSeleccionado));
                    FiltrarEventos(); // Método que filtra los eventos basado en el tipo seleccionado
                }
            }
        }


         public string CodigoEventoSeleccionado
        {
            get => _codigoEventoSeleccionado;
            set
            {
                if (_codigoEventoSeleccionado != value)
                {
                    _codigoEventoSeleccionado = value;
                    OnPropertyChanged(nameof(CodigoEventoSeleccionado));
                    FiltrarEventos();
                }
            }
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
        public ICommand Even_documCommand { get; }

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

            LoadCompanyLogo(); // Carga el logo al inicializar el ViewModel
            // Llamar directamente a LoadEventos aquí
            LoadEventos();

        }


        private void LoadEventos()
        {
            DateTime fechaInicio = DateTime.Now.AddDays(-7);
            DateTime fechaFin = DateTime.Now;

            var eventosLista = EventosService.ObtenerEventos(fechaInicio, fechaFin);

            Eventos.Clear();

            foreach (var evento in eventosLista)
            {
                if (string.IsNullOrWhiteSpace(evento.EvenDocum) || string.IsNullOrWhiteSpace(evento.EvenEvento))
                {
                    continue; // Ignorar registros vacíos
                }

                // Si está en "VALIDANDO", hacemos la consulta cruzada
                if (evento.EvenCodigo == "VALIDANDO")
                {
                    if (RecepcionService.ExisteDocumentoEnValidacion(evento.EvenDocum)) // Validar contra doc_recepcion
                    {
                        // Actualizar doc_recepcion: estado = 1
                        RecepcionService.MarcarComoConsultado(evento.EvenDocum);

                        // Actualizar evento: EvenCodigo = PREPARADO y EvenResponse = Documento Listo Para Recepcionar
                        evento.EvenCodigo = "PREPARADO";
                        evento.EvenResponse = "Documento Listo Para Recepcionar";
                        evento.EvenEvento = "Listo";

                        // Actualizar en BD eventos
                        EventosService.ActualizarEvento(evento);
                    }
                }

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

            if (openFileDialog.ShowDialog() == true)
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
                            var documento = ProcesarXmlDesdeStream(xmlStream);
                            if (documento == null)
                            {
                                return;
                            }

                            if (EventosService.ExisteFactura(documento.PrefijoFactura))
                            {
                                MessageBox.Show($"La factura {documento.PrefijoFactura} ya se encuentra en la base.",
                                                "Factura Duplicada", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }

                            // Enviar correo antes de guardar en BD
                            if (!EnvioCorreoService.EnviarCorreo(documento, rutaZip))
                            {
                                MessageBox.Show("No se pudo enviar el correo, la factura no será guardada.",
                                                "Error de Envío", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;  // No guardamos si el correo falla
                            }

                            // Si el correo fue exitoso, guardamos en la base de datos
                            EventosService.GuardarEvento(documento);
                            MessageBox.Show("Documento procesado y guardado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadEventos();
                        }
                        return;
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
            {
                MessageBox.Show("El XML no es un documento válido.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;  // Termina aquí sin excepción
            }

            if (!EsDocumentoCredito(doc))
            {
                MessageBox.Show("La factura no es de crédito. No se puede recepcionar.",
                                "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;  // Simplemente termina el proceso sin continuar
            }

            var ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            ns.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");

            var documento = new DocumentoAdjunto();

            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                if (node.LocalName == "SenderParty")
                {
                    ProcesarSenderParty(node, documento, ns);
                }
                else if (node.LocalName == "ReceiverParty")
                {
                    ValidarReceiverParty(node, documento, ns);
                }
                else if (node.LocalName == "Attachment")
                {
                    ProcesarAttachment(node, documento, ns);
                }
                else if (node.LocalName == "ParentDocumentLineReference")
                {
                    ProcesarReferenciaDocumento(node, documento, ns);
                }
                else if (node.LocalName == "ParentDocumentID")
                {
                    documento.PrefijoFactura = node.InnerText;
                }
            }

            documento.XmlBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(doc.InnerXml));
            return documento;
        }


        private bool EsDocumentoCredito(XmlDocument attachedDocument)
        {
            var ns = new XmlNamespaceManager(attachedDocument.NameTable);
            ns.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            ns.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");

            // Buscar el nodo Description (que tiene el XML de la factura embebida)
            var descriptionNode = attachedDocument.SelectSingleNode("//cac:Attachment/cac:ExternalReference/cbc:Description", ns);
            if (descriptionNode == null)
            {
                MessageBox.Show("No se encontró la factura embebida dentro del documento adjunto.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Cargar la factura embebida como un nuevo XmlDocument
            var embeddedInvoice = new XmlDocument();
            embeddedInvoice.LoadXml(descriptionNode.InnerText);

            // Crear un nuevo namespace manager para la factura
            var nsInvoice = new XmlNamespaceManager(embeddedInvoice.NameTable);
            nsInvoice.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            nsInvoice.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");

            // Buscar el PaymentMeans dentro de la factura embebida
            var paymentMeansNode = embeddedInvoice.SelectSingleNode("//cac:PaymentMeans", nsInvoice);
            if (paymentMeansNode != null)
            {
                var idNode = paymentMeansNode.SelectSingleNode("cbc:ID", nsInvoice);
                if (idNode != null && idNode.InnerText.Trim() == "2")
                {
                    return true; // Es crédito
                }
            }

            return false; // No es crédito
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
                var registrationName = taxScheme.SelectSingleNode("cbc:RegistrationName", ns);
                var companyID = taxScheme.SelectSingleNode("cbc:CompanyID", ns);

                if (registrationName != null)
                {
                    doc.Emisor = registrationName.InnerText;  // Aquí es donde pones el nombre
                }

                if (companyID != null)
                {
                    doc.Identificacion = companyID.InnerText;  // Aquí es donde pones el NIT
                }
            }
        }



        private void ValidarReceiverParty(XmlNode node, DocumentoAdjunto doc, XmlNamespaceManager ns)
        {
            var taxScheme = node.SelectSingleNode("cac:PartyTaxScheme", ns);
            if (taxScheme == null)
            {
                MessageBox.Show("No se encontró el nodo PartyTaxScheme en ReceiverParty.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Extraer el RegistrationName (nombre del receptor)
            var registrationNameNode = taxScheme.SelectSingleNode("cbc:RegistrationName", ns);
            if (registrationNameNode == null)
            {
                MessageBox.Show("No se encontró el nombre del receptor (RegistrationName) en PartyTaxScheme.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            doc.Receptor = registrationNameNode.InnerText.Trim();  // Guardamos el nombre en el documento

            // Extraer el CompanyID (NIT)
            var companyIdNode = taxScheme.SelectSingleNode("cbc:CompanyID", ns);
            if (companyIdNode == null)
            {
                MessageBox.Show("No se encontró el nodo CompanyID en ReceiverParty.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var nitEnXml = companyIdNode.InnerText;
            var dvEnXml = companyIdNode.Attributes["schemeID"]?.Value;

            var nitEsperado = "75036432"; // Para pruebas, ajusta para que lo leas de configuración o BD
            var dvEsperado = "7";

            if (nitEnXml != nitEsperado)
            {
                MessageBox.Show($"El NIT recibido ({nitEnXml}) no coincide con el esperado ({nitEsperado}).",
                                "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (dvEnXml != dvEsperado)
            {
                MessageBox.Show($"El DV recibido ({dvEnXml ?? "(no definido)"}) no coincide con el esperado ({dvEsperado}).",
                                "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            doc.Idnit = nitEnXml;
            doc.Dv = dvEnXml;
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
            LoadEventos();
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
            
        }

        // Método para manejar el cambio de selección del ComboBox "Filtro de evento"



        private void FiltrarEventos()
        {
            var eventosFiltrados = EventosService.ObtenerEventos(DateTime.MinValue, DateTime.MaxValue);

            if (!string.IsNullOrEmpty(TipoEventoSeleccionado))
            {
                eventosFiltrados = eventosFiltrados.Where(e => e.EvenEvento == TipoEventoSeleccionado).ToList();
            }

            if (!string.IsNullOrEmpty(CodigoEventoSeleccionado))
            {
                eventosFiltrados = eventosFiltrados.Where(e => e.EvenCodigo == CodigoEventoSeleccionado).ToList();
            }

            Eventos = new ObservableCollection<EventosModel>(eventosFiltrados);
        }

        private void Filtroevento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Aquí se puede agregar la lógica para filtrar por evento si es necesario en el futuro
        }

        // Método para manejar el cambio de selección del ComboBox "Filtro de código"



        private void LoadCompanyLogo()
        {
            // Construir la ruta relativa a la carpeta de imágenes dentro del proyecto
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string relativePath = @"..\..\Imagenes\LogoEmp.png";
            string logoPath = Path.Combine(basePath, relativePath);

            if (File.Exists(logoPath))
            {
                LogoEmpresa = new BitmapImage(new Uri(logoPath, UriKind.Absolute));
            }
            else
            {
                Console.WriteLine("No se encontró el logo en: " + logoPath); // Mensaje de error en la consola
                                                                             // Cargar una imagen por defecto si no existe el logo
                LogoEmpresa = new BitmapImage(new Uri("/Imagenes/DefaultLogo.png"));
            }
        }


    }
}
