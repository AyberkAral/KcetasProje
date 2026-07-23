using KcetasWeb.Models;
using System.Collections.Generic;

namespace KcetasWeb.ViewModels
{
    public class SayacListeViewModel : PaginationBaseViewModel
    {
        public string? FiltreSeriNo { get; set; }
        public string? FiltreMarka { get; set; }
        public string? FiltreDurum { get; set; }

        public List<Sayac> Sayaclar { get; set; } = new();
    }
}
