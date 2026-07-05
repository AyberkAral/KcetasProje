using KcetasWeb.Models.entities;   // <-- DÜZELTİLDİ

namespace KcetasWeb.Services.Interfaces
{
    public static class RolListesi
    {
        public static readonly List<Rol> Roller = new()
        {
            new Rol { RolId = 1, RolAdi = "Yonetici", Aciklama = "Sistem Yöneticisi", CreatedAt = DateTime.Now },
            new Rol { RolId = 2, RolAdi = "Kullanici", Aciklama = "Personel", CreatedAt = DateTime.Now },
            new Rol { RolId = 3, RolAdi = "Abone", Aciklama = "Abone / Müşteri", CreatedAt = DateTime.Now }
        };

        public static Rol? BulRolId(short rolId)
        {
            return Roller.FirstOrDefault(r => r.RolId == rolId);
        }
    }
}