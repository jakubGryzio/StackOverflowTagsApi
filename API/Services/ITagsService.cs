using StackOverflowTagsApi.Entities;
using StackOverflowTagsApi.Helpers;
using StackOverflowTagsApi.Models.Response;

namespace StackOverflowTagsApi.Services
{
    public interface ITagsService
    {
        Task<PaginatedResponse> GetTagsAsync(QueryParams queryParams);

        Task FetchTagsAsync();

        Task RefreshTagsAsync(int limit);
    }
}
