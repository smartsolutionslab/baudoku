using System.Text.Json;
using AwesomeAssertions;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.ServiceDefaults;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace BauDoku.BuildingBlocks.UnitTests.ExceptionHandling;

public sealed class GlobalExceptionHandlerTests
{
    private readonly GlobalExceptionHandler handler;
    private readonly IHostEnvironment environment;

    public GlobalExceptionHandlerTests()
    {
        environment = Substitute.For<IHostEnvironment>();
        environment.EnvironmentName.Returns("Production");
        handler = new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance, environment);
    }

    private static DefaultHttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<ProblemDetails> ReadProblemDetails(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        return (await JsonSerializer.DeserializeAsync<ProblemDetails>(context.Response.Body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }))!;
    }

    [Fact]
    public async Task TryHandleAsync_ValidationException_Returns400()
    {
        var context = CreateHttpContext();
        var failures = new List<ValidationFailure> { new("Name", "Name is required") };
        var exception = new ValidationException(failures);

        var result = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        result.Should().BeTrue();
        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task TryHandleAsync_ArgumentException_Returns400()
    {
        var context = CreateHttpContext();
        var exception = new ArgumentException("Invalid argument");

        var result = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        result.Should().BeTrue();
        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        var problemDetails = await ReadProblemDetails(context);
        problemDetails.Title.Should().Be("Bad Request");
    }

    [Fact]
    public async Task TryHandleAsync_ArgumentNullException_Returns400()
    {
        var context = CreateHttpContext();
        var exception = new ArgumentNullException("param");

        var result = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        result.Should().BeTrue();
        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task TryHandleAsync_KeyNotFoundException_Returns404()
    {
        var context = CreateHttpContext();
        var exception = new KeyNotFoundException("Entity not found");

        var result = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        result.Should().BeTrue();
        context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        var problemDetails = await ReadProblemDetails(context);
        problemDetails.Title.Should().Be("Not Found");
    }

    [Fact]
    public async Task TryHandleAsync_FileNotFoundException_Returns404()
    {
        var context = CreateHttpContext();
        var exception = new FileNotFoundException("File not found");

        var result = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        result.Should().BeTrue();
        context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task TryHandleAsync_BusinessRuleException_Returns422()
    {
        var context = CreateHttpContext();
        var rule = Substitute.For<IBusinessRule>();
        rule.Message.Returns("Business rule violated");
        rule.IsBroken().Returns(true);
        var exception = new BusinessRuleException(rule);

        var result = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        result.Should().BeTrue();
        context.Response.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
        var problemDetails = await ReadProblemDetails(context);
        problemDetails.Title.Should().Be("Unprocessable Entity");
    }

    [Fact]
    public async Task TryHandleAsync_DbUpdateConcurrencyException_Returns409()
    {
        var context = CreateHttpContext();
        var exception = new DbUpdateConcurrencyException("Concurrency conflict");

        var result = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        result.Should().BeTrue();
        context.Response.StatusCode.Should().Be(StatusCodes.Status409Conflict);
        var problemDetails = await ReadProblemDetails(context);
        problemDetails.Title.Should().Be("Conflict");
    }

    [Fact]
    public async Task TryHandleAsync_UnhandledException_Returns500_WithoutStackTraceInProd()
    {
        var context = CreateHttpContext();
        var exception = new InvalidOperationException("Something went wrong");

        var result = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        result.Should().BeTrue();
        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        var problemDetails = await ReadProblemDetails(context);
        problemDetails.Detail.Should().NotContain("Something went wrong");
    }

    [Fact]
    public async Task TryHandleAsync_UnhandledException_Returns500_WithDetailInDev()
    {
        environment.EnvironmentName.Returns("Development");
        var devHandler = new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance, environment);
        var context = CreateHttpContext();
        var exception = new InvalidOperationException("Something went wrong");

        var result = await devHandler.TryHandleAsync(context, exception, CancellationToken.None);

        result.Should().BeTrue();
        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        var problemDetails = await ReadProblemDetails(context);
        problemDetails.Detail.Should().Contain("Something went wrong");
    }
}
