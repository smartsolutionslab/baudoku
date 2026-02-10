using System.Reflection;

namespace BauDoku.Documentation.Application;

public static class DependencyInjection
{
    public static Assembly Assembly => typeof(DependencyInjection).Assembly;
}
