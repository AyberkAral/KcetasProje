namespace KcetasWeb.Models
{
    using System;

    public class Sayac
    {
        [System.Text.Json.Serialization.JsonPropertyName("sayacId")]
        public long sayac_id { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("seriNo")]
        public string seri_no { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("tuketimNoktasiId")]
        public int? tuketim_noktasi_id { get; set; }
        
        public string marka { get; set; }
        public string model { get; set; }
        public string faz { get; set; }
        public decimal carpan { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("muhurNo")]
        public string? muhur_no { get; set; }
        
        public string sökme_nedeni {get; set;}
        public string MuhurDurumu {get; set;}
        
        [System.Text.Json.Serialization.JsonPropertyName("durum")]
        public string durum { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("status")]
        public string status { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("sonEndeks")]
        public decimal? son_endeks { get; set; }
        
        public string? aciklama { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("createdBy")]
        public int? created_by { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("updatedBy")]
        public int? updated_by { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("uretimYili")]
        public int uretim_yili { get; set; }
    }
}