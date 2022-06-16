using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Snackbar;
using Plugin.CurrentActivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(AplicacionU2_Cliente.Droid.MensajesNotificaciones))]
namespace AplicacionU2_Cliente.Droid
{
    public class MensajesNotificaciones : INotificacion
    {
        public void SnackAlert(string mensaje)
        {
            Activity activity = MainActivity.CurrentActivity;
            Android.Views.View view = activity.FindViewById(Android.Resource.Id.Content);
            Snackbar.Make(view, mensaje, Snackbar.LengthLong).Show();
        }

        public void ToastAlert(string mensaje)
        {
            Toast.MakeText(Application.Context, mensaje, ToastLength.Long).Show();
        }
    }
}