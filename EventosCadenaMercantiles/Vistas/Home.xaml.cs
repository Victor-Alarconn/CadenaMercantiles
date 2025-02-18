using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;

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
    }
}
