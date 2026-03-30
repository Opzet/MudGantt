using MudBlazor;
using MudGantt;
using MudGanttDemoApp.Web.GanttApi;

namespace MudGanttDemoApp.Web.Components.Pages;

internal static class HomePageSupport
{
    public static GanttTaskDto ToDto(MudGanttTask task)
    {
        return new GanttTaskDto
        {
            Id = task.Id,
            Name = task.Name,
            StartDate = task.StartDate,
            EndDate = task.EndDate,
            Progress = task.Progress,
            Color = task.Color,
            Links = task.Links.Select(link => new GanttTaskLinkDto { Id = link.Id, LinkType = link.LinkType }).ToList()
        };
    }

    public static MudGanttTask ToTask(GanttTaskDto dto)
    {
        return new MudGanttTask
        {
            Id = dto.Id,
            Name = dto.Name,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Progress = dto.Progress,
            Color = dto.Color,
            Links = dto.Links.Select(link => new Link(link.Id, link.LinkType)).ToArray()
        };
    }

    public static string GetDuration(MudGanttTask task)
    {
        if (task.StartDate.HasValue && task.EndDate.HasValue)
        {
            return (task.EndDate.Value - task.StartDate.Value).Days.ToString();
        }

        return "-";
    }

    public static string FormatStartDate(MudGanttTask task) => task.StartDate?.ToString("MM/dd/yyyy") ?? string.Empty;
    public static string FormatEndDate(MudGanttTask task) => task.EndDate?.ToString("MM/dd/yyyy") ?? string.Empty;
    public static string FormatProgress(MudGanttTask task) => (task.Progress ?? 0).ToString("P0");

    public static Color GetProgressColor(MudGanttTask task)
    {
        var progress = task.Progress ?? 0;
        if (progress >= 1.0) return Color.Success;
        if (progress >= 0.5) return Color.Info;
        if (progress > 0) return Color.Warning;
        return Color.Default;
    }

    public static string GetOverallProgress(IReadOnlyList<MudGanttTask> tasks)
    {
        if (tasks.Count == 0)
        {
            return "0%";
        }

        var average = tasks.Average(task => task.Progress ?? 0) * 100;
        return $"{average:F1}%";
    }

    public static IReadOnlyList<MudGanttEvent> CreateDefaultEvents()
    {
        var anchor = AnchorDay().AddDays(-2);

        return
        [
            new MudGanttEvent { Id = "code-complete", Name = "Code Complete", Date = At(anchor, 2, 18) },
            new MudGanttEvent { Id = "general-availability", Name = "GA", Date = At(anchor, 9, 18) }
        ];
    }

    public static IReadOnlyList<MudGanttTask> CreateDefaultTasks()
    {
        var anchor = AnchorDay().AddDays(-2);

        return
        [
            new MudGanttTask
            {
                Id = "implementation-epic",
                Name = "Gantt chart development",
                StartDate = At(anchor, 0, 0),
                EndDate = At(anchor, 2, 18),
                Progress = 0.8
            },
            new MudGanttTask
            {
                Id = "implementation-js",
                Name = "gantt.js",
                StartDate = At(anchor, 0, 8),
                EndDate = At(anchor, 2, 16),
                Progress = 0.85,
                Links = [new Link("implementation-epic", LinkType.StartToStart), new Link("implementation-epic", LinkType.FinishToFinish)]
            },
            new MudGanttTask
            {
                Id = "implementation-blazor",
                Name = "Blazor component",
                StartDate = At(anchor, 1, 0),
                EndDate = At(anchor, 2, 18),
                Progress = 0.95,
                Links = [new Link("implementation-epic", LinkType.StartToStart), new Link("implementation-epic", LinkType.FinishToFinish)]
            },
            new MudGanttTask
            {
                Id = "unit-tests",
                Name = "Unit Tests",
                StartDate = At(anchor, 3, 9),
                EndDate = At(anchor, 3, 10),
                Links = [new Link("implementation-epic", LinkType.FinishToStart)],
                Progress = 1.0
            },
            new MudGanttTask
            {
                Id = "integration-tests",
                Name = "Integration Tests",
                StartDate = At(anchor, 3, 9),
                EndDate = At(anchor, 3, 17),
                Links = [new Link("implementation-epic", LinkType.FinishToStart)],
                Progress = 0.8
            },
            new MudGanttTask
            {
                Id = "bug-fixes",
                Name = "Bug fixes",
                Color = "#0d7694",
                StartDate = At(anchor, 4, 0),
                EndDate = At(anchor, 5, 16),
                Progress = 0.35,
                Links = [new Link("integration-tests", LinkType.FinishToStart), new Link("unit-tests", LinkType.FinishToStart)]
            },
            new MudGanttTask
            {
                Id = "release1",
                Color = "#40B090",
                Name = "Release nuget package",
                StartDate = At(anchor, 8, 0),
                EndDate = At(anchor, 9, 0),
                Links = [new Link("bug-fixes", LinkType.FinishToStart)]
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
