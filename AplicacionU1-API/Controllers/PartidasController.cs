using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AplicacionU1_API.Repositories;
using AplicacionU1_API.Models;
using Microsoft.AspNetCore.Authorization;

namespace AplicacionU1_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PartidasController : ControllerBase
    {
        public itesrcne_181g0138Context Context { get; }
        PartidasRepository repos;
        public PartidasController(itesrcne_181g0138Context context)
        {
            Context = context;
            repos = new PartidasRepository(context);
        }

        //get para traer una lista de partidas de un usuario en específico
        [HttpGet("{idUsuario}")]
        public IActionResult Get(int idUsuario)
        {
            if (idUsuario == 0)
                return BadRequest();

            var partidas = repos.GetPartidas(idUsuario);
            //verificar que existan partidas
            if (partidas != null)
            {
                return Ok(partidas);
            }
            else
            {
                return NotFound("No existen partidas registradas con este usuario.");
            }

        }

        //post para agregar el registro de una partida 
        [HttpPost]
        public IActionResult Post(Partidas partida)
        {
            if(partida == null)
            {
                return BadRequest();
            }

            if(partida.Puntuacion < 0)
            {
                ModelState.AddModelError("", "La puntuación no puede ser menor a 0.");
            }
            if (ModelState.IsValid)
            {
                repos.Insert(partida);
                return Ok();
            }
            else
            {
                return BadRequest(ModelState);
            } 
        }
    }
}
