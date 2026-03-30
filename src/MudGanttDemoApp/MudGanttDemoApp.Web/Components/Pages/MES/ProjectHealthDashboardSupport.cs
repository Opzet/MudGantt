using MudBlazor;

using MudGanttDemoApp.Web.ManufacturingApi;
using MudGanttDemoApp.Web.SchedulerApi;

namespace MudGanttDemoApp.Web.Components.Pages.MES;

internal static class ProjectHealthDashboardSupport
{
    public const string AllClients = "All clients";
    public const string AllPhases = "All phases";

    public static IReadOnlyList<ProjectRiskItem> BuildRiskItems(IReadOnlyList<SchedulerTaskItemDto> scopedTasks, IReadOnlyList<ManufacturingJobDto> scopedJobs, DateTime throughDate)
    {
        var items = new List<ProjectRiskItem>();

        foreach (var task in scopedTasks)
        {
            if ((task.Progress ?? 0) < 1.0 && task.EndDate?.Date < throughDate)
            {
                items.Add(new ProjectRiskItem("Schedule", task.Phase ?? "Project", task.Name, "Task is overdue and not completed.", "High", Color.Error));
            }
            else if ((task.Progress ?? 0) < 0.6 && task.EndDate?.Date <= throughDate.AddDays(5))
            {
                items.Add(new ProjectRiskItem("Execution", task.Phase ?? "Project", task.Name, "Task pacing indicates delivery risk.", "Medium", Color.Warning));
            }
        }

        foreach (var job in scopedJobs.Where(job => job.HasConflict || job.DelayHours > 0).Take(20))
        {
            items.Add(new ProjectRiskItem(
                job.HasConflict ? "Capacity" : "Delivery",
                job.Client,
                job.Name,
                job.HasConflict ? $"Machine conflict on {job.Machine}." : $"Delayed by {job.DelayHours:0.#}h.",
                job.HasConflict ? "High" : "Medium",
                job.HasConflict ? Color.Error : Color.Warning));
        }

        return items;
    }

    public static BurndownResult BuildBurndown(IReadOnlyList<SchedulerTaskItemDto> scopedTasks)
    {
        if (scopedTasks.Count == 0)
        {
            return new BurndownResult([], []);
        }

        var orderedTasks = scopedTasks.OrderBy(task => task.EndDate).ToList();
        var total = orderedTasks.Sum(task => task.EffortHours);
        var actualRemaining = new List<double>();
        var idealRemaining = new List<double>();
        var labels = new List<string>();
        double completed = 0;

        for (var i = 0; i < orderedTasks.Count; i++)
        {
            var task = orderedTasks[i];
            completed += task.EffortHours * (task.Progress ?? 0);
            actualRemaining.Add(Math.Max(0, total - completed));
            idealRemaining.Add(Math.Max(0, total - ((i + 1d) / orderedTasks.Count * total)));
            labels.Add(task.EndDate?.ToString("dd MMM") ?? $"T{i + 1}");
        }

        return new BurndownResult(labels.ToArray(),
        [
            new ChartSeries<double> { Name = "Ideal Remaining", Data = idealRemaining.ToArray() },
            new ChartSeries<double> { Name = "Actual Remaining", Data = actualRemaining.ToArray() }
        ]);
    }

    public static ProjectHealthMetrics BuildMetrics(IReadOnlyList<SchedulerTaskItemDto> scopedTasks, IReadOnlyList<ManufacturingJobDto> scopedJobs, DateTime throughDate)
    {
        var plannedHours = (decimal)scopedTasks.Sum(task => task.EffortHours);
        var earnedHours = (decimal)scopedTasks.Sum(task => task.EffortHours * (task.Progress ?? 0));
        var completionPercent = scopedTasks.Count == 0 ? 0 : (decimal)(scopedTasks.Average(task => task.Progress ?? 0) * 100);
        var overdueTasks = scopedTasks.Count(task => (task.Progress ?? 0) < 1.0 && task.EndDate?.Date < throughDate);
        var atRiskTasks = scopedTasks.Count(task => (task.Progress ?? 0) < 0.6 && task.EndDate?.Date <= throughDate.AddDays(5))
            + scopedJobs.Count(job => job.HasConflict || job.DelayHours > 0);
        var riskScore = Math.Min(100, (overdueTasks * 10m) + (atRiskTasks * 6m) + Math.Max(0m, (decimal)scopedJobs.Sum(job => job.DelayHours)));

        return new ProjectHealthMetrics(plannedHours, earnedHours, completionPercent, overdueTasks, atRiskTasks, riskScore);
    }
}

internal sealed record ProjectHealthMetrics(decimal PlannedHours, decimal EarnedHours, decimal CompletionPercent, int OverdueTasks, int AtRiskTasks, decimal RiskScore);
internal sealed record BurndownResult(string[] Labels, List<ChartSeries<double>> Series);
internal sealed record ProjectRiskItem(string Category, string ScopeName, string ItemName, string Summary, string Severity, Color Color);
