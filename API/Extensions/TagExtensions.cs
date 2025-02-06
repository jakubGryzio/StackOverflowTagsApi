using StackOverflowTagsApi.Entities;
using StackOverflowTagsApi.Helpers;

namespace StackOverflowTagsApi.Extensions
{
    public static class TagExtensions
    {
        public static IEnumerable<Tag> Paginate(this IEnumerable<Tag> tags, QueryParams queryParams)
        {
            return tags
            .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize);
        }

        public static IEnumerable<Tag> Sort(this IEnumerable<Tag> tags, QueryParams queryParams)
        {
            return queryParams.SortBy switch
            {
                "name" => queryParams.Order switch
                {
                    "asc" => tags.OrderBy(tag => tag.Name),
                    "desc" => tags.OrderByDescending(tag => tag.Name),
                    _ => throw new Exception("Invalid order value. Valid order values are (asc, desc)"),
                },
                "share" => queryParams.Order switch
                {
                    "asc" => tags.OrderBy(tag => Convert.ToDouble(tag.Share)),
                    "desc" => tags.OrderByDescending(tag => Convert.ToDouble(tag.Share)),
                    _ => throw new Exception("Invalid order"),
                },
                _ => throw new Exception("Invalid sort value. Valid sort by values are (name, share)"),
            };
        }
    }
}
