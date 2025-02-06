using System.Numerics;

namespace StackOverflowTagsApi.Models.Dto
{
    public class TagDto
    {
        public bool HasSynonyms { get; set; } = default!;
        public bool IsmoderatorOnly { get; set; } = default!;
        public bool IsRequired { get; set; } = default!;
        public long Count { get; set; } = default!;
        public string Name { get; set; } = default!;
    }
}
