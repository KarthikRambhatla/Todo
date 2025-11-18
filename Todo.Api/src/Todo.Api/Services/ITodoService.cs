using System;
using System.Collections.Generic;
using Todo.Api.Models;

namespace Todo.Api.Services;

public interface ITodoService
{
    Task<IReadOnlyList<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TodoItem?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TodoItem?> AddAsync(string title, CancellationToken cancellationToken = default);
    Task<bool> MarkDoneAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TodoItem?> UpdateAsync(TodoItem item, CancellationToken cancellationToken = default);
    Task<TodoItem?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
