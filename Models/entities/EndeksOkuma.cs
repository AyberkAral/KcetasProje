namespace KcetasWeb.Models
{
    using System;

    public class EndeksOkuma
    {
        public long EndeksOkumaId { get; set; }
        public long TuketimNoktasiId { get; set; }
        public long SayacId { get; set; }
        public DateTime OkumaTarihi { get; set; }
        public decimal IlkEndeks { get; set; }
        public decimal SonEndeks { get; set; }
        public decimal TuketimMiktari { get; set; }
        public string OkumaTipi { get; set; }
        public string Durum { get; set; }
        
        // MVP Eksikleri:
        public string? OkunamamaNedeni { get; set; }
        public string? AnomaliAciklamasi { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}