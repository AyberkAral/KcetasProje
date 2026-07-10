using System.Collections.Generic;
using KcetasWeb.Models;

namespace KcetasWeb.Services.Interfaces
{
    public interface ISozlesmeService
    {
        List<Sozlesme> GetAll();
        Sozlesme? GetById(string sozlesmeNo);
        void Create(Sozlesme sozlesme);
        void Update(Sozlesme sozlesme);
        void Delete(string sozlesmeNo);
    }
}
