namespace MudGantt
{
    /// <summary>
    /// Represents a single task in a Gantt chart, including scheduling, progress, color, and dependencies.
    /// </summary>
    public class MudGanttTask
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
        /// Gets or sets the start date of the task.
        /// </summary>
        public DateTimeOffset? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date of the task.
        /// </summary>
        public DateTimeOffset? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the progress of the task, as a value between 0.0 and 1.0.
        /// If null the task progress cannot be changed.
        /// </summary>
        public double? Progress { get; set; }

        /// <summary>
        /// Gets or sets the color of the task bar (hex or CSS color string).
        /// If null, the color specified for the MudGanttChart will be used (e.g. MudColor.Primary).
        /// </summary>
        public string? Color { get; set; }

        /// <summary>
        /// Gets or sets the list of links with task IDs and type.
        /// </summary>
        public Link[] Links { get; set; } = [];

        /// <summary>
        /// Gets the color used for the progress bar, derived from <see cref="Color"/> and lightened.
        /// </summary>
        public string? ProgressColor
        {
            get
            {
                if(Color is not null)
                {
                    if(MudColor.TryParse(Color, out var color))
                    {
                        var lightenedColor = color.ColorLighten(0.2);
                        return lightenedColor.ToString(MudColorOutputFormats.Hex);
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets estimated hours for the task. Used to compare progress pacing against tracked effort.
        /// </summary>
        public double? EstimatedHours { get; set; }

        /// <summary>
        /// Gets or sets actual tracked hours for the task.
        /// </summary>
        public double? ActualHours { get; set; }

        /// <summary>
        /// Gets ratio of actual-vs-estimated hours for rendering the hourly-progress overlay.
        /// </summary>
        public double? ActualProgressRatio
        {
            get
            {
                if (EstimatedHours is null or <= 0 || ActualHours is null)
                {
                    return null;
                }

                return Math.Max(0, ActualHours.Value / EstimatedHours.Value);
            }
        }

        /// <summary>
        /// Gets color for hourly-progress overlay. Green if within estimate, red if over estimate.
        /// </summary>
        public string? ActualProgressColor
        {
            get
            {
                if (EstimatedHours is null or <= 0 || ActualHours is null)
                {
                    return null;
                }

                return ActualHours <= EstimatedHours
                    ? "var(--mud-palette-success)"
                    : "var(--mud-palette-error)";
            }
        }

        /// <summary>
        /// Optional tooltip text shown for the task bar in the chart.
        /// </summary>
        public string? Tooltip { get; set; }
    }
}
