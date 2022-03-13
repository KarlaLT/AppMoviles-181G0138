using System;
using System.Collections.Generic;
using System.Text;

namespace AplicacionU1_Cliente.Models
{
    public enum Estado { Agregado, Modificado, Eliminado}
    public class ElementosBuffer
    {
        public Recomendaciones Recomendacion { get; set; }
        public Estado Estado { get; set; }
    }
}
