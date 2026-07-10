using System.Net.Http.Json;
using System.Text.Json;
using KcetasWeb.Helpers;
using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Services.Api
{
    public class ApiSayacService : ISayacService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiSayacService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeToCamelCaseNamingPolicy(),
                PropertyNameCaseInsensitive = true
            };
        }

        public List<Sayac> GetAll()
        {
            try
            {
                var result = _httpClient.GetFromJsonAsync<List<Sayac>>("/api/Sayac", _jsonOptions).GetAwaiter().GetResult();
                return result ?? new List<Sayac>();
            }
            catch
            {
                return new List<Sayac>();
            }
        }

        public Sayac? GetById(long id)
        {
            try
            {
                return _httpClient.GetFromJsonAsync<Sayac>($"/api/Sayac/{id}", _jsonOptions).GetAwaiter().GetResult();
            }
            catch
            {
                return null;
            }
        }

        public void Create(Sayac sayac)
        {
            _httpClient.PostAsJsonAsync("/api/Sayac", sayac).GetAwaiter().GetResult();
        }

        public void Update(Sayac sayac)
        {
            _httpClient.PutAsJsonAsync($"/api/Sayac/{sayac.sayac_id}", sayac).GetAwaiter().GetResult();
        }

        public void Delete(long id)
        {
            _httpClient.DeleteAsync($"/api/Sayac/{id}").GetAwaiter().GetResult();
        }
    }
}
