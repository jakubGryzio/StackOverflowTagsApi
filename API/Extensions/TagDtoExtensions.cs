using StackOverflowTagsApi.Entities;
using StackOverflowTagsApi.Models.Dto;

namespace StackOverflowTagsApi.Extensions
{
    public static class TagDtoExtensions
    {
        public static IEnumerable<Tag> ConvertToTags(this IEnumerable<TagDto> tagsDto)
        {
            var tagsSum = (double)tagsDto.Sum(tag => tag.Count);
            return tagsDto.Select(tagDto =>
            {
                return new Tag
                {
                    Name = tagDto.Name,
                    Count = tagDto.Count,
                    Share = (tagDto.Count * 100 / tagsSum).ToString("F2")
                };
            });
        }
    }
}
