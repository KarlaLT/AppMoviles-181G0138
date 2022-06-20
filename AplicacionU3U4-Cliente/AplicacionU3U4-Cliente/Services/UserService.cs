using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using AplicacionU3U4_Cliente.Views;
using Xamarin.Forms;

namespace AplicacionU3U4_Cliente.Services
{
    public class UserService
    {
        //issuer, audience, claims, secret y fecha de expiración, se necesitan siempre de un token, ahorita sólo claims y fecha pq es nuestra api
        public bool IsAuthenticated
        {
            get
            {
                var token = Xamarin.Essentials.SecureStorage.GetAsync("Token").Result;
                var fecha = Xamarin.Essentials.SecureStorage.GetAsync("ValidTo").Result;

                if (fecha != null)
                {
                    var fechaExp = Convert.ToDateTime(fecha);
                    return token != null && DateTime.UtcNow <= fechaExp;
                }
                else
                    return false;
                //devolver el token si no es nulo y si es válido aún según su fecha de expiración
            }
        } //cuando existe un token y no está expirado
        public ClaimsPrincipal User { get; private set; }
        public async void LogIn(string jwtToken)
        {
            // guarda el token en el almacenamiento seguro y con el handler leemos el token y sus claims
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);

            //verificar si es para mí if(token.Audicence=="yo") (ahorita sí pq es mi app)

            //guardar el token en el securitystorage del celular
            await Xamarin.Essentials.SecureStorage.SetAsync("Token", jwtToken);
            await Xamarin.Essentials.SecureStorage.SetAsync("ValidTo", token.ValidTo.ToString());

            var claims = token.Claims;
            //guardar los datos de quien inició sesión creando su identidad
            User = new ClaimsPrincipal(new ClaimsIdentity(claims));
            var id = User.Claims.FirstOrDefault(x => x.Type == "nameid").Value;
            var nombre = User.Claims.FirstOrDefault(x => x.Type == "unique_name").Value;

            await Xamarin.Essentials.SecureStorage.SetAsync("idUsuario", id);
            await Xamarin.Essentials.SecureStorage.SetAsync("nombreUsuario", nombre);
        }

        public void LogOut()
        {
            //borra el token del almacenamiento seguro, borra claims y redirige a vista de login
            Xamarin.Essentials.SecureStorage.RemoveAll();
            //  FechaExpiracion = DateTime.MinValue;
            User = null;
            Redirect();
        }

        public void Redirect()
        {
            //se encarga de redirigir a la ventana correspondiente según la autenticación y su identidad
            if (IsAuthenticated)
            {
                App.Current.MainPage = new NavigationPage(new JuegoView());
            }
            else
            {//login view
                App.Current.MainPage = new NavigationPage(new LoginView());
            }
        }
    }
}
