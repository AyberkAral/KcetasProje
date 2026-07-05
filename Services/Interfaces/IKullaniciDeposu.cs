using KcetasWeb.Models.entities;

namespace KcetasWeb.Services.Interfaces
{
    public interface IKullaniciDeposu
    {
        bool KullaniciAdiVarMi(string kullaniciAdi);
        Kullanici Ekle(Kullanici kullanici);
        Kullanici? BulKullaniciAdiIle(string kullaniciAdi);
        List<Kullanici> Listele();
        Kullanici? BulId(long id);
        bool Guncelle(Kullanici kullanici);
        bool Sil(long id);
    }
}