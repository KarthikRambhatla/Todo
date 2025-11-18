using System;

namespace Todo.Api.Models
{
    public class TodoItem
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string OwnerId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public bool IsDone { get; set; } = false;
        public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset DeletedAt { get; set; }
    }
}
