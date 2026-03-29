using Heron.MudCalendar;

namespace MudGanttDemoApp.Web.Components.Pages;

public sealed class CalendarExampleItem : CalendarItem
{
    public string? Client { get; set; }
    public string? Machine { get; set; }
    public string? JobId { get; set; }
    public string? Category { get; set; }
}
