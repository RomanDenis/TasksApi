using System.ComponentModel.DataAnnotations;

namespace TasksApi.Models;

public class TaskEntity
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(2000)]
    public string? Description { get; set; }
    
    public TaskStatus Status { get; set; } = TaskStatus.Todo;
    
    public Priority Priority { get; set; } = Priority.Medium;
    
    public DateTime? DueDate { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    [Required]
    [MaxLength(256)]
    public string CreatedBy { get; set; } = string.Empty;
    
    public bool IsDeleted { get; set; } = false;
}

public enum TaskStatus
{
    Todo = 0,
    InProgress = 1,
    Done = 2
}

public enum Priority
{
    Low = 0,
    Medium = 1,
    High = 2
}