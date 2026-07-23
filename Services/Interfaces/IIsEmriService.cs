using KcetasWeb.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KcetasWeb.Services.Interfaces;

public interface IIsEmriService
{
    List<IsEmri> GetAll();
    Task<List<IsEmri>> GetAllAsync();
    IsEmri? GetById(long id);
    Task<IsEmri?> GetByIdAsync(long id);
    List<IsEmri> Filtrele(string? tip, string? durum, DateTime? baslangic, DateTime? bitis, string? arama);
    Task<List<IsEmri>> FiltreleAsync(string? tip, string? durum, DateTime? baslangic, DateTime? bitis, string? arama);
    void TutanakKaydet(long isEmriId, string tutanakNo, string sahaSonucu, string? gerekce, string? muhurNo, decimal? kesmeEndeksi, decimal? acmaEndeksi, string? eskiSayacNo, string? yeniSayacNo, decimal? eskiSonEndeks, decimal? yeniIlkEndeks);
    Task TutanakKaydetAsync(long isEmriId, string tutanakNo, string sahaSonucu, string? gerekce, string? muhurNo, decimal? kesmeEndeksi, decimal? acmaEndeksi, string? eskiSayacNo, string? yeniSayacNo, decimal? eskiSonEndeks, decimal? yeniIlkEndeks);
    IsEmri Ekle(IsEmri isEmri);
    Task<IsEmri> EkleAsync(IsEmri isEmri);
    void DurumGuncelle(long id, KcetasWeb.Models.Enums.IsEmriDurumu yeniDurum);
    Task DurumGuncelleAsync(long id, KcetasWeb.Models.Enums.IsEmriDurumu yeniDurum);
    void PersonelAta(long id, long personelId);
    Task PersonelAtaAsync(long id, long personelId);
}
