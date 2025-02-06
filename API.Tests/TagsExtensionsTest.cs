using StackOverflowTagsApi.Entities;
using StackOverflowTagsApi.Extensions;
using StackOverflowTagsApi.Helpers;

namespace API.Tests
{
    [TestFixture]
    public class TagsExtensionsTest
    {
        private readonly List<Tag> tags = new List<Tag>
        {
            new Tag { Name = "TagC", Share = "10,5", Count = 500 },
            new Tag { Name = "TagA", Share = "5,2", Count = 1000 },
            new Tag { Name = "TagB", Share = "7,8", Count = 456 },
            new Tag { Name = "TagD", Share = "3,3", Count = 325 },
            new Tag { Name = "TagE", Share = "15,0", Count = 111 }
        };

        [Test]
        public void Paginate_ShouldReturnCorrectPage()
        {
            var queryParams = new QueryParams { PageNumber = 1, PageSize = 2 };
            var expectedTags = tags.Skip(0).Take(2).ToList();

            var result = tags.Paginate(queryParams).ToList();

            Assert.That(result, Is.EqualTo(expectedTags));
        }

        [Test]
        public void Paginate_ShouldHandlePageNumberGreaterThanTotalPages()
        {
            var queryParams = new QueryParams { PageNumber = 10, PageSize = 2 };

            var result = tags.Paginate(queryParams).ToList();

            Assert.IsEmpty(result);
        }

        [Test]
        public void Sort_ShouldSortByNameAscending()
        {
            var queryParams = new QueryParams { SortBy = "name", Order = "asc" };
            var expectedTags = tags.OrderBy(t => t.Name).ToList();

            var result = tags.Sort(queryParams).ToList();

            Assert.That(result, Is.EqualTo(expectedTags));
        }

        [Test]
        public void Sort_ShouldSortByNameDescending()
        {
            var queryParams = new QueryParams { SortBy = "name", Order = "desc" };
            var expectedTags = tags.OrderByDescending(t => t.Name).ToList();

            var result = tags.Sort(queryParams).ToList();

            Assert.That(result, Is.EqualTo(expectedTags));
        }

        [Test]
        public void Sort_ShouldSortByShareAscending()
        {
            var queryParams = new QueryParams { SortBy = "share", Order = "asc" };
            var expectedTags = tags.OrderBy(t => Convert.ToDouble(t.Share)).ToList();

            var result = tags.Sort(queryParams).ToList();

            Assert.That(result, Is.EqualTo(expectedTags));
        }

        [Test]
        public void Sort_ShouldSortByShareDescending()
        {
            var queryParams = new QueryParams { SortBy = "share", Order = "desc" };
            var expectedTags = tags.OrderByDescending(t => Convert.ToDouble(t.Share)).ToList();

            var result = tags.Sort(queryParams).ToList();

            Assert.That(result, Is.EqualTo(expectedTags));
        }

        [Test]
        public void Sort_ShouldThrowExceptionForInvalidSortBy()
        {
            var queryParams = new QueryParams { SortBy = "invalid", Order = "asc" };

            var exception = Assert.Throws<Exception>(() =>  tags.Sort(queryParams).ToList());
            Assert.That(exception.Message, Is.EqualTo("Invalid sort value. Valid sort by values are (name, share)"));
        }

        [Test]
        public void Sort_ShouldThrowExceptionForInvalidOrder()
        {
            var queryParams = new QueryParams { SortBy = "name", Order = "invalid" };

            var exception = Assert.Throws<Exception>(() => tags.Sort(queryParams).ToList());
            Assert.That(exception.Message, Is.EqualTo("Invalid order value. Valid order values are (asc, desc)"));
        }
    }
}