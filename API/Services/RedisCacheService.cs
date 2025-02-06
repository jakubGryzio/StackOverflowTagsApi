using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using StackOverflowTagsApi.Entities;
using StackOverflowTagsApi.Extensions;
using StackOverflowTagsApi.Models.Dto;
using System.Text.Json;

namespace StackOverflowTagsApi.Services
{
    public class RedisCacheService : ICacheService
    {
        const string TagsCountKey = "TagsCount";
        private readonly IDistributedCache cache;
        private readonly ConnectionMultiplexer redis;
        private readonly IServer server;
        private readonly ILogger<RedisCacheService> logger;

        public RedisCacheService(IDistributedCache cache, ConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
        {
            this.cache = cache;
            this.redis = redis;
            this.logger = logger;
            this.server = redis.GetServer(redis.GetEndPoints().First());
        }

        public async Task FlushDatabaseAsync()
        {
            try
            {
                var server = redis.GetServer(redis.GetEndPoints().First());
                await server.FlushDatabaseAsync();
                logger.LogInformation("Database flushed");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error flushing database from {nameof(RedisCacheService)}", ex);
            }
        }

        public async Task<IEnumerable<Tag>> GetDataAsync()
        {

            var tags = new List<Tag>();
            logger.LogInformation("Getting tags from database has been started");
            try
            {
                const string pattern = "Tags_*";
                var keys = server.Keys(pattern: pattern).ToList();
                foreach (var key in keys)
                {
                    var value = await cache.GetStringAsync(key!);
                    tags.Add(JsonSerializer.Deserialize<Tag>(value!)!);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting tags from database", ex);
            }

            logger.LogInformation("Getting tags from database has been succeeded");
            return tags;
        }

        public async Task<IEnumerable<Tag>> GetCacheDataAsync(int pageNumber, int pageSize)
        {
            try
            {
                string cacheKey = $"PaginatedTags_PageNumber{pageNumber}_PageSize{pageSize}";
                var value = await cache.GetStringAsync(cacheKey);
                if (value == null)
                {
                    logger.LogInformation("There is no cached paginated tags");
                    return [];
                }
                logger.LogInformation("Cached paginated tags has been found");
                return JsonSerializer.Deserialize<List<Tag>>(value!)!;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting cached paginated tags", ex);
            }
        }

        public async Task SetDataAsync(IEnumerable<TagDto> data)
        {
            if (data == null || data.Count() == 0)
            {
                logger.LogWarning("No tags provided for processing.");
                return;
            }

            try
            {
                var tags = data.ConvertToTags();
                var tasks = tags.Select(async tag =>
                {
                    await cache.SetStringAsync($"Tags_{tag.Name}", JsonSerializer.Serialize(tag));
                });

                await Task.WhenAll(tasks);

                var dataCount = data.Count();
                await cache.SetStringAsync(TagsCountKey, $"{dataCount}");

                logger.LogInformation("{count} tags has been inserted to database", dataCount);
            }
            catch (Exception ex)
            {
                throw new Exception("Error setting tags to database", ex);
            }
        }

        public async Task SetCacheDataAsync(IEnumerable<Tag> data, int pageNumber)
        {
            if (data == null)
            {
                logger.LogWarning("Attempted to cache null data for page {PageNumber}.", pageNumber);
                return;
            }

            int dataCount = data.Count();
            string cacheKey = $"PaginatedTags_PageNumber{pageNumber}_PageSize{dataCount}";

            try
            {
                string serializedData = JsonSerializer.Serialize(data);
                await cache.SetStringAsync(cacheKey, serializedData);

                logger.LogInformation("Cached {DataCount} tags with key: {CacheKey}", dataCount, cacheKey);
            }
            catch (Exception ex)
            {
                throw new Exception("Error setting cached tags to database", ex);
            }
        }

        public async Task<bool> IsEmpty()
        {
            try
            {
                var isEmpty = await server.DatabaseSizeAsync() == 0;
                logger.LogInformation("Database is {empty}", isEmpty ? "empty": "not empty");
                return isEmpty;
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking database is empty", ex);
            }
        }

        public async Task<int> GetTagsCount()
        {
            try
            {
                return Convert.ToInt32(await cache.GetStringAsync(TagsCountKey));
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking tags count", ex);
            }
        }
    }
}
