namespace KcetasWeb.Models
{
    using System;

    public class Sozlesme
    {
        public int sozlesme_id { get; set; }
        public string sozlesme_no { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("tuketimNoktasiId")]
        public int tuketim_noktasi_id { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("aboneId")]
        public int? abone_id { get; set; }

        public int? tarife_id { get; set; }
        public string sozlesme_tipi { get; set; }
        public DateTime? baslangic_tarihi { get; set; }
        public DateTime? bitis_tarihi { get; set; }
        public decimal? guvence_bedeli { get; set; }
        public string durum { get; set; }

        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}