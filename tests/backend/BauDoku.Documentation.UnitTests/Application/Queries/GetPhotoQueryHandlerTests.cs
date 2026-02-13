using AwesomeAssertions;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Application.Queries.GetPhoto;
using NSubstitute;

namespace BauDoku.Documentation.UnitTests.Application.Queries;

public sealed class GetPhotoQueryHandlerTests
{
    private readonly IPhotoReadRepository readRepository;
    private readonly GetPhotoQueryHandler handler;

    public GetPhotoQueryHandlerTests()
    {
        readRepository = Substitute.For<IPhotoReadRepository>();
        handler = new GetPhotoQueryHandler(readRepository);
    }

    [Fact]
    public async Task Handle_WhenPhotoExists_ShouldReturnDto()
    {
        var photoId = Guid.NewGuid();
        var expected = new PhotoDto(
            photoId,
            Guid.NewGuid(),
            "photo.jpg",
            "https://blob/photo.jpg",
            "image/jpeg",
            1024,
            "before",
            "Vorher-Foto",
            "Kabeltrasse vor Installation",
            48.137154,
            11.576124,
            520.0,
            3.5,
            "internal_gps",
            null,
            null,
            null,
            null,
            null,
            DateTime.UtcNow);

        readRepository.GetByIdAsync(photoId, Arg.Any<CancellationToken>())
            .Returns(expected);

        var result = await handler.Handle(new GetPhotoQuery(photoId), CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeSameAs(expected);
    }

    [Fact]
    public async Task Handle_WhenPhotoNotFound_ShouldReturnNull()
    {
        var photoId = Guid.NewGuid();

        readRepository.GetByIdAsync(photoId, Arg.Any<CancellationToken>())
            .Returns((PhotoDto?)null);

        var result = await handler.Handle(new GetPhotoQuery(photoId), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldPassCorrectPhotoIdToRepository()
    {
        var photoId = Guid.NewGuid();

        readRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((PhotoDto?)null);

        await handler.Handle(new GetPhotoQuery(photoId), CancellationToken.None);

        await readRepository.Received(1).GetByIdAsync(photoId, Arg.Any<CancellationToken>());
    }
}
