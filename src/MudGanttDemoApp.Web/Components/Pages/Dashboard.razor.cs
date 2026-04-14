using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MudGanttDemoApp.Web.Components.Pages;

public partial class Dashboard
{
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    private List<ProjectSummary> _projects = DashboardData.CreateProjects();
    private string _filterGroup = "All";
    private string _filterStatus = "Not Started, Open";

    // ── aggregate totals ──────────────────────────────────────────────────
    private int TotalTasks => _projects.Sum(p => p.TaskCount);
    private int TotalNotStarted => _projects.Sum(p => p.NotStartedTasks);
    private int TotalComplete => _projects.Sum(p => p.CompletedTasks);
    private int TotalInProgress => _projects.Sum(p => p.InProgressTasks);

    // ── SVG donut chart values (radius=72, cx=cy=100) ─────────────────────
    private const double DonutRadius = 72.0;
    private double DonutCirc => 2 * Math.PI * DonutRadius;

    private double NotStartedArc => TotalTasks > 0 ? (TotalNotStarted / (double)TotalTasks) * DonutCirc : 0;
    private double CompleteArc   => TotalTasks > 0 ? (TotalComplete   / (double)TotalTasks) * DonutCirc : 0;
    private double InProgressArc => TotalTasks > 0 ? (TotalInProgress / (double)TotalTasks) * DonutCirc : 0;

    // dashoffset: start all at top (segments laid out clockwise from 12 o'clock)
    // stroke-dashoffset = negative sum of preceding arc lengths (with rotate(-90) on each circle)
    private double NotStartedOffset => 0;
    private double CompleteOffset   => -(NotStartedArc);
    private double InProgressOffset => -(NotStartedArc + CompleteArc);

    // ── cost chart max for bar scaling ────────────────────────────────────
    private double MaxBudget => _projects.Max(p => p.BudgetCost);

    private static double CostBarWidth(double cost, double max, double maxPx = 160) =>
        max > 0 ? Math.Min(maxPx, cost / max * maxPx) : 0;

    // ── health dot color ──────────────────────────────────────────────────
    private static string DotClass(string health) => health switch
    {
        "orange" => "pm-dot-orange",
        "red"    => "pm-dot-red",
        _        => "pm-dot-green"
    };

    private void ApplyFilters() =>
        Snackbar.Add($"Filters applied: {_filterGroup} / {_filterStatus}", Severity.Info);
}
