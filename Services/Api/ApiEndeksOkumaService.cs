using System.Net.Http.Json;
using System.Text.Json;
using KcetasWeb.Helpers;
using KcetasWeb.Models;
using KcetasWeb.Models.entities;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Services.Api
{
    public class ApiEndeksOkumaService : IEndeksOkumaService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiEndeksOkumaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeToCamelCaseNamingPolicy(),
                PropertyNameCaseInsensitive = true
            };

        }

        public async System.Threading.Tasks.Task<List<EndeksOkuma>> GetAllAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<EndeksOkuma>>("/api/EndeksOkuma", _jsonOptions);
                return result ?? new List<EndeksOkuma>();
            }
            catch
            {
                return new List<EndeksOkuma>();
            }
        }

        public async System.Threading.Tasks.Task<EndeksOkuma?> GetByIdAsync(long id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<EndeksOkuma>($"/api/EndeksOkuma/{id}", _jsonOptions);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async System.Threading.Tasks.Task<List<EndeksOkuma>> FiltreleAsync(string? okumaTipi, string? durum, DateTime? baslangic, DateTime? bitis, string? arama)
        {
            var data = await GetAllAsync();
            var query = data.AsQueryable();

            if (!string.IsNullOrEmpty(okumaTipi))
                query = query.Where(x => ((int?)x.okuma_tipi).ToString() == okumaTipi || x.okuma_tipi.ToString() == okumaTipi);

            if (!string.IsNullOrEmpty(durum))
                query = query.Where(x => x.status == durum);

            if (baslangic.HasValue)
                query = query.Where(x => x.okuma_zamani >= baslangic.Value);

            if (bitis.HasValue)
                query = query.Where(x => x.okuma_zamani <= bitis.Value);

            // Arama sayac_id veya benzeri bir field üzerinden yapılabilir (modelde sayac_id var string değil)
            
            return query.ToList();
        }

        public async System.Threading.Tasks.Task<(int Toplam, int Manuel, int OSOS, int Anomali, decimal OrtalamaTuketim)> GetIstatistiklerAsync()
        {
            var data = await GetAllAsync();
            
            int toplam = data.Count;
            int manuel = data.Count(x => x.okuma_kaynagi == KcetasWeb.Models.Enums.OkumaKaynagi.Manuel);
            int osos = data.Count(x => x.okuma_kaynagi == KcetasWeb.Models.Enums.OkumaKaynagi.Osos);
            int anomali = data.Count(x => x.anomali_mi == true);
            
            decimal ortalama = toplam > 0 ? data.Average(x => (x.yeni_endeks ?? 0) - (x.onceki_endeks ?? 0)) : 0;
            
            return (toplam, manuel, osos, anomali, ortalama);
        }

        public async System.Threading.Tasks.Task CreateAsync(EndeksOkuma model)
        {
            var jsonString = System.Text.Json.JsonSerializer.Serialize(model, _jsonOptions);
            System.IO.File.WriteAllText("debug_json.txt", jsonString);

            var response = await _httpClient.PostAsJsonAsync("/api/EndeksOkuma", model, _jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Hatası: {response.StatusCode} - Endeks okuması oluşturulamadı. Detay: {errorContent}");
            }
        }
        public async System.Threading.Tasks.Task UpdateAsync(EndeksOkuma model)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/EndeksOkuma/{model.okuma_id}", model, _jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Hatası: {response.StatusCode} - Endeks okuması güncellenemedi. Detay: {errorContent}");
            }
        }
    }
}

