using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using AplicacionU2_Cliente.Models;
using AplicacionU2_Cliente.Repositories;
using AplicacionU2_Cliente.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AplicacionU2_Cliente.ViewModels
{
    public class CuponesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        //para notificaciones toast y snack
        INotificacion Notificacion = DependencyService.Get<INotificacion>();
        //al inicio de la aplicación el usuario tiene que elegir de qué categoría va a ver los cupones
        public Command ElegirCategoriaCommand { get; set; }
        public Command RegresarInicioCommand { get; set; }
        public Command VerDetallesCommand { get; set; }

        //ventanas
        DetallesCuponView detallesVw;
        CuponesPorCategoriaView cuponesVw;

        //repositorio para agregar los datos la bd local del dispositivo
        CuponesRepository repos = new CuponesRepository();
        //helper para traer los cupones al elegir cateogoria
        Helpers.HttpClientHelper<Cupones> client = new Helpers.HttpClientHelper<Cupones>(new Uri("https://181g0138.82g.itesrc.net/api/cupones"));
        //lista de cupones en pantalla
        private ObservableCollection<Cupones> cupones;
        public ObservableCollection<Cupones> Cupones
        {
            get { return cupones; }
            set { cupones = value; }
        }

        //propiedad cupon para la vista detalles
        private Cupones cupon;
        public Cupones Cupon
        {
            get { return cupon; }
            set
            {
                cupon = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Cupon"));
            }
        }
        private string error;
        public string Error
        {
            get { return error; }
            set
            {
                error = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Error"));
            }
        }

        //propiedad categoria que se estará utilizando para traer la lista de cupones al abrir la ventana y al estar actualizando
        private string categoria;
        public string Categoria
        {
            get { return categoria; }
            set
            {
                categoria = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Categoria"));
            }
        }

        public void Actualizar()
        {//traer los cupones de la bd local cada que se actualice según se agregue algo o elimine
            if (!string.IsNullOrWhiteSpace(Categoria))
            {
                var datos = repos.GetByCategoria(Categoria);
                Cupones.Clear();
                foreach (var item in datos)
                {
                    Cupones.Add(item);
                }
            }
            Notificacion.SnackAlert("Se ha actualizado la lista de cupones.");
            Toast();
        }
        private void ElegirCategoria(string cat)
        {
            Categoria = cat;
            var datos = repos.GetByCategoria(Categoria);
            Cupones.Clear();
            foreach (var item in datos)
            {//verificar aqui que traiga la cantidad correcta de cupones
                Cupones.Add(item);
            }

            if (cuponesVw == null)
                cuponesVw = new CuponesPorCategoriaView() { BindingContext = this };

            cuponesVw.BindingContext = this;
            App.Current.MainPage.Navigation.PushAsync(cuponesVw);
            Toast();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        private void Regresar()
        {//cerramos la ventana de la lista de cupones o de detalles cupon
            App.Current.MainPage.Navigation.PopAsync();
            Toast();
        }
        private void VerDetalles(Cupones cupon)
        {
            Cupon = cupon;
            if (detallesVw == null)
                detallesVw = new DetallesCuponView() { BindingContext = this };
            if (Cupon != null)
                //abrimos la ventana detalles aquí se verán las propiedadades de cupon 
                App.Current.MainPage.Navigation.PushAsync(detallesVw);
            Toast();
        }
        public CuponesViewModel()
        {

            ElegirCategoriaCommand = new Command<string>(ElegirCategoria);
            RegresarInicioCommand = new Command(Regresar);
            VerDetallesCommand = new Command<Cupones>(VerDetalles);

            if (!Preferences.ContainsKey("Download"))
                DescargarUnicaVez();
            Cupones = new ObservableCollection<Cupones>();
            App.CuponesActualizados += App_CuponesActualizados;
            Toast();
        }
        private void Toast()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.None)
            {
                Notificacion.ToastAlert("Se ha perdido la conexión a internet.");
            }
        }
        private void App_CuponesActualizados()
        {
            Actualizar();
            Toast();
        }

        //traer todos los cupones la primera vez que abra la app
        public async void DescargarUnicaVez()
        {
            var fechaupdate = DateTime.Now;

            var cupones = await client.Get();

            if (cupones != null)
            {
                foreach (var c in cupones)
                {
                    repos.Insert(c);
                }
                Preferences.Set("Download", fechaupdate);
            }
        }
    }
}
