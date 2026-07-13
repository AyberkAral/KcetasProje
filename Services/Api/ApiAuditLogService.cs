using System.Net.Http.Json;
using System.Text.Json;
using KcetasWeb.Helpers;
using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Services.Api
{
    public class ApiAuditLogService : IAuditLogService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiAuditLogService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeToCamelCaseNamingPolicy(),
                PropertyNameCaseInsensitive = true
            };
        }

        public void Ekle(string varlikTipi, long varlikId, string islemTipi, string eskiDeger, string yeniDeger, int kullaniciId, long? islemGerekcesi = null)
        {
            var log = new AuditLog
            {
                varlik_tipi = varlikTipi,
                varlik_id = varlikId,
                islem_tipi = islemTipi,
                eski_deger = eskiDeger,
                yeni_deger = yeniDeger,
                kullanici_id = kullaniciId,
                islem_gerekcesi = islemGerekcesi,
                islem_zamani = DateTime.Now
            };

            var response = _httpClient.PostAsJsonAsync("/api/AuditLogs", log, _jsonOptions).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                // Hata durumunda loglanabilir, şimdilik sessizce yutalım veya fırlatalım
                // throw new Exception($"API Hatası: {response.StatusCode} - AuditLog oluşturulamadı.");
            }
        }

        public List<AuditLog> GetirByVarlik(string varlikTipi, long varlikId)
        {
            try
            {
                var result = _httpClient.GetFromJsonAsync<List<AuditLog>>($"/api/AuditLogs?varlikTipi={varlikTipi}&varlikId={varlikId}", _jsonOptions).GetAwaiter().GetResult();
                return result ?? new List<AuditLog>();
            }
            catch
            {
                return new List<AuditLog>();
            }
        }
    }
}
