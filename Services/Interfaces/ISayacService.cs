using System.Collections.Generic;
using KcetasWeb.Models;

namespace KcetasWeb.Services.Interfaces
{
    public interface ISayacService
    {
        List<Sayac> GetAll();
        Sayac? GetById(long id);
        void Create(Sayac sayac);
        void Update(Sayac sayac);
        void Delete(long id);
    }
}
