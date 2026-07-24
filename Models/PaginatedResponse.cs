using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace KcetasWeb.Models
{
    public class PaginatedResponse<T>
    {
        [System.Text.Json.Serialization.JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        public int Count { get => TotalCount; set => TotalCount = value; }

        [System.Text.Json.Serialization.JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("currentPage")]
        public int CurrentPage { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("data")]
        public List<T> Data { get; set; } = new List<T>();
    }
}
