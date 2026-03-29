namespace MudGantt;

/// <summary>
/// Represents an overlay range rendered on either a specific task row or across the whole chart timeline.
/// </summary>
public sealed class MudGanttRangeOverlay
{
    public required string Id { get; set; }
    public string? TaskId { get; set; }
    public required DateTimeOffset StartDate { get; set; }
    public required DateTimeOffset EndDate { get; set; }
    public string? Color { get; set; }
    public string? Label { get; set; }
}
