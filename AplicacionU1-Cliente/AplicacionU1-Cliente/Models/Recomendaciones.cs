using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AplicacionU1_Cliente.Models
{
    public class Recomendaciones
    {
        [PrimaryKey] 
                public int Id { get; set; }
        public string TituloLibro { get; set; }
        public string Autor { get; set; }
        public string Genero { get; set; }
        public string Opinion { get; set; }
        public byte Puntuacion { get; set; }
    }

}
