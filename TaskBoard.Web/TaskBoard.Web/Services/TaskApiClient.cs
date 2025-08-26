using System.Net;
using TaskBoard.Web.Contracts;

namespace TaskBoard.Web.Services
{
    public class TaskApiClient : ITaskApiClient
    {
        private readonly HttpClient _http;
        public TaskApiClient(HttpClient http) => _http = http;

        public async Task<IReadOnlyList<TaskItemDto>> GetTasksAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var data = await _http.GetFromJsonAsync<IReadOnlyList<TaskItemDto>>($"/api/tasks?skip={skip}&take={take}", ct);
            return data ?? Array.Empty<TaskItemDto>();
        }

        public async Task<TaskItemDto?> GetTaskAsync(int id, CancellationToken ct = default)
            => await _http.GetFromJsonAsync<TaskItemDto>($"/api/tasks/{id}", ct);

        public async Task<TaskItemDto?> CreateAsync(string title, CancellationToken ct = default)
        {
            var res = await _http.PostAsJsonAsync("/api/tasks", new { title }, ct);
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<TaskItemDto>(cancellationToken: ct);
        }

        public async Task<bool> UpdateAsync(int id, string title, bool isDone, CancellationToken ct = default)
        {
            var res = await _http.PutAsJsonAsync($"/api/tasks/{id}", new { id, title, isDone }, ct);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var res = await _http.DeleteAsync($"/api/tasks/{id}", ct);
            return res.StatusCode is HttpStatusCode.NoContent;
        }
    }
}
