using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Canciones.Datos;
using Canciones.Entidades;

namespace Canciones.Logica
{
    public class CancionService
    {
        private readonly ICancionRepository _repository;

        public CancionService(string connectionString)
        {
            _repository = new CancionRepository(connectionString);
        }

        // Obtener todas
        public async Task<List<Cancion>> ObtenerTodasAsync()
        {
            return await _repository.ObtenerTodasAsync();
        }

        // Agregar con validaciones (reglas de negocio)
        public async Task<bool> AgregarCancionAsync(Cancion cancion)
        {
            if (string.IsNullOrWhiteSpace(cancion.Tittle))
                throw new ArgumentException("El título es obligatorio.");
            if (string.IsNullOrWhiteSpace(cancion.Artist))
                throw new ArgumentException("El artista es obligatorio.");
            if (string.IsNullOrWhiteSpace(cancion.Genre))
                throw new ArgumentException("El género es obligatorio.");

            await _repository.AgregarAsync(cancion);
            return true;
        }

        // Eliminar
        public async Task<bool> EliminarCancionAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID inválido.");
            return await _repository.EliminarAsync(id);
        }

        // Sobrecarga de búsqueda: un solo criterio (título)
        public async Task<List<Cancion>> BuscarCancionesAsync(string titulo)
        {
            return await _repository.BuscarPorTituloAsync(titulo);
        }

        // Sobrecarga de búsqueda: dos criterios (título y artista)
        public async Task<List<Cancion>> BuscarCancionesAsync(string titulo, string artista)
        {
            return await _repository.BuscarPorTituloYArtistaAsync(titulo, artista);
        }

        // Obtener por ID
        public async Task<Cancion> ObtenerPorIdAsync(int id)
        {
            return await _repository.ObtenerPorIdAsync(id);
        }

        public async Task<bool> ActualizarAsync(Cancion cancion)
        {
            if (cancion.Id <= 0) throw new ArgumentException("ID inválido.");
            if (string.IsNullOrWhiteSpace(cancion.Tittle)) throw new ArgumentException("El título es obligatorio.");
            if (string.IsNullOrWhiteSpace(cancion.Artist)) throw new ArgumentException("El artista es obligatorio.");
            if (string.IsNullOrWhiteSpace(cancion.Genre)) throw new ArgumentException("El género es obligatorio.");
            await _repository.ActualizarAsync(cancion);
            return true;
        }

        

       
    }
}
