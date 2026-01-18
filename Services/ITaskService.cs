using TasksApi.Models;

namespace TasksApi.Services;

public interface ITaskService
{
    Task<TaskResponseDto?> GetByIdAsync(Guid id);
    Task<List<TaskResponseDto>> GetAllAsync();
    Task<List<TaskResponseDto>> GetByStatusAsync(Models.TaskStatus status);
    Task<TaskResponseDto> CreateAsync(TaskCreateDto dto, string userId);
    Task<TaskResponseDto> UpdateAsync(Guid id, TaskUpdateDto dto);
    Task DeleteAsync(Guid id);
}