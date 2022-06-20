using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AplicacionU1_API.Models;
using AplicacionU1_API.Repositories;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;

namespace AplicacionU1_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public IConfiguration Configuration { get; }

        public itesrcne_181g0138Context Context { get; }
        LoginRepository repos;
        public AccountController(itesrcne_181g0138Context context, IConfiguration config)
        {
            Configuration = config;
            Context = context;
            repos = new LoginRepository(context);
        }

        //método que recibe user y password para verificar si el usuario existe en el sistema
        [HttpPost]
        public IActionResult Post(LoginData datos)
        {
            if (string.IsNullOrWhiteSpace(datos.username))
            {
                ModelState.AddModelError("", "El nombre de usuario es requerido.");
            }
            if (string.IsNullOrWhiteSpace(datos.password))
            {
                ModelState.AddModelError("", "La contraseña es requerida.");
            }

            if (ModelState.IsValid)
            {
                var user = repos.Login(datos.username, datos.password);
                //se verifica que se haya regresado un usuario y se crea el token con las claims
                if (user != null)
                {
                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()));
                    claims.Add(new Claim(ClaimTypes.Name, user.Username));

                    //crear token
                    var handler = new JwtSecurityTokenHandler();
                    var descriptor = new SecurityTokenDescriptor();

                    descriptor.Issuer = Configuration["Jwt:Issuer"];
                    descriptor.Audience = Configuration["Jwt:Audience"];
                    //La identidad a quien se emite el token.
                    descriptor.Subject = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
                    //En cuanto tiempo se expira el token que mandamos
                    descriptor.Expires = DateTime.UtcNow.AddDays(20);
                    descriptor.IssuedAt = DateTime.UtcNow;
                    descriptor.SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Secret"])), SecurityAlgorithms.HmacSha256);

                    var token = handler.CreateToken(descriptor);
                    var tokenSerializado = handler.WriteToken(token);

                    return Ok(tokenSerializado);
                }
                else
                {
                    return Unauthorized();

                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPost("register")]
        public IActionResult Registrar(LoginData datos)
        {
            if (string.IsNullOrWhiteSpace(datos.username))
            {
                ModelState.AddModelError("", "El nombre de usuario es requerido.");
            }
            if (string.IsNullOrWhiteSpace(datos.password))
            {
                ModelState.AddModelError("", "La contraseña es requerida.");
            }

            if (ModelState.IsValid)
            {
                Usuarios usuario = new()
                {
                    Username = datos.username,
                    Password = datos.password
                };
                repos.Insert(usuario);

                //se crea el token con las claims
              
                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()));
                    claims.Add(new Claim(ClaimTypes.Name, usuario.Username));

                    //crear token
                    var handler = new JwtSecurityTokenHandler();
                    var descriptor = new SecurityTokenDescriptor();

                    descriptor.Issuer = Configuration["Jwt:Issuer"];
                    descriptor.Audience = Configuration["Jwt:Audience"];
                    //La identidad a quien se emite el token.
                    descriptor.Subject = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
                    //En cuanto tiempo se expira el token que mandamos
                    descriptor.Expires = DateTime.UtcNow.AddDays(20);
                    descriptor.IssuedAt = DateTime.UtcNow;
                    descriptor.SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Secret"])), SecurityAlgorithms.HmacSha256);

                    var token = handler.CreateToken(descriptor);
                    var tokenSerializado = handler.WriteToken(token);

                    return Ok(tokenSerializado);
              
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}
