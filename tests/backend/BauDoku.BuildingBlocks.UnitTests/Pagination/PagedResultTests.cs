using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Pagination;

namespace BauDoku.BuildingBlocks.UnitTests.Pagination;

public sealed class PagedResultTests
{
    [Fact]
    public void Create_ShouldSetProperties()
    {
        var items = new List<string> { "a", "b", "c" };

        var result = new PagedResult<string>(items, 10, 1, 3);

        result.Items.Should().HaveCount(3);
        result.TotalCount.Should().Be(10);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(3);
    }

    [Fact]
    public void TotalPages_ShouldCalculateCorrectly()
    {
        var result = new PagedResult<int>([], 25, 1, 10);

        result.TotalPages.Should().Be(3);
    }

    [Fact]
    public void TotalPages_WithExactDivision_ShouldCalculateCorrectly()
    {
        var result = new PagedResult<int>([], 20, 1, 10);

        result.TotalPages.Should().Be(2);
    }

    [Fact]
    public void TotalPages_WithZeroItems_ShouldBeZero()
    {
        var result = new PagedResult<int>([], 0, 1, 10);

        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public void HasNextPage_OnFirstPageOfMultiple_ShouldBeTrue()
    {
        var result = new PagedResult<int>([], 25, 1, 10);

        result.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public void HasNextPage_OnLastPage_ShouldBeFalse()
    {
        var result = new PagedResult<int>([], 25, 3, 10);

        result.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void HasPreviousPage_OnFirstPage_ShouldBeFalse()
    {
        var result = new PagedResult<int>([], 25, 1, 10);

        result.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public void HasPreviousPage_OnSecondPage_ShouldBeTrue()
    {
        var result = new PagedResult<int>([], 25, 2, 10);

        result.HasPreviousPage.Should().BeTrue();
    }
}
