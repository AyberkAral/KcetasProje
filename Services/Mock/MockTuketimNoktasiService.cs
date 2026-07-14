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
            new TuketimNoktasi { tuketim_noktasi_id = 1001, tekil_kod = "TK-1001", mahalle = "Mimarsinan", acik_adres = "Test adres 1", bina_no = "12", bagimsiz_bolum_no = "3", tuketici_grubu = "Mesken", status = "Aktif", baglanti_gucu_kw = 5.0m },
            new TuketimNoktasi { tuketim_noktasi_id = 1002, tekil_kod = "TK-1002", mahalle = "Alpaslan", acik_adres = "Sivas Bulvarı No:45", bina_no = "45", bagimsiz_bolum_no = "1", tuketici_grubu = "Ticarethane", status = "Aktif", baglanti_gucu_kw = 15.5m },
            new TuketimNoktasi { tuketim_noktasi_id = 1003, tekil_kod = "TK-1003", mahalle = "Mevlana", acik_adres = "Cemil Baba Cd. No:8", bina_no = "8", bagimsiz_bolum_no = "14", tuketici_grubu = "Mesken", status = "Aktif", baglanti_gucu_kw = 7.0m },
            new TuketimNoktasi { tuketim_noktasi_id = 1004, tekil_kod = "TK-1004", mahalle = "Aygözme", acik_adres = "Lise Cd. No:2", bina_no = "2", bagimsiz_bolum_no = "1", tuketici_grubu = "Tarımsal Sulama", status = "Aktif", baglanti_gucu_kw = 25.0m },
            new TuketimNoktasi { tuketim_noktasi_id = 1005, tekil_kod = "TK-1005", mahalle = "Fevziçakmak", acik_adres = "Fevziçakmak Mh. No:11", bina_no = "11", bagimsiz_bolum_no = "A Blok", tuketici_grubu = "Sanayi", status = "Pasif", baglanti_gucu_kw = 150.0m }
        };
        public List<TuketimNoktasi> GetAll() => _tuketimNoktalari;
        public TuketimNoktasi? GetById(string tekilKod) => _tuketimNoktalari.FirstOrDefault(x => x.tekil_kod == tekilKod || x.tuketim_noktasi_id.ToString() == tekilKod);
        public void Create(TuketimNoktasi tuketimNoktasi) { tuketimNoktasi.tuketim_noktasi_id = _tuketimNoktalari.Max(x=>x.tuketim_noktasi_id) + 1; _tuketimNoktalari.Add(tuketimNoktasi); }
        public void Update(TuketimNoktasi tuketimNoktasi) { }
        public void Delete(string tekilKod) { }
    }
}
