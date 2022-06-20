using System;
using System.Collections.Generic;

#nullable disable

namespace AplicacionU1_API.Models
{
    public partial class Usuarios
    {
        public Usuarios()
        {
            Partidas = new HashSet<Partidas>();
        }

        public int IdUsuario { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public virtual ICollection<Partidas> Partidas { get; set; }
    }
}
