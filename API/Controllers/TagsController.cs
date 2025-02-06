using Microsoft.AspNetCore.Mvc;
using StackOverflowTagsApi.Helpers;
using StackOverflowTagsApi.Models.Response;
using StackOverflowTagsApi.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace StackOverflowTagsApi.Controllers
{
    public class TagsController(ITagsService tagsService) : BaseApiController
    {
        private readonly ITagsService tagsService = tagsService;

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PaginatedResponse), StatusCodes.Status200OK)]
        [SwaggerOperation(
            Summary = "Get paginated tags",
            Description = "Retrieves a paginated list of tags based on the provided query parameters.",
            OperationId = "GetPaginatedTags"
        )]
        public async Task<ActionResult<PaginatedResponse>> Get([FromQuery] QueryParams queryParams)
        {
            var paginatedResponse = await tagsService.GetTagsAsync(queryParams);
            return Ok(paginatedResponse);
        }

        [HttpPost("RefreshTags")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(
            Summary = "Refresh tags",
            Description = "Refreshes the tags by fetching new tags from an external source.",
            OperationId = "FetchTags"
        )]
        public async Task<ActionResult<MessageResponse>> FetchTags(int limit)
        {
            if (limit <= 0 || limit > 10000)
            {
                return BadRequest(new MessageResponse { Message = "Limit must be between 1 and 10000" });
            }

            await tagsService.RefreshTagsAsync(limit);
            return Ok(new MessageResponse { Message = $"Refreshing tags has been complted with new {limit} tags." });
        }
    }
}
