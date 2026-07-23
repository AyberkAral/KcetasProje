using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KcetasWeb.Services.Mock;

public class MockIsEmriService : IIsEmriService
{
    private static List<IsEmri> _isEmirleri = new();

    public List<IsEmri> GetAll()
    {
        return _isEmirleri.OrderByDescending(x => x.created_at).ToList();
    }

    public IsEmri? GetById(long id)
    {
        return _isEmirleri.FirstOrDefault(x => x.is_emri_id == id);
    }

    public List<IsEmri> Filtrele(string? tip, string? durum, DateTime? baslangic, DateTime? bitis, string? arama)
    {
        var query = _isEmirleri.AsEnumerable();

        if (!string.IsNullOrEmpty(tip)) query = query.Where(x => ((int?)x.tip).ToString() == tip);
        if (!string.IsNullOrEmpty(durum)) query = query.Where(x => ((int?)x.durum).ToString() == durum);
        if (baslangic.HasValue) query = query.Where(x => x.planlanan_tarih >= baslangic.Value);
        if (bitis.HasValue) query = query.Where(x => x.planlanan_tarih <= bitis.Value);

        if (!string.IsNullOrWhiteSpace(arama))
        {
            var aramaLower = arama.ToLower();
            query = query.Where(x =>
                (x.is_emri_no != null && x.is_emri_no.ToLower().Contains(aramaLower)) ||
                x.tuketim_noktasi_id.ToString().Contains(aramaLower)
            );
        }

        return query.OrderByDescending(x => x.created_at).ToList();
    }

    public void TutanakKaydet(long isEmriId, string tutanakNo, string sahaSonucu, string? gerekce, string? muhurNo, decimal? kesmeEndeksi, decimal? acmaEndeksi, string? eskiSayacNo, string? yeniSayacNo, decimal? eskiSonEndeks, decimal? yeniIlkEndeks)
    {
        var isEmri = _isEmirleri.FirstOrDefault(x => x.is_emri_id == isEmriId);
        if (isEmri != null)
        {
            isEmri.tutanak_no = tutanakNo;
            isEmri.saha_sonucu = sahaSonucu;
            isEmri.gerekce = gerekce ?? "";
            isEmri.muhur_no = muhurNo ?? "";
            
            isEmri.durum = KcetasWeb.Models.Enums.IsEmriDurumu.Tamamlandi;
            isEmri.updated_at = DateTime.Now;
        }
    }

    public IsEmri Ekle(IsEmri isEmri)
    {
        isEmri.is_emri_id = _isEmirleri.Any() ? _isEmirleri.Max(x => x.is_emri_id) + 1 : 1;
        if (string.IsNullOrEmpty(isEmri.is_emri_no))
        {
            isEmri.is_emri_no = $"IE-{DateTime.Now.Year}-{(isEmri.is_emri_id * 10).ToString().PadLeft(4, '0')}";
        }
        _isEmirleri.Add(isEmri);
        return isEmri;
    }

    public void DurumGuncelle(long id, KcetasWeb.Models.Enums.IsEmriDurumu yeniDurum)
    {
        var isEmri = _isEmirleri.FirstOrDefault(x => x.is_emri_id == id);
        if (isEmri != null)
        {
            isEmri.durum = yeniDurum;
            isEmri.updated_at = DateTime.Now;
        }
    }

    public void PersonelAta(long id, long personelId)
    {
        var isEmri = GetById(id);
        if (isEmri == null) return;

        isEmri.atanan_kullanici_id = personelId;
        isEmri.durum = KcetasWeb.Models.Enums.IsEmriDurumu.Atandi;
        isEmri.updated_at = DateTime.Now;
    }

    public Task<List<IsEmri>> GetAllAsync() => Task.FromResult(GetAll());
    public Task<IsEmri?> GetByIdAsync(long id) => Task.FromResult(GetById(id));
    public Task<List<IsEmri>> FiltreleAsync(string? tip, string? durum, DateTime? baslangic, DateTime? bitis, string? arama) => Task.FromResult(Filtrele(tip, durum, baslangic, bitis, arama));
    public Task TutanakKaydetAsync(long isEmriId, string tutanakNo, string sahaSonucu, string? gerekce, string? muhurNo, decimal? kesmeEndeksi, decimal? acmaEndeksi, string? eskiSayacNo, string? yeniSayacNo, decimal? eskiSonEndeks, decimal? yeniIlkEndeks)
    {
        TutanakKaydet(isEmriId, tutanakNo, sahaSonucu, gerekce, muhurNo, kesmeEndeksi, acmaEndeksi, eskiSayacNo, yeniSayacNo, eskiSonEndeks, yeniIlkEndeks);
        return Task.CompletedTask;
    }
    public Task<IsEmri> EkleAsync(IsEmri isEmri) => Task.FromResult(Ekle(isEmri));
    public Task DurumGuncelleAsync(long id, KcetasWeb.Models.Enums.IsEmriDurumu yeniDurum)
    {
        DurumGuncelle(id, yeniDurum);
        return Task.CompletedTask;
    }
    public Task PersonelAtaAsync(long id, long personelId)
    {
        PersonelAta(id, personelId);
        return Task.CompletedTask;
    }
}
