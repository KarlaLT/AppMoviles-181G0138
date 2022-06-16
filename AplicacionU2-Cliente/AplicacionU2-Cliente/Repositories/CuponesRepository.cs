using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using AplicacionU2_Cliente.Models;

namespace AplicacionU2_Cliente.Repositories
{
    public class CuponesRepository
    {
        public SQLiteConnection Context { get; set; }

        public CuponesRepository()
        {
            var ruta = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "cupones.db3";

            //crea la bd local si existe y la abre si ya existe
            Context = new SQLiteConnection(ruta);

            //si no existe la tabla la crea 
            Context.CreateTable<Cupones>();
        }
        public Cupones Get(int id)
        {
            return Context.Table<Cupones>().FirstOrDefault(x => x.IdCupon == id);
        }
        public IEnumerable<Cupones> GetByCategoria(string categoria)
        {
            return Context.Table<Cupones>().Where(x => x.Categoria.ToUpper() == categoria.ToUpper()).OrderByDescending(x => x.FechaFin).ThenBy(x => x.Porciento);
        }
        public IEnumerable<Cupones> GetAll()
        {
            return Context.Table<Cupones>().OrderByDescending(x => x.FechaFin).ThenBy(x => x.Porciento);
        }
        public void Insert(Cupones c)
        {
            Context.Insert(c);
        }
        public void Update(Cupones c)
        {
            Context.Update(c);
        }
        public void Delete(Cupones c)
        {
            Context.Delete(c);
        }
    }
}
