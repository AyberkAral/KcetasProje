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
                var all = GetAll();
                return all.FirstOrDefault(x => x.tekil_kod == tekilKod);
            }
            catch
            {
                return null;
            }
        }

        public void Create(TuketimNoktasi tuketimNoktasi)
        {
            var dto = new
            {
                ilceId = tuketimNoktasi.ilce_id,
                mahalle = tuketimNoktasi.mahalle ?? "Bilinmiyor",
                binaNo = tuketimNoktasi.bina_no,
                bagimsizBolumNo = tuketimNoktasi.bagimsiz_bolum_no,
                acikAdres = tuketimNoktasi.acik_adres ?? "Belirtilmemiş",
                koordinatLat = tuketimNoktasi.koordinat_lat,
                koordinatLon = tuketimNoktasi.koordinat_lot,
                baglantiGucuKw = tuketimNoktasi.baglanti_gucu_kw ?? 0m,
                tuketiciGrubu = tuketimNoktasi.tuketici_grubu ?? "Mesken",
                baglantiDurumu = tuketimNoktasi.baglanti_grubu
            };

            var response = _httpClient.PostAsJsonAsync("/api/TuketimNoktasi", dto, _jsonOptions).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                throw new Exception($"API Hatası: {response.StatusCode} - Tüketim noktası oluşturulamadı. Detay: {errorContent}");
            }
        }

        public void Update(TuketimNoktasi tuketimNoktasi)
        {
            var response = _httpClient.PutAsJsonAsync($"/api/TuketimNoktasi/{tuketimNoktasi.tekil_kod}", tuketimNoktasi, _jsonOptions).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API Hatası: {response.StatusCode} - Tüketim noktası güncellenemedi.");
            }
        }

        public void Delete(string tekilKod)
        {
            _httpClient.DeleteAsync($"/api/TuketimNoktasi/{tekilKod}").GetAwaiter().GetResult();
        }
    }
}
