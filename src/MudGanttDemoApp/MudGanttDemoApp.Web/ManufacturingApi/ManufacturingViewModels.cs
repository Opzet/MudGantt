using MudGantt;

namespace MudGanttDemoApp.Web.ManufacturingApi;

public sealed class ManufacturingFilterState
{
    public string Client { get; set; } = "All clients";
    public string Machine { get; set; } = "All machines";
    public string Search { get; set; } = string.Empty;
    public bool CriticalOnly { get; set; }
    public bool ConflictsOnly { get; set; }
    public bool DelayedOnly { get; set; }
}

public static class ManufacturingQueries
{
    public const string AllClients = "All clients";
    public const string AllMachines = "All machines";

    public static IReadOnlyList<ManufacturingJobDto> FilterJobs(IEnumerable<ManufacturingJobDto> jobs, ManufacturingFilterState filter)
    {
        return jobs.Where(job => Matches(job, filter)).ToList();
    }

    public static IReadOnlyList<ManufacturingMachineSlotDto> FilterSlots(IEnumerable<ManufacturingMachineSlotDto> slots, ManufacturingFilterState filter)
    {
        return slots.Where(slot => Matches(slot, filter)).ToList();
    }

    public static IReadOnlyList<ManufacturingResourceDto> FilterResources(IEnumerable<ManufacturingResourceDto> resources, string search)
    {
        return resources
            .Where(resource => string.IsNullOrWhiteSpace(search)
                || resource.Machine.Contains(search, StringComparison.OrdinalIgnoreCase)
                || resource.Worker.Contains(search, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(resource => resource.IsOverloaded)
            .ThenByDescending(resource => resource.UtilizationPercent)
            .ToList();
    }

    public static IReadOnlyList<ManufacturingResourceDto> VisibleResourcesForJobs(IEnumerable<ManufacturingResourceDto> resources, IEnumerable<ManufacturingJobDto> jobs)
    {
        var machines = jobs.Select(job => job.Machine).ToHashSet(StringComparer.OrdinalIgnoreCase);
        return resources.Where(resource => machines.Contains(resource.Machine)).ToList();
    }

    private static bool Matches(ManufacturingJobDto job, ManufacturingFilterState filter)
    {
        if (filter.Client != AllClients && job.Client != filter.Client)
            return false;
        if (filter.Machine != AllMachines && job.Machine != filter.Machine)
            return false;
        if (filter.CriticalOnly && !job.IsCritical)
            return false;
        if (filter.ConflictsOnly && !job.HasConflict)
            return false;
        if (filter.DelayedOnly && job.DelayHours <= 0)
            return false;
        if (string.IsNullOrWhiteSpace(filter.Search))
            return true;

        return job.Name.Contains(filter.Search, StringComparison.OrdinalIgnoreCase)
            || job.Machine.Contains(filter.Search, StringComparison.OrdinalIgnoreCase)
            || job.Worker.Contains(filter.Search, StringComparison.OrdinalIgnoreCase)
            || job.Client.Contains(filter.Search, StringComparison.OrdinalIgnoreCase);
    }

    private static bool Matches(ManufacturingMachineSlotDto slot, ManufacturingFilterState filter)
    {
        if (filter.Client != AllClients && slot.Client != filter.Client)
            return false;
        if (filter.Machine != AllMachines && slot.Machine != filter.Machine)
            return false;
        if (filter.CriticalOnly && !slot.IsCritical)
            return false;
        if (filter.ConflictsOnly && !slot.HasConflict)
            return false;
        if (string.IsNullOrWhiteSpace(filter.Search))
            return true;

        return slot.JobId.Contains(filter.Search, StringComparison.OrdinalIgnoreCase)
            || slot.Machine.Contains(filter.Search, StringComparison.OrdinalIgnoreCase)
            || slot.Worker.Contains(filter.Search, StringComparison.OrdinalIgnoreCase)
            || slot.Client.Contains(filter.Search, StringComparison.OrdinalIgnoreCase);
    }
}

public static class ManufacturingTaskMapper
{
    public static MudGanttTask ToClientJobTask(ManufacturingJobDto job)
    {
        return new MudGanttTask
        {
            Id = job.Id,
            Name = $"{job.Client} · {job.Name}",
            StartDate = job.StartDate,
            EndDate = job.EndDate,
            Progress = job.Progress,
            Color = job.IsCritical ? "#dc3545" : "#2563eb",
            BaselineStartDate = job.BaselineStartDate,
            BaselineEndDate = job.BaselineEndDate,
            BaselineColor = "#94a3b8",
            IsCritical = job.IsCritical || job.HasConflict,
            RightLabel = $"{job.Machine} · {job.Worker}",
            Links = string.IsNullOrWhiteSpace(job.PreviousJobId) ? [] : [new Link(job.PreviousJobId, LinkType.FinishToStart)]
        };
    }

    public static MudGanttTask ToMachineTask(ManufacturingMachineSlotDto slot)
    {
        return new MudGanttTask
        {
            Id = slot.Id,
            Name = $"{slot.Machine} · {slot.Worker}",
            StartDate = slot.StartDate,
            EndDate = slot.EndDate,
            Progress = slot.Progress,
            Color = slot.IsCritical ? "#c62828" : "#0f766e",
            IsCritical = slot.IsCritical || slot.HasConflict,
            RightLabel = slot.JobId.Replace("job-", "Job ")
        };
    }

    public static MudGanttTask ToClientPortalTask(ManufacturingJobDto job)
    {
        return new MudGanttTask
        {
            Id = job.Id,
            Name = job.Name,
            StartDate = job.StartDate,
            EndDate = job.EndDate,
            Progress = job.Progress,
            Color = job.IsCritical ? "#dc2626" : "#16a34a",
            BaselineStartDate = job.BaselineStartDate,
            BaselineEndDate = job.BaselineEndDate,
            BaselineColor = "#94a3b8",
            IsCritical = job.IsCritical || job.HasConflict,
            RightLabel = job.Machine,
            Links = string.IsNullOrWhiteSpace(job.PreviousJobId) ? [] : [new Link(job.PreviousJobId, LinkType.FinishToStart)]
        };
    }
}
