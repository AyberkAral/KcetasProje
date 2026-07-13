using KcetasWeb.Models.entities;
using KcetasWeb.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace KcetasWeb.Services.Mock
{
    public class MockKullaniciDeposu : IKullaniciDeposu
    {
        private static readonly List<Kullanici> _kullanicilar = new List<Kullanici>
        {
            new Kullanici { kullanici_id = 1, kullanici_adi = "admin", sifre_hash = "123", rol_adi = "BTYoneticisi", ad_soyad = "Admin Yönetici", durum = "Aktif" }
        };

        public bool KullaniciAdiVarMi(string kullaniciAdi) => _kullanicilar.Any(x => x.kullanici_adi == kullaniciAdi);
        public Kullanici Ekle(Kullanici kullanici) { kullanici.kullanici_id = _kullanicilar.Max(x => x.kullanici_id) + 1; _kullanicilar.Add(kullanici); return kullanici; }
        public Kullanici? BulKullaniciAdiIle(string kullaniciAdi) => _kullanicilar.FirstOrDefault(x => x.kullanici_adi == kullaniciAdi);
        public List<Kullanici> Listele() => _kullanicilar;
        public Kullanici? BulId(long id) => _kullanicilar.FirstOrDefault(x => x.kullanici_id == id);
        public bool Guncelle(Kullanici kullanici) { var varolan = BulId(kullanici.kullanici_id); if(varolan!=null){ varolan.ad_soyad = kullanici.ad_soyad; return true; } return false; }
        public bool Sil(long id) { var k = BulId(id); if(k!=null) { _kullanicilar.Remove(k); return true; } return false; }
    }
}
