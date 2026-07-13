using KcetasWeb.Models;
using System.Collections.Generic;

namespace KcetasWeb.Services.Interfaces
{
    public interface IAuditLogService
    {
        void Ekle(string varlikTipi, long varlikId, string islemTipi, string eskiDeger, string yeniDeger, int kullaniciId, long? islemGerekcesi = null);
        List<AuditLog> GetirByVarlik(string varlikTipi, long varlikId);
    }
}
