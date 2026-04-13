using System;

namespace Canciones.Entidades
{
    public class Cancion : EntidadBase
    {
        // Propiedades
        public string Tittle { get; set; }
        public string Artist { get; set; }
        public string Genre { get; set; }
        public int?  Year { get; set; }

        // Constructores (sobrecarga de constructores)
        public Cancion() { }

        public Cancion(string tittle, string artist, string genre)
        {
            Tittle = tittle;
            Artist = artist;
            Genre = genre;
        }

        public Cancion(string tittle, string artist, string genre, int? year)
            : this(tittle, artist, genre)
        {
            Year = year;
        }

        public Cancion(int id, string tittle, string artist, string genre, int? year)
            : this(tittle, artist, genre, year)
        {
            Id = id;
        }

        // Implementación del método abstracto
        public override void MostrarInfo()
        {
            string yearStr = Year.HasValue ? $"[{Year}]" : "[Año desconocido]";
            Console.WriteLine($"{Id}. {Tittle} - {Artist} ({Genre}) {yearStr}");
        }
    }
}
