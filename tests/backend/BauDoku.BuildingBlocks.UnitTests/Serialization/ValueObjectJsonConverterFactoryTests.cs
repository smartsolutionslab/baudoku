using System.Text.Json;
using AwesomeAssertions;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Infrastructure.Serialization;

namespace BauDoku.BuildingBlocks.UnitTests.Serialization;

public sealed class ValueObjectJsonConverterFactoryTests
{
    private readonly JsonSerializerOptions options;

    public ValueObjectJsonConverterFactoryTests()
    {
        options = new JsonSerializerOptions();
        options.Converters.Add(new ValueObjectJsonConverterFactory());
    }

    // Test ValueObjects for serialization tests
    private sealed record TestStringVo : ValueObject
    {
        public string Value { get; }
        private TestStringVo(string value) => Value = value;
        public static TestStringVo From(string value) => new(value);
    }

    private sealed record TestGuidVo : ValueObject
    {
        public Guid Value { get; }
        private TestGuidVo(Guid value) => Value = value;
        public static TestGuidVo From(Guid value) => new(value);
    }

    private sealed record TestIntVo : ValueObject
    {
        public int Value { get; }
        private TestIntVo(int value) => Value = value;
        public static TestIntVo From(int value) => new(value);
    }

    private sealed record TestConstructorOnlyVo(string Value) : ValueObject;

    private sealed record TestDto(TestStringVo Name, TestGuidVo Id, TestIntVo Count);

    [Fact]
    public void CanConvert_WithValueObjectType_ReturnsTrue()
    {
        var factory = new ValueObjectJsonConverterFactory();
        factory.CanConvert(typeof(TestStringVo)).Should().BeTrue();
    }

    [Fact]
    public void CanConvert_WithNonValueObjectType_ReturnsFalse()
    {
        var factory = new ValueObjectJsonConverterFactory();
        factory.CanConvert(typeof(string)).Should().BeFalse();
    }

    [Fact]
    public void CanConvert_WithAbstractValueObject_ReturnsFalse()
    {
        var factory = new ValueObjectJsonConverterFactory();
        factory.CanConvert(typeof(ValueObject)).Should().BeFalse();
    }

    [Fact]
    public void Serialize_StringValueObject_WritesRawValue()
    {
        var vo = TestStringVo.From("hello");
        var json = JsonSerializer.Serialize(vo, options);
        json.Should().Be("\"hello\"");
    }

    [Fact]
    public void Serialize_GuidValueObject_WritesRawGuid()
    {
        var id = Guid.Parse("12345678-1234-1234-1234-123456789abc");
        var vo = TestGuidVo.From(id);
        var json = JsonSerializer.Serialize(vo, options);
        json.Should().Be("\"12345678-1234-1234-1234-123456789abc\"");
    }

    [Fact]
    public void Serialize_IntValueObject_WritesRawInt()
    {
        var vo = TestIntVo.From(42);
        var json = JsonSerializer.Serialize(vo, options);
        json.Should().Be("42");
    }

    [Fact]
    public void Deserialize_StringValueObject_UsesFromMethod()
    {
        var result = JsonSerializer.Deserialize<TestStringVo>("\"world\"", options);
        result.Should().NotBeNull();
        result!.Value.Should().Be("world");
    }

    [Fact]
    public void Deserialize_GuidValueObject_UsesFromMethod()
    {
        var id = Guid.Parse("12345678-1234-1234-1234-123456789abc");
        var result = JsonSerializer.Deserialize<TestGuidVo>($"\"{id}\"", options);
        result.Should().NotBeNull();
        result!.Value.Should().Be(id);
    }

    [Fact]
    public void Deserialize_IntValueObject_UsesFromMethod()
    {
        var result = JsonSerializer.Deserialize<TestIntVo>("42", options);
        result.Should().NotBeNull();
        result!.Value.Should().Be(42);
    }

    [Fact]
    public void Deserialize_ConstructorOnly_FallsBackToConstructor()
    {
        var result = JsonSerializer.Deserialize<TestConstructorOnlyVo>("\"test\"", options);
        result.Should().NotBeNull();
        result!.Value.Should().Be("test");
    }

    [Fact]
    public void RoundTrip_Dto_PreservesValues()
    {
        var id = Guid.NewGuid();
        var dto = new TestDto(TestStringVo.From("test"), TestGuidVo.From(id), TestIntVo.From(7));

        var json = JsonSerializer.Serialize(dto, options);
        var result = JsonSerializer.Deserialize<TestDto>(json, options);

        result.Should().NotBeNull();
        result!.Name.Value.Should().Be("test");
        result.Id.Value.Should().Be(id);
        result.Count.Value.Should().Be(7);
    }
}
