using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AplicacionU2_Cliente.Helpers
{
    public class HttpClientHelper<T> where T : class
    {
        public HttpClientHelper(Uri uri)
        {
            Client = new HttpClient();
            Uri = uri;
        }
        public HttpClient Client { get; private set; }
        public Uri Uri { get; set; }

        public async Task<T> Get(int id)
        {
            var response = await Client.GetAsync(Uri + $"/{id}");

            //captura excepciones y aquí detiene el método
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            T datos = JsonConvert.DeserializeObject<T>(json);

            return datos;
        }
        public async Task<IEnumerable<T>> Get()
        {
            var response = await Client.GetAsync(Uri);

            //captura excepciones y aquí detiene el método
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            List<T> datos = JsonConvert.DeserializeObject<List<T>>(json);

            return datos;
        }
        public async Task<IEnumerable<T>> Get(string categoria)
        {    //siempre que se va regresar algoo se pone el task
            var response = await Client.GetAsync(Uri + "/categoria/" + categoria);

            //captura excepciones y aquí detiene el método
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            List<T> datos = JsonConvert.DeserializeObject<List<T>>(json);

            return datos;
        }
        public async Task<Object> Post(T model)
        {
            var json = JsonConvert.SerializeObject(model);
            var response = await Client.PostAsync(Uri, new StringContent(json, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            if (response.Content.Headers.ContentLength == 0)
                return null;
            else
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject(result);
            }
        }
        public async Task<Object> Post(string uri, Object model)
        {
            var json = JsonConvert.SerializeObject(model);
            var response = await Client.PostAsync(Uri + uri, new StringContent(json, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            if (response.Content.Headers.ContentLength == 0)
                return null;
            else
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject(result);
            }
        }
        public async Task Put(T model)
        {
            var json = JsonConvert.SerializeObject(model);
            var response = await Client.PutAsync(Uri, new StringContent(json, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
        }
        public async Task Delete(int id)
        {
            var response = await Client.DeleteAsync(Uri + $"/{id}");

            response.EnsureSuccessStatusCode();
        }
        public async Task Delete(T model)
        {
            var json = JsonConvert.SerializeObject(model);

            HttpRequestMessage rm = new HttpRequestMessage()
            {
                Method = HttpMethod.Delete,
                RequestUri = Uri,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = await Client.SendAsync(rm);

            response.EnsureSuccessStatusCode();
        }

    }
}
