using System;
using System.Collections.Generic;
using System.Linq;
using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Services.Mock
{
    public class MockAboneService : IAboneService
    {
        private static List<Abone> _aboneler = new()
        {
            new Abone { abone_id = 1, abone_no = "AB-00101", abone_tipi = "Gerçek", Ad = "Ahmet", Soyad = "Yılmaz", Unvan = "", tckn = "12345678901", vkn = "", telefon = "5551234567", e_posta = "ahmet@mail.com", Durum = "Aktif", CreatedAt = DateTime.Now.AddMonths(-12) },
            new Abone { abone_id = 2, abone_no = "AB-00102", abone_tipi = "Gerçek", Ad = "Ayşe", Soyad = "Demir", Unvan = "", tckn = "98765432101", vkn = "", telefon = "5559876543", e_posta = "ayse@mail.com", Durum = "Aktif", CreatedAt = DateTime.Now.AddMonths(-8) },
            new Abone { abone_id = 3, abone_no = "AB-00103", abone_tipi = "Tüzel", Ad = "", Soyad = "", Unvan = "Örnek Enerji Ltd. Şti.", tckn = "", vkn = "1234567890", telefon = "2121234567", e_posta = "info@ornek.com", Durum = "Pasif", CreatedAt = DateTime.Now.AddMonths(-2) },
            new Abone { abone_id = 4, abone_no = "AB-00104", abone_tipi = "Gerçek", Ad = "Mehmet", Soyad = "Kaya", Unvan = "", tckn = "55544433322", vkn = "", telefon = "5551112233", e_posta = "mehmetk@mail.com", Durum = "Aktif", CreatedAt = DateTime.Now.AddDays(-15) },
            new Abone { abone_id = 5, abone_no = "AB-00105", abone_tipi = "Tüzel", Ad = "", Soyad = "", Unvan = "Kayseri Lojistik A.Ş.", tckn = "", vkn = "9876543210", telefon = "3523334455", e_posta = "muhasebe@kayserilojistik.com", Durum = "Aktif", CreatedAt = DateTime.Now.AddMonths(-5) }
        };

        public List<Abone> GetAll()
        {
            return _aboneler.OrderByDescending(x => x.CreatedAt).ToList();
        }

        public Abone GetById(int id)
        {
            return _aboneler.FirstOrDefault(x => x.abone_id == id);
        }

        public void Create(Abone abone)
        {
            abone.abone_id = _aboneler.Any() ? _aboneler.Max(x => x.abone_id) + 1 : 1;
            abone.abone_no = "AB-" + (100 + abone.abone_id).ToString("D3");
            abone.CreatedAt = DateTime.Now;
            _aboneler.Add(abone);
        }

        public void Update(Abone abone)
        {
            var existing = GetById(abone.abone_id);
            if (existing != null)
            {
                existing.Ad = abone.Ad;
                existing.Soyad = abone.Soyad;
                existing.Unvan = abone.Unvan;
                existing.telefon = abone.telefon;
                existing.e_posta = abone.e_posta;
                existing.Durum = abone.Durum;
                existing.UpdatedAt = DateTime.Now;
            }
        }

        public void Delete(int id)
        {
            var existing = GetById(id);
            if (existing != null)
            {
                _aboneler.Remove(existing);
            }
        }
    }
}
