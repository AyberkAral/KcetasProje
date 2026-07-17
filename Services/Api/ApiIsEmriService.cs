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

        // API'deki PUT problemi düzelene kadar ekranda değişiklikleri görebilmek için geçici bellek:
        private static readonly Dictionary<long, string> _geciciDurumlar = new();
        private static readonly Dictionary<long, long> _geciciPersoneller = new();
        private static readonly Dictionary<long, string> _geciciTutanaklar = new();

        public ApiIsEmriService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeToCamelCaseNamingPolicy(),
                PropertyNameCaseInsensitive = true
            };
        }

        public List<IsEmri> GetAll()
        {
            try
            {
                List<IsEmri> list = new List<IsEmri>();
                int currentPage = 1;
                int totalPages = 1;

                do
                {
                    var json = _httpClient.GetFromJsonAsync<JsonElement>($"/api/IsEmirleri?includeCompleted=true&page={currentPage}&pageSize=100", _jsonOptions).GetAwaiter().GetResult();
                    
                    if (json.TryGetProperty("totalPages", out var tp) && tp.ValueKind == JsonValueKind.Number)
                    {
                        totalPages = tp.GetInt32();
                    }

                    if (json.ValueKind == JsonValueKind.Array)
                    {
                        var items = json.Deserialize<List<IsEmri>>(_jsonOptions);
                        if (items != null) list.AddRange(items);
                        break; // Dizi döndüyse sayfalama yoktur
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
                
                // Geçici bellekten üzerine yaz
                foreach (var item in list)
                {
                    if (item == null) continue;
                    
                    if (_geciciDurumlar.ContainsKey(item.is_emri_id)) item.durum = _geciciDurumlar[item.is_emri_id];
                    if (_geciciPersoneller.ContainsKey(item.is_emri_id)) item.atanan_kullanici_id = _geciciPersoneller[item.is_emri_id];
                    if (_geciciTutanaklar.ContainsKey(item.is_emri_id)) item.tutanak_no = _geciciTutanaklar[item.is_emri_id];
                }
                
                return list;
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText("is_emri_debug.txt", $"ERROR: {ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}\n");
                return new List<IsEmri>();
            }
        }

        public IsEmri? GetById(long id)
        {
            try
            {
                var isEmri = _httpClient.GetFromJsonAsync<IsEmri>($"/api/IsEmirleri/{id}", _jsonOptions).GetAwaiter().GetResult();
                if (isEmri != null)
                {
                    if (_geciciDurumlar.ContainsKey(id)) isEmri.durum = _geciciDurumlar[id];
                    if (_geciciPersoneller.ContainsKey(id)) isEmri.atanan_kullanici_id = _geciciPersoneller[id];
                    if (_geciciTutanaklar.ContainsKey(id)) isEmri.tutanak_no = _geciciTutanaklar[id];
                }
                return isEmri;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
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

        public void TutanakKaydet(long isEmriId, string tutanakNo, string sahaSonucu, string? gerekce, string? muhurNo, decimal? kesmeEndeksi, decimal? acmaEndeksi, string? eskiSayacNo, string? yeniSayacNo, decimal? eskiSonEndeks, decimal? yeniIlkEndeks)
        {
            var isEmri = GetById(isEmriId);
            if (isEmri == null) return;

            isEmri.tutanak_no = tutanakNo;
            isEmri.saha_sonucu = sahaSonucu;
            isEmri.gerekce = gerekce;
            isEmri.muhur_no = muhurNo;
            isEmri.durum = "TAMAMLANDI";
            isEmri.updated_at = DateTime.Now;

            _geciciDurumlar[isEmriId] = "TAMAMLANDI";
            _geciciTutanaklar[isEmriId] = tutanakNo;

            try { _httpClient.PutAsJsonAsync($"/api/IsEmirleri/{isEmriId}", isEmri, _jsonOptions).GetAwaiter().GetResult(); } catch { }
        }

        public IsEmri Ekle(IsEmri isEmri)
        {
            var response = _httpClient.PostAsJsonAsync("/api/IsEmirleri", isEmri, _jsonOptions).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                try {
                    return response.Content.ReadFromJsonAsync<IsEmri>(_jsonOptions).GetAwaiter().GetResult();
                } catch { }
            }
            else
            {
                var err = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                System.IO.File.WriteAllText("is_emri_ekle_err.txt", $"POST Hata: {response.StatusCode} - {err}");
            }
            return isEmri;
        }

        public void DurumGuncelle(long id, string yeniDurum)
        {
            var isEmri = GetById(id);
            if (isEmri == null) return;

            isEmri.durum = yeniDurum;
            isEmri.updated_at = DateTime.Now;

            _geciciDurumlar[id] = yeniDurum;

            try { _httpClient.PutAsJsonAsync($"/api/IsEmirleri/{id}", isEmri, _jsonOptions).GetAwaiter().GetResult(); } catch { }
        }

        public void PersonelAta(long id, long personelId)
        {
            var isEmri = GetById(id);
            if (isEmri == null) return;

            isEmri.atanan_kullanici_id = personelId;
            isEmri.durum = "ATANDI";
            isEmri.updated_at = DateTime.Now;

            _geciciPersoneller[id] = personelId;
            _geciciDurumlar[id] = "ATANDI";

            try { _httpClient.PutAsJsonAsync($"/api/IsEmirleri/{id}", isEmri, _jsonOptions).GetAwaiter().GetResult(); } catch { }
        }
    }
}
