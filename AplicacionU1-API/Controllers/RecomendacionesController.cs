using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AplicacionU1_API.Models;
using AplicacionU1_API.Repositories;

namespace AplicacionU1_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecomendacionesController : ControllerBase
    {
        public itesrcne_181G0138Context Context { get; }
        RecomendacionesRepository repos;
        public RecomendacionesController(itesrcne_181G0138Context context)
        {
            Context = context;
            repos = new(Context);
        }
        public IActionResult Get()
        {
            var all = repos.GetAll().OrderByDescending(x => x.Puntuacion).ThenBy(x => x.TituloLibro).Select(
                x => new
                {
                    x.Id,
                    x.TituloLibro,
                    x.Autor,
                    x.Genero,
                    x.Opinion,
                    x.Puntuacion
                });
            return Ok(all);
        }
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var r = repos.Get(id);

            if (r == null)
                return NotFound();
            else
                return Ok(r);
        }
        [HttpPost("sincronizar")]
        public IActionResult Post([FromBody] DateTime date)
        {
            return Ok(repos.GetSinceDate(date));
        }

        [HttpPost]
        public IActionResult Post([FromBody] Recomendacione r)
        {
            string errores = "";
            if (r == null)
                return BadRequest();

            if (string.IsNullOrWhiteSpace(r.TituloLibro))
                errores += "Ingrese el nombre del libro que recomienda." + "\n";
            if (string.IsNullOrWhiteSpace(r.Autor))
                errores += "Ingrese el nombre del autor del libro." + "\n";
            if (string.IsNullOrWhiteSpace(r.Genero))
                errores += "Ingrese el género al que pertenece el libro." + "\n";
            if (string.IsNullOrWhiteSpace(r.Opinion))
                errores += "Ingrese su opinión sobre el libro." + "\n";
            if (r.Puntuacion == 0)
                errores += "No olvides darle una puntuación al libro que recomiendas." + "\n";

            var x = Context.Recomendaciones.Any(x => x.TituloLibro.ToLower() == r.TituloLibro.ToLower() && x.Eliminado==0);
            if (x)
                errores += "Ya existe un libro registrado con este título.";

            if (errores == "")
            {
                r.Id = 0;
                repos.Insert(r);
                return Ok();
            }
            else
                return BadRequest(errores);
        }
        [HttpPut]
        public IActionResult Put([FromBody] Recomendacione r)
        {
            string errores = "";
            var rec = repos.Get(r.Id);

            if (rec == null)
                return NotFound();
            if (r == null)
                return BadRequest();


            if (string.IsNullOrWhiteSpace(r.TituloLibro))
                errores += "Ingrese el nombre del libro que recomienda." + "\n";
            if (string.IsNullOrWhiteSpace(r.Autor))
                errores += "Ingrese el nombre del autor del libro." + "\n";
            if (string.IsNullOrWhiteSpace(r.Genero))
                errores += "Ingrese el género al que pertenece el libro." + "\n";
            if (string.IsNullOrWhiteSpace(r.Opinion))
                errores += "Ingrese su opinión sobre el libro." + "\n";
            if (r.Puntuacion == 0)
                errores += "No olvides darle una puntuación al libro que recomiendas." + "\n";

            var x = Context.Recomendaciones.Any(x => x.TituloLibro.ToLower() == r.TituloLibro.ToLower() && x.Eliminado == 0  && x.Id != r.Id);
            if (x)
                errores += "Ya existe un libro registrado con este título.";

            if (errores == "")
            {
                rec.TituloLibro = r.TituloLibro;
                rec.Autor = r.Autor;
                rec.Genero = r.Genero;
                rec.Opinion = r.Opinion;
                rec.Puntuacion = r.Puntuacion;

                repos.Update(rec);
                return Ok();
            }
            else
                return BadRequest(errores);
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var rec = repos.Get(id);

            if (rec == null)
                return NotFound();
            else
            {
                repos.Delete(rec);
                return Ok();
            }
        }
    }
}
