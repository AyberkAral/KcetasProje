using System.Net.Http.Json;
using System.Text.Json;
using KcetasWeb.Helpers;
using KcetasWeb.Models;
using KcetasWeb.ViewModels;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Services.Api
{
    public class ApiOutboxService : IOutboxService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiOutboxService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeToCamelCaseNamingPolicy(),
                PropertyNameCaseInsensitive = true
            };
        }

        public List<EntegrasyonOutbox> GetAll()
        {
            try
            {
                var result = _httpClient.GetFromJsonAsync<List<EntegrasyonOutbox>>("/api/EntegrasyonOutbox", _jsonOptions).GetAwaiter().GetResult();
                return (result ?? new List<EntegrasyonOutbox>())
                    .OrderByDescending(x => x.created_at)
                    .ToList();
            }
            catch
            {
                return new List<EntegrasyonOutbox>();
            }
        }

        public EntegrasyonOutbox? GetById(long id)
        {
            try
            {
                return _httpClient.GetFromJsonAsync<EntegrasyonOutbox>($"/api/EntegrasyonOutbox/{id}", _jsonOptions).GetAwaiter().GetResult();
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

        public List<EntegrasyonOutbox> Filtrele(string? durum, string? hedefSistem, DateTime? baslangic, DateTime? bitis)
        {
            var query = GetAll().AsQueryable();
            var normalizedDurum = OutboxListeViewModel.NormalizeDurum(durum);

            if (!string.IsNullOrEmpty(normalizedDurum))
                query = query.Where(x => OutboxListeViewModel.NormalizeDurum(x.durum) == normalizedDurum);

            if (!string.IsNullOrEmpty(hedefSistem))
                query = query.Where(x => x.hedef_sistem != null && x.hedef_sistem.Contains(hedefSistem, StringComparison.OrdinalIgnoreCase));

            if (baslangic.HasValue)
                query = query.Where(x => x.created_at >= baslangic.Value);

            if (bitis.HasValue)
            {
                var bitisGunSonu = bitis.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(x => x.created_at <= bitisGunSonu);
            }

            return query.ToList();
        }

        public (int Toplam, int Bekleyen, int Gonderilmis, int Basarisiz) GetIstatistikler()
        {
            var all = GetAll();
            var toplam = all.Count;
            var bekleyen = all.Count(x => OutboxListeViewModel.NormalizeDurum(x.durum) == "BEKLIYOR");
            var gonderilmis = all.Count(x => OutboxListeViewModel.NormalizeDurum(x.durum) == "GONDERILDI");
            var basarisiz = all.Count(x =>
            {
                var durum = OutboxListeViewModel.NormalizeDurum(x.durum);
                return durum == "BASARISIZ" || durum == "HATALI";
            });

            return (toplam, bekleyen, gonderilmis, basarisiz);
        }

        public bool YenidenGonder(long id)
        {
            var kayit = GetById(id);
            if (kayit == null) return false;

            var updateDto = new
            {
                outboxId = kayit.outbox_id,
                durum = "BEKLIYOR",
                retryCount = kayit.retry_count,
                hataMesaji = (string?)null
            };

            var response = _httpClient.PutAsJsonAsync($"/api/EntegrasyonOutbox/{id}", updateDto, _jsonOptions).GetAwaiter().GetResult();
            return response.IsSuccessStatusCode;
        }
    }
}
