using System.Net.Http.Json;
using System.Text.Json;
using KcetasWeb.Helpers;
using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Services.Api
{
    public class ApiAboneService : IAboneService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiAboneService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeToCamelCaseNamingPolicy(),
                PropertyNameCaseInsensitive = true
            };

        }



        public async System.Threading.Tasks.Task<PaginatedResponse<Abone>> GetPagedAsync(int page, int pageSize)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<PaginatedResponse<Abone>>($"/api/Aboneler?page={page}&pageSize={pageSize}", _jsonOptions);
                return response ?? new PaginatedResponse<Abone> { CurrentPage = page, PageSize = pageSize };
            }
            catch
            {
                return new PaginatedResponse<Abone> { CurrentPage = page, PageSize = pageSize };
            }
        }

        public async System.Threading.Tasks.Task<KcetasWeb.Models.Dtos.PagedResultDto<KcetasWeb.Models.Dtos.AboneListDto>> GetPagedCursorAsync(long? lastId, int limit)
        {
            try
            {
                var qs = lastId.HasValue ? $"?lastId={lastId}&limit={limit}" : $"?limit={limit}";
                var response = await _httpClient.GetFromJsonAsync<KcetasWeb.Models.Dtos.PagedResultDto<KcetasWeb.Models.Dtos.AboneListDto>>($"/api/Aboneler/Cursor{qs}", _jsonOptions);
                return response ?? new KcetasWeb.Models.Dtos.PagedResultDto<KcetasWeb.Models.Dtos.AboneListDto> { PageSize = limit };
            }
            catch
            {
                return new KcetasWeb.Models.Dtos.PagedResultDto<KcetasWeb.Models.Dtos.AboneListDto> { PageSize = limit };
            }
        }

        public async System.Threading.Tasks.Task<List<Abone>> GetAllAsync()
        {
            try
            {
                var jsonElement = await _httpClient.GetFromJsonAsync<JsonElement>("/api/Aboneler?page=1&pageSize=2000", _jsonOptions);
                if (jsonElement.ValueKind == JsonValueKind.Array)
                {
                    var result = jsonElement.Deserialize<List<Abone>>(_jsonOptions);
                    return result ?? new List<Abone>();
                }
                else if (jsonElement.TryGetProperty("data", out var dataProp))
                {
                    var result = dataProp.Deserialize<List<Abone>>(_jsonOptions);
                    return result ?? new List<Abone>();
                }
                return new List<Abone>();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new List<Abone>();
            }
            catch
            {
                return new List<Abone>();
            }
        }

        public async System.Threading.Tasks.Task<Abone?> GetByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Abone>($"/api/Aboneler/{id}", _jsonOptions);
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

        public async System.Threading.Tasks.Task CreateAsync(Abone abone)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Aboneler", abone, _jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Hatası: {response.StatusCode} - Abone oluşturulamadı. Detay: {err}");
            }
        }

        public async System.Threading.Tasks.Task UpdateAsync(Abone abone)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Aboneler/{abone.abone_id}", abone, _jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Hatası: {response.StatusCode} - Abone güncellenemedi. Detay: {err}");
            }
        }

        public async System.Threading.Tasks.Task DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Aboneler/{id}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API Hatası: {response.StatusCode} - Abone silinemedi.");
            }
        }
    }
}
