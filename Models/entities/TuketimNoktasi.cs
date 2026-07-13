namespace KcetasWeb.Models
{
    using System;

    public class TuketimNoktasi
    {
        [System.Text.Json.Serialization.JsonPropertyName("tuketimNoktasiId")]
        public long TuketimNoktasiId { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("tekilKod")]
        public string tekil_kod { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("musteriAd")]
        public string? musteri_ad { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("musteriSoyad")]
        public string? musteri_soyad { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("musteriUnvan")]
        public string? musteri_unvan { get; set; }
        public string? tckn { get; set; }
        public string? vkn { get; set; }
        public string? telefon { get; set; }
        public string? e_posta { get; set; }
        public string? iletisim_tercihi { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("ilceId")]
        public int ilce_id { get; set; }
        public string? il_adi { get; set; }
        public string? ilce_adi { get; set; }
        public string? mahalle { get; set; }
        public string? bina_no { get; set; }
        public string? bagimsiz_bolum_no{ get; set; }
        public string? acik_adres { get; set; }
        public double? koordinat_lat { get; set; }
        public double? koordinat_lot { get; set; }
        public decimal? baglanti_gucu_kw { get; set; }
        public string? tuketici_grubu { get; set; }
        public string? baglanti_grubu { get; set; }
        public string? status { get; set; }

        
        // MVP Eksikleri:
        [System.Text.Json.Serialization.JsonIgnore]
        public decimal BaglantiGucuKw { get; set; }
        
        [System.Text.Json.Serialization.JsonIgnore]
        public string? Enlem { get; set; }
        
        [System.Text.Json.Serialization.JsonIgnore]
        public string? Boylam { get; set; }
        
        public int? created_by { get; set; }
        public int? updated_by { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // TuketimNoktasi.cs içinde olması gereken örnek tanım
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual TuketiciGrubu TuketiciGrubu { get; set; }

        // Kullanıcının Detay sayfasında eklediği yeni alanlar
        public string? sayac_id { get; set; }
        public string? sozlesme_id { get; set; }
        public string? is_emri_no { get; set; }
        public string? okuma_id { get; set; }
    }


    
        

        
}