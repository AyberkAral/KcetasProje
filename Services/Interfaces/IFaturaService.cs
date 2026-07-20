namespace KcetasWeb.Services.Interfaces;

/// <summary>
/// Fatura simülasyon kalemi DTO'su.
/// Her bir fatura kaleminin adı, miktarı, birim fiyatı ve tutarını içerir.
/// </summary>
public record SimulasyonKalemDto(string KalemAdi, decimal Miktar, decimal BirimFiyat, decimal Tutar);

/// <summary>
/// Fatura servisi arayüzü.
/// Tarife grubuna ve tüketim miktarına göre fatura simülasyonu hesaplar.
/// </summary>
public interface IFaturaService
{
    /// <summary>
    /// Verilen tarife grubu ve tüketim miktarına göre fatura simülasyonu hesaplar.
    /// Tüm kalem detaylarını ve toplam tutarı döner.
    /// </summary>
    /// <param name="tarifeGrubu">Tarife grubu: Mesken, Ticarethane, Sanayi</param>
    /// <param name="tuketimMiktari">Tüketim miktarı (kWh)</param>
    /// <returns>Hesaplanan fatura kalemleri ve toplam tutarlar</returns>
    (decimal BirimFiyat, decimal EnerjiBedeli, decimal DagitimBedeli, decimal TrtPayi,
     decimal EnerjiFonu, decimal KdvTutari, decimal ToplamTutar,
     System.Collections.Generic.List<SimulasyonKalemDto> Kalemler) SimulasyonHesapla(string tarifeGrubu, decimal tuketimMiktari);

    System.Collections.Generic.List<KcetasWeb.Models.Fatura> GetAll();
    System.Threading.Tasks.Task<System.Collections.Generic.List<KcetasWeb.Models.Fatura>> GetAllAsync();
    
    KcetasWeb.Models.Fatura? GetById(int id);
    System.Threading.Tasks.Task<KcetasWeb.Models.Fatura?> GetByIdAsync(int id);
    
    void Ekle(KcetasWeb.Models.Fatura fatura);
    System.Threading.Tasks.Task<KcetasWeb.Models.Fatura> EkleAsync(KcetasWeb.Models.Fatura fatura);
    
    void Guncelle(KcetasWeb.Models.Fatura fatura);
    System.Threading.Tasks.Task GuncelleAsync(KcetasWeb.Models.Fatura fatura);
    
    void Sil(int id);
    System.Threading.Tasks.Task SilAsync(int id);
}
