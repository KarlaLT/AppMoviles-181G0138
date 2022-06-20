using System;
using System.Collections.Generic;
using System.Text;

namespace AplicacionU3U4_Cliente.Models
{
    public class Partidas
    {
        public int IdPartida { get; set; }
        public int Puntuacion { get; set; }
        public DateTime Fecha { get; set; }
        public int? FkIdUsuario { get; set; }

    }
}
