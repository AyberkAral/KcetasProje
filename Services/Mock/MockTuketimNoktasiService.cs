using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace KcetasWeb.Services.Mock
{
    public class MockTuketimNoktasiService : ITuketimNoktasiService
    {
        private static readonly List<TuketimNoktasi> _tuketimNoktalari = new List<TuketimNoktasi>
        {
            new TuketimNoktasi { TuketimNoktasiId = 1001, tekil_kod = "TK-1001", il_adi = "Kayseri", ilce_adi = "Kocasinan", mahalle = "Mimarsinan", acik_adres = "Test adres 1", tckn = "12345678901", vkn = "", bina_no = "12", bagimsiz_bolum_no = "3", tuketici_grubu = "Mesken", status = "Aktif", BaglantiGucuKw = 5.0m },
            new TuketimNoktasi { TuketimNoktasiId = 1002, tekil_kod = "TK-1002", il_adi = "Kayseri", ilce_adi = "Melikgazi", mahalle = "Alpaslan", acik_adres = "Sivas Bulvarı No:45", tckn = "", vkn = "9876543210", bina_no = "45", bagimsiz_bolum_no = "1", tuketici_grubu = "Ticarethane", status = "Aktif", BaglantiGucuKw = 15.5m },
            new TuketimNoktasi { TuketimNoktasiId = 1003, tekil_kod = "TK-1003", il_adi = "Kayseri", ilce_adi = "Talas", mahalle = "Mevlana", acik_adres = "Cemil Baba Cd. No:8", tckn = "55544433322", vkn = "", bina_no = "8", bagimsiz_bolum_no = "14", tuketici_grubu = "Mesken", status = "Aktif", BaglantiGucuKw = 7.0m },
            new TuketimNoktasi { TuketimNoktasiId = 1004, tekil_kod = "TK-1004", il_adi = "Kayseri", ilce_adi = "Develi", mahalle = "Aygözme", acik_adres = "Lise Cd. No:2", tckn = "11122233344", vkn = "", bina_no = "2", bagimsiz_bolum_no = "1", tuketici_grubu = "Tarımsal Sulama", status = "Aktif", BaglantiGucuKw = 25.0m },
            new TuketimNoktasi { TuketimNoktasiId = 1005, tekil_kod = "TK-1005", il_adi = "Kayseri", ilce_adi = "Kocasinan", mahalle = "Fevziçakmak", acik_adres = "Fevziçakmak Mh. No:11", tckn = "", vkn = "5556667778", bina_no = "11", bagimsiz_bolum_no = "A Blok", tuketici_grubu = "Sanayi", status = "Pasif", BaglantiGucuKw = 150.0m }
        };
        public List<TuketimNoktasi> GetAll() => _tuketimNoktalari;
        public TuketimNoktasi? GetById(string tekilKod) => _tuketimNoktalari.FirstOrDefault(x => x.tekil_kod == tekilKod || x.TuketimNoktasiId.ToString() == tekilKod);
        public void Create(TuketimNoktasi tuketimNoktasi) { tuketimNoktasi.TuketimNoktasiId = _tuketimNoktalari.Max(x=>x.TuketimNoktasiId) + 1; _tuketimNoktalari.Add(tuketimNoktasi); }
        public void Update(TuketimNoktasi tuketimNoktasi) { }
        public void Delete(string tekilKod) { }
    }
}
