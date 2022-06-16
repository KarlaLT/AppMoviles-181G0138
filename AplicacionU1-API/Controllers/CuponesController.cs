using AplicacionU1_API.Models;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AplicacionU1_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuponesController : ControllerBase
    {
        public itesrcne_181g0138Context Context { get; }
        Repositories.Repository<Cupones> repos;
        public CuponesController(itesrcne_181g0138Context context)
        {
            Context = context;
            repos = new(Context);

            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile("cupones.json")
                }) ;
            }
        }
        [HttpGet]
        public IActionResult Get()
        {
            var cupones = repos.GetAll().Where(x => x.FechaFin >= DateTime.Now).OrderByDescending(x => x.FechaFin)
                    .ThenByDescending(x => x.Porciento);

            //var vencidos = repos.GetAll().Where(x => x.FechaFin < DateTime.Now);
            //foreach (var cupon in vencidos)
            //{
            //    repos.Delete(cupon);
            //}
            return Ok(cupones);
        }

        //los cupones se dividirán en categorías: alimentos, vestimenta, entretenimiento y tecnología
        [HttpGet("categoria/{categoria}")]
        public IActionResult Get(string categoria)
        {//traer una lista de los cupones que aún no vencen ordenados primero por quienes vencen primero y al final los que tienen
         //más tiempo disponible
            try
            {
                if (categoria.ToLower() == "alimentos" || categoria.ToLower() == "vestimenta" || categoria.ToLower() == "entretenimiento")
                {
                    var cupones = repos.GetAll().Where(x => x.Categoria.ToLower() == categoria.ToLower()).Where(x => x.FechaFin >= DateTime.Now).OrderByDescending(x => x.FechaFin)
                     .ThenByDescending(x => x.Porciento);

                    //eliminar los q ya estén vencidos
                    //var vencidos = repos.GetAll().Where(x => x.FechaFin < DateTime.Now);
                    //foreach (var cupon in vencidos)
                    //{
                    //    repos.Delete(cupon);
                    //}
                    return Ok(cupones);
                }
                else
                {
                    return BadRequest("Categoría inexistente.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{idCupon}")]
        public IActionResult Get(int idCupon)
        {
            try
            {
                if (idCupon == 0)
                {
                    return BadRequest();
                }

                var cupon = repos.Get(idCupon);
                if (cupon == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(cupon);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(Cupones cupon)
        {
            try
            {
                string errores = "";
                if (cupon == null)
                {
                    return BadRequest();
                }
                if (string.IsNullOrWhiteSpace(cupon.Titulo))
                {
                    errores += "Ingrese el título del cupón.";
                }
                if (string.IsNullOrWhiteSpace(cupon.Descripcion))
                {
                    errores += "Ingrese una descripción para el cupón.";
                }
                if (string.IsNullOrWhiteSpace(cupon.Tienda))
                {
                    errores += "Ingrese el nombre de la tienda en que es válido el cupón.";
                }
                if (string.IsNullOrWhiteSpace(cupon.Categoria))
                {
                    errores += "Ingrese la categoría a la que pertenece el cupón.";
                }
                if (cupon.Porciento <= 0)
                {
                    errores += "El descuento del cupón no puede ser menor o igual a 0%.";
                }
                if (cupon.FechaFin <= DateTime.Now)
                {
                    errores += "La fecha de vencimiento del cupón tiene que ser mayor al día actual.";
                }//la fecha de inicio puede ser antes al día de hoy o después del día de hoy, no importa mientras la de fin sea válida

                //verificar que no exista ya el cupón registrado
                if (Context.Cupones.Any(x => x.Titulo.ToLower() == cupon.Titulo.ToLower()))
                {
                    errores += "Este cupón ya está registrado.";
                }

                if (errores == "")
                {
                    repos.Insert(cupon);

                    //Notificaciones
                    Message m = new Message()
                    {//quienes se suscriban a este topic son quienes recibirán las notificaciones
                        Topic="cupones",
                        Data= new Dictionary<string, string>()
                        {
                            {"Id", cupon.IdCupon.ToString() },
                            //{"Categoria", cupon.Categoria },
                            //{"Titulo", cupon.Titulo },
                            //{"Descripcion", cupon.Descripcion },
                            //{"Tienda", cupon.Tienda },
                            //{"FechaInicio", cupon.FechaInicio.ToString() },
                            //{"FechaFin", cupon.FechaFin.ToString() },
                            //{"PorcientoDescuento", cupon.Porciento.ToString() },
                            {"Accion", "Nuevo" }
                        }
                    };
                   await FirebaseMessaging.DefaultInstance.SendAsync(m);
                    return Ok();
                }
                else
                {
                    return BadRequest(errores);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{idCupon}")]
        public async  Task<IActionResult> Delete(int idCupon)
        {
            try
            {
                if (idCupon == 0)
                {
                    return BadRequest();
                }

                var cupon = repos.Get(idCupon);
                if (cupon == null)
                {
                    return NotFound();
                }
                else
                {
                    Message m = new Message()
                    {//quienes se suscriban a este topic son quienes recibirán las notificaciones
                        Topic = "cupones",
                        Data = new Dictionary<string, string>()
                        {
                            {"Id", cupon.IdCupon.ToString() },
                            {"Accion", "Eliminar" }
                        }
                    };
                    await FirebaseMessaging.DefaultInstance.SendAsync(m);
                    repos.Delete(cupon);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
