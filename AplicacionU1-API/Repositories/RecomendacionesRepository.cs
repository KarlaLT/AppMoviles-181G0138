using System;
using System.Collections.Generic;
using System.Linq;
using AplicacionU1_API.Models;

namespace AplicacionU1_API.Repositories
{
    public class RecomendacionesRepository : Repository<Recomendaciones>
    {
        public override itesrcne_181g0138Context Context { get; }

        public RecomendacionesRepository(itesrcne_181g0138Context context) : base(context)
        {
            Context = context;
        }
        public override IEnumerable<Recomendaciones> GetAll()
        {
            return Context.Set<Recomendaciones>().Where(x => x.Eliminado != 1);
        }
        public override Recomendaciones Get(object id)
        {
            var r = Context.Find<Recomendaciones>(id);
            if (r.Eliminado != 1)
                return base.Get(id);
            else
                return null;
        }
        public override void Insert(Recomendaciones entidad)
        {
            entidad.TimeStamp = DateTime.Now;

            base.Insert(entidad);
        }
        public override void Update(Recomendaciones entidad)
        {
            entidad.TimeStamp = DateTime.Now;

            base.Update(entidad);
        }
        public override void Delete(Recomendaciones entidad)
        {
            entidad.Eliminado = 1;
            entidad.TimeStamp = DateTime.Now;
            base.Update(entidad);

            //Eliminar los q excedan el límite de días en la bd
            int ttl = 30;
            var fechaVencimiento = DateTime.Now.Subtract(TimeSpan.FromDays(ttl));

            var vencidos = Context.Set<Recomendaciones>().Where(x => x.Eliminado == 1 && x.TimeStamp < fechaVencimiento);

            foreach (var item in vencidos)
            {
                Context.Remove(item);
            }
            Context.SaveChanges();
        }
        public List<object> GetSinceDate(DateTime date)
        {
            var noeliminados = Context.Set<Recomendaciones>().Where(x => x.Eliminado == 0 && x.TimeStamp >= date).Select(x =>
                  new
                  {
                      x.Id,
                      x.TituloLibro,
                      x.Autor,
                      x.Genero,
                      x.Opinion,
                      x.Puntuacion
                  });
            var eliminados = Context.Set<Recomendaciones>().Where(x => x.Eliminado == 1 && x.TimeStamp >= date).Select(x =>
                  new
                  {
                      x.Id
                  });
            List<object> actualizados = new();
            actualizados.AddRange(noeliminados);
            actualizados.AddRange(eliminados);
            return actualizados;
        }

    }
}
