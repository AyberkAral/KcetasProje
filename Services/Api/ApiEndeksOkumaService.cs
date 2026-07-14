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

        public List<EndeksOkuma> GetAll()
        {
            try
            {
                var result = _httpClient.GetFromJsonAsync<List<EndeksOkuma>>("/api/EndeksOkuma", _jsonOptions).GetAwaiter().GetResult();
                return result ?? new List<EndeksOkuma>();
            }
            catch
            {
                return new List<EndeksOkuma>();
            }
        }

        public EndeksOkuma? GetById(long id)
        {
            try
            {
                return _httpClient.GetFromJsonAsync<EndeksOkuma>($"/api/EndeksOkuma/{id}", _jsonOptions).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public List<EndeksOkuma> Filtrele(string? okumaTipi, string? durum, DateTime? baslangic, DateTime? bitis, string? arama)
        {
            var query = GetAll().AsQueryable();

            if (!string.IsNullOrEmpty(okumaTipi))
                query = query.Where(x => x.okuma_tipi == okumaTipi);

            if (!string.IsNullOrEmpty(durum))
                query = query.Where(x => x.status == durum);

            if (baslangic.HasValue)
                query = query.Where(x => x.okuma_zamani >= baslangic.Value);

            if (bitis.HasValue)
                query = query.Where(x => x.okuma_zamani <= bitis.Value);

            // Arama sayac_id veya benzeri bir field üzerinden yapılabilir (modelde sayac_id var string değil)
            
            return query.ToList();
        }

        public (int Toplam, int Manuel, int OSOS, int Anomali, decimal OrtalamaTuketim) GetIstatistikler()
        {
            var data = GetAll();
            
            int toplam = data.Count;
            int manuel = data.Count(x => x.okuma_tipi == "Manuel");
            int osos = data.Count(x => x.okuma_tipi == "OSOS");
            int anomali = data.Count(x => x.anomali_mi == true);
            
            decimal ortalama = toplam > 0 ? data.Average(x => (x.yeni_endeks ?? 0) - (x.onceki_endeks ?? 0)) : 0;
            
            return (toplam, manuel, osos, anomali, ortalama);
        }

        public void Create(EndeksOkuma model)
        {
            var jsonString = System.Text.Json.JsonSerializer.Serialize(model, _jsonOptions);
            System.IO.File.WriteAllText("debug_json.txt", jsonString);

            var response = _httpClient.PostAsJsonAsync("/api/EndeksOkuma", model, _jsonOptions).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                throw new Exception($"API Hatası: {response.StatusCode} - Endeks okuması oluşturulamadı. Detay: {errorContent}");
            }
        }
    }
}
