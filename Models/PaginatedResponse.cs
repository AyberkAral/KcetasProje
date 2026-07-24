using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace KcetasWeb.Models
{
    public class PaginatedResponse<T>
    {
        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("currentPage")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("data")]
        public List<T> Data { get; set; } = new List<T>();
    }
}
