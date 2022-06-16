using System;
using System.Collections.Generic;

#nullable disable

namespace AplicacionU1_API.Models
{
    public partial class Cupones
    {
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
