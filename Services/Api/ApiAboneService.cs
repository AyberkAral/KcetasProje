using System.Net.Http.Json;
using System.Text.Json;
using KcetasWeb.Helpers;
using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Services.Api
{
    public class ApiAboneService : IAboneService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiAboneService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeToCamelCaseNamingPolicy(),
                PropertyNameCaseInsensitive = true
            };
            _jsonOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        }

        public List<Abone> GetAll()
        {
            try
            {
                var jsonStr = _httpClient.GetStringAsync("/api/Aboneler/all").GetAwaiter().GetResult();
                try { System.IO.File.WriteAllText("abone_raw.json", jsonStr); } catch { }
                var result = JsonSerializer.Deserialize<List<Abone>>(jsonStr, _jsonOptions);
                return result ?? new List<Abone>();
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText("abone_err.txt", ex.ToString());
                return new List<Abone>();
            }
        }

        public Abone? GetById(int id)
        {
            try
            {
                return _httpClient.GetFromJsonAsync<Abone>($"/api/Aboneler/{id}", _jsonOptions).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch
            {
                return null;
            }
        }

        public void Create(Abone abone)
        {
            var jsonStr = JsonSerializer.Serialize(abone, _jsonOptions);
            try { System.IO.File.WriteAllText("abone_post_raw.json", jsonStr); } catch { }

            var response = _httpClient.PostAsJsonAsync("/api/Aboneler", abone, _jsonOptions).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API Hatası: {response.StatusCode} - Abone oluşturulamadı.");
            }
        }

        public void Update(Abone abone)
        {
            var response = _httpClient.PutAsJsonAsync($"/api/Aboneler/{abone.abone_id}", abone, _jsonOptions).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API Hatası: {response.StatusCode} - Abone güncellenemedi.");
            }
        }

        public void Delete(int id)
        {
            var response = _httpClient.DeleteAsync($"/api/Aboneler/{id}").GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API Hatası: {response.StatusCode} - Abone silinemedi.");
            }
        }
    }
}

