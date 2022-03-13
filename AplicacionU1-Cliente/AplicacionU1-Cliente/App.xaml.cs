using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AplicacionU1_Cliente.Views;
using AplicacionU1_Cliente.Models;
using AplicacionU1_Cliente.Services;
namespace AplicacionU1_Cliente
{
    public partial class App : Application
    {
        public static CatalogoRecomendaciones Catalogo { get; set; }
        public static SincronizadorService SincronizarService { get; set; }
        public App()
        {
            InitializeComponent();

            Catalogo = new CatalogoRecomendaciones();
            SincronizarService = new SincronizadorService(Catalogo);

            MainPage = new NavigationPage(new InicioView());
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
