using System;
using System.Threading.Tasks;
using Canciones.Entidades;
using Canciones.Logica;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Collections.Generic;

namespace Canciones.Presentacion
{
    class Program
    {
        private static CancionService _service;
        private static string connectionString;

        static async Task Main(string[] args)
        {
            // Configuración desde appsettings.json (optimización de configuración)
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();
            connectionString = config.GetConnectionString("DefaultConnection");
            _service = new CancionService(connectionString);

            Console.WriteLine("=== SONICA ===\n");

            bool exit = false;
            while (!exit)
            {
                MostrarMenu();
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        await ListarCanciones();
                        break;
                    case "2":
                        await AgregarCancion();
                        break;
                    case "3":
                        await BuscarCanciones();
                        break;
                    case "4":
                        await EliminarCancion();
                        break;
                    case "5":
                        exit = true;
                        Console.WriteLine("¡Hasta luego!");
                        break;
                    default:
                        Console.WriteLine("Opción no válida. Intente de nuevo.");
                        break;
                }
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        static void MostrarMenu()
        {
            Console.WriteLine("===== MENÚ PRINCIPAL =====");
            Console.WriteLine("1. Listar todas las canciones");
            Console.WriteLine("2. Agregar nueva canción");
            Console.WriteLine("3. Buscar canciones");
            Console.WriteLine("4. Eliminar canción");
            Console.WriteLine("5. Salir");
            Console.Write("Seleccione una opción: ");
        }

        static async Task ListarCanciones()
        {
            Console.WriteLine("\n--- LISTADO DE CANCIONES ---");
            var songs = await _service.ObtenerTodasAsync();
            if (songs.Count == 0)
            {
                Console.WriteLine("No hay canciones registradas.");
                return;
            }

            foreach (var cancion in songs)
            {
                // Uso de polimorfismo: cada objeto Cancion ejecuta su propia implementación de MostrarInfo()
                cancion.MostrarInfo();
            }
        }

        static async Task AgregarCancion()
        {
            Console.WriteLine("\n--- AGREGAR NUEVA CANCIÓN ---");
            Console.Write("Título: ");
            string tittle = Console.ReadLine();
            Console.Write("Artista: ");
            string artist = Console.ReadLine();
            Console.Write("Genero: ");
            string genre = Console.ReadLine();
            Console.Write("Fecha: ");
            string yearInput = Console.ReadLine();
            int? year = string.IsNullOrWhiteSpace(yearInput) ? (int?)null : int.Parse(yearInput);

            // Uso del constructor con parámetros
            var nuevaCancion = new Cancion(tittle, artist, genre, year);

            try
            {
                await _service.AgregarCancionAsync(nuevaCancion);
                Console.WriteLine("¡Canción agregada exitosamente!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static async Task BuscarCanciones()
        {
            Console.WriteLine("\n--- BÚSQUEDA DE CANCIONES ---");
            Console.WriteLine("1. Buscar por título");
            Console.WriteLine("2. Buscar por título y artista");
            Console.Write("Opción: ");
            string opt = Console.ReadLine();

            List<Cancion> results = new List<Cancion>();

            if (opt == "1")
            {
                Console.Write("Ingrese título (o parte): ");
                string tittle = Console.ReadLine();
                results = await _service.BuscarCancionesAsync(tittle); // Sobrecarga con 1 parámetro
            }
            else if (opt == "2")
            {
                Console.Write("Título: ");
                string tittle = Console.ReadLine();
                Console.Write("Artista: ");
                string artist = Console.ReadLine();
                results = await _service.BuscarCancionesAsync(tittle, artist); // Sobrecarga con 2 parámetros
            }
            else
            {
                Console.WriteLine("Opción inválida.");
                return;
            }

            if (results.Count == 0)
                Console.WriteLine("No se encontraron coincidencias.");
            else
            {
                Console.WriteLine($"\nResultados ({results.Count}):");
                foreach (var c in results)
                    c.MostrarInfo(); // Polimorfismo nuevamente
            }
        }

        static async Task EliminarCancion()
        {
            Console.WriteLine("\n--- ELIMINAR CANCIÓN ---");
            await ListarCanciones();
            Console.Write("Ingrese el ID de la canción a eliminar: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                try
                {
                    bool delete = await _service.EliminarCancionAsync(id);
                    if (delete)
                        Console.WriteLine("Canción eliminada correctamente.");
                    else
                        Console.WriteLine("No se encontró una canción con ese ID.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("ID inválido.");
            }
        }
    }
}