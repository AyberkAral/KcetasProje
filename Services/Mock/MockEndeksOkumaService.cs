using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KcetasWeb.Services.Mock;

public class MockEndeksOkumaService : IEndeksOkumaService
{
    private static readonly List<EndeksOkuma> _okumalar = new();

    public List<EndeksOkuma> GetAll()
    {
        return _okumalar.OrderByDescending(x => x.okuma_zamani).ToList();
    }

    public EndeksOkuma? GetById(long id)
    {
        return _okumalar.FirstOrDefault(x => x.okuma_id == id);
    }

    public List<EndeksOkuma> Filtrele(string? okumaTipi, string? durum, DateTime? baslangic, DateTime? bitis, string? arama)
    {
        var query = _okumalar.AsEnumerable();

        if (!string.IsNullOrEmpty(okumaTipi)) query = query.Where(x => x.okuma_tipi.ToString() == okumaTipi);
        if (!string.IsNullOrEmpty(durum)) query = query.Where(x => x.status == durum);
        if (baslangic.HasValue) query = query.Where(x => x.okuma_zamani >= baslangic.Value);
        if (bitis.HasValue) query = query.Where(x => x.okuma_zamani <= bitis.Value);

        if (!string.IsNullOrWhiteSpace(arama))
        {
            var aramaLower = arama.ToLower();
            query = query.Where(x =>
                x.sayac_id.ToString().Contains(aramaLower)
            );
        }

        return query.OrderByDescending(x => x.okuma_zamani).ToList();
    }

    public (int Toplam, int Manuel, int OSOS, int Anomali, decimal OrtalamaTuketim) GetIstatistikler()
    {
        var toplam = _okumalar.Count;
        var manuel = _okumalar.Count(o => o.okuma_kaynagi == KcetasWeb.Models.Enums.OkumaKaynagi.Manuel);
        var osos = _okumalar.Count(o => o.okuma_kaynagi == KcetasWeb.Models.Enums.OkumaKaynagi.Osos);
        var anomali = _okumalar.Count(o => o.anomali_mi == true);
        var ortalama = _okumalar.Any() ? _okumalar.Average(o => (o.yeni_endeks ?? 0) - (o.onceki_endeks ?? 0)) : 0;

        return (toplam, manuel, osos, anomali, ortalama);
    }

    public void Create(EndeksOkuma model)
    {
        model.okuma_id = _okumalar.Count > 0 ? _okumalar.Max(x => x.okuma_id) + 1 : 1;
        model.created_at = DateTime.Now;
        _okumalar.Add(model);
    }

    public void Update(EndeksOkuma model)
    {
        var existing = _okumalar.FirstOrDefault(x => x.okuma_id == model.okuma_id);
        if (existing != null)
        {
            existing.dogrulama_durumu = model.dogrulama_durumu;
            existing.status = model.status;
            // update other fields if necessary
        }
    }
}
