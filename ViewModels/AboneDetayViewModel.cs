namespace KcetasWeb.ViewModels;
using KcetasWeb.Extensions;

public class AboneDetayViewModel
{
    public long AboneId { get; set; }
    public string AboneNo { get; set; } = "";
    public string AdSoyadUnvan { get; set; } = "";
    public string AboneTipi { get; set; } = "";
    public string TcKimlikVergiNo { get; set; } = "";
    public string Telefon { get; set; } = "";
    public string EPosta { get; set; } = "";
    public string Durum { get; set; } = "";
    public string OrijinalTcNo { get; set; } = "";

    // Ekranda gösterilecek maskelenmiş versiyonları property olarak tanımlıyoruz:
        public string MaskeliTcNo => OrijinalTcNo.Maskele(2, 2); 
        public string MaskeliEmail => EPosta.EmailMaskele();
    
}