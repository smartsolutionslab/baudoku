namespace BauDoku.Projects.Application.Contracts;

public interface IProjectCountProvider
{
    Task<int> GetActiveCountAsync(CancellationToken ct = default);
}
