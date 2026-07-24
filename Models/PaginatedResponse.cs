using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace KcetasWeb.Models
{
    public class PaginatedResponse<T>
    {
        public int TotalCount { get; set; }

        public int Count { get => TotalCount; set => TotalCount = value; }

        public int TotalPages { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public List<T> Data { get; set; } = new List<T>();
    }
}
