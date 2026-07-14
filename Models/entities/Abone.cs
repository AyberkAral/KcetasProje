namespace KcetasWeb.Models
{
    using System;

    public class Abone
    {
        [System.Text.Json.Serialization.JsonPropertyName("aboneId")]
        public int abone_id { get; set; }
        public string abone_no { get; set; }
        public string abone_tipi { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("ad")]
        public string Ad { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("soyad")]
        public string Soyad { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("unvan")]
        public string Unvan { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("tckn")]
        public string tckn { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("vkn")]
        public string vkn { get; set; }
        public string telefon { get; set; }
        public string e_posta { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        

        [System.Text.Json.Serialization.JsonIgnore]
        public int AboneId { get => abone_id; set => abone_id = value; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string AboneNo { get => abone_no; set => abone_no = value; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string AboneTipi { get => abone_tipi; set => abone_tipi = value; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string TcKimlikNo { get => tckn; set => tckn = value; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string VergiNo { get => vkn; set => vkn = value; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string Telefon { get => telefon; set => telefon = value; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string EPosta { get => e_posta; set => e_posta = value; }
        public string Durum { get; set; } = "Aktif";
        
    }
}