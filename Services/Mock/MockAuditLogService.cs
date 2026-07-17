using KcetasWeb.Models;
using KcetasWeb.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KcetasWeb.Services.Mock
{
    public class MockAuditLogService : IAuditLogService
    {
        private static readonly List<AuditLog> _logs = new List<AuditLog>();

        public void Ekle(string varlikTipi, int varlikId, string islemTipi, string eskiDeger, string yeniDeger, int kullaniciId, string islemGerekcesi = null)
        {
            var log = new AuditLog
            {
                audit_id = _logs.Count > 0 ? _logs.Max(x => x.audit_id) + 1 : 1,
                varlik_tipi = varlikTipi,
                varlik_id = varlikId,
                islem_tipi = islemTipi,
                eski_deger = eskiDeger,
                yeni_deger = yeniDeger,
                kullanici_id = kullaniciId,
                islem_gerekcesi = islemGerekcesi,
                islem_zamani = DateTime.Now
            };
            _logs.Add(log);
        }

        public List<AuditLog> GetirByVarlik(string varlikTipi, int varlikId)
        {
            return _logs.Where(x => x.varlik_tipi == varlikTipi && x.varlik_id == varlikId)
                        .OrderByDescending(x => x.islem_zamani)
                        .ToList();
        }

        public List<AuditLog> GetAll(int page = 1, int pageSize = 100)
        {
            return _logs.OrderByDescending(x => x.islem_zamani)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
        }
    }
}
