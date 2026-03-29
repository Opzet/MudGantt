namespace MudGantt;

/// <summary>
/// Represents an event which has a datetime, but no range. For example a milestone.
/// </summary>
public class MudGanttEvent
{
    /// <summary>
    /// Gets or sets the unique identifier for the task.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Gets or sets the display name of the task.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Date
    /// </summary>
    public required DateTimeOffset Date { get; set; }

    /// <summary>
    /// Optional end date for rendering a time-range overlay instead of a single marker.
    /// </summary>
    public DateTimeOffset? EndDate { get; set; }

    /// <summary>
    /// Optional color for event marker or time-range overlay.
    /// </summary>
    public string? Color { get; set; }
}
