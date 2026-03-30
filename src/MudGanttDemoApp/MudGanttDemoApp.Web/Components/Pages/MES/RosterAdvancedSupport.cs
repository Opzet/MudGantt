using MudBlazor;

using MudGantt;

using MudGanttDemoApp.Web.SchedulerApi;

namespace MudGanttDemoApp.Web.Components.Pages.MES;

internal static class RosterAdvancedSupport
{
    public const string AllRoles = "All roles";

    public static string GetTemplateLabel(RotationTemplate template) => template switch
    {
        RotationTemplate.SevenOnSevenOff => "7 On / 7 Off",
        RotationTemplate.FourOnThreeOff => "4 On / 3 Off",
        RotationTemplate.FourOnFourOff => "4 On / 4 Off",
        RotationTemplate.SevenDaySevenNightSevenOff => "7 Day / 7 Night / 7 Off",
        _ => template.ToString()
    };

    public static RotationDay BuildRotationDay(RotationTemplate template, DateOnly date, int offset) => template switch
    {
        RotationTemplate.SevenOnSevenOff => offset % 14 < 7 ? RotationDay.Day(date) : RotationDay.Off(date),
        RotationTemplate.FourOnThreeOff => offset % 7 < 4 ? RotationDay.Day(date) : RotationDay.Off(date),
        RotationTemplate.FourOnFourOff => offset % 8 < 4 ? RotationDay.Day(date) : RotationDay.Off(date),
        RotationTemplate.SevenDaySevenNightSevenOff => offset % 21 < 7 ? RotationDay.Day(date) : offset % 21 < 14 ? RotationDay.Night(date) : RotationDay.Off(date),
        _ => RotationDay.Day(date)
    };

    public static IReadOnlyList<PreviewResourceRow> BuildPreviewRows(IReadOnlyList<SchedulerResourceDto> resources, IReadOnlyCollection<string> selectedResourceIds, IReadOnlyList<RotationDay> previewDays)
    {
        return resources
            .Where(resource => selectedResourceIds.Contains(resource.Id))
            .Select(resource => new PreviewResourceRow
            {
                ResourceId = resource.Id,
                ResourceName = resource.Name,
                Cells = previewDays.Select(day => new PreviewCell
                {
                    Date = day.Date,
                    ShiftCode = day.ShiftCode,
                    Color = day.IsOff ? Color.Default : day.ShiftCode == "N" ? Color.Secondary : Color.Success
                }).ToList()
            })
            .ToList();
    }

    public static IReadOnlyList<MudGanttTask> BuildPreviewTasks(IReadOnlyList<SchedulerResourceDto> resources, IReadOnlyCollection<string> selectedResourceIds, IReadOnlyList<RotationDay> previewDays)
    {
        return resources
            .Where(resource => selectedResourceIds.Contains(resource.Id))
            .SelectMany(resource => previewDays.Where(day => !day.IsOff).Select(day => new MudGanttTask
            {
                Id = $"{resource.Id}-{day.Date:yyyyMMdd}-{day.ShiftCode}",
                Name = resource.Name,
                StartDate = new DateTimeOffset(day.Date.ToDateTime(day.StartTime), TimeSpan.Zero),
                EndDate = new DateTimeOffset(day.Date.ToDateTime(day.EndTime), TimeSpan.Zero).Add(day.ShiftCode == "N" ? TimeSpan.FromDays(1) : TimeSpan.Zero),
                Color = day.ShiftCode == "N" ? "#7c3aed" : "#16a34a",
                RightLabel = $"{resource.Role} · {day.ShiftCode}"
            }))
            .ToList();
    }
}

internal enum RotationTemplate
{
    SevenOnSevenOff,
    FourOnThreeOff,
    FourOnFourOff,
    SevenDaySevenNightSevenOff
}

internal sealed class RotationDay
{
    public DateOnly Date { get; init; }
    public string ShiftCode { get; init; } = string.Empty;
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public bool IsOff { get; init; }

    public static RotationDay Day(DateOnly date) => new() { Date = date, ShiftCode = "D", StartTime = new TimeOnly(6, 0), EndTime = new TimeOnly(18, 0) };
    public static RotationDay Night(DateOnly date) => new() { Date = date, ShiftCode = "N", StartTime = new TimeOnly(18, 0), EndTime = new TimeOnly(6, 0) };
    public static RotationDay Off(DateOnly date) => new() { Date = date, ShiftCode = "OFF", IsOff = true };
}

internal sealed class PreviewResourceRow
{
    public string ResourceId { get; init; } = string.Empty;
    public string ResourceName { get; init; } = string.Empty;
    public List<PreviewCell> Cells { get; init; } = [];
}

internal sealed class PreviewCell
{
    public DateOnly Date { get; init; }
    public string ShiftCode { get; init; } = string.Empty;
    public Color Color { get; init; }
}
