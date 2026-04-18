using System;
using System.IO;
using System.Windows;
using Canciones.PresentacionWPF.ViewModels;
using Microsoft.Extensions.Configuration;

namespace Canciones.PresentacionWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            string connectionString = null;
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                IConfiguration config = builder.Build();
                connectionString = config.GetConnectionString("DefaultConnection");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al leer configuración: {ex.Message}\nLa aplicación se cerrará.", "Error crítico", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }

            // Crear el ViewModel con la cadena de conexión
            var viewModel = new MainViewModel(connectionString);

            // Asignarlo al DataContext de la ventana
            this.DataContext = viewModel;
        
    }

        
    }
}