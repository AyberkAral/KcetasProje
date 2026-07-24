using KcetasWeb.Models;
using System;
using System.Collections.Generic;

namespace KcetasWeb.Services.Interfaces;

public interface IEndeksOkumaService
{
    System.Threading.Tasks.Task<List<EndeksOkuma>> GetAllAsync();

    System.Threading.Tasks.Task<EndeksOkuma?> GetByIdAsync(long id);

    System.Threading.Tasks.Task<List<EndeksOkuma>> FiltreleAsync(
        string? okumaTipi,
        string? durum,
        DateTime? baslangic,
        DateTime? bitis,
        string? arama);

    System.Threading.Tasks.Task<(int Toplam, int Manuel, int OSOS, int Anomali, decimal OrtalamaTuketim)>
        GetIstatistiklerAsync();

    System.Threading.Tasks.Task CreateAsync(EndeksOkuma model);
    
    System.Threading.Tasks.Task UpdateAsync(EndeksOkuma model);
}