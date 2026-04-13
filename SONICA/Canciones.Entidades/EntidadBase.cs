using System;

namespace Canciones.Entidades 
{
    public abstract class EntidadBase
    {
        public int Id { get; set; }


        public abstract void MostrarInfo();
    }
}