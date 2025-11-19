using Todo.Api.Models;

namespace Todo.Api.Services;

public class TodoService(ITodoRepository repository, IUserContext userContext, ILogger<TodoService> logger) : ITodoService
{
    string GetOwnerId() => userContext.GetCurrentUserId() ?? throw new InvalidOperationException("No authenticated user.");

    public async Task<IReadOnlyList<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await repository.GetAllAsync(GetOwnerId(), cancellationToken);
    }

    public async Task<TodoItem?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await repository.GetAsync(id, GetOwnerId(), cancellationToken);
    }

    public async Task<TodoItem?> AddAsync(string title, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            logger.LogWarning("Attempted to add a TodoItem with an invalid title.");
            return null;
        }

        var item = new TodoItem
        {
            Title = title,
            OwnerId = GetOwnerId(),
        };

        return await repository.AddAsync(item, cancellationToken);
    }

    public async Task<TodoItem?> UpdateAsync(TodoItem item, CancellationToken cancellationToken = default)
    {
        var ownerId = GetOwnerId();
        if (item.OwnerId != ownerId)
        {
            logger.LogWarning("User {OwnerId} attempted to update TodoItem {ItemId} owned by {ItemOwnerId}.", ownerId, item.Id, item.OwnerId);
            return null;
        }

        item.UpdatedAt = DateTimeOffset.UtcNow;
        return await repository.UpdateAsync(item, cancellationToken);
    }

    public async Task<TodoItem?> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var ownerId = GetOwnerId();
        var item = await repository.GetAsync(id, ownerId, cancellationToken);
        if (item == null)
        {
            logger.LogWarning("User {OwnerId} attempted to delete non-existent TodoItem {ItemId}.", ownerId, id);
            return null;
        }
        if (item.OwnerId != ownerId)
        {
            logger.LogWarning("User {OwnerId} attempted to delete TodoItem {ItemId} owned by {ItemOwnerId}.", ownerId, id, item.OwnerId);
            return null;
        }
        item.DeletedAt = DateTimeOffset.UtcNow;
        return await repository.DeleteAsync(id, ownerId, cancellationToken);
    }
   
    public async Task<bool> MarkDoneAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await this.GetAsync(id, cancellationToken);
        if (item == null)
        {
            logger.LogWarning("TodoItem {ItemId} not found for marking as done.", id);
            return false;
        }
        item.IsDone = true;
        await this.UpdateAsync(item, cancellationToken);
        return true;

    }
}
