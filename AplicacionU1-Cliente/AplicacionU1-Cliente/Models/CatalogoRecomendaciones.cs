using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AplicacionU1_Cliente.Models
{
    public class CatalogoRecomendaciones
    {
       public  SQLiteConnection connection { get; set; }

        public CatalogoRecomendaciones()
        {
            var ruta = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/bdRecomendaciones.db3";
            connection = new SQLiteConnection(ruta);

            connection.CreateTable<Recomendaciones>();
        }

        public void InsertOrReplace(Recomendaciones r)
        {
            
            var rec = connection.Find<Recomendaciones>(r.Id);


            if (r.TituloLibro == null && r.Autor == null && r.Opinion == null)
            {
                if (rec != null)
                    connection.Delete(rec);
            }
            else if (rec == null)
                connection.Insert(r);
            else
            {
                rec.Autor = r.Autor;
                rec.Genero = r.Genero;
                rec.Opinion = r.Opinion;
                rec.Puntuacion = r.Puntuacion;
                rec.TituloLibro = r.TituloLibro;

                connection.Update(rec);
                
            }

            var x = connection.Table<Recomendaciones>().Count();
        }

        public IEnumerable<Recomendaciones> GetAll()
        {
            return connection.Table<Recomendaciones>().OrderByDescending(X=>X.Puntuacion).ThenBy(X=>X.TituloLibro);
        }
        public Recomendaciones GetById(int id)
        {
            return connection.Find<Recomendaciones>(id);
        }
    }
}
