using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.BuildingBlocks.UnitTests.Pagination;

public sealed class PagedResultTests
{
    [Fact]
    public void Create_ShouldSetProperties()
    {
        var items = new List<string> { "a", "b", "c" };

        var result = new PagedResult<string>(items, 10, PageNumber.From(1), PageSize.From(3));

        result.Items.Should().HaveCount(3);
        result.TotalCount.Should().Be(10);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(3);
    }

    [Fact]
    public void TotalPages_ShouldCalculateCorrectly()
    {
        var result = new PagedResult<int>([], 25, PageNumber.From(1), PageSize.From(10));

        result.TotalPages.Should().Be(3);
    }

    [Fact]
    public void TotalPages_WithExactDivision_ShouldCalculateCorrectly()
    {
        var result = new PagedResult<int>([], 20, PageNumber.From(1), PageSize.From(10));

        result.TotalPages.Should().Be(2);
    }

    [Fact]
    public void TotalPages_WithZeroItems_ShouldBeZero()
    {
        var result = new PagedResult<int>([], 0, PageNumber.From(1), PageSize.From(10));

        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public void HasNextPage_OnFirstPageOfMultiple_ShouldBeTrue()
    {
        var result = new PagedResult<int>([], 25, PageNumber.From(1), PageSize.From(10));

        result.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public void HasNextPage_OnLastPage_ShouldBeFalse()
    {
        var result = new PagedResult<int>([], 25, PageNumber.From(3), PageSize.From(10));

        result.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void HasPreviousPage_OnFirstPage_ShouldBeFalse()
    {
        var result = new PagedResult<int>([], 25, PageNumber.From(1), PageSize.From(10));

        result.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public void HasPreviousPage_OnSecondPage_ShouldBeTrue()
    {
        var result = new PagedResult<int>([], 25, PageNumber.From(2), PageSize.From(10));

        result.HasPreviousPage.Should().BeTrue();
    }
}
