using System.Reflection;

namespace BauDoku.Projects.Application;

public static class DependencyInjection
{
    public static Assembly Assembly => typeof(DependencyInjection).Assembly;
}
