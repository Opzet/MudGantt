using System.Text;

namespace MudGanttDemoApp.Web.ManufacturingApi;

internal static class ManufacturingReportBuilder
{
    public static byte[] BuildJobsCsv(ManufacturingDatasetDto dataset, string? client)
    {
        var jobs = dataset.Jobs
            .Where(job => string.IsNullOrWhiteSpace(client) || string.Equals(job.Client, client, StringComparison.OrdinalIgnoreCase))
            .OrderBy(job => job.Client)
            .ThenBy(job => job.StartDate)
            .ToList();

        var builder = new StringBuilder();
        builder.AppendLine("JobId,Client,Name,Machine,Worker,StartDate,EndDate,Progress,IsCritical,HasConflict,DelayHours");
        foreach (var job in jobs)
        {
            builder.AppendLine(string.Join(',',
                Csv(job.Id),
                Csv(job.Client),
                Csv(job.Name),
                Csv(job.Machine),
                Csv(job.Worker),
                Csv(job.StartDate.ToString("O")),
                Csv(job.EndDate.ToString("O")),
                job.Progress.ToString("0.##"),
                job.IsCritical,
                job.HasConflict,
                job.DelayHours.ToString("0.##")));
        }

        return Encoding.UTF8.GetBytes(builder.ToString());
    }

    public static byte[] BuildResourcesCsv(ManufacturingDatasetDto dataset, string? client)
    {
        var relevantMachines = dataset.Jobs
            .Where(job => string.IsNullOrWhiteSpace(client) || string.Equals(job.Client, client, StringComparison.OrdinalIgnoreCase))
            .Select(job => job.Machine)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var resources = dataset.Resources
            .Where(resource => relevantMachines.Count == 0 || relevantMachines.Contains(resource.Machine))
            .OrderByDescending(resource => resource.IsOverloaded)
            .ThenByDescending(resource => resource.UtilizationPercent)
            .ToList();

        var builder = new StringBuilder();
        builder.AppendLine("Machine,Workers,BookedJobs,PlannedHours,UtilizationPercent,ConflictCount,IsOverloaded");
        foreach (var resource in resources)
        {
            builder.AppendLine(string.Join(',',
                Csv(resource.Machine),
                Csv(resource.Worker),
                resource.BookedJobs,
                resource.PlannedHours.ToString("0.##"),
                resource.UtilizationPercent.ToString("0.##"),
                resource.ConflictCount,
                resource.IsOverloaded));
        }

        return Encoding.UTF8.GetBytes(builder.ToString());
    }

    private static string Csv(string? value)
    {
        var escaped = (value ?? string.Empty).Replace("\"", "\"\"");
        return $"\"{escaped}\"";
    }
}
