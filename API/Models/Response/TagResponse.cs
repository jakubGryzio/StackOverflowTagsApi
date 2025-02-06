using StackOverflowTagsApi.Models.Dto;

namespace StackOverflowTagsApi.Models.Response
{
    public class TagResponse
    {
        public TagDto[] Items { get; set; } = default!;
    }
}
