using System;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;


namespace EventosCadenaMercantiles.Vistas
{
    public partial class Home : Window
    {
        public Home()
        {
            InitializeComponent();

            // Asignar un controlador de eventos para detectar clics fuera del Popup
            this.PreviewMouseDown += (s, e) => ClosePopupOnClickOutside(e);
        }

        // Método para abrir el archivo seleccionado
        private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archivos XML (*.xml)|*.xml";

            // Mostrar el cuadro de diálogo y verificar si se ha seleccionado un archivo
            if (openFileDialog.ShowDialog() == true)
            {

                // Obtener la ruta del archivo seleccionado
                string filePath = openFileDialog.FileName;

                // Mostrar el nombre del archivo en el label
                LbNombreArchivo.Content = Path.GetFileName(filePath);

                // Aquí puedes agregar el procesamiento del archivo (como validarlo, leerlo, etc.)
                ProcessXmlFile(filePath);
            }
        }

        // Método para procesar el archivo XML (puedes adaptarlo según tus necesidades)
        private void ProcessXmlFile(string filePath)
        {
            // Lógica para procesar el archivo XML
          
            try
            {
                // Aquí agregas tu lógica de procesamiento del archivo XML
                MessageBox.Show($"Archivo seleccionado: {filePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al procesar el archivo: {ex.Message}");
            }
        }

        // Método para el botón "Refrescar"
        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            // Lógica para refrescar la vista
            MessageBox.Show("Vista refrescada.");
        }

        // Método para el botón "Terminar"
        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            // Cerrar la ventana
            this.Close();
        }

        // Método para el botón "Enviar"
        private void BtnEnvioPostEnventoMercantil_Click(object sender, RoutedEventArgs e)
        {
            // Lógica para enviar el archivo XML o realizar otra acción
            MessageBox.Show("Enviando el documento XML...");
        }

        private void ClosePopupOnClickOutside(MouseButtonEventArgs e)
        {
            // Si el clic no ocurrió dentro del Popup
            if (!PopupEvento.IsMouseOver && !PopupCoRechazo.IsMouseOver)
            {
                PopupEvento.IsOpen = false;
                PopupCoRechazo.IsOpen = false;
            }
        }



        // Mostrar las opciones para "Evento"
        private void ShowEventoOptions(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var position = button.TransformToAncestor(this).Transform(new Point(0, 0));

            // Establecer las posiciones del Popup
            PopupEvento.HorizontalOffset = position.X;  // Colocarlo horizontalmente al lado del botón
            PopupEvento.VerticalOffset = position.Y + button.ActualHeight; // Colocarlo justo debajo del botón

            // Alternar la visibilidad del PopupEvento
            PopupEvento.IsOpen = !PopupEvento.IsOpen;
        }


        // Mostrar las opciones para "CoRechazo"
        private void ShowCoRechazoOptions(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var position = button.TransformToAncestor(this).Transform(new Point(0, 0));

            // Establecer las posiciones del Popup
            PopupCoRechazo.HorizontalOffset = position.X;  // Colocarlo horizontalmente al lado del botón
            PopupCoRechazo.VerticalOffset = position.Y + button.ActualHeight; // Colocarlo justo debajo del botón

            // Alternar la visibilidad del PopupCoRechazo
            PopupCoRechazo.IsOpen = !PopupCoRechazo.IsOpen;
        }



        // Manejar el clic en el 'Evento' y actualizar el texto del botón
        private void MenuItem_Evento_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;

            // Cambiar el contenido del botón de Evento
            BtnEvento.Content = item.Header.ToString();

            // Cerrar el Popup
            PopupEvento.IsOpen = false;
        }

        // Manejar el clic en el 'CoRechazo' y actualizar el texto del botón
        private void MenuItem_CoRechazo_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;

            // Cambiar el contenido del botón de CoRechazo
            BtnCoRechazo.Content = item.Header.ToString();

            // Cerrar el Popup
            PopupCoRechazo.IsOpen = false;
        }



        // Método para manejar el clic en la ventana y cerrar los Popup si se hace clic fuera
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Si el clic ocurrió fuera del PopupEvento y el Popup está abierto, cerramos el Popup
            if (PopupEvento.IsOpen && !PopupEvento.IsMouseOver)
            {
                PopupEvento.IsOpen = false;
            }

            // Si el clic ocurrió fuera del PopupCoRechazo y el Popup está abierto, cerramos el Popup
            if (PopupCoRechazo.IsOpen && !PopupCoRechazo.IsMouseOver)
            {
                PopupCoRechazo.IsOpen = false;
            }
        }




    }
}
