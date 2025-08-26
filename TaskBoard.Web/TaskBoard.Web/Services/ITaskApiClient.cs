using TaskBoard.Web.Contracts;

namespace TaskBoard.Web.Services
{
    public interface ITaskApiClient
    {
        Task<IReadOnlyList<TaskItemDto>> GetTasksAsync(int skip = 0, int take = 50, CancellationToken ct = default);
        Task<TaskItemDto?> GetTaskAsync(int id, CancellationToken ct = default);
        Task<TaskItemDto?> CreateAsync(string title, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, string title, bool isDone, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
