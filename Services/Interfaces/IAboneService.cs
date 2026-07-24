using System.Collections.Generic;
using KcetasWeb.Models;

namespace KcetasWeb.Services.Interfaces
{
    public interface IAboneService
    {
        System.Threading.Tasks.Task<List<Abone>> GetAllAsync();
        System.Threading.Tasks.Task<PaginatedResponse<Abone>> GetPagedAsync(int page, int pageSize);
        System.Threading.Tasks.Task<KcetasWeb.Models.Dtos.PagedResultDto<KcetasWeb.Models.Dtos.AboneListDto>> GetPagedCursorAsync(long? lastId, int limit);
        System.Threading.Tasks.Task<Abone> GetByIdAsync(int id);
        System.Threading.Tasks.Task CreateAsync(Abone abone);
        System.Threading.Tasks.Task UpdateAsync(Abone abone);
        System.Threading.Tasks.Task DeleteAsync(int id);
    }
}
