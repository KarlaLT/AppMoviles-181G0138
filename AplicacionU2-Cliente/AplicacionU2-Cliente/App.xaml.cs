using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AplicacionU2_Cliente.Views;
using Microsoft.Extensions.DependencyInjection;
namespace AplicacionU2_Cliente
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; set; }
        public static event Action CuponesActualizados;

        public static void Actualizar()
        {
            CuponesActualizados?.Invoke();
        }
        public App()
        {
            InitializeComponent();
            SetupServices();
            MainPage = new NavigationPage(new InicioView());
        }

        private void SetupServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<Repositories.CuponesRepository>();
            ServiceProvider = services.BuildServiceProvider();
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
