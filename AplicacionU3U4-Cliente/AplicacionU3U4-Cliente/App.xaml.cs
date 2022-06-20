using AplicacionU3U4_Cliente.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AplicacionU3U4_Cliente
{
    public partial class App : Application
    {
        public static UserService User { get; set; } = new UserService();
        public App()
        {
            InitializeComponent();
            User.Redirect();
          //  MainPage = new NavigationPage(new AplicacionU3U4_Cliente.Views.LoginView());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
