using System;
using System.Collections.Generic;

#nullable disable

namespace AplicacionU1_API.Models
{
    public partial class Recomendacione
    {
        public int Id { get; set; }
        public string TituloLibro { get; set; }
        public string Autor { get; set; }
        public string Opinion { get; set; }
        public string Genero { get; set; }
        public int Puntuacion { get; set; }
        public int? Eliminado { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
