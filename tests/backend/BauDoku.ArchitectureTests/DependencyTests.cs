using System.Reflection;
using AwesomeAssertions;

namespace BauDoku.ArchitectureTests;

public sealed class DependencyTests
{
    private static readonly Assembly BuildingBlocksDomain =
        typeof(BauDoku.BuildingBlocks.Domain.ValueObject).Assembly;

    private static readonly Assembly BuildingBlocksApplication =
        typeof(BauDoku.BuildingBlocks.Application.DependencyInjection).Assembly;

    private static readonly Assembly BuildingBlocksInfrastructure =
        typeof(BauDoku.BuildingBlocks.Infrastructure.DependencyInjection).Assembly;

    private static Assembly LoadAssembly(string name) => Assembly.Load(name);

    [Fact]
    public void Domain_ShouldNotDependOn_Application()
    {
        var refs = BuildingBlocksDomain.GetReferencedAssemblies().Select(a => a.Name!).ToList();
        refs.Should().NotContain(name => name.Contains("Application"),
            "Domain layer must not depend on Application layer");
    }

    [Fact]
    public void Domain_ShouldNotDependOn_Infrastructure()
    {
        var refs = BuildingBlocksDomain.GetReferencedAssemblies().Select(a => a.Name!).ToList();
        refs.Should().NotContain(name => name.Contains("Infrastructure"),
            "Domain layer must not depend on Infrastructure layer");
    }

    [Fact]
    public void Domain_ShouldNotDependOn_Api()
    {
        var refs = BuildingBlocksDomain.GetReferencedAssemblies().Select(a => a.Name!).ToList();
        refs.Should().NotContain(name => name.Contains(".Api"),
            "Domain layer must not depend on Api layer");
    }

    [Fact]
    public void Application_ShouldNotDependOn_Infrastructure()
    {
        var refs = BuildingBlocksApplication.GetReferencedAssemblies().Select(a => a.Name!).ToList();
        refs.Should().NotContain(name => name.Contains("Infrastructure"),
            "Application layer must not depend on Infrastructure layer");
    }

    [Fact]
    public void Application_ShouldNotDependOn_Api()
    {
        var refs = BuildingBlocksApplication.GetReferencedAssemblies().Select(a => a.Name!).ToList();
        refs.Should().NotContain(name => name.Contains(".Api"),
            "Application layer must not depend on Api layer");
    }

    [Theory]
    [InlineData("BauDoku.Projects.Domain")]
    [InlineData("BauDoku.Documentation.Domain")]
    [InlineData("BauDoku.Sync.Domain")]
    public void ServiceDomain_ShouldOnlyDependOn_BuildingBlocksDomain(string assemblyName)
    {
        var assembly = LoadAssembly(assemblyName);

        var baudokuRefs = assembly.GetReferencedAssemblies()
            .Where(a => a.Name!.StartsWith("BauDoku"))
            .Select(a => a.Name!)
            .ToList();

        baudokuRefs.Should().OnlyContain(
            name => name == "BauDoku.BuildingBlocks.Domain",
            "Service Domain should only reference BuildingBlocks.Domain");
    }

    [Theory]
    [InlineData("BauDoku.Projects.Application")]
    [InlineData("BauDoku.Documentation.Application")]
    [InlineData("BauDoku.Sync.Application")]
    public void ServiceApplication_ShouldNotDependOn_Infrastructure(string assemblyName)
    {
        var assembly = LoadAssembly(assemblyName);

        var refs = assembly.GetReferencedAssemblies().Select(a => a.Name!).ToList();
        refs.Should().NotContain(
            name => name.Contains("Infrastructure"),
            "Service Application should not reference Infrastructure");
    }

    [Theory]
    [InlineData("BauDoku.Projects.Application", "Documentation", "Sync")]
    [InlineData("BauDoku.Documentation.Application", "Projects", "Sync")]
    [InlineData("BauDoku.Sync.Application", "Projects", "Documentation")]
    public void ServiceApplication_ShouldNotReferenceCrossBoundedContext(
        string assemblyName, string otherBc1, string otherBc2)
    {
        var assembly = LoadAssembly(assemblyName);

        var refs = assembly.GetReferencedAssemblies().Select(a => a.Name!).ToList();
        refs.Should().NotContain(
            name => name.Contains($".{otherBc1}.") || name.Contains($".{otherBc2}."),
            $"Application must not reference other bounded contexts ({otherBc1}, {otherBc2})");
    }

    [Theory]
    [InlineData("BauDoku.Projects.Domain")]
    [InlineData("BauDoku.Documentation.Domain")]
    [InlineData("BauDoku.Sync.Domain")]
    public void ServiceDomain_ShouldNotReferenceEntityFramework(string assemblyName)
    {
        var assembly = LoadAssembly(assemblyName);

        var refs = assembly.GetReferencedAssemblies().Select(a => a.Name!).ToList();
        refs.Should().NotContain(
            name => name.Contains("EntityFramework"),
            "Domain must not reference Entity Framework");
    }

    [Theory]
    [InlineData("BauDoku.Projects.Api", "Documentation", "Sync")]
    [InlineData("BauDoku.Documentation.Api", "Projects", "Sync")]
    [InlineData("BauDoku.Sync.Api", "Projects", "Documentation")]
    public void ServiceApi_ShouldNotReferenceCrossBoundedContextApi(
        string assemblyName, string otherBc1, string otherBc2)
    {
        var assembly = LoadAssembly(assemblyName);

        var refs = assembly.GetReferencedAssemblies().Select(a => a.Name!).ToList();
        refs.Should().NotContain(
            name => name.Contains($"{otherBc1}.Api") || name.Contains($"{otherBc2}.Api"),
            $"Api must not reference other Api projects ({otherBc1}, {otherBc2})");
    }

    [Theory]
    [InlineData("BauDoku.Projects.Domain")]
    [InlineData("BauDoku.Documentation.Domain")]
    [InlineData("BauDoku.Sync.Domain")]
    public void AllValueObjects_ShouldImplement_IValueObject(string assemblyName)
    {
        var assembly = LoadAssembly(assemblyName);

        var valueObjectBase = typeof(BauDoku.BuildingBlocks.Domain.ValueObject);
        var iValueObject = typeof(BauDoku.BuildingBlocks.Domain.IValueObject);

        var valueObjectTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && valueObjectBase.IsAssignableFrom(t))
            .ToList();

        valueObjectTypes.Should().NotBeEmpty(
            $"Assembly {assemblyName} should contain at least one ValueObject");

        foreach (var voType in valueObjectTypes)
        {
            iValueObject.IsAssignableFrom(voType).Should().BeTrue(
                $"{voType.Name} inherits from ValueObject and should implement IValueObject");
        }
    }
}
