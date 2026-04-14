using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MudGanttDemoApp.Web.Components.Pages;

public partial class Timesheet
{
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    private List<TimesheetEntry> _entries = DashboardData.CreateTimesheet();
    private string _assignee = "Natalie Waters";
    private DateTimeOffset _weekStart = GetMonday(DateTimeOffset.Now);

    private static DateTimeOffset GetMonday(DateTimeOffset d)
    {
        var diff = (int)d.DayOfWeek - (int)DayOfWeek.Monday;
        if (diff < 0) diff += 7;
        return d.AddDays(-diff).Date;
    }

    private IEnumerable<DateTimeOffset> WeekDays =>
        Enumerable.Range(0, 5).Select(i => _weekStart.AddDays(i));

    private int DayTotal(int dayIndex) => _entries.Sum(e => e.Hours[dayIndex]);
    private int GrandTotal => _entries.Sum(e => e.Total);

    private void PreviousWeek()
    {
        _weekStart = _weekStart.AddDays(-7);
        Snackbar.Add($"Week: {_weekStart:MMM d}", Severity.Info);
    }

    private void NextWeek()
    {
        _weekStart = _weekStart.AddDays(7);
        Snackbar.Add($"Week: {_weekStart:MMM d}", Severity.Info);
    }

    private void CopyLastWeek()
    {
        Snackbar.Add("Last week's entries copied.", Severity.Success);
    }

    private void AutoFill()
    {
        foreach (var entry in _entries)
        {
            for (int i = 0; i < 5; i++)
                if (entry.Hours[i] == 0) entry.Hours[i] = 8;
        }
        Snackbar.Add("Auto-filled with 8 hours per empty day.", Severity.Info);
    }

    private void AddTask()
    {
        _entries.Add(new TimesheetEntry
        {
            Project = "New Project",
            Task = "New Task",
            Hours = [0, 0, 0, 0, 0]
        });
    }

    private void Save()
    {
        Snackbar.Add($"Timesheet saved for {_assignee} — week of {_weekStart:MMM d, yyyy}", Severity.Success);
    }
}
