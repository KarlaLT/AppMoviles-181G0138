using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicacionU2_ClienteWPF.Models
{
    public class Cupones
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
