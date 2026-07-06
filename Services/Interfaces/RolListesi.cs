using KcetasWeb.Models.entities;

namespace KcetasWeb.Services.Interfaces
{
    // GEÇİCİ: PostgreSQL bağlanana kadar roller bellekte sabit tutulur.
    public static class RolListesi
    {
        public static readonly List<Rol> Roller = new()
        {
            new Rol { RolId = 1, RolAdi = AppRoles.BTYoneticisi, Aciklama = "Entegrasyon/BT Yöneticisi", CreatedAt = DateTime.Now },
            new Rol { RolId = 2, RolAdi = AppRoles.MusteriTemsilcisi, Aciklama = "Müşteri Temsilcisi", CreatedAt = DateTime.Now },
            new Rol { RolId = 3, RolAdi = AppRoles.SozlesmeYetkilisi, Aciklama = "Sözleşme Yetkilisi", CreatedAt = DateTime.Now },
            new Rol { RolId = 4, RolAdi = AppRoles.SayacOkumaPersoneli, Aciklama = "Sayaç Okuma Personeli", CreatedAt = DateTime.Now },
            new Rol { RolId = 5, RolAdi = AppRoles.SahaOperasyonAmiri, Aciklama = "Saha Operasyon Amiri", CreatedAt = DateTime.Now },
            new Rol { RolId = 6, RolAdi = AppRoles.FaturalamaUzmani, Aciklama = "Faturalama Uzmanı", CreatedAt = DateTime.Now },
            new Rol { RolId = 7, RolAdi = AppRoles.Denetci, Aciklama = "Denetçi/Rapor Kullanıcısı", CreatedAt = DateTime.Now }
        };

        public static Rol? BulRolId(short rolId)
        {
            return Roller.FirstOrDefault(r => r.RolId == rolId);
        }
    }
}