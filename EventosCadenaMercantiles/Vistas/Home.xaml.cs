using System;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Windows.Controls;


namespace EventosCadenaMercantiles.Vistas
{
    public partial class Home : Window
    {
        public Home()
        {
            InitializeComponent();
        }

        // Método para abrir el archivo seleccionado
        private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            // Crear una instancia del cuadro de diálogo para seleccionar archivos
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Establecer el filtro para mostrar solo archivos .xml
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
            // Por ejemplo, leer el archivo y realizar acciones según el contenido
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




        // Mostrar las opciones para "Evento"
        private void ShowEventoOptions(object sender, RoutedEventArgs e)
        {
            // Alternar la visibilidad del PopupEvento
            PopupEvento.IsOpen = !PopupEvento.IsOpen;
        }

        // Mostrar las opciones para "CoRechazo"
        private void ShowCoRechazoOptions(object sender, RoutedEventArgs e)
        {
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




    }
}
