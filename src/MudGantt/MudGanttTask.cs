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

        /// <summary>
        /// Optional baseline start date for comparing plan vs actual schedule.
        /// </summary>
        public DateTimeOffset? BaselineStartDate { get; set; }

        /// <summary>
        /// Optional baseline finish date for comparing plan vs actual schedule.
        /// </summary>
        public DateTimeOffset? BaselineEndDate { get; set; }

        /// <summary>
        /// Optional color for baseline rendering.
        /// </summary>
        public string? BaselineColor { get; set; }

        /// <summary>
        /// Indicates whether the task belongs to the critical path.
        /// </summary>
        public bool IsCritical { get; set; }

        /// <summary>
        /// Optional label rendered to the right of the task bar.
        /// </summary>
        public string? RightLabel { get; set; }

        /// <summary>WBS hierarchy number (e.g., "1", "1.1", "1.2").</summary>
        public string? WbsNumber { get; set; }

        /// <summary>Task workflow status. Common values: "Not Started", "In Progress", "On Hold", "Complete", "At Risk".</summary>
        public string? Status { get; set; }

        /// <summary>Assignee display name shown in the task list.</summary>
        public string? AssigneeName { get; set; }

        /// <summary>Assignee initials for avatar rendering (e.g., "JD").</summary>
        public string? AssigneeInitials { get; set; }

        /// <summary>Background colour for the assignee avatar (CSS colour string).</summary>
        public string? AssigneeAvatarColor { get; set; }

        /// <summary>Parent task id for hierarchy; null indicates a top-level task.</summary>
        public string? ParentId { get; set; }

        /// <summary>If true, this row is a phase/group header that may contain child tasks.</summary>
        public bool IsPhase { get; set; }

        /// <summary>If true and IsPhase is true, the phase's child rows are hidden in the task list.</summary>
        public bool IsCollapsed { get; set; }

        /// <summary>Visual indent depth for hierarchy display (0 = root level).</summary>
        public int IndentLevel { get; set; }
    }
}
