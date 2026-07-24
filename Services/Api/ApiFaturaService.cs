using System.Net.Http.Json;
using System.Text.Json;
using KcetasWeb.Helpers;
using KcetasWeb.Models;
using KcetasWeb.Models.entities;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Services.Api
{
    public class ApiFaturaService : IFaturaService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiFaturaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeToCamelCaseNamingPolicy(),
                PropertyNameCaseInsensitive = true
            };
        }

        public (decimal BirimFiyat, decimal EnerjiBedeli, decimal DagitimBedeli, decimal TrtPayi, decimal EnerjiFonu, decimal KdvTutari, decimal ToplamTutar, List<SimulasyonKalemDto> Kalemler) SimulasyonHesapla(string tarifeGrubu, decimal tuketimMiktari)
        {
            // Eğer Swagger/API tarafında simülasyon endpointi yoksa (şimdilik mock hesabı API içinde çalışıyormuş gibi localde simüle edebiliriz
            // ya da doğrudan API'den "/api/Fatura/Simulasyon" gibi bir endpoint kullanılabilir. 
            // Biz şimdilik mock mantığına yakın local hesap yapıp API servis imzasını sağlıyoruz.
            
            decimal birimFiyat = tarifeGrubu switch
            {
                "Ticarethane" => 3.45m,
                "Sanayi" => 2.65m,
                _ => 2.85m // Mesken
            };

            decimal dagitimBirimFiyat = 0.65m;

            decimal enerjiBedeli = tuketimMiktari * birimFiyat;
            decimal dagitimBedeli = tuketimMiktari * dagitimBirimFiyat;
            decimal trtPayi = enerjiBedeli * 0.02m;
            decimal enerjiFonu = enerjiBedeli * 0.01m;

            decimal matrah = enerjiBedeli + dagitimBedeli + trtPayi + enerjiFonu;
            decimal kdvTutari = matrah * 0.20m;
            decimal toplamTutar = matrah + kdvTutari;

            var kalemler = new List<SimulasyonKalemDto>
            {
                new SimulasyonKalemDto("Aktif Enerji Bedeli", tuketimMiktari, birimFiyat, enerjiBedeli),
                new SimulasyonKalemDto("Dağıtım Bedeli", tuketimMiktari, dagitimBirimFiyat, dagitimBedeli),
                new SimulasyonKalemDto("TRT Payı", 1, 0, trtPayi),
                new SimulasyonKalemDto("Enerji Fonu", 1, 0, enerjiFonu),
                new SimulasyonKalemDto("KDV (%20)", 1, 0, kdvTutari)
            };

            return (birimFiyat, enerjiBedeli, dagitimBedeli, trtPayi, enerjiFonu, kdvTutari, toplamTutar, kalemler);
        }

        public async Task<List<Fatura>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/Fatura?page=1&pageSize=1000");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<List<Fatura>>(_jsonOptions);
                return result ?? new List<Fatura>();
            }
            catch (Exception ex)
            {
                // In a real app we should use ILogger to log ex.Message
                System.Diagnostics.Debug.WriteLine($"GetAllAsync Hata: {ex.Message}");
                throw; // Do not swallow!
            }
        }

        public List<Fatura> GetAll()
        {
            return GetAllAsync().GetAwaiter().GetResult();
        }

        public async Task<Fatura?> GetByIdAsync(int id)
        {
            try
            {
                var all = await GetAllAsync();
                return all.FirstOrDefault(x => x.fatura_id == id);
            }
            catch
            {
                throw;
            }
        }

        public Fatura? GetById(int id)
        {
            return GetByIdAsync(id).GetAwaiter().GetResult();
        }

        public async Task<Fatura> EkleAsync(Fatura fatura)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Fatura", fatura, _jsonOptions);
            response.EnsureSuccessStatusCode();
            return fatura;
        }

        public void Ekle(Fatura fatura)
        {
            EkleAsync(fatura).GetAwaiter().GetResult();
        }

        public async Task GuncelleAsync(Fatura fatura)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Fatura/{fatura.fatura_id}", fatura, _jsonOptions);
            response.EnsureSuccessStatusCode();
        }

        public void Guncelle(Fatura fatura)
        {
            GuncelleAsync(fatura).GetAwaiter().GetResult();
        }

        public async Task SilAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Fatura/{id}");
            response.EnsureSuccessStatusCode();
        }

        public void Sil(int id)
        {
            SilAsync(id).GetAwaiter().GetResult();
        }
    }
}
