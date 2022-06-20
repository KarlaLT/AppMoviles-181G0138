using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AplicacionU1_API.Models;
namespace AplicacionU1_API.Repositories
{
    public class PartidasRepository: Repository<Partidas>
    {
        public override itesrcne_181g0138Context Context { get; }

        public PartidasRepository(itesrcne_181g0138Context context) : base(context)
        {
            Context = context;
        }

        public IEnumerable<Partidas> GetPartidas(int idUsuario)
        {
            var partidas = Context.Set<Partidas>().Where(x => x.FkIdUsuario == idUsuario);

            if (partidas != null)
                return partidas;
            else
                return null;
        }
    }
}


