using Microsoft.AspNetCore.Mvc;
using Moq;
using StackOverflowTagsApi.Controllers;
using StackOverflowTagsApi.Entities;
using StackOverflowTagsApi.Helpers;
using StackOverflowTagsApi.Models.Response;
using StackOverflowTagsApi.Services;

namespace API.Tests
{
    [TestFixture]
    public class TagsControllerTests
    {
        private Mock<ITagsService> _mockTagsService;
        private TagsController _tagsController;

        [SetUp]
        public void Setup()
        {
            _mockTagsService = new Mock<ITagsService>();
            _tagsController = new TagsController(_mockTagsService.Object);
        }

        [Test]
        public async Task Get_ShouldReturnPaginatedResponse()
        {
            var queryParams = new QueryParams 
            {
                PageNumber = 1,
                PageSize = 2,
                SortBy = "name",
                Order = "asc"
            };
            var expectedResponse = new PaginatedResponse 
            {
                Tags = new List<Tag>
                {
                    new Tag { Name = "TagC", Share = "10,5", Count = 500 },
                    new Tag { Name = "TagA", Share = "5,2", Count = 1000 },
                },
                TotalCount = 5,
                TotalPages = 3,
                CurrentPages = 1
            };
            _mockTagsService.Setup(service => service.GetTagsAsync(queryParams))
                            .ReturnsAsync(expectedResponse);

            var result = await _tagsController.Get(queryParams);
            Assert.That(result, Is.InstanceOf<ActionResult<PaginatedResponse>>());

            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var actualResponse = okResult.Value as PaginatedResponse;

            Assert.That(actualResponse, Is.Not.Null);
            Assert.That(actualResponse, Is.EqualTo(expectedResponse));
        }

        [Test]
        public async Task FetchTags_ShouldReturnBadRequest_WhenLimitIsInvalid()
        {
            int invalidLimit = 0;

            var result = await _tagsController.FetchTags(invalidLimit);
            Assert.That(result, Is.InstanceOf<ActionResult<MessageResponse>>());

            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);

            var messageResponse = badRequestResult.Value as MessageResponse;
            Assert.That(badRequestResult.Value, Is.InstanceOf<MessageResponse>());

            Assert.IsNotNull(messageResponse);
            Assert.That(messageResponse.Message, Is.EqualTo("Limit must be between 1 and 10000"));
        }

        [Test]
        public async Task FetchTags_ShouldReturnOk_WhenLimitIsValid()
        {
            int validLimit = 100;
            _mockTagsService.Setup(service => service.RefreshTagsAsync(validLimit))
                            .Returns(Task.CompletedTask);

            var result = await _tagsController.FetchTags(validLimit);
            Assert.That(result, Is.InstanceOf<ActionResult<MessageResponse>>());

            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value, Is.InstanceOf<MessageResponse>());

            var messageResponse = okResult.Value as MessageResponse;
            Assert.That(messageResponse, Is.Not.Null);
            Assert.That(messageResponse.Message, Is.EqualTo($"Refreshing tags has been complted with new {validLimit} tags."));
        }

        [Test]
        public async Task FetchTags_ShouldCallRefreshTagsAsync_WhenLimitIsValid()
        {
            int validLimit = 100;
            _mockTagsService.Setup(service => service.RefreshTagsAsync(validLimit))
                            .Returns(Task.CompletedTask)
                            .Verifiable();

            await _tagsController.FetchTags(validLimit);

            _mockTagsService.Verify(service => service.RefreshTagsAsync(validLimit), Times.Once);
        }
    }
}
