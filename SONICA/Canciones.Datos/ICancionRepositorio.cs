using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Canciones.Entidades;

namespace Canciones.Datos
{
    public class CancionRepository : ICancionRepository
    {
        private readonly string _connectionString;

        public CancionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Uso de async/await y using para liberar recursos (optimización)
        public async Task<List<Cancion>> ObtenerTodasAsync()
        {
            var songs = new List<Cancion>();
            const string query = "SELECT Id, Titulo, Artista, Genero, Fecha FROM Canciones ORDER BY Titulo";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        songs.Add(MapToCancion(reader));
                    }
                }
            }
            return songs;
        }

        public async Task<Cancion> ObtenerPorIdAsync(int id)
        {
            const string query = "SELECT Id, Titulo, Artista, Genero, Fecha FROM Canciones WHERE Id = @Id";
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                        return MapToCancion(reader);
                }
            }
            return null;
        }

        public async Task AgregarAsync(Cancion song)
        {
            const string query = @"INSERT INTO Canciones (Titulo, Artista, Genero, Fecha) 
                                   VALUES (@Titulo, @Artista, @Genero, @Fecha)";
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Titulo", song.Tittle);
                command.Parameters.AddWithValue("@Artista", song.Artist);
                command.Parameters.AddWithValue("@Genero", song.Genre);
                command.Parameters.AddWithValue("@Fecha", (object)song.Year ?? DBNull.Value);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task ActualizarAsync(Cancion song)
        {
            const string query = @"UPDATE Canciones SET Titulo = @Titulo, Artista = @Artista, 
                                   Genero = @Genero, Fecha = @Fecha WHERE Id = @Id";
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", song.Id);
                command.Parameters.AddWithValue("@Titulo", song.Tittle);
                command.Parameters.AddWithValue("@Artista", song.Artist);
                command.Parameters.AddWithValue("@Genero", song.Genre);
                command.Parameters.AddWithValue("@Fecha", (object)song.Year ?? DBNull.Value);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> EliminarAsync(int id)
        {
            const string query = "DELETE FROM Canciones WHERE Id = @Id";
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                await connection.OpenAsync();
                int rows = await command.ExecuteNonQueryAsync();
                return rows > 0;
            }
        }

        // Sobrecarga de métodos de búsqueda (sobrecarga de métodos)
        public async Task<List<Cancion>> BuscarPorTituloAsync(string tittle)
        {
            return await BuscarPorTituloYArtistaAsync(tittle, null);
        }

        public async Task<List<Cancion>> BuscarPorTituloYArtistaAsync(string tittle, string artist)
        {
            var song = new List<Cancion>();
            string query = "SELECT Id, Titulo, Artista, Genero, Fecha FROM Canciones WHERE 1=1";
            if (!string.IsNullOrEmpty(tittle))
                query += " AND Titulo LIKE @Titulo";
            if (!string.IsNullOrEmpty(artist))
                query += " AND Artista LIKE @Artista";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                if (!string.IsNullOrEmpty(tittle))
                    command.Parameters.AddWithValue("@Titulo", $"%{tittle}%");
                if (!string.IsNullOrEmpty(artist))
                    command.Parameters.AddWithValue("@Artista", $"%{artist}%");

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                        song.Add(MapToCancion(reader));
                }
            }
            return song;
        }

        // Método privado de mapeo (evita repetición)
        private Cancion MapToCancion(SqlDataReader reader)
        {
            return new Cancion(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4)
            );
        }
    }
}
