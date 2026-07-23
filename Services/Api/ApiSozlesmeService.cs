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
            _jsonOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        }

        public List<Sozlesme> GetAll()
        {
            try
            {
                var result = _httpClient.GetFromJsonAsync<List<Sozlesme>>("/api/Sozlesmeler?page=1&pageSize=10000", _jsonOptions).GetAwaiter().GetResult();
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
            var dto = new
            {
                sozlesmeId = sozlesme.sozlesme_id,
                sozlesmeNo = sozlesme.sozlesme_no,
                tuketimNoktasiId = sozlesme.tuketim_noktasi_id,
                aboneId = sozlesme.abone_id,
                tarifeId = sozlesme.tarife_id,
                sozlesmeTipi = sozlesme.sozlesme_tipi,
                baslangicTarihi = sozlesme.baslangic_tarihi?.ToString("yyyy-MM-dd"),
                bitisTarihi = sozlesme.bitis_tarihi?.ToString("yyyy-MM-dd"),
                guvenceBedeli = sozlesme.guvence_bedeli,
                durum = sozlesme.durum,
                createdAt = sozlesme.created_at,
                updatedAt = sozlesme.updated_at
            };

            var response = _httpClient.PostAsJsonAsync("/api/Sozlesmeler", dto, _jsonOptions).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                throw new Exception($"API Hatası: {response.StatusCode} - Sözleşme oluşturulamadı. Detay: {errorContent}");
            }
        }

        public void Update(Sozlesme sozlesme)
        {
            var dto = new
            {
                sozlesmeId = sozlesme.sozlesme_id,
                sozlesmeNo = sozlesme.sozlesme_no,
                tuketimNoktasiId = sozlesme.tuketim_noktasi_id,
                aboneId = sozlesme.abone_id,
                tarifeId = sozlesme.tarife_id,
                sozlesmeTipi = sozlesme.sozlesme_tipi,
                baslangicTarihi = sozlesme.baslangic_tarihi?.ToString("yyyy-MM-dd"),
                bitisTarihi = sozlesme.bitis_tarihi?.ToString("yyyy-MM-dd"),
                guvenceBedeli = sozlesme.guvence_bedeli,
                durum = sozlesme.durum,
                createdAt = sozlesme.created_at,
                updatedAt = sozlesme.updated_at
            };

            var response = _httpClient.PutAsJsonAsync($"/api/Sozlesmeler/{sozlesme.sozlesme_id}", dto, _jsonOptions).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                throw new Exception($"API Hatası: {response.StatusCode} - Sözleşme güncellenemedi. Detay: {errorContent}");
            }
        }

        public void Delete(string sozlesmeNo)
        {
            _httpClient.DeleteAsync($"/api/Sozlesmeler/{sozlesmeNo}").GetAwaiter().GetResult();
        }
    }
}

