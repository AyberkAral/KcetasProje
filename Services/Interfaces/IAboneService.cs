using System.Collections.Generic;
using KcetasWeb.Models;

namespace KcetasWeb.Services.Interfaces
{
    public interface IAboneService
    {
        List<Abone> GetAll();
        Abone GetById(int id);
        void Create(Abone abone);
        void Update(Abone abone);
        void Delete(int id);
    }
}
