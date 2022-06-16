using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Firebase.Messaging;
using Firebase;
using Plugin.CurrentActivity;

namespace AplicacionU2_Cliente.Droid
{
    [Activity(Label = "MyCuppon", Icon = "@mipmap/MyCupponIcon", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static Activity CurrentActivity { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            CurrentActivity = this;
            base.OnCreate(savedInstanceState);
//            CrossCurrentActivity.Current.Init(this);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            //suscribirnos al topic de las notificaciones, viene en la api
            FirebaseMessaging.Instance.SubscribeToTopic("cupones");
            NotificationManager manager = Application.Context.GetSystemService(Application.NotificationService)
                         as NotificationManager;
            manager.CreateNotificationChannel(new NotificationChannel("CANALCUPONES", "Canal de MyCuppon", NotificationImportance.Max));
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}