using TodoApi.Models;

namespace TodoApi.Services;

public interface ITodoRepository
{
    Task<IReadOnlyList<TodoItem>> GetAllAsync(string ownerId, CancellationToken cancellationToken = default);
    Task<TodoItem?> GetAsync(Guid id, string ownerId, CancellationToken cancellationToken = default);
    Task<TodoItem> AddAsync(TodoItem item, CancellationToken cancellationToken = default);
    Task<TodoItem?> UpdateAsync(TodoItem item, CancellationToken cancellationToken = default);
    Task<TodoItem?> DeleteAsync(Guid id, string ownerId, CancellationToken cancellationToken = default);
}
