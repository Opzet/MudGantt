namespace MudGanttDemoApp.Web.GanttApi;

public interface IGanttTaskStore
{
    Task<IReadOnlyList<GanttTaskDto>> GetAllAsync(string? search, CancellationToken cancellationToken);
    Task<GanttTaskDto?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<GanttTaskDto> CreateAsync(GanttTaskDto dto, CancellationToken cancellationToken);
    Task<GanttTaskDto?> UpdateAsync(string id, GanttTaskDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken);
}
