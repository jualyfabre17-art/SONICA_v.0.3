using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Canciones.Entidades;
using Canciones.Logica;
using Microsoft.Extensions.Configuration;

namespace Canciones.PresentacionWPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly CancionService _songService;
        private ObservableCollection<Cancion> _songs;
        private Cancion _songSelected;
        private string _tittleSearch;
        private string _artistSearch;
        private string _tittle;
        private string _artist;
        private string _genre;
        private string _year;

        public MainViewModel(string connectionString)
        {
            _songService = new CancionService(connectionString);

            // Inicializar comandos
            CargarCancionesCommand = new RelayCommand(async _ => await CargarCancionesAsync());
            AgregarCommand = new RelayCommand(async _ => await AgregarCancionAsync(), _ => PuedeAgregar());
            ActualizarCommand = new RelayCommand(async _ => await ActualizarCancionAsync(), _ => PuedeActualizar());
            EliminarCommand = new RelayCommand(async _ => await EliminarCancionAsync(), _ => CancionSeleccionada != null);
            BuscarCommand = new RelayCommand(async _ => await BuscarCancionesAsync());
            LimpiarCommand = new RelayCommand(_ => LimpiarFormulario());

            // Opcional: cargar canciones al inicio
            Task.Run(async () => await CargarCancionesAsync());
        }

        public ObservableCollection<Cancion> Canciones
        {
            get => _songs;
            set { _songs = value; OnPropertyChanged(); }
        }

        public Cancion CancionSeleccionada
        {
            get => _songSelected;
            set
            {
                _songSelected = value;
                OnPropertyChanged();
                if (value != null)
                {
                    Titulo = value.Tittle;
                    Artista = value.Artist;
                    Genero = value.Genre;
                    Anio = value.Year?.ToString();
                }
                (ActualizarCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (EliminarCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string TituloBusqueda
        {
            get => _tittleSearch;
            set { _tittleSearch = value; OnPropertyChanged(); }
        }

        public string ArtistaBusqueda
        {
            get => _artistSearch;
            set { _artistSearch = value; OnPropertyChanged(); }
        }

        public string Titulo
        {
            get => _tittle;
            set { _tittle = value; OnPropertyChanged(); (AgregarCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
        }

        public string Artista
        {
            get => _artist;
            set { _artist = value; OnPropertyChanged(); (AgregarCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
        }

        public string Genero
        {
            get => _genre;
            set { _genre = value; OnPropertyChanged(); (AgregarCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
        }

        public string Anio
        {
            get => _year;
            set { _year = value; OnPropertyChanged(); }
        }

        public ICommand CargarCancionesCommand { get; }
        public ICommand AgregarCommand { get; }
        public ICommand ActualizarCommand { get; }
        public ICommand EliminarCommand { get; }
        public ICommand BuscarCommand { get; }
        public ICommand LimpiarCommand { get; }

        private async Task CargarCancionesAsync()
        {
            try
            {
                var lista = await _songService.ObtenerTodasAsync();
                Canciones = new ObservableCollection<Cancion>(lista);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar canciones: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task AgregarCancionAsync()
        {
            try
            {
                int? anioInt = null;
                if (!string.IsNullOrWhiteSpace(Anio) && int.TryParse(Anio, out int a))
                    anioInt = a;

                var nueva = new Cancion(Titulo, Artista, Genero, anioInt);
                await _songService.AgregarCancionAsync(nueva);
                await CargarCancionesAsync();
                LimpiarFormulario();
                MessageBox.Show("Canción agregada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ActualizarCancionAsync()
        {
            if (CancionSeleccionada == null) return;
            try
            {
                int? anioInt = null;
                if (!string.IsNullOrWhiteSpace(Anio) && int.TryParse(Anio, out int a))
                    anioInt = a;

                CancionSeleccionada.Tittle = Titulo;
                CancionSeleccionada.Artist = Artista;
                CancionSeleccionada.Genre = Genero;
                CancionSeleccionada.Year = anioInt;

                await _songService.ActualizarAsync(CancionSeleccionada);
                await CargarCancionesAsync();
                LimpiarFormulario();
                MessageBox.Show("Canción actualizada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task EliminarCancionAsync()
        {
            if (CancionSeleccionada == null) return;
            var result = MessageBox.Show($"¿Eliminar '{CancionSeleccionada.Tittle}'?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _songService.EliminarCancionAsync(CancionSeleccionada.Id);
                    await CargarCancionesAsync();
                    LimpiarFormulario();
                    MessageBox.Show("Canción eliminada.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task BuscarCancionesAsync()
        {
            try
            {
                List<Cancion> resultados;
                if (string.IsNullOrWhiteSpace(ArtistaBusqueda))
                    resultados = await _songService.BuscarCancionesAsync(TituloBusqueda);
                else
                    resultados = await _songService.BuscarCancionesAsync(TituloBusqueda, ArtistaBusqueda);

                Canciones = new ObservableCollection<Cancion>(resultados);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en búsqueda: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool PuedeAgregar() => !string.IsNullOrWhiteSpace(Titulo) && !string.IsNullOrWhiteSpace(Artista) && !string.IsNullOrWhiteSpace(Genero);
        private bool PuedeActualizar() => CancionSeleccionada != null && PuedeAgregar();

        public void LimpiarFormulario()
        {
            Titulo = "";
            Artista = "";
            Genero = "";
            Anio = "";
            CancionSeleccionada = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
