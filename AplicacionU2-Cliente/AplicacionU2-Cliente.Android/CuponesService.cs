using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AplicacionU2_Cliente.Models;
using AplicacionU2_Cliente.Repositories;
using AplicacionU2_Cliente.Helpers;
using Firebase.Messaging;
using AndroidX.Core.App;
using System.Globalization;

[assembly: Xamarin.Forms.Dependency(typeof(AplicacionU2_Cliente.Droid.CuponesService))]

namespace AplicacionU2_Cliente.Droid
{
    [Service(Exported = true)]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class CuponesService : FirebaseMessagingService
    {
       // CuponesRepository repos = new CuponesRepository();
        HttpClientHelper<Cupones> Cliente;

        public  override void OnMessageReceived(RemoteMessage message)
        {
            try
            {
                CuponesRepository repos = new CuponesRepository();
                Cliente = new HttpClientHelper<Cupones>(new Uri("https://181g0138.82g.itesrc.net/api/cupones"));
                var datos = message.Data;

                if (datos["Accion"] == "Nuevo")
                {
                    int id = int.Parse(datos["Id"]);                 
                    //agregar a la bd local
                    var cupon =  Cliente.Get(id).Result;
                     repos.Insert(cupon);

                    if (App.Current == null)
                    {//si la app está cerrada, llega notif
                        if (cupon != null)
                           ShowNotification(id, cupon.Titulo, cupon.Descripcion);
                    }
                    else
                    {//si la aplicaciónestá abierta se actualiza
                        App.Actualizar();
                    }
                }
                else if (datos["Accion"] == "Eliminar")
                {
                    int id = int.Parse(datos["Id"]);
                    var cupon = repos.Get(id);

                    //verificar que exista en la bd local y eliminarlo si sí
                    if (cupon != null)
                    {
                        //eliminar de la bd local
                        repos.Delete(cupon);
                        App.Actualizar();
                    }
                }
            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }
            base.OnMessageReceived(message);
        }

        //mostrar notificacion
        public void ShowNotification(int id, string title, string text)
        {
            NotificationCompat.Builder builder = new NotificationCompat.Builder(Application.Context,
                 "CANALCUPONES")
                 .SetContentTitle(title)
                 .SetContentText(text)
                 .SetPriority(NotificationCompat.PriorityMax)
                 .SetShowWhen(true)
                 .SetSmallIcon(Resource.Drawable.cupon3);

            NotificationManager manager = Application.Context.GetSystemService(Application.NotificationService)
                as NotificationManager;

            manager.Notify(id, builder.Build());
        }
    }

}