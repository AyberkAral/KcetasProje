using System.Collections.Generic;
using KcetasWeb.Models;

namespace KcetasWeb.Services.Interfaces
{
    public interface ISayacService
    {
        Task<List<Sayac>> GetAllAsync();
        Task<Sayac?> GetByIdAsync(long id);
        Task CreateAsync(Sayac sayac);
        Task UpdateAsync(Sayac sayac);
        Task DeleteAsync(long id);
    }
}
