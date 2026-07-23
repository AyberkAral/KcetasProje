using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace KcetasWeb.Services.Mock
{
    public class MockSayacService : ISayacService
    {
        private static readonly List<Sayac> _sayaclar = new List<Sayac>
        {
            new Sayac { sayac_id = 5001, seri_no = "SN-38-1001", tuketim_noktasi_id = 1001, marka = "Luna", model = "LSM", faz = KcetasWeb.Models.Enums.SayacFaz.Monofaze, carpan = 1, muhur_no = "123456", durum = KcetasWeb.Models.Enums.SayacDurumu.Bagli },
            new Sayac { sayac_id = 5002, seri_no = "SN-38-1002", tuketim_noktasi_id = 1002, marka = "Makel", model = "M500", faz = KcetasWeb.Models.Enums.SayacFaz.Trifaze, carpan = 1, muhur_no = "123457", durum = KcetasWeb.Models.Enums.SayacDurumu.Bagli },
            new Sayac { sayac_id = 5003, seri_no = "SN-38-1003", tuketim_noktasi_id = 1003, marka = "Viko", model = "V10", faz = KcetasWeb.Models.Enums.SayacFaz.Monofaze, carpan = 1, muhur_no = "123458", durum = KcetasWeb.Models.Enums.SayacDurumu.Bagli },
            new Sayac { sayac_id = 5004, seri_no = "SN-38-1004", tuketim_noktasi_id = 0, marka = "Luna", model = "LSM", faz = KcetasWeb.Models.Enums.SayacFaz.Trifaze, carpan = 2, muhur_no = "", durum = KcetasWeb.Models.Enums.SayacDurumu.Depoda },
            new Sayac { sayac_id = 5005, seri_no = "SN-38-1005", tuketim_noktasi_id = 1004, marka = "Köhler", model = "K20", faz = KcetasWeb.Models.Enums.SayacFaz.Monofaze, carpan = 1, muhur_no = "123460", durum = KcetasWeb.Models.Enums.SayacDurumu.Bagli },
            new Sayac { sayac_id = 5006, seri_no = "SN-38-1006", tuketim_noktasi_id = 0, marka = "Makel", model = "M200", faz = KcetasWeb.Models.Enums.SayacFaz.Monofaze, carpan = 1, muhur_no = "", durum = KcetasWeb.Models.Enums.SayacDurumu.Arizali }
        };
        public List<Sayac> GetAll() => _sayaclar;
        public Sayac? GetById(long id) => _sayaclar.FirstOrDefault(x => x.sayac_id == id);
        public void Create(Sayac sayac) { sayac.sayac_id = _sayaclar.Max(x=>x.sayac_id) + 1; _sayaclar.Add(sayac); }
        public void Update(Sayac sayac) { }
        public void Delete(long id) { }
    }
}
