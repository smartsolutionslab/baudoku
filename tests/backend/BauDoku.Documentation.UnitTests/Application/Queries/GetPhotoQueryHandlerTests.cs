using AwesomeAssertions;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Application.Queries.GetPhoto;
using BauDoku.Documentation.Domain;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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

        readRepository.GetByIdAsync(Arg.Any<PhotoIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(expected);

        var result = await handler.Handle(new GetPhotoQuery(photoId), CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeSameAs(expected);
    }

    [Fact]
    public async Task Handle_WhenPhotoNotFound_ShouldThrow()
    {
        var photoId = Guid.NewGuid();

        readRepository.GetByIdAsync(Arg.Any<PhotoIdentifier>(), Arg.Any<CancellationToken>())
            .Throws(new KeyNotFoundException("Foto nicht gefunden."));

        var act = () => handler.Handle(new GetPhotoQuery(photoId), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldPassCorrectPhotoIdToRepository()
    {
        var photoId = Guid.NewGuid();
        var expected = new PhotoDto(
            photoId, Guid.NewGuid(), "photo.jpg", "https://blob/photo.jpg", "image/jpeg",
            1024, "before", null, null, null, null, null, null, null, null, null, null, null, null, DateTime.UtcNow);

        readRepository.GetByIdAsync(Arg.Any<PhotoIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(expected);

        await handler.Handle(new GetPhotoQuery(photoId), CancellationToken.None);

        await readRepository.Received(1).GetByIdAsync(
            Arg.Is<PhotoIdentifier>(p => p.Value == photoId),
            Arg.Any<CancellationToken>());
    }
}
