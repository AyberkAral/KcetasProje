namespace KcetasWeb.Models
{
    using System;

    public class Abone
    {
        public long AboneId { get; set; }
        public string AboneNo { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string TcKimlikNo { get; set; }
        public string Telefon { get; set; }
        public string EPosta { get; set; }
        public string Durum { get; set; }
        
        // MVP Eksikleri:
        public string AboneTipi { get; set; } // "Bireysel" veya "Kurumsal"
        public string? Unvan { get; set; }
        public string? VergiNo { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}