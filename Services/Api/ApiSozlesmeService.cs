using System.Net.Http.Json;
using System.Text.Json;
using KcetasWeb.Helpers;
using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Services.Api
{
    public class ApiSozlesmeService : ISozlesmeService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiSozlesmeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeToCamelCaseNamingPolicy(),
                PropertyNameCaseInsensitive = true
            };
        }

        public List<Sozlesme> GetAll()
        {
            try
            {
                var result = _httpClient.GetFromJsonAsync<List<Sozlesme>>("/api/Sozlesme", _jsonOptions).GetAwaiter().GetResult();
                return result ?? new List<Sozlesme>();
            }
            catch
            {
                return new List<Sozlesme>();
            }
        }

        public Sozlesme? GetById(string sozlesmeNo)
        {
            try
            {
                return _httpClient.GetFromJsonAsync<Sozlesme>($"/api/Sozlesme/{sozlesmeNo}", _jsonOptions).GetAwaiter().GetResult();
            }
            catch
            {
                return null;
            }
        }

        public void Create(Sozlesme sozlesme)
        {
            _httpClient.PostAsJsonAsync("/api/Sozlesme", sozlesme).GetAwaiter().GetResult();
        }

        public void Update(Sozlesme sozlesme)
        {
            _httpClient.PutAsJsonAsync($"/api/Sozlesme/{sozlesme.sozlesme_no}", sozlesme).GetAwaiter().GetResult();
        }

        public void Delete(string sozlesmeNo)
        {
            _httpClient.DeleteAsync($"/api/Sozlesme/{sozlesmeNo}").GetAwaiter().GetResult();
        }
    }
}
