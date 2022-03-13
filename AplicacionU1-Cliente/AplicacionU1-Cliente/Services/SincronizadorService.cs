using AplicacionU1_Cliente.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace AplicacionU1_Cliente.Services
{
    public class SincronizadorService
    {
        CatalogoRecomendaciones Catalogo;
        Thread hilo;
        HttpClient client = new HttpClient() { BaseAddress = new Uri("https://181g0138.82g.itesrc.net/") };
        public event Action Actualizar;
        public List<ElementosBuffer> Buffer { get; set; } = new List<ElementosBuffer>();
        public SincronizadorService(CatalogoRecomendaciones catalogo)
        {
            Catalogo = catalogo;
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            if (!Preferences.ContainsKey("Update"))
                DescargarUnicaVez();
            hilo = new Thread(new ThreadStart(CicloSincronizar));
            hilo.Start();
        }

        private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            await Sincronizar();
        }

        public async void CicloSincronizar()
        {
            while (true)
            {
                var x = Catalogo.GetAll();
                await Sincronizar();

                Thread.Sleep(TimeSpan.FromSeconds(30));
            }
        }
        private async Task Sincronizar()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                // primero hay que vaciar el contenido del buffer
                if (Buffer.Count > 0)
                {
                    foreach (var item in Buffer.ToArray())
                    {
                        Buffer.Remove(item);

                        switch (item.Estado)
                        {
                            case Estado.Agregado:
                                await Agregar(item.Recomendacion);
                                break;
                            case Estado.Modificado:
                                await Editar(item.Recomendacion);
                                break;
                            case Estado.Eliminado:
                                await Eliminar(item.Recomendacion);
                                break;
                        }
                    }
                }

                var fecha = Preferences.Get("Update", DateTime.MinValue);

                var json = JsonConvert.SerializeObject(fecha);
                fecha = DateTime.Now;

                var result = await client.PostAsync("api/recomendaciones/sincronizar",
                    new StringContent(json, Encoding.UTF8, "application/json"));


                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    json = await result.Content.ReadAsStringAsync();
                    List<Recomendaciones> listaRec = JsonConvert.DeserializeObject<List<Recomendaciones>>(json);

                    foreach (var t in listaRec)
                    {
                        Catalogo.InsertOrReplace(t);
                    }
                    Preferences.Set("Update", fecha);

                    if (listaRec.Count > 0)
                        Actualizar?.Invoke();
                }

            }
        }

        public async Task<string> Eliminar(Recomendaciones r)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                return await EnviarDatos(r, HttpMethod.Delete);
            else
            {
                ElementosBuffer b = new ElementosBuffer()
                {
                    Recomendacion = r,
                    Estado = Estado.Eliminado
                };
                Buffer.Add(b);
                Actualizar?.Invoke();
            }
            return null;
        }

        public async Task<string> Editar(Recomendaciones r)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                return await EnviarDatos(r, HttpMethod.Put);
            else
            {
                ElementosBuffer b = new ElementosBuffer()
                {
                    Recomendacion = r,
                    Estado = Estado.Modificado
                };
                Buffer.Add(b);
                Actualizar?.Invoke();
            }
            return null;
        }

        public async Task<string> Agregar(Recomendaciones r)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                return await EnviarDatos(r, HttpMethod.Post);
            else
            {
                ElementosBuffer b = new ElementosBuffer()
                {
                    Recomendacion = r,
                    Estado = Estado.Agregado
                };
                Buffer.Add(b);
                Actualizar?.Invoke();
            }
            return null;
        }

        public async void DescargarUnicaVez()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var fechaupdate = DateTime.Now;

                var result = await client.GetAsync("api/recomendaciones");

                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var json = await result.Content.ReadAsStringAsync();
                    List<Recomendaciones> listaRec = JsonConvert.DeserializeObject<List<Recomendaciones>>(json);

                    foreach (var t in listaRec)
                    {
                        Catalogo.InsertOrReplace(t);
                    }
                    Preferences.Set("Update", fechaupdate);

                    if (listaRec.Count > 0)
                        Actualizar?.Invoke();
                }
            }
        }


        private async Task<string> EnviarDatos(Recomendaciones rec, HttpMethod method)
        {
            var json = JsonConvert.SerializeObject(rec);
            var request = new HttpRequestMessage(method, client.BaseAddress + "api/recomendaciones");

            if (method == HttpMethod.Post || method == HttpMethod.Put)
            {
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }
            else
            {
                request = new HttpRequestMessage(method, client.BaseAddress + "api/recomendaciones/" + rec.Id);
            }

            var result = await client.SendAsync(request);

            if (result.IsSuccessStatusCode)
            {
                await Sincronizar();
                return null;
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var jsonErrores = await result.Content.ReadAsStringAsync();

                return jsonErrores;                
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return "La solicitud no pudo ser procesada, la recomendación no se encontró en el servidor."; 
            }
            else
            {
                return "La solicitud no pudo ser procesada, error #"+ result.StatusCode.ToString() ;
            }
        }
    }
}