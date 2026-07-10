using System.Collections.Generic;
using KcetasWeb.Models;

namespace KcetasWeb.Services.Interfaces
{
    public interface ITuketimNoktasiService
    {
        List<TuketimNoktasi> GetAll();
        TuketimNoktasi? GetById(string tekilKod);
        void Create(TuketimNoktasi tuketimNoktasi);
        void Update(TuketimNoktasi tuketimNoktasi);
        void Delete(string tekilKod);
    }
}
