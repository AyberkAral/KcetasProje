using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace KcetasWeb.Services.Mock
{
    public class MockSozlesmeService : ISozlesmeService
    {
        private static readonly List<Sozlesme> _sozlesmeler = new List<Sozlesme>
        {
            new Sozlesme { sozlesme_id = 1001, sozlesme_no = "SZL-2026-001", tuketim_noktasi_id = 1001, ad = "Ali", soyad = "Yılmaz", unvan = "", tckn = "12345678901", tarife_grubu = "Mesken", statu = "Aktif", guvence_bedeli = 650.0m, baslangic_tarihi = System.DateTime.Now.AddYears(-1) },
            new Sozlesme { sozlesme_id = 1002, sozlesme_no = "SZL-2026-002", tuketim_noktasi_id = 1002, ad = "", soyad = "", unvan = "Kayseri Lojistik A.Ş.", vkn = "9876543210", tarife_grubu = "Ticarethane", statu = "Aktif", guvence_bedeli = 3500.0m, baslangic_tarihi = System.DateTime.Now.AddMonths(-5) },
            new Sozlesme { sozlesme_id = 1003, sozlesme_no = "SZL-2026-003", tuketim_noktasi_id = 1003, ad = "Ayşe", soyad = "Kaya", unvan = "", tckn = "55544433322", tarife_grubu = "Mesken", statu = "Güvence Bekliyor", guvence_bedeli = 650.0m, baslangic_tarihi = System.DateTime.Now.AddDays(-2) },
            new Sozlesme { sozlesme_id = 1004, sozlesme_no = "SZL-2026-004", tuketim_noktasi_id = 1004, ad = "Mehmet", soyad = "Demir", unvan = "", tckn = "11122233344", tarife_grubu = "Tarımsal Sulama", statu = "Aktif", guvence_bedeli = 1200.0m, baslangic_tarihi = System.DateTime.Now.AddYears(-2) }
        };
        public List<Sozlesme> GetAll() => _sozlesmeler;
        public Sozlesme? GetById(string sozlesmeNo) => _sozlesmeler.FirstOrDefault(x => x.sozlesme_no == sozlesmeNo || x.sozlesme_id.ToString() == sozlesmeNo);
        public void Create(Sozlesme sozlesme) { sozlesme.sozlesme_id = _sozlesmeler.Max(x=>x.sozlesme_id) + 1; _sozlesmeler.Add(sozlesme); }
        public void Update(Sozlesme sozlesme) { }
        public void Delete(string sozlesmeNo) { }
    }
}
