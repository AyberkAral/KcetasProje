namespace KcetasWeb.Models
{
    using System;

    public class AuditLog
    {
        public int audit_id { get; set; }
        public string varlik_tipi { get; set; }
        public int varlik_id { get; set; }
        public string islem_tipi { get; set; }
        public string eski_deger { get; set; }
        public string yeni_deger { get; set; }
        public int kullanici_id { get; set; }
        public string? kullanici_adi { get; set; }
        public string islem_gerekcesi { get; set; }
        public DateTime islem_zamani { get; set; }
        
    }
}
