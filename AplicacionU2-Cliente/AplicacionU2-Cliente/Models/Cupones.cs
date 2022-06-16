using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace AplicacionU2_Cliente.Models
{
    public class Cupones
    {
        [PrimaryKey]
        public int IdCupon { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public float Porciento { get; set; }
        public string Tienda { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Categoria { get; set; }

      
    }
}
