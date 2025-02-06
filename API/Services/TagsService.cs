using StackOverflowTagsApi.Entities;
using StackOverflowTagsApi.Extensions;
using StackOverflowTagsApi.Helpers;
using StackOverflowTagsApi.Models.Dto;
using StackOverflowTagsApi.Models.Response;
using System.Text.Json;

namespace StackOverflowTagsApi.Services
{
    public class TagsService(ICacheService cacheService, ILogger<TagsService> logger) : ITagsService
    {
        private readonly ICacheService cacheService = cacheService;
        private readonly ILogger<TagsService> logger = logger;

        public async Task<PaginatedResponse> GetTagsAsync(QueryParams queryParams)
        {
            var tagsCount = await cacheService.GetTagsCount();
            var paginatedTags = await GetPaginatedTags(queryParams);
            return new PaginatedResponse
            {
                Tags = paginatedTags.Sort(queryParams),
                TotalCount = tagsCount,
                TotalPages = (int)Math.Ceiling((double)tagsCount / queryParams.PageSize),
                CurrentPages = queryParams.PageNumber
            };
        }

        public async Task FetchTagsAsync()
        {
            if (!await cacheService.IsEmpty()) return;
            await this.FetchTags(1000);
            logger.LogInformation("Tags have been fetched and stored");
        }

        public async Task RefreshTagsAsync(int limit)
        {
            await cacheService.FlushDatabaseAsync();
            await this.FetchTags(limit);
            logger.LogInformation("Tags have been fetched and stored");
        }

        private async Task FetchTags(int limit)
        {
            List<TagDto> tags = new();
            object lockObj = new();

            var pageCount = (int)Math.Ceiling((double)limit / 100);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 Safari/537.36");
                logger.LogInformation("Fetching tags has been started");
                await Parallel.ForEachAsync(Enumerable.Range(1, pageCount), async (page, _) =>
                {
                    var url = $"https://api.stackexchange.com/2.3/tags?key=rl_r992ZxEMBxLKKEzzFaRuGDoxo&site=stackoverflow&page={page}&pagesize=100&sort=popular";
                    try
                    {
                        var pageData = await client.GetFromJsonAsync<TagResponse>(url);
                        if (pageData != null)
                        {
                            lock (lockObj)
                            {
                                tags.AddRange(pageData.Items);
                            }
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        throw new Exception($"Failed to fetch data from {url}: {ex.Message}");
                    }
                    catch (JsonException ex)
                    {
                        throw new Exception($"Failed to parse JSON from {url}: {ex.Message}");
                    }
                });
            }

            await cacheService.SetDataAsync(tags);
            logger.LogInformation("Fetching tags has been ended successfully");
        }

        private async Task<IEnumerable<Tag>> GetPaginatedTags(QueryParams queryParams)
        {
            var paginatedCacheTags = await cacheService.GetCacheDataAsync(queryParams.PageNumber, queryParams.PageSize);
            if (paginatedCacheTags.Any()) return paginatedCacheTags;
            var tags = await cacheService.GetDataAsync();
            var paginatedTags = tags.Paginate(queryParams);
            await cacheService.SetCacheDataAsync(paginatedTags, queryParams.PageNumber);
            return paginatedTags;
        }
    }
}
