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
                var result = _httpClient.GetFromJsonAsync<List<Sozlesme>>("/api/Sozlesmeler", _jsonOptions).GetAwaiter().GetResult();
                return result ?? new List<Sozlesme>();
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText("sozlesme_err.txt", ex.ToString());
                return new List<Sozlesme>();
            }
        }

        public Sozlesme? GetById(string sozlesmeNo)
        {
            try
            {
                var all = GetAll();
                return all.FirstOrDefault(x => x.sozlesme_no == sozlesmeNo);
            }
            catch
            {
                return null;
            }
        }

        public void Create(Sozlesme sozlesme)
        {
            var response = _httpClient.PostAsJsonAsync("/api/Sozlesmeler", sozlesme, _jsonOptions).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API Hatası: {response.StatusCode} - Sözleşme oluşturulamadı.");
            }
        }

        public void Update(Sozlesme sozlesme)
        {
            var response = _httpClient.PutAsJsonAsync($"/api/Sozlesmeler/{sozlesme.sozlesme_no}", sozlesme, _jsonOptions).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API Hatası: {response.StatusCode} - Sözleşme güncellenemedi.");
            }
        }

        public void Delete(string sozlesmeNo)
        {
            _httpClient.DeleteAsync($"/api/Sozlesmeler/{sozlesmeNo}").GetAwaiter().GetResult();
        }
    }
}
