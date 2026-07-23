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
            _jsonOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        }

        public List<Sayac> GetAll()
        {
            try
            {
                var result = _httpClient.GetFromJsonAsync<List<Sayac>>("/api/Sayaclar", _jsonOptions).GetAwaiter().GetResult();
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
                return _httpClient.GetFromJsonAsync<Sayac>($"/api/Sayaclar/{id}", _jsonOptions).GetAwaiter().GetResult();
            }
            catch
            {
                return null;
            }
        }

        public void Create(Sayac sayac)
        {
            var response = _httpClient.PostAsJsonAsync("/api/Sayaclar", sayac, _jsonOptions).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API Hatası: {response.StatusCode} - Sayaç oluşturulamadı.");
            }
        }

        public void Update(Sayac sayac)
        {
            var response = _httpClient.PutAsJsonAsync($"/api/Sayaclar/{sayac.sayac_id}", sayac, _jsonOptions).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API Hatası: {response.StatusCode} - Sayaç güncellenemedi.");
            }
        }

        public void Delete(long id)
        {
            _httpClient.DeleteAsync($"/api/Sayaclar/{id}").GetAwaiter().GetResult();
        }
    }
}

