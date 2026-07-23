using System.Net.Http.Json;
using System.Text.Json;
using KcetasWeb.Helpers;
using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Services.Api
{
    public class PaginatedAuditLogResponse
    {
        public int Sayfa { get; set; }
        public int Limit { get; set; }
        public List<AuditLog> Data { get; set; }
    }

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
            _jsonOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        }

        public void Ekle(string varlikTipi, int varlikId, string islemTipi, string eskiDeger, string yeniDeger, int kullaniciId, string islemGerekcesi = null)
        {
            var log = new AuditLog
            {
                varlik_tipi = varlikTipi,
                varlik_id = varlikId,
                islem_tipi = Enum.TryParse<KcetasWeb.Models.Enums.AuditIslemTipi>(islemTipi, true, out var pType) ? pType : null,
                eski_deger = eskiDeger,
                yeni_deger = yeniDeger,
                kullanici_id = kullaniciId,
                islem_gerekcesi = islemGerekcesi,
                islem_zamani = DateTime.Now
            };

            var response = _httpClient.PostAsJsonAsync("/api/AuditLog", log, _jsonOptions).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                // Hata durumunda loglanabilir, şimdilik sessizce yutalım veya fırlatalım
                // throw new Exception($"API Hatası: {response.StatusCode} - AuditLog oluşturulamadı.");
            }
        }

        public List<AuditLog> GetirByVarlik(string varlikTipi, int varlikId)
        {
            try
            {
                var result = _httpClient.GetFromJsonAsync<PaginatedAuditLogResponse>($"/api/AuditLog?varlikTipi={varlikTipi}&varlikId={varlikId}", _jsonOptions).GetAwaiter().GetResult();
                return result?.Data ?? new List<AuditLog>();
            }
            catch
            {
                return new List<AuditLog>();
            }
        }

        public List<AuditLog> GetAll(int page = 1, int pageSize = 100)
        {
            try
            {
                // API sayfalama sarmalayıcısı (wrapper) ile yanıt dönüyor
                var result = _httpClient.GetFromJsonAsync<PaginatedAuditLogResponse>($"/api/AuditLog?page={page}&pageSize={pageSize}", _jsonOptions).GetAwaiter().GetResult();
                
                if (result != null && result.Data != null)
                {
                    return result.Data;
                }
                return new List<AuditLog>();
            }
            catch
            {
                return new List<AuditLog>();
            }
        }
    }
}

