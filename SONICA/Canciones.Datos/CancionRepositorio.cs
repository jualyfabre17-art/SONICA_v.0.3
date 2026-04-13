using System.Collections.Generic;
using System.Threading.Tasks;
using Canciones.Entidades;

namespace Canciones.Datos
{
    public interface ICancionRepository
    {
        Task<List<Cancion>> ObtenerTodasAsync();
        Task<Cancion> ObtenerPorIdAsync(int id);
        Task AgregarAsync(Cancion song);
        Task ActualizarAsync(Cancion song);
        Task<bool> EliminarAsync(int id);
        Task<List<Cancion>> BuscarPorTituloAsync(string tittle);
        Task<List<Cancion>> BuscarPorTituloYArtistaAsync(string tittle, string artist);
    }
}