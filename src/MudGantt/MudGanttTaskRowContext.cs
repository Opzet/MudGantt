namespace MudGantt;

/// <summary>
/// Context supplied to synchronized task-list row templates.
/// </summary>
/// <param name="Task">The task represented by the row.</param>
/// <param name="Index">The zero-based row index.</param>
/// <param name="IsSelected">Indicates whether the row is currently selected.</param>
public sealed record MudGanttTaskRowContext(MudGanttTask Task, int Index, bool IsSelected);
