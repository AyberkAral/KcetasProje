using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KcetasWeb.Services.Mock;

public class MockIsEmriService : IIsEmriService
{
    private static List<IsEmri> _isEmirleri = new()
    {
        new IsEmri
        {
            is_emri_id = 1,
            is_emri_no = "IE-2026-0001",
            tuketim_noktasi_id = 1001,
            sayac_id = 5001,
            tip = "Sayaç Sökme",
            oncelik = "Yüksek",
            planlanan_tarih = new DateTime(2026, 4, 10),
            atanan_kullanici_id = 1, // Ahmet Yılmaz (Saha Operasyon Amiri)
            durum = "Tamamlandı",
            saha_sonucu = "Sayaç başarıyla söküldü",
            gerekce = "Sayaç arızası nedeniyle sökme",
            tutanak_no = "TT-2026-0001",
            status = "Active",
            created_at = DateTime.Now.AddDays(-20)
        },
        new IsEmri
        {
            is_emri_id = 2,
            is_emri_no = "IE-2026-0002",
            tuketim_noktasi_id = 1002,
            sayac_id = 5002,
            tip = "Kesme",
            oncelik = "Normal",
            planlanan_tarih = new DateTime(2026, 4, 15),
            atanan_kullanici_id = 2, // Ayşe Demir (Müşteri Temsilcisi)
            durum = "Devam Ediyor",
            status = "Active",
            created_at = DateTime.Now.AddDays(-15)
        },
        new IsEmri
        {
            is_emri_id = 3,
            is_emri_no = "IE-2026-0003",
            tuketim_noktasi_id = 1003,
            sayac_id = 5003,
            tip = "Sayaç Bağlama",
            oncelik = "Düşük",
            planlanan_tarih = new DateTime(2026, 4, 16),
            atanan_kullanici_id = 3, // Veli Şahin
            durum = "Oluşturuldu",
            status = "Active",
            created_at = DateTime.Now.AddDays(-2)
        },
        new IsEmri
        {
            is_emri_id = 4,
            is_emri_no = "IE-2026-0004",
            tuketim_noktasi_id = 1004,
            sayac_id = 5005,
            tip = "Açma",
            oncelik = "Yüksek",
            planlanan_tarih = new DateTime(2026, 4, 11),
            atanan_kullanici_id = 1,
            durum = "Ekibe Atandı",
            status = "Active",
            created_at = DateTime.Now.AddDays(-5)
        },
        new IsEmri
        {
            is_emri_id = 5,
            is_emri_no = "IE-2026-0005",
            tuketim_noktasi_id = 1005,
            sayac_id = 5006,
            tip = "Sayaç Değiştirme",
            oncelik = "Normal",
            planlanan_tarih = new DateTime(2026, 4, 18),
            atanan_kullanici_id = 2,
            durum = "Oluşturuldu",
            status = "Active",
            created_at = DateTime.Now.AddDays(-1)
        }
    };

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

        if (!string.IsNullOrEmpty(tip)) query = query.Where(x => x.tip == tip);
        if (!string.IsNullOrEmpty(durum)) query = query.Where(x => x.durum == durum);
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
            
            isEmri.durum = "Tamamlandı";
            isEmri.updated_at = DateTime.Now;
        }
    }

    public IsEmri Ekle(IsEmri isEmri)
    {
        isEmri.is_emri_id = _isEmirleri.Max(x => x.is_emri_id) + 1;
        if (string.IsNullOrEmpty(isEmri.is_emri_no))
        {
            isEmri.is_emri_no = $"IE-{DateTime.Now.Year}-{(isEmri.is_emri_id * 10).ToString().PadLeft(4, '0')}";
        }
        _isEmirleri.Add(isEmri);
        return isEmri;
    }

    public void DurumGuncelle(long id, string yeniDurum)
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
        isEmri.durum = "EkibeAtandi";
        isEmri.updated_at = DateTime.Now;
    }
}
