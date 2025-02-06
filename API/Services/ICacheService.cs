using StackOverflowTagsApi.Entities;
using StackOverflowTagsApi.Models.Dto;

namespace StackOverflowTagsApi.Services
{
    public interface ICacheService
    {
        Task SetDataAsync(IEnumerable<TagDto> data);

        Task SetCacheDataAsync(IEnumerable<Tag> data, int pageNumber);

        Task<IEnumerable<Tag>> GetCacheDataAsync(int pageNumber, int pageSize);

        Task<IEnumerable<Tag>> GetDataAsync();

        Task FlushDatabaseAsync();

        Task<bool> IsEmpty();

        Task<int> GetTagsCount();
    }
}
