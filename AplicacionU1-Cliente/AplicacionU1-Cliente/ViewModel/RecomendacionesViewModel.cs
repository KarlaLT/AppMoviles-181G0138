using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using AplicacionU1_Cliente.Models;
using AplicacionU1_Cliente.Views;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace AplicacionU1_Cliente.ViewModel
{

    public class RecomendacionesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        AgregarView agregar; EditarView editar; LibroView libro;


        public RecomendacionesViewModel()
        {
            ListaRecomendaciones = new ObservableCollection<Recomendaciones>();
            Recomendacion = new Recomendaciones();
            VerAgregarCommand = new Command(VerAgregar);
            VerEditarCommand = new Command(VerEditar);
            VerLibroCommand = new Command(VerLibro);
            CancelarCommand = new Command(Cancelar);

            AgregarCommand = new Command(Agregar);
            EditarCommand = new Command(Editar);
            EliminarCommand = new Command(Eliminar);
            BuscarCommand = new Command(Buscar);

            Actualizar();
            App.SincronizarService.Actualizar += SincronizarService_Actualizar;
        }

        private void SincronizarService_Actualizar()
        {
            Actualizar();
        }

        public ICommand VerAgregarCommand { get; set; }
        public ICommand AgregarCommand { get; set; }
        public ICommand VerEditarCommand { get; set; }
        public ICommand EditarCommand { get; set; }
        public ICommand VerLibroCommand { get; set; }
        public ICommand EliminarCommand { get; set; }
        public ICommand BuscarCommand { get; set; }
        public ICommand CancelarCommand { get; set; }
        private ObservableCollection<Recomendaciones> lista;
        public ObservableCollection<Recomendaciones> ListaRecomendaciones
        {
            get { return lista; }
            set { lista = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ListaRecomendaciones")); }
        }
        private string errores;
        public string Errores
        {
            get { return errores; }
            set
            {
                errores = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Errores"));
            }
        }
        private string palabra;
        public string Palabra
        {
            get { return palabra; }
            set
            {
                palabra = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Palabra"));
            }
        }

        private Recomendaciones recomendacion;
        public Recomendaciones Recomendacion
        {
            get { return recomendacion; }
            set
            {
                recomendacion = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Recomendacion"));
            }
        }

        private async void VerAgregar(object obj)
        {
            Errores = "";
            if (agregar == null)
                agregar = new AgregarView() { BindingContext = this };
            Recomendacion = new Recomendaciones();
            await Application.Current.MainPage.Navigation.PushAsync(agregar);
        }
        private async void Agregar(object obj)
        {
            if (ValidarLocal(Recomendacion))
            {
                var result = await App.SincronizarService.Agregar(Recomendacion);

                if (result == null)
                    Cancelar();
                else
                    Errores = result;
            }           
        }


        private async void VerEditar(object obj)
        {
            Errores = "";
            if (editar == null)
                editar = new EditarView() { BindingContext = this };

            await Application.Current.MainPage.Navigation.PushAsync(editar);
        }

        private async void Editar(object obj)
        {
            if (ValidarLocal(Recomendacion))
            {
                var result = await App.SincronizarService.Editar(Recomendacion);

                if (result == null)
                {
                    Errores = "";
                    Cancelar();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Recomendacion"));
                }
                else
                    Errores = result;
            }
        }
        private async void Eliminar(object obj)
        {
            var result = await Application.Current.MainPage.DisplayAlert($"¿Desea eliminar de la lista a: {Recomendacion.TituloLibro}?", "Esta acción no se podrá deshacer.", "Confirmar", "Cancelar");

            if (result)
            {
                var x = await App.SincronizarService.Eliminar(Recomendacion);
                if (x != null)
                    Errores = x;
                else
                    Cancelar();
            }
        }

        private async void VerLibro(object obj)
        {
            Errores = "";
            var x = obj as Recomendaciones;

            if (libro == null)
                libro = new LibroView() { BindingContext = this };

            Recomendacion = x;

            await Application.Current.MainPage.Navigation.PushAsync(libro);
        }
        public void Buscar()
        {
            // Método para buscar/filtrar con sólo una palabra entre las recomendaciones de libros existentes en el catálogo local
            var palabra = Palabra.Trim();
            ListaRecomendaciones.Clear();
            List<Recomendaciones> lista = new List<Recomendaciones>();
            lista= App.Catalogo.GetAll().ToList();

            var buscados = lista.Where(x => x.TituloLibro.ToUpper().Contains(palabra.ToUpper()) || x.Autor.ToUpper().Contains(palabra.ToUpper()) ||
            x.Genero.ToUpper().Contains(palabra.ToUpper()));

            foreach (var item in buscados)
            {
                ListaRecomendaciones.Add(item);
            }
        }
        void Cancelar()
        {
            Errores = "";
            Actualizar();
            Application.Current.MainPage.Navigation.PopAsync();
        }
        bool ValidarLocal(Recomendaciones r)
        {
            Errores = "";
            if (string.IsNullOrWhiteSpace(r.TituloLibro))
                Errores += "Ingrese el título del libro."+"\n";
            if (string.IsNullOrWhiteSpace(r.Autor))
                Errores += "Ingrese el nombre del autor." + "\n";
            if (string.IsNullOrWhiteSpace(r.Genero))
                Errores += "Seleccione el género." + "\n";
            if (string.IsNullOrWhiteSpace(r.Opinion))
                Errores += "Escriba la opinión que tiene sobre este libro." + "\n";
            if (r.Puntuacion < 1)
                Errores += "Seleccione la puntuación que le da a este libro." + "\n";

            if (Errores == "")
                return true;
            else
                return false;
        }
        private void Actualizar()
        {
            ListaRecomendaciones.Clear();

            List<Recomendaciones> lista = App.Catalogo.GetAll().ToList();

            //Agregarlos a la vista del cliente pero no a la bd
            foreach (var item in App.SincronizarService.Buffer)
            {
                switch (item.Estado)
                {
                    
                    case Estado.Agregado:
                            lista.Add(item.Recomendacion);                                                   
                        break;
                    case Estado.Modificado:
                        var r = lista.FirstOrDefault(x => x.Id == item.Recomendacion.Id);
                        if (r != null)
                        {
                                r.TituloLibro = item.Recomendacion.TituloLibro;
                                r.Autor = item.Recomendacion.Autor;
                                r.Genero = item.Recomendacion.Genero;
                                r.Opinion = item.Recomendacion.Opinion;
                                r.Puntuacion = item.Recomendacion.Puntuacion;
                        }
                        break;
                    case Estado.Eliminado:
                        r = lista.FirstOrDefault(X => X.Id == item.Recomendacion.Id);
                        if (r != null)
                            lista.Remove(r);
                        break;
                }
            }
            foreach (var item in lista)
            {
                ListaRecomendaciones.Add(item);
            }
            ListaRecomendaciones.OrderByDescending(x => x.Puntuacion).ThenBy(x => x.TituloLibro);
        }

        public List<int> Puntuaciones { get; set; } = new List<int>()
    {
        1, 2, 3, 4, 5
    };

        public List<string> Generos { get; set; } = new List<string>()
    {
        "Suspenso", "Drama", "Romance", "Terror", "Fantasía", "Aventuras", "Ciencia Ficción", "Cuentos", "Gótica", "Policíaco",
        "Paranormal", "Distópico", "Biográfico", "Histórico"

    };
    }
}
