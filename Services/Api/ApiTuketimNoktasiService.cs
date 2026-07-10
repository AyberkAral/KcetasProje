using System.Net.Http.Json;
using System.Text.Json;
using KcetasWeb.Helpers;
using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Services.Api
{
    public class ApiTuketimNoktasiService : ITuketimNoktasiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiTuketimNoktasiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeToCamelCaseNamingPolicy(),
                PropertyNameCaseInsensitive = true
            };
        }

        public List<TuketimNoktasi> GetAll()
        {
            try
            {
                var result = _httpClient.GetFromJsonAsync<List<TuketimNoktasi>>("/api/TuketimNoktasi", _jsonOptions).GetAwaiter().GetResult();
                return result ?? new List<TuketimNoktasi>();
            }
            catch
            {
                return new List<TuketimNoktasi>();
            }
        }

        public TuketimNoktasi? GetById(string tekilKod)
        {
            try
            {
                return _httpClient.GetFromJsonAsync<TuketimNoktasi>($"/api/TuketimNoktasi/{tekilKod}", _jsonOptions).GetAwaiter().GetResult();
            }
            catch
            {
                return null;
            }
        }

        public void Create(TuketimNoktasi tuketimNoktasi)
        {
            _httpClient.PostAsJsonAsync("/api/TuketimNoktasi", tuketimNoktasi).GetAwaiter().GetResult();
        }

        public void Update(TuketimNoktasi tuketimNoktasi)
        {
            _httpClient.PutAsJsonAsync($"/api/TuketimNoktasi/{tuketimNoktasi.tekil_kod}", tuketimNoktasi).GetAwaiter().GetResult();
        }

        public void Delete(string tekilKod)
        {
            _httpClient.DeleteAsync($"/api/TuketimNoktasi/{tekilKod}").GetAwaiter().GetResult();
        }
    }
}
