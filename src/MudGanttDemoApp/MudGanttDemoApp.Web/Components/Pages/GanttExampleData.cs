using MudGantt;

namespace MudGanttDemoApp.Web.Components.Pages;

internal static class GanttExampleData
{
    public static IReadOnlyList<MudGanttTask> CreateProjectTasks()
    {
        var anchor = AnchorDay().AddDays(-1);

        return
        [
            new MudGanttTask
            {
                Id = "planning",
                Name = "Planning",
                StartDate = At(anchor, 0, 8),
                EndDate = At(anchor, 0, 17),
                Progress = 1.0,
                Color = "#6c8ef5"
            },
            new MudGanttTask
            {
                Id = "backend",
                Name = "Backend implementation",
                StartDate = At(anchor, 1, 8),
                EndDate = At(anchor, 3, 17),
                Progress = 0.8,
                Links = [new Link("planning", LinkType.FinishToStart)],
                Color = "#5ca47a"
            },
            new MudGanttTask
            {
                Id = "frontend",
                Name = "Frontend implementation",
                StartDate = At(anchor, 1, 9),
                EndDate = At(anchor, 4, 17),
                Progress = 0.65,
                Links = [new Link("planning", LinkType.FinishToStart)],
                Color = "#f59e0b"
            },
            new MudGanttTask
            {
                Id = "qa",
                Name = "QA validation",
                StartDate = At(anchor, 6, 9),
                EndDate = At(anchor, 7, 15),
                Progress = 0.3,
                Links = [new Link("backend", LinkType.FinishToStart), new Link("frontend", LinkType.FinishToStart)],
                Color = "#e06666"
            },
            new MudGanttTask
            {
                Id = "release",
                Name = "Release",
                StartDate = At(anchor, 8, 10),
                EndDate = At(anchor, 8, 14),
                Progress = 0.0,
                Links = [new Link("qa", LinkType.FinishToStart)],
                Color = "#8b5cf6"
            }
        ];
    }

    public static IReadOnlyList<MudGanttEvent> CreateProjectEvents()
    {
        var anchor = AnchorDay().AddDays(-1);

        return
        [
            new MudGanttEvent { Id = "kickoff", Name = "Kickoff", Date = At(anchor, 0, 9) },
            new MudGanttEvent { Id = "demo", Name = "Demo", Date = At(anchor, 7, 16) }
        ];
    }

    public static IReadOnlyList<MudGanttTask> CreateDependencyTasks()
    {
        var anchor = AnchorDay().AddDays(-2);

        return
        [
            new MudGanttTask
            {
                Id = "design",
                Name = "UX design",
                StartDate = At(anchor, 0, 8),
                EndDate = At(anchor, 1, 17),
                Progress = 1.0,
                Color = "#4f46e5"
            },
            new MudGanttTask
            {
                Id = "api",
                Name = "API layer",
                StartDate = At(anchor, 2, 8),
                EndDate = At(anchor, 4, 17),
                Progress = 0.75,
                Links = [new Link("design", LinkType.FinishToStart)],
                Color = "#059669"
            },
            new MudGanttTask
            {
                Id = "web",
                Name = "Web UI",
                StartDate = At(anchor, 2, 9),
                EndDate = At(anchor, 5, 17),
                Progress = 0.55,
                Links = [new Link("design", LinkType.FinishToStart)],
                Color = "#f97316"
            },
            new MudGanttTask
            {
                Id = "integration",
                Name = "Integration",
                StartDate = At(anchor, 6, 8),
                EndDate = At(anchor, 6, 18),
                Progress = 0.25,
                Links = [new Link("api", LinkType.FinishToStart), new Link("web", LinkType.FinishToStart)],
                Color = "#dc2626"
            }
        ];
    }

    public static IReadOnlyList<MudGanttTask> CreateOperationsTasks()
    {
        var anchor = AnchorDay().AddDays(1);

        return
        Enumerable.Range(0, 12)
            .Select(i => new MudGanttTask
            {
                Id = $"ops-{i + 1}",
                Name = $"Support lane {i + 1}",
                StartDate = At(anchor, 0, 6 + (i % 4), (i * 10) % 60),
                EndDate = At(anchor, 0, 12 + (i % 5), ((i * 10) + 30) % 60),
                Progress = null,
                Color = i % 2 == 0 ? "#9ec9d1" : "#b7d8b0"
            })
            .ToList();
    }

    public static IReadOnlyList<MudGanttTask> CreateBaselineCriticalTasks()
    {
        var anchor = AnchorDay().AddDays(3);

        return
        [
            new MudGanttTask
            {
                Id = "cp-1",
                Name = "Order confirmation",
                StartDate = At(anchor, 0, 8),
                EndDate = At(anchor, 0, 16),
                BaselineStartDate = At(anchor, 0, 8),
                BaselineEndDate = At(anchor, 0, 12),
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
                StartDate = At(anchor, 1, 8),
                EndDate = At(anchor, 2, 17),
                BaselineStartDate = At(anchor, 1, 8),
                BaselineEndDate = At(anchor, 2, 12),
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
                StartDate = At(anchor, 3, 6),
                EndDate = At(anchor, 4, 14),
                BaselineStartDate = At(anchor, 3, 6),
                BaselineEndDate = At(anchor, 4, 10),
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
                StartDate = At(anchor, 3, 8),
                EndDate = At(anchor, 3, 18),
                BaselineStartDate = At(anchor, 4, 8),
                BaselineEndDate = At(anchor, 4, 18),
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
                StartDate = At(anchor, 7, 8),
                EndDate = At(anchor, 8, 15),
                BaselineStartDate = At(anchor, 7, 8),
                BaselineEndDate = At(anchor, 8, 12),
                Progress = 0.1,
                IsCritical = true,
                RightLabel = "Ship gate",
                Links = [new Link("cp-3", LinkType.FinishToStart), new Link("cp-4", LinkType.FinishToStart)],
                Color = "#d32f2f",
                BaselineColor = "#94a3b8"
            }
        ];
    }

    private static DateTimeOffset AnchorDay()
    {
        var now = DateTimeOffset.Now;
        return new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, now.Offset);
    }

    private static DateTimeOffset At(DateTimeOffset anchor, int dayOffset, int hour, int minute = 0) =>
        anchor.AddDays(dayOffset).AddHours(hour).AddMinutes(minute);
}
