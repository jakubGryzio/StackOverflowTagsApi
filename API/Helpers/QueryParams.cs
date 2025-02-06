using System.ComponentModel.DataAnnotations;

namespace StackOverflowTagsApi.Helpers
{
    public class QueryParams
    {
        public int PageNumber { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "PageSize must be between {1} and {2}.")]
        public int PageSize { get; set; } = 1;

        [AllowedValues("name", "share", ErrorMessage = "Invalid sort value. Valid sort by values are (name, share)")]
        public string SortBy { get; set; } = default!;

        [AllowedValues("asc", "desc", ErrorMessage = "Invalid order value. Valid order values are (asc, desc)")]
        public string Order { get; set; } = default!;
    }
}
