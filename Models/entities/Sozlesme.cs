namespace KcetasWeb.Models
{
    using System;

    public class Sozlesme
    {
        public long sozlesme_id { get; set; }
        public string sozlesme_no { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("tuketimNoktasiId")]
        public long tuketim_noktasi_id { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("aboneId")]
        public long abone_id { get; set; }

        public string ad { get; set; }
        public string soyad { get; set; }
        public string unvan { get; set; }
        public string tckn { get; set; }
        public string vkn { get; set; }
        public string telefon { get; set; }
        public string e_posta { get; set; }
        public string iletisim_tercihi { get; set; }
        public string sozlesme_tipi { get; set; }
        public DateTime baslangic_tarihi { get; set; }
        public DateTime? bitis_tarihi { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("durum")]
        public string statu { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("tarifeId")]
        public int tarife_id { get; set; }
        
        public string tarife_grubu { get; set; }
        public decimal guvence_bedeli { get; set; }
        public string status{ get; set; }
        public string? sayac { get; set; }
        public decimal baslangic_endeksi { get; set; }
        public string? odeme_sekli { get; set; }
        public int? created_by { get; set; }
        public int? updated_by { get; set; }


        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}