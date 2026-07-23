using System;

namespace KcetasWeb.ViewModels
{
    public class PaginationBaseViewModel
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public int TotalItems { get; set; }
        public int TotalPages => TotalItems == 0 ? 1 : (int)Math.Ceiling((double)TotalItems / PageSize);
    }
}
