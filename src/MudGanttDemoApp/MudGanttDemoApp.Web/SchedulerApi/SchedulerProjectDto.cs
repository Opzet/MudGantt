using MudGantt;

namespace MudGanttDemoApp.Web.SchedulerApi;

public sealed class SchedulerProjectDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public List<SchedulerTaskItemDto> Tasks { get; set; } = [];
    public List<SchedulerResourceDto> Resources { get; set; } = [];
    public List<SchedulerAssignmentDto> Assignments { get; set; } = [];
    public List<WorkingCalendarDayDto> Calendar { get; set; } = [];
}

public sealed class SchedulerTaskItemDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public double? Progress { get; set; }
    public string? Color { get; set; }
    public List<SchedulerTaskLinkDto> Links { get; set; } = [];
    public double EffortHours { get; set; }
    public string? Phase { get; set; }
}

public sealed class SchedulerTaskLinkDto
{
    public required string Id { get; set; }
    public LinkType LinkType { get; set; }
}

public sealed class SchedulerResourceDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Role { get; set; }
    public double CapacityHoursPerDay { get; set; }
}

public sealed class SchedulerAssignmentDto
{
    public required string TaskId { get; set; }
    public required string ResourceId { get; set; }
    public int AllocationPercent { get; set; }
}

public sealed class WorkingCalendarDayDto
{
    public required string Day { get; set; }
    public required string WorkingHours { get; set; }
}
