using KcetasWeb.Models;
using System.Collections.Generic;

namespace KcetasWeb.Services.Interfaces
{
    public interface IAuditLogService
    {
        void Ekle(string varlikTipi, int varlikId, string islemTipi, string eskiDeger, string yeniDeger, int kullaniciId, string islemGerekcesi = null);
        List<AuditLog> GetirByVarlik(string varlikTipi, int varlikId);
        List<AuditLog> GetAll(int page = 1, int pageSize = 100);
    }
}
