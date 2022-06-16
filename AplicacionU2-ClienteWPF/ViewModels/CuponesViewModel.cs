using AplicacionU2_ClienteWPF.Helpers;
using AplicacionU2_ClienteWPF.Models;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AplicacionU2_ClienteWPF.ViewModels
{
    public enum Vistas { Agregar, Editar, Eliminar, Inicio }

    public class CuponesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public HttpClientHelper<Cupones> Client;

        //Constructor
        public CuponesViewModel()
        {
            Uri uri = new("https://181g0138.82G.itesrc.net/api/cupones");
            Client = new(uri);
            _ = Descargar();

            Vista = Vistas.Inicio;
            VerAgregarCommand = new RelayCommand(VerAgregar);
            AgregarCommand = new RelayCommand(Agregar);
            VerEliminarCommand = new RelayCommand(VerEliminar);
            EliminarCommand = new RelayCommand(Eliminar);
            CancelarCommand = new RelayCommand(Cancelar);
        }
        //Comandos
        public ICommand VerAgregarCommand { get; set; }
        public ICommand AgregarCommand { get; set; }
        public ICommand VerEliminarCommand { get; set; }
        public ICommand EliminarCommand { get; set; }
        public ICommand CancelarCommand { get; set; }

        //Propiedades
        public ObservableCollection<Cupones> CuponesAlimentos { get; set; } = new();
        public ObservableCollection<Cupones> CuponesVestimenta { get; set; } = new();
        public ObservableCollection<Cupones> CuponesEntretenimiento { get; set; } = new();
        
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
        private Vistas vista;

        public Vistas Vista
        {
            get { return vista; }
            set
            {
                vista = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Vista"));
            }
        }

        //Métodos
        public async Task Descargar()
        {
            CuponesAlimentos = new(); CuponesVestimenta = new(); CuponesEntretenimiento = new();

            var alimentos = await Client.Get("alimentos");
            var vestimenta = await Client.Get("vestimenta");
            var entretenimiento = await Client.Get("entretenimiento");

            foreach (var c in alimentos)
            {
                CuponesAlimentos.Add(c);
            }
            foreach (var c in vestimenta)
            {
                CuponesVestimenta.Add(c);
            }
            foreach (var c in entretenimiento)
            {
                CuponesEntretenimiento.Add(c);
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
        private  void VerAgregar()
        {
            Cupon = new();
            Error = "";
            Vista = Vistas.Agregar;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
        private async void Agregar()
        {
            if (Cupon != null)
            {
                try
                {
                    await Client.Post(Cupon);
                    await Descargar();
                    Vista = Vistas.Inicio;
                    Error = "";
                }
                catch (HttpRequestException ex)
                {
                    Error = ex.Message;
                }
            }
        }
        private  void VerEliminar()
        {
            if (Cupon != null)
            {
                Error = "";
                Vista = Vistas.Eliminar;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }
        private async void Eliminar()
        {
            if (Cupon != null)
            {
                try
                {
                    await Client.Delete(Cupon.IdCupon);
                    Error = "";
                    await Descargar();
                    Vista = Vistas.Inicio;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
                catch (HttpRequestException ex)
                {
                    Error = ex.Message;
                }
            }
        }
        private async void Cancelar()
        {
            Error = "";
            await Descargar();
            Vista = Vistas.Inicio;
            Cupon = null;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
