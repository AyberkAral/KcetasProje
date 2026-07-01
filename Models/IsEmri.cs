namespace KcetasWeb.Models
{
    public class IsEmri
    {
        public long IsEmriId { get; set; }
        public string IsEmriNo { get; set; }
        
        // Hangi tesisata ve sayaca gidilecek?
        public long TuketimNoktasiId { get; set; }
        public long SayacId { get; set; }
        
        // İş emrinin tipi (Örn: Okuma, Kesme, Açma, Arıza)
        public string Tip { get; set; }
        public string Oncelik { get; set; }
        public DateTime PlanlananTarih { get; set; }
        
        // Hangi personele atandı?
        public long AtananKullaniciId { get; set; }
        
        public string Durum { get; set; }
        public string SahaSonucu { get; set; }
        public string Gerekce { get; set; }
        public string Aciklama { get; set; }
        
        // Sayaç sökme/takma işlemlerinde kullanılacak alanlar
        public string EskiSayacNo { get; set; }
        public string YeniSayacNo { get; set; }
        public decimal? YeniIlkEndeks { get; set; } // Nullable olabilir
        public string MuhurNo { get; set; }
        public string TutanakNo { get; set; }
        public decimal? KesmeEndeksi { get; set; }
        
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}