using System;
using TodoApi.Models;

namespace TodoApi.Dtos;

public record TodoItemDto(
        Guid Id,
        string Title,
        bool IsDone,
        DateTimeOffset CreatedAt,
        DateTimeOffset? UpdatedAt)
{
    public static TodoItemDto From(TodoItem item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        return new TodoItemDto(
            item.Id,
            item.Title ?? string.Empty,
            item.IsDone,
            item.CreatedAt,
            item.UpdatedAt);
    }
}
