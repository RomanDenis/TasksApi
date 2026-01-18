using Microsoft.EntityFrameworkCore;
using Polly.Retry;
using TasksApi.Data;
using TasksApi.Models;

namespace TasksApi.Services;

public class TaskService : ITaskService
{
    private readonly AppDbContext _context;
    private readonly ILogger<TaskService> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public TaskService(AppDbContext context, ILogger<TaskService> logger)
    {
        _context = context;
        _logger = logger;
        _retryPolicy = PollyPolicies.GetRetryPolicy();
    }

    public async Task<TaskResponseDto?> GetByIdAsync(Guid id)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var task = await _context.Tasks
                .Where(t => !t.IsDeleted && t.Id == id)
                .FirstOrDefaultAsync();

            return task == null ? null : MapToDto(task);
        });
    }

    public async Task<List<TaskResponseDto>> GetAllAsync()
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var tasks = await _context.Tasks
                .Where(t => !t.IsDeleted)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return tasks.Select(MapToDto).ToList();
        });
    }

    public async Task<List<TaskResponseDto>> GetByStatusAsync(Models.TaskStatus status)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var tasks = await _context.Tasks
                .Where(t => !t.IsDeleted && t.Status == status)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return tasks.Select(MapToDto).ToList();
        });
    }

    public async Task<TaskResponseDto> CreateAsync(TaskCreateDto dto, string userId)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var entity = new TaskEntity
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                Status = dto.Status,
                Priority = dto.Priority,
                DueDate = dto.DueDate,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(entity);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Task created: {TaskId} by {UserId}", entity.Id, userId);

            return MapToDto(entity);
        });
    }

    public async Task<TaskResponseDto> UpdateAsync(Guid id, TaskUpdateDto dto)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

            if (task == null)
                throw new KeyNotFoundException($"Task with ID {id} not found");

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Status = dto.Status;
            task.Priority = dto.Priority;
            task.DueDate = dto.DueDate;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Task updated: {TaskId}", id);

            return MapToDto(task);
        });
    }

    public async Task DeleteAsync(Guid id)
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

            if (task == null)
                throw new KeyNotFoundException($"Task with ID {id} not found");

            task.IsDeleted = true;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Task deleted: {TaskId}", id);
        });
    }

    private static TaskResponseDto MapToDto(TaskEntity entity)
    {
        return new TaskResponseDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            Status = entity.Status,
            Priority = entity.Priority,
            DueDate = entity.DueDate,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            CreatedBy = entity.CreatedBy
        };
    }
}