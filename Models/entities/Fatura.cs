namespace KcetasWeb.Models
{
    using System;

    public class Fatura
    {
        [System.Text.Json.Serialization.JsonPropertyName("faturaId")]
        public int fatura_id { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("faturaNo")]
        public string? fatura_no { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("sozlesmeId")]
        public int sozlesme_id { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("tekilKod")]
        public string? tekil_kod { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("faturaTipi")]
        public string? fatura_tipi { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("donem")]
        public string? donem { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("faturaTarihi")]
        public DateTime? fatura_tarihi { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("sonOdemeTarihi")]
        public DateTime? son_odeme_tarihi { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("okumaId")]
        public int? okuma_id { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("kullaniciId")]
        public int? kullanici_id { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("ilkEndeks")]
        public decimal? ilk_endeks { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("sonEndeks")]
        public decimal? son_endeks { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("tuketimKwh")]
        public decimal? tuketim_kwh { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("reaktifEnduktif")]
        public decimal? reaktif_enduktif { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("reaktifKapasitif")]
        public decimal? reaktif_kapasitif { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("carpan")]
        public decimal? carpan { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("enerjiBedeli")]
        public decimal? enerji_bedeli { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("dagitimBedeli")]
        public decimal? dagatim_bedeli { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("hizmetBedeli")]
        public decimal? hizmet_bedeli { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("kesmeBaglamaBedeli")]
        public decimal? kesme_baglama_bedeli { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("vergiFonToplam")]
        public decimal? vergi_fon_toplam { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("toplamTutar")]
        public decimal? toplam_tutar { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("durum")]
        public string? durum { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("status")]
        public string? status { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("createdAt")]
        public DateTime? created_at { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("updatedAt")]
        public DateTime? updated_at { get; set; }
    }
}