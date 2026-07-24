using KcetasWeb.Models;
using System;
using System.Collections.Generic;

namespace KcetasWeb.Services.Interfaces;

public interface IOutboxService
{
    Task<List<EntegrasyonOutbox>> GetAllAsync();
    Task<EntegrasyonOutbox?> GetByIdAsync(long id);
    Task<List<EntegrasyonOutbox>> FiltreleAsync(string? durum, string? hedefSistem, DateTime? baslangic, DateTime? bitis);
    Task<(int Toplam, int Bekleyen, int Gonderilmis, int Basarisiz)> GetIstatistiklerAsync();
    Task<bool> YenidenGonderAsync(long id);
}
