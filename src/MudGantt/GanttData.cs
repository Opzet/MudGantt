
namespace MudGantt
{
    /// <summary>
    /// Internal data passed with JS interop for rendering the Gantt chart.
    /// </summary>
    internal class GanttData
    {
        /// <summary>
        /// Tasks
        /// </summary>
        public required IReadOnlyList<MudGanttTask> Items { get; set; }

        /// <summary>
        /// Events, can be null
        /// </summary>
        public IReadOnlyList<MudGanttEvent>? Events { get; set; }

        /// <summary>
        /// Optional range overlays rendered either on a task row or across the chart.
        /// </summary>
        public IReadOnlyList<MudGanttRangeOverlay>? RangeOverlays { get; set; }

        /// <summary>
        /// If true, the tasks cannot be moved and progress cannot be changed
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Dense layout?
        /// </summary>
        public bool Dense { get; set; }

        /// <summary>
        /// Size of chart items
        /// </summary>
        public Size Size { get; set; }

        /// <summary>
        /// Optional selected task id for highlighting and scroll-into-view.
        /// </summary>
        public string? SelectedTaskId { get; set; }

        /// <summary>
        /// If true, the selected task should be scrolled into view when selection changes.
        /// </summary>
        public bool ScrollSelectedIntoView { get; set; } = true;

        /// <summary>
        /// Additional task ids to highlight alongside the selected task.
        /// </summary>
        public IReadOnlyList<string>? HighlightedTaskIds { get; set; }

        /// <summary>
        /// If true, tasks outside the selected/highlighted set are visually dimmed.
        /// </summary>
        public bool DimNonHighlighted { get; set; }
    }
}
