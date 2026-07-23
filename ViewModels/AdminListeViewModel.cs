using System.Collections.Generic;
using KcetasWeb.Models.entities;

namespace KcetasWeb.ViewModels
{
    public class AdminListeViewModel : PaginationBaseViewModel
    {
        public string? FiltreKullaniciAdi { get; set; }
        public string? FiltreAdSoyad { get; set; }
        public short? FiltreRol { get; set; }

        public List<Kullanici> Kullanicilar { get; set; } = new();
    }
}
