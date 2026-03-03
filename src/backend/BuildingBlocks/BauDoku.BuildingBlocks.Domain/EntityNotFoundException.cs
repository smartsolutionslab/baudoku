namespace SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;

public static class EntityNotFoundException
{
    public static T OrNotFound<T>(this T? entity, string entityName, object id) where T : class
        => entity ?? throw new KeyNotFoundException($"{entityName} mit ID '{id}' wurde nicht gefunden.");
}
