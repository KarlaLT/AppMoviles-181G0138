using System;
using System.Collections.Generic;

#nullable disable

namespace AplicacionU1_API.Models
{
    public partial class Partidas
    {
        public int IdPartida { get; set; }
        public int Puntuacion { get; set; }
        public DateTime Fecha { get; set; }
        public int? FkIdUsuario { get; set; }

        public virtual Usuarios FkIdUsuarioNavigation { get; set; }
    }
}
