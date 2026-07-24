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

        public async Task<List<EntegrasyonOutbox>> GetAllAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<EntegrasyonOutbox>>("/api/EntegrasyonOutbox", _jsonOptions);
                return (result ?? new List<EntegrasyonOutbox>())
                    .OrderByDescending(x => x.created_at)
                    .ToList();
            }
            catch
            {
                return new List<EntegrasyonOutbox>();
            }
        }

        public async Task<EntegrasyonOutbox?> GetByIdAsync(long id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<EntegrasyonOutbox>($"/api/EntegrasyonOutbox/{id}", _jsonOptions);
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

        public async Task<List<EntegrasyonOutbox>> FiltreleAsync(string? durum, string? hedefSistem, DateTime? baslangic, DateTime? bitis)
        {
            var all = await GetAllAsync();
            var query = all.AsQueryable();
            var normalizedDurum = OutboxListeViewModel.NormalizeDurum(durum);

            if (!string.IsNullOrEmpty(normalizedDurum))
                query = query.Where(x => OutboxListeViewModel.NormalizeDurum(x.durum.HasValue ? x.durum.Value.ToString() : null) == normalizedDurum);

            if (!string.IsNullOrEmpty(hedefSistem))
                query = query.Where(x => x.hedef_sistem != null && x.hedef_sistem.ToString().Contains(hedefSistem, StringComparison.OrdinalIgnoreCase));

            if (baslangic.HasValue)
                query = query.Where(x => x.created_at >= baslangic.Value);

            if (bitis.HasValue)
            {
                var bitisGunSonu = bitis.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(x => x.created_at <= bitisGunSonu);
            }

            return query.ToList();
        }

        public async Task<(int Toplam, int Bekleyen, int Gonderilmis, int Basarisiz)> GetIstatistiklerAsync()
        {
            var all = await GetAllAsync();
            var toplam = all.Count;
            var bekleyen = all.Count(x => OutboxListeViewModel.NormalizeDurum(x.durum.HasValue ? x.durum.Value.ToString() : null) == "BEKLIYOR");
            var gonderilmis = all.Count(x => OutboxListeViewModel.NormalizeDurum(x.durum.HasValue ? x.durum.Value.ToString() : null) == "GONDERILDI");
            var basarisiz = all.Count(x =>
            {
                var durum = OutboxListeViewModel.NormalizeDurum(x.durum.HasValue ? x.durum.Value.ToString() : null);
                return durum == "BASARISIZ" || durum == "HATALI";
            });

            return (toplam, bekleyen, gonderilmis, basarisiz);
        }

        public async Task<bool> YenidenGonderAsync(long id)
        {
            var kayit = await GetByIdAsync(id);
            if (kayit == null) return false;

            var updateDto = new
            {
                outboxId = kayit.outbox_id,
                durum = "BEKLIYOR",
                retryCount = kayit.retry_count,
                hataMesaji = (string?)null
            };

            var response = await _httpClient.PutAsJsonAsync($"/api/EntegrasyonOutbox/{id}", updateDto, _jsonOptions);
            return response.IsSuccessStatusCode;
        }
    }
}

