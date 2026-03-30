using MudBlazor;

namespace MudGantt;

/// <summary>
/// Shared row and header sizing for Gantt-aligned UI surfaces.
/// </summary>
public sealed record MudGanttLayoutMetrics(int AxisHeight, int TaskHeight, int TaskSpacing)
{
    /// <summary>
    /// Total row height including spacing between tasks.
    /// </summary>
    public int RowHeight => TaskHeight + TaskSpacing;

    /// <summary>
    /// Resolves the default layout metrics used by the chart for the current density and size.
    /// </summary>
    public static MudGanttLayoutMetrics From(bool dense, Size size)
    {
        var taskSpacing = dense ? 2 : 10;

        return size switch
        {
            Size.Small => new MudGanttLayoutMetrics(40, 35, taskSpacing),
            Size.Large => new MudGanttLayoutMetrics(60, 60, taskSpacing),
            _ => new MudGanttLayoutMetrics(50, 50, taskSpacing)
        };
    }
}
