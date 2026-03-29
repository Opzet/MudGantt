using MudGantt;

namespace MudGanttDemoApp.Web.Components.Pages;

internal static class GanttExampleData
{
    public static IReadOnlyList<MudGanttTask> CreateProjectTasks() =>
    [
        new MudGanttTask
        {
            Id = "planning",
            Name = "Planning",
            StartDate = new DateTimeOffset(2025, 6, 24, 8, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(2025, 6, 24, 17, 0, 0, TimeSpan.Zero),
            Progress = 1.0,
            Color = "#6c8ef5"
        },
        new MudGanttTask
        {
            Id = "backend",
            Name = "Backend implementation",
            StartDate = new DateTimeOffset(2025, 6, 25, 8, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(2025, 6, 27, 17, 0, 0, TimeSpan.Zero),
            Progress = 0.8,
            Links = [new Link("planning", LinkType.FinishToStart)],
            Color = "#5ca47a"
        },
        new MudGanttTask
        {
            Id = "frontend",
            Name = "Frontend implementation",
            StartDate = new DateTimeOffset(2025, 6, 25, 9, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(2025, 6, 28, 17, 0, 0, TimeSpan.Zero),
            Progress = 0.65,
            Links = [new Link("planning", LinkType.FinishToStart)],
            Color = "#f59e0b"
        },
        new MudGanttTask
        {
            Id = "qa",
            Name = "QA validation",
            StartDate = new DateTimeOffset(2025, 6, 30, 9, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(2025, 7, 1, 15, 0, 0, TimeSpan.Zero),
            Progress = 0.3,
            Links = [new Link("backend", LinkType.FinishToStart), new Link("frontend", LinkType.FinishToStart)],
            Color = "#e06666"
        },
        new MudGanttTask
        {
            Id = "release",
            Name = "Release",
            StartDate = new DateTimeOffset(2025, 7, 2, 10, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(2025, 7, 2, 14, 0, 0, TimeSpan.Zero),
            Progress = 0.0,
            Links = [new Link("qa", LinkType.FinishToStart)],
            Color = "#8b5cf6"
        }
    ];

    public static IReadOnlyList<MudGanttEvent> CreateProjectEvents() =>
    [
        new MudGanttEvent { Id = "kickoff", Name = "Kickoff", Date = new DateTimeOffset(2025, 6, 24, 9, 0, 0, TimeSpan.Zero) },
        new MudGanttEvent { Id = "demo", Name = "Demo", Date = new DateTimeOffset(2025, 7, 1, 16, 0, 0, TimeSpan.Zero) }
    ];

    public static IReadOnlyList<MudGanttTask> CreateDependencyTasks() =>
    [
        new MudGanttTask
        {
            Id = "design",
            Name = "UX design",
            StartDate = new DateTimeOffset(2025, 8, 1, 8, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(2025, 8, 2, 17, 0, 0, TimeSpan.Zero),
            Progress = 1.0,
            Color = "#4f46e5"
        },
        new MudGanttTask
        {
            Id = "api",
            Name = "API layer",
            StartDate = new DateTimeOffset(2025, 8, 3, 8, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(2025, 8, 5, 17, 0, 0, TimeSpan.Zero),
            Progress = 0.75,
            Links = [new Link("design", LinkType.FinishToStart)],
            Color = "#059669"
        },
        new MudGanttTask
        {
            Id = "web",
            Name = "Web UI",
            StartDate = new DateTimeOffset(2025, 8, 3, 9, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(2025, 8, 6, 17, 0, 0, TimeSpan.Zero),
            Progress = 0.55,
            Links = [new Link("design", LinkType.FinishToStart)],
            Color = "#f97316"
        },
        new MudGanttTask
        {
            Id = "integration",
            Name = "Integration",
            StartDate = new DateTimeOffset(2025, 8, 7, 8, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(2025, 8, 7, 18, 0, 0, TimeSpan.Zero),
            Progress = 0.25,
            Links = [new Link("api", LinkType.FinishToStart), new Link("web", LinkType.FinishToStart)],
            Color = "#dc2626"
        }
    ];

    public static IReadOnlyList<MudGanttTask> CreateOperationsTasks() =>
        Enumerable.Range(0, 12)
            .Select(i => new MudGanttTask
            {
                Id = $"ops-{i + 1}",
                Name = $"Support lane {i + 1}",
                StartDate = new DateTimeOffset(2025, 9, 1, 6 + (i % 4), (i * 10) % 60, 0, TimeSpan.Zero),
                EndDate = new DateTimeOffset(2025, 9, 1, 12 + (i % 5), ((i * 10) + 30) % 60, 0, TimeSpan.Zero),
                Progress = null,
                Color = i % 2 == 0 ? "#9ec9d1" : "#b7d8b0"
            })
            .ToList();

    public static IReadOnlyList<MudGanttTask> CreateBaselineCriticalTasks() =>
    [
        new MudGanttTask
        {
            Id = "cp-1",
            Name = "Order confirmation",
            StartDate = new DateTimeOffset(2025, 11, 3, 8, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(2025, 11, 3, 16, 0, 0, TimeSpan.Zero),
            BaselineStartDate = new DateTimeOffset(2025, 11, 3, 8, 0, 0, TimeSpan.Zero),
            BaselineEndDate = new DateTimeOffset(2025, 11, 3, 12, 0, 0, TimeSpan.Zero),
            Progress = 1.0,
            IsCritical = true,
            RightLabel = "Critical",
            Color = "#2563eb",
            BaselineColor = "#94a3b8"
        },
        new MudGanttTask
        {
            Id = "cp-2",
            Name = "Material allocation",
            StartDate = new DateTimeOffset(2025, 11, 4, 8, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(2025, 11, 5, 17, 0, 0, TimeSpan.Zero),
            BaselineStartDate = new DateTimeOffset(2025, 11, 4, 8, 0, 0, TimeSpan.Zero),
            BaselineEndDate = new DateTimeOffset(2025, 11, 5, 12, 0, 0, TimeSpan.Zero),
            Progress = 0.85,
            IsCritical = true,
            RightLabel = "+5h slip",
            Links = [new Link("cp-1", LinkType.FinishToStart)],
            Color = "#0f9d58",
            BaselineColor = "#94a3b8"
        },
        new MudGanttTask
        {
            Id = "cp-3",
            Name = "CNC cell 4 run",
            StartDate = new DateTimeOffset(2025, 11, 6, 6, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(2025, 11, 7, 14, 0, 0, TimeSpan.Zero),
            BaselineStartDate = new DateTimeOffset(2025, 11, 6, 6, 0, 0, TimeSpan.Zero),
            BaselineEndDate = new DateTimeOffset(2025, 11, 7, 10, 0, 0, TimeSpan.Zero),
            Progress = 0.55,
            IsCritical = true,
            RightLabel = "Critical machine",
            Links = [new Link("cp-2", LinkType.FinishToStart)],
            Color = "#ef6c00",
            BaselineColor = "#94a3b8"
        },
        new MudGanttTask
        {
            Id = "cp-4",
            Name = "Paint booth",
            StartDate = new DateTimeOffset(2025, 11, 6, 8, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(2025, 11, 6, 18, 0, 0, TimeSpan.Zero),
            BaselineStartDate = new DateTimeOffset(2025, 11, 7, 8, 0, 0, TimeSpan.Zero),
            BaselineEndDate = new DateTimeOffset(2025, 11, 7, 18, 0, 0, TimeSpan.Zero),
            Progress = 0.4,
            IsCritical = false,
            RightLabel = "Float available",
            Links = [new Link("cp-2", LinkType.FinishToStart)],
            Color = "#8e24aa",
            BaselineColor = "#94a3b8"
        },
        new MudGanttTask
        {
            Id = "cp-5",
            Name = "Final assembly",
            StartDate = new DateTimeOffset(2025, 11, 10, 8, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(2025, 11, 11, 15, 0, 0, TimeSpan.Zero),
            BaselineStartDate = new DateTimeOffset(2025, 11, 10, 8, 0, 0, TimeSpan.Zero),
            BaselineEndDate = new DateTimeOffset(2025, 11, 11, 12, 0, 0, TimeSpan.Zero),
            Progress = 0.1,
            IsCritical = true,
            RightLabel = "Ship gate",
            Links = [new Link("cp-3", LinkType.FinishToStart), new Link("cp-4", LinkType.FinishToStart)],
            Color = "#d32f2f",
            BaselineColor = "#94a3b8"
        }
    ];
}
