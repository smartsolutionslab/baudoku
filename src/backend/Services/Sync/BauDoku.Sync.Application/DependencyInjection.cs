using System.Reflection;

namespace BauDoku.Sync.Application;

public static class DependencyInjection
{
    public static Assembly Assembly => typeof(DependencyInjection).Assembly;
}
