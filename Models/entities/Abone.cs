namespace KcetasWeb.Models
{
    using System;

    public class Abone
    {
        public int abone_id { get; set; }
        public string abone_no { get; set; }
        public string abone_tipi { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string? Unvan { get; set; }
        public string tckn { get; set; }
        public string vkn { get; set; }
        public string telefon { get; set; }
        public string e_posta { get; set; }
        public int AboneId { get => abone_id; set => abone_id = value; }
        public string AboneNo { get => abone_no; set => abone_no = value; }
        public string AboneTipi { get => abone_tipi; set => abone_tipi = value; }
        public string TcKimlikNo { get => tckn; set => tckn = value; }
        public string VergiNo { get => vkn; set => vkn = value; }
        public string Telefon { get => telefon; set => telefon = value; }
        public string EPosta { get => e_posta; set => e_posta = value; }
        public string Durum { get; set; } = "Aktif";
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}