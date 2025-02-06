using StackOverflowTagsApi.Entities;

namespace StackOverflowTagsApi.Models.Response
{
    public class PaginatedResponse
    {
        public IEnumerable<Tag> Tags { get; set; } = default!;
        public int TotalCount { get; set; } = default!;
        public int TotalPages { get; set; } = default!;
        public int CurrentPages { get; set; } = default!;
    }
}
