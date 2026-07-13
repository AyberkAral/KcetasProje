using System.Collections.Generic;

namespace KcetasWeb.ViewModels
{
    public class AboneListeViewModel
    {
        public List<AboneSatirViewModel> Aboneler { get; set; } = new();
    }

    public class AboneSatirViewModel
    {
        public int AboneId { get; set; }
        public string AboneNo { get; set; } = null!;
        public string AdSoyad { get; set; } = null!;
        public string KimlikNoMaskeli { get; set; } = null!;
        public string Telefon { get; set; } = null!;
        public string Mail { get; set; } = null!;
        public string AboneTipi { get; set; } = null!;
        public string Durum { get; set; } = null!;
    }
}