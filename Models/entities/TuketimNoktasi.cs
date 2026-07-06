namespace KcetasWeb.Models
{
    using System;

    public class TuketimNoktasi
    {
        public long TuketimNoktasiId { get; set; }
        public string TuketimNoktasiNo { get; set; }
        public long AboneId { get; set; }
        public int SokakId { get; set; }
        public string BinaNo { get; set; }
        public string DaireNo { get; set; }
        public int TuketiciGrubuId { get; set; }
        public int TarifeTipiId { get; set; }
        public string Durum { get; set; }
        
        // MVP Eksikleri:
        public decimal BaglantiGucuKw { get; set; }
        public string? Enlem { get; set; }
        public string? Boylam { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        // TuketimNoktasi.cs içinde olması gereken örnek tanım
        public virtual TuketiciGrubu TuketiciGrubu { get; set; }

    }


    
        

        
}