using Todo.Api.Models;

namespace Todo.Api.Services;

public class InMemoryTodoRepository(ILogger<InMemoryTodoRepository> logger) : ITodoRepository
{
    private readonly Dictionary<Guid, TodoItem> _itemsById = [];
    private readonly Dictionary<string,HashSet<Guid>> _itemsByOwner = [];

    public Task<IReadOnlyList<TodoItem>> GetAllAsync(string ownerId, CancellationToken cancellationToken = default)
    {
        if (!_itemsByOwner.TryGetValue(ownerId, out var guids))
        {
            logger.LogDebug("No items found for owner {OwnerId}", ownerId);
            return Task.FromResult<IReadOnlyList<TodoItem>>([]);
        }
        // If I store directly items, i can give GetAll in O(1) time, but when updating and deleting to be consistent is a bit hard
        // so storing it in hashset of guids and getting items from _itemsById. Now this becomes O(n) but may be acceptable for small n.

        var items = guids.Select(id => _itemsById[id]).ToList(); 

        return Task.FromResult<IReadOnlyList<TodoItem>>(items);
    }

    public Task<TodoItem?> GetAsync(Guid id, string ownerId, CancellationToken cancellationToken = default)
    {
        if (_itemsById.TryGetValue(id, out var item))
        {
           if (item.OwnerId != ownerId)
            {
                logger.LogWarning("Item {ItemId} does not belong to owner {OwnerId}", id, ownerId);
                return Task.FromResult<TodoItem?>(null);
            }
           return Task.FromResult<TodoItem?>(item);
        }

        return Task.FromResult<TodoItem?>(null);
    }

    public Task<TodoItem> AddAsync(TodoItem item, CancellationToken cancellationToken = default)
    {
        if (_itemsById.TryAdd(item.Id, item))
        {
            if (!_itemsByOwner.TryGetValue(item.OwnerId, out var set))
            {
                set = [];
                _itemsByOwner[item.OwnerId] = set;
            }
            set.Add(item.Id);
        }
        else
        {
            logger.LogInformation("Item {ItemId} already exists", item.Id);
        }

        return Task.FromResult(item);
    }

    public Task<TodoItem?> UpdateAsync(TodoItem item, CancellationToken cancellationToken = default)
    {
        if (!_itemsById.TryGetValue(item.Id, out var existing))
        {
            logger.LogWarning("Item {ItemId} not found for update", item.Id);
            return Task.FromResult<TodoItem?>(null);
        }

        if (existing.OwnerId != item.OwnerId)
        {
            logger.LogWarning("Item {ItemId} does not belong to owner {OwnerId}", item.Id, item.OwnerId);
            return Task.FromResult<TodoItem?>(null);
        }

        item.UpdatedAt = DateTimeOffset.UtcNow;
        // There could be race conditions like while we reach here, the item is removed. 
        // Note that since using _itemsByOwner only stores Guid, we need not do anything there in an update to item.
        _itemsById[item.Id] = item;

        return Task.FromResult<TodoItem?>(item);
    }

    public Task<TodoItem?> DeleteAsync(Guid id, string ownerId, CancellationToken cancellationToken = default)
    {
        if(!_itemsById.TryGetValue(id, out var item))
        {
            logger.LogInformation("Item {ItemId} not found for deletion", id);
            return Task.FromResult<TodoItem?>(null);
        }

        if(item?.OwnerId != ownerId)
        {
            logger.LogWarning("Item {ItemId} does not belong to owner {OwnerId}", id, ownerId);
            return Task.FromResult<TodoItem?>(null);
        }

        //just in case, we want to handle soft deletes and do not want to immediately and provide an undo button for user
        item.DeletedAt = DateTimeOffset.UtcNow;

        _itemsById.Remove(id);
        if(_itemsByOwner.TryGetValue(ownerId, out var set))
        {
            set.Remove(id);
            if(set.Count == 0)
            {
                _itemsByOwner.Remove(ownerId);
            }
        }

        return Task.FromResult<TodoItem?>(item);
    }
}
