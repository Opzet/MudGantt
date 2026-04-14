using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MudGanttDemoApp.Web.Components.Pages;

public partial class ResourceLoading
{
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    private List<ResourceMember> _members = DashboardData.CreateResourceMembers();
    private string _period = "This month";
    private string _projectFilter = "All Projects";
    private DateTimeOffset _calendarStart = GetMonday(DateTimeOffset.Now.AddDays(-7));

    // 3 weeks × 7 days = 21 day columns
    private const int TotalDays = 21;

    private static DateTimeOffset GetMonday(DateTimeOffset d)
    {
        var diff = (int)d.DayOfWeek - (int)DayOfWeek.Monday;
        if (diff < 0) diff += 7;
        return d.AddDays(-diff).Date;
    }

    private IEnumerable<DateTimeOffset> CalendarDays =>
        Enumerable.Range(0, TotalDays).Select(i => _calendarStart.AddDays(i));

    // Group days by week (7 per group)
    private IEnumerable<IGrouping<int, (DateTimeOffset Date, int Index)>> WeekGroups =>
        CalendarDays
            .Select((d, i) => (Date: d, Index: i))
            .GroupBy(x => x.Index / 7);

    private bool IsWeekend(DateTimeOffset d) =>
        d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday;

    private void ToggleMember(ResourceMember m)
    {
        m.Expanded = !m.Expanded;
    }

    private int GetMemberHours(ResourceMember member, int dayIndex)
    {
        return member.Tasks.Sum(t =>
            t.HoursByDayIndex.TryGetValue(dayIndex, out var h) ? h : 0);
    }

    private static string GetDayCellClass(int hours, bool isWeekend)
    {
        if (isWeekend) return "pm-res-nonworking";
        if (hours == 0) return "";
        if (hours > 8) return "pm-res-overassigned";
        return "pm-res-assigned";
    }

    private void PreviousPeriod() => _calendarStart = _calendarStart.AddDays(-7);
    private void NextPeriod() => _calendarStart = _calendarStart.AddDays(7);

    private void ExportWorkload()
    {
        Snackbar.Add("Workload export queued.", Severity.Info);
    }
}
