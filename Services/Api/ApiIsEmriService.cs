using System.Net.Http.Json;
using System.Text.Json;
using KcetasWeb.Helpers;
using KcetasWeb.Models;
using KcetasWeb.Models.entities;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Services.Api
{
    public class ApiIsEmriService : IIsEmriService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<ApiIsEmriService> _logger;

        public ApiIsEmriService(HttpClient httpClient, ILogger<ApiIsEmriService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeToCamelCaseNamingPolicy(),
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<IsEmri>> GetAllAsync()
        {
            try
            {
                List<IsEmri> list = new List<IsEmri>();
                int currentPage = 1;
                int totalPages = 1;

                do
                {
                    var response = await _httpClient.GetAsync($"/api/IsEmirleri?includeCompleted=true&page={currentPage}&pageSize=100");
                    response.EnsureSuccessStatusCode();
                    
                    var json = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);
                    
                    if (json.TryGetProperty("totalPages", out var tp) && tp.ValueKind == JsonValueKind.Number)
                    {
                        totalPages = tp.GetInt32();
                    }

                    if (json.ValueKind == JsonValueKind.Array)
                    {
                        var items = json.Deserialize<List<IsEmri>>(_jsonOptions);
                        if (items != null) list.AddRange(items);
                        break;
                    }
                    else if (json.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array)
                    {
                        var items = data.Deserialize<List<IsEmri>>(_jsonOptions);
                        if (items != null) list.AddRange(items);
                    }
                    else if (json.TryGetProperty("items", out var itemsNode) && itemsNode.ValueKind == JsonValueKind.Array)
                    {
                        var items = itemsNode.Deserialize<List<IsEmri>>(_jsonOptions);
                        if (items != null) list.AddRange(items);
                    }
                    
                    currentPage++;
                } while (currentPage <= totalPages);
                
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İş Emirleri API'den çekilirken hata oluştu.");
                throw; // Hatayı yutmak yerine fırlatıyoruz, böylece arayüz haberdar olur.
            }
        }

        public List<IsEmri> GetAll()
        {
            return GetAllAsync().GetAwaiter().GetResult();
        }

        public async Task<IsEmri?> GetByIdAsync(long id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IsEmri>($"/api/IsEmirleri/{id}", _jsonOptions);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"İş Emri (ID: {id}) API'den çekilirken hata oluştu.");
                throw;
            }
        }

        public IsEmri? GetById(long id)
        {
            return GetByIdAsync(id).GetAwaiter().GetResult();
        }

        public List<IsEmri> Filtrele(string? tip, string? durum, DateTime? baslangic, DateTime? bitis, string? arama)
        {
            var query = GetAll().AsQueryable();

            if (!string.IsNullOrEmpty(tip))
                query = query.Where(x => x.tip == tip);

            if (!string.IsNullOrEmpty(durum))
                query = query.Where(x => x.durum == durum);

            if (baslangic.HasValue)
                query = query.Where(x => x.planlanan_tarih >= baslangic.Value);

            if (bitis.HasValue)
                query = query.Where(x => x.planlanan_tarih <= bitis.Value);

            if (!string.IsNullOrEmpty(arama))
            {
                arama = arama.ToLower();
                query = query.Where(x => 
                    (x.is_emri_no != null && x.is_emri_no.ToLower().Contains(arama))
                );
            }

            return query.ToList();
        }

        public async Task<List<IsEmri>> FiltreleAsync(string? tip, string? durum, DateTime? baslangic, DateTime? bitis, string? arama)
        {
            var all = await GetAllAsync();
            var query = all.AsQueryable();

            if (!string.IsNullOrEmpty(tip))
                query = query.Where(x => x.tip == tip);

            if (!string.IsNullOrEmpty(durum))
                query = query.Where(x => x.durum == durum);

            if (baslangic.HasValue)
                query = query.Where(x => x.planlanan_tarih >= baslangic.Value);

            if (bitis.HasValue)
                query = query.Where(x => x.planlanan_tarih <= bitis.Value);

            if (!string.IsNullOrEmpty(arama))
            {
                arama = arama.ToLower();
                query = query.Where(x => 
                    (x.is_emri_no != null && x.is_emri_no.ToLower().Contains(arama))
                );
            }

            return query.ToList();
        }

        public async Task TutanakKaydetAsync(long isEmriId, string tutanakNo, string sahaSonucu, string? gerekce, string? muhurNo, decimal? kesmeEndeksi, decimal? acmaEndeksi, string? eskiSayacNo, string? yeniSayacNo, decimal? eskiSonEndeks, decimal? yeniIlkEndeks)
        {
            var isEmri = await GetByIdAsync(isEmriId);
            if (isEmri == null) return;

            isEmri.tutanak_no = tutanakNo;
            isEmri.saha_sonucu = sahaSonucu;
            isEmri.gerekce = gerekce;
            isEmri.muhur_no = muhurNo;
            isEmri.durum = KcetasWeb.Constants.IsEmriDurumlari.TamamlandiKucuk;
            isEmri.updated_at = DateTime.Now;

            var response = await _httpClient.PutAsJsonAsync($"/api/IsEmirleri/{isEmriId}", isEmri, _jsonOptions);
            response.EnsureSuccessStatusCode();
        }

        public void TutanakKaydet(long isEmriId, string tutanakNo, string sahaSonucu, string? gerekce, string? muhurNo, decimal? kesmeEndeksi, decimal? acmaEndeksi, string? eskiSayacNo, string? yeniSayacNo, decimal? eskiSonEndeks, decimal? yeniIlkEndeks)
        {
            TutanakKaydetAsync(isEmriId, tutanakNo, sahaSonucu, gerekce, muhurNo, kesmeEndeksi, acmaEndeksi, eskiSayacNo, yeniSayacNo, eskiSonEndeks, yeniIlkEndeks).GetAwaiter().GetResult();
        }

        public async Task<IsEmri> EkleAsync(IsEmri isEmri)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/IsEmirleri", isEmri, _jsonOptions);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<IsEmri>(_jsonOptions);
            return result ?? isEmri;
        }

        public IsEmri Ekle(IsEmri isEmri)
        {
            return EkleAsync(isEmri).GetAwaiter().GetResult();
        }

        public async Task DurumGuncelleAsync(long id, string yeniDurum)
        {
            var isEmri = await GetByIdAsync(id);
            if (isEmri == null) return;

            isEmri.durum = yeniDurum;
            isEmri.updated_at = DateTime.Now;

            var response = await _httpClient.PutAsJsonAsync($"/api/IsEmirleri/{id}", isEmri, _jsonOptions);
            response.EnsureSuccessStatusCode();
        }

        public void DurumGuncelle(long id, string yeniDurum)
        {
            DurumGuncelleAsync(id, yeniDurum).GetAwaiter().GetResult();
        }

        public async Task PersonelAtaAsync(long id, long personelId)
        {
            var isEmri = await GetByIdAsync(id);
            if (isEmri == null) return;

            isEmri.atanan_kullanici_id = personelId;
            isEmri.durum = KcetasWeb.Constants.IsEmriDurumlari.Atandi;
            isEmri.updated_at = DateTime.Now;

            var response = await _httpClient.PutAsJsonAsync($"/api/IsEmirleri/{id}", isEmri, _jsonOptions);
            response.EnsureSuccessStatusCode();
        }

        public void PersonelAta(long id, long personelId)
        {
            PersonelAtaAsync(id, personelId).GetAwaiter().GetResult();
        }
    }
}
