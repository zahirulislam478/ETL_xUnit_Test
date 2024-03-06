using System;
using System.Net.Http;
using System.Threading.Tasks;
using ETL_Shared.InputModels;
using Newtonsoft.Json;

namespace ETL_Front.Services
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpClient client;

        public HttpClientWrapper()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5015");
        }

        public async Task<string> GetStringAsync(string uri)
        {
            HttpResponseMessage response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task PostJsonAsync(string uri, PatientInputModel model)
        {
            string modelJson = JsonConvert.SerializeObject(model);
            HttpContent content = new StringContent(modelJson, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(uri, content);
            response.EnsureSuccessStatusCode();
        }
    }
}
