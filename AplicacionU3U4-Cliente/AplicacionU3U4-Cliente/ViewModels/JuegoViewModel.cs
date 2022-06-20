using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using AplicacionU3U4_Cliente.Views;
using Xamarin.Forms;
using AplicacionU3U4_Cliente.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MarcTron.Plugin;

namespace AplicacionU3U4_Cliente.ViewModels
{
    public class JuegoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        HttpClient client = new HttpClient() { BaseAddress = new Uri("https://181g0138.82g.itesrc.net/") };
        //cronometro para la jugada
        System.Timers.Timer timer = new System.Timers.Timer();
        public JuegoViewModel()
        {
            //cargar vídeo de anuncio interstitial
            CrossMTAdmob.Current.LoadInterstitial("ca-app-pub-2238878027553821/2528447412");
            //cargar vídeo reward
            CrossMTAdmob.Current.LoadRewardedVideo("ca-app-pub-2238878027553821/4781445357");

            CrossMTAdmob.Current.OnRewardedVideoAdCompleted += Current_OnRewardedVideoAdCompleted;

            LoginCommand = new Command(Login);
            VerRegistrarmeCommand = new Command(VerRegistrarme);
            RegistrarmeCommand = new Command(Registrarme);
            CancelarCommand = new Command(Regresar);
            VerTablaPartidasCommand = new Command(VerTablaPartidas);
            LogoutCommand = new Command(Logout);
            IniciarJuegoCommand = new Command(EmpezarJuego);
            EnviarRespuestaCommand = new Command(EnviarRespuesta);
            AgregarTiempoCommand = new Command(AgregarTiempo);
            Usuario = new LoginData();

            timer.Elapsed += Timer_Elapsed;
            Bloqueo = false;
        }

        private void Current_OnRewardedVideoAdCompleted(object sender, EventArgs e)
        {//si el vídeo de reward terminó, se agregan 10 seg a la partida
            SegundosRestantes += 10;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (SegundosRestantes > 0)
            {
                SegundosRestantes -= 1;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
            else
            {
                timer.Stop();
                Bloqueo = false;
                //se manda post a la api agregando la nueva partida jugada
                PostPartida();
                Respuesta = 0;
                Puntuacion = 0;
                Error = "Dirígete a registro de partidas para ver tus resultados.";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));

            }
        }
        public int Puntuacion { get; set; } = 0;
        private bool bloqueo;

        public bool Bloqueo
        {
            get { return bloqueo; }
            set { bloqueo = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null)); }
        }

        public void EnviarRespuesta()
        {
            if (Respuesta == Resultado)
            {//si la respuesta es correcta aumentan puntos
                Puntuacion += 100;
            }
            //lanza una nueva operación a resolver
            Respuesta = 0;
            Random r = new Random();
            Numero1 = r.Next(1, 11);
            Numero2 = r.Next(1, 11);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        //comandos
        public Command LoginCommand { get; set; }
        public Command LogoutCommand { get; set; }
        public Command VerRegistrarmeCommand { get; set; }
        public Command RegistrarmeCommand { get; set; }
        public Command CancelarCommand { get; set; }
        public Command IniciarJuegoCommand { get; set; }
        public Command EnviarRespuestaCommand { get; set; }
        public Command VerTablaPartidasCommand { get; set; }
        public Command AgregarTiempoCommand { get; set; }

        //vistas
        RegistrarView registrarView;
        PartidasView partidasView;

        //propiedades
        private LoginData usuario;
        public LoginData Usuario
        {
            get { return usuario; }
            set { usuario = value; }
        }

        private Partidas partidas;

        public Partidas Partida
        {
            get { return partidas; }
            set
            {
                partidas = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }
        private List<Partidas> listaPartidas;

        public List<Partidas> ListaPartidas
        {
            get { return listaPartidas; }
            set
            {
                listaPartidas = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }
        private int numero1;

        public int Numero1
        {
            get { return numero1; }
            set
            {
                numero1 = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }

        private int numero2;

        public int Numero2
        {
            get { return numero2; }
            set
            {
                numero2 = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }

        public int Resultado
        {
            get { return numero1 * numero2; }
        }
        private int respuest;

        public int Respuesta
        {
            get { return respuest; }
            set
            {
                respuest = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }
        private string error;

        public string Error
        {
            get { return error; }
            set
            {
                error = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }
        private int segundosRestantes = 30;

        public int SegundosRestantes
        {
            get { return segundosRestantes; }
            set { segundosRestantes = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null)); }
        }

        public bool Indicador { get; set; }

        //métodos
        public async void Login()
        {
            Error = "";
            //enviar solicitud a api/account con datos de usuario
            if (string.IsNullOrWhiteSpace(Usuario.username))
            {
                Error = "El nombre de usuario es requerido.";
            }
            if (string.IsNullOrWhiteSpace(Usuario.password))
            {
                Error = "El nombre de usuario es requerido.";
            }

            if (Error == "")
            {
                Indicador = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));

                var json = JsonConvert.SerializeObject(Usuario);
                var result = await client.PostAsync("api/account", new
                    StringContent(json, Encoding.UTF8, "application/json"));

                //verificar que se devuelva un Ok y guardar el token recibido
                if (result.IsSuccessStatusCode)
                {
                    var token = await result.Content.ReadAsStringAsync();
                    App.User.LogIn(token);
                    App.User.Redirect();
                    Indicador = false;
                }
                else
                {
                    Error = "Usuario o contraseña incorrectos.";
                }
            }
        }

        //cerrar sesión, se elimina token y datos del claims principal
        void Logout()
        {
            App.User.LogOut();
        }
        void VerRegistrarme()
        {
            if (CrossMTAdmob.Current.IsInterstitialLoaded())
                CrossMTAdmob.Current.ShowInterstitial();

            if (registrarView == null)
                registrarView = new RegistrarView() { BindingContext = this };
            Application.Current.MainPage.Navigation.PushAsync(registrarView);
        }

        //get de partidas como peticion a la api con el id del usuario parametro
        public async void VerTablaPartidas()
        {
            Usuario = new LoginData();
            //traer el id del usuario para hacer la peticion y el token para el auth
            var token = Xamarin.Essentials.SecureStorage.GetAsync("Token").Result;

            //ESTO DA ERROR NULL EXCEPTION
            var id = Xamarin.Essentials.SecureStorage.GetAsync("idUsuario").Result;
            var nombre = Xamarin.Essentials.SecureStorage.GetAsync("nombreUsuario").Result;
            Usuario.username = nombre;

            var auth = client.DefaultRequestHeaders.Authorization;
            if (auth == null)
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var result = await client.GetAsync("api/partidas/" + id);
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                ListaPartidas = new List<Partidas>();
                ListaPartidas = JsonConvert.DeserializeObject<List<Partidas>>(json);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));

                if (partidasView == null)
                    partidasView = new PartidasView() { BindingContext = this };
                await Application.Current.MainPage.Navigation.PushAsync(partidasView);
            }
            else
            {
                Error = "No hay partidas registradas con este usuario.";
            }
        }

        //post de partida
        public async void PostPartida()
        {
            //traer el id del usuario para hacer la peticion y el token para el auth
            var token = Xamarin.Essentials.SecureStorage.GetAsync("Token").Result;

            var id = Xamarin.Essentials.SecureStorage.GetAsync("idUsuario").Result;

            Partida = new Partidas()
            {
                Puntuacion = Puntuacion,
                Fecha = DateTime.Now,
                FkIdUsuario = int.Parse(id)
            };
            var json = JsonConvert.SerializeObject(Partida);

            var auth = client.DefaultRequestHeaders.Authorization;
            if (auth == null)
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            //se envia la peticion con la partida serializada
            var result = await client.PostAsync("api/partidas", new StringContent(json, Encoding.UTF8, "application/json"));
            if (!result.IsSuccessStatusCode)
            {
                Error = "Lo sentimos no se pudo registrar la partida, problemas en el sistema.";
            }
        }
        void Regresar()
        {
            Application.Current.MainPage.Navigation.PopAsync();
        }

        //empezar juego
        private void EmpezarJuego()
        {
            SegundosRestantes = 30;
            Random r = new Random();
            Numero1 = r.Next(1, 11);
            Numero2 = r.Next(1, 11);
            // Resultado = Numero1 * Numero2
            timer.Interval = 1000;
            timer.Start();
            Bloqueo = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        //método para registrarme como nuevo usuarip
        public async void Registrarme()
        {
            Error = "";
            if (string.IsNullOrWhiteSpace(Usuario.username))
            {
                Error += "Es requerido un nombre de usuario.";
            }
            if (string.IsNullOrWhiteSpace(Usuario.password))
            {
                Error += "Es requerida una contraseña.";
            }

            if (Error == "")
            {
                var json = JsonConvert.SerializeObject(Usuario);
                var result = await client.PostAsync("api/account/register", new StringContent(json, Encoding.UTF8, "application/json"));
                Indicador = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));

                if (result.IsSuccessStatusCode)
                {
                    //hacer login con el nuevo usuario registrado
                    //al registrar un nuevo usuario la api tmb debe de devolver un token

                    var token = await result.Content.ReadAsStringAsync();
                    App.User.LogIn(token);
                    App.User.Redirect();
                    Indicador = false;
                }
                else
                {
                    json = await result.Content.ReadAsStringAsync();
                    json.Replace("\"", "");
                    Error = json;
                }
            }
        }

        //vídeo reward
        public void AgregarTiempo()
        {
            if (timer.Enabled)
            {
                if (CrossMTAdmob.Current.IsRewardedVideoLoaded())
                    CrossMTAdmob.Current.ShowRewardedVideo();
            }
        }
    }
}
