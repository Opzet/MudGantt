using MudGantt;

namespace MudGanttDemoApp.Web.GanttApi;

public sealed class GanttTaskDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public double? Progress { get; set; }
    public string? Color { get; set; }
    public List<GanttTaskLinkDto> Links { get; set; } = [];
}

public sealed class GanttTaskLinkDto
{
    public required string Id { get; set; }
    public LinkType LinkType { get; set; }
}
