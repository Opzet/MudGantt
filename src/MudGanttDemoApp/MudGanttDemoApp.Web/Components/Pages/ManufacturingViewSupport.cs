using System.Text.Json;
using MudGantt;
using MudGanttDemoApp.Web.ManufacturingApi;

namespace MudGanttDemoApp.Web.Components.Pages;

public sealed class ManufacturingViewPreset
{
    public required string Name { get; set; }
    public required string Client { get; set; }
    public required string Machine { get; set; }
    public required string Search { get; set; }
    public bool CriticalOnly { get; set; }
    public bool ConflictsOnly { get; set; }
    public bool DelayedOnly { get; set; }
}

public sealed class ManufacturingConflictSuggestion
{
    public required string Title { get; set; }
    public required string Detail { get; set; }
    public required string Impact { get; set; }
    public required string Machine { get; set; }
    public required string Worker { get; set; }
    public required DateTimeOffset StartDate { get; set; }
    public required DateTimeOffset EndDate { get; set; }
    public double DelayHours { get; set; }
}

internal static class ManufacturingViewSupport
{
    public static string SerializePresets(IReadOnlyList<ManufacturingViewPreset> presets) => JsonSerializer.Serialize(presets);

    public static IReadOnlyList<ManufacturingViewPreset> DeserializePresets(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        return JsonSerializer.Deserialize<List<ManufacturingViewPreset>>(json) ?? [];
    }

    public static IReadOnlyList<MudGanttEvent> BuildHeatmapRanges(IReadOnlyList<ManufacturingMachineSlotDto> slots)
    {
        return slots
            .GroupBy(slot => new { slot.Machine, Day = slot.StartDate.Date })
            .Where(group => group.Count() >= 3)
            .Select(group => new MudGanttEvent
            {
                Id = $"heat-{group.Key.Machine}-{group.Key.Day:yyyyMMdd}",
                Name = $"Peak load {group.Key.Machine}",
                Date = group.Min(slot => slot.StartDate),
                EndDate = group.Max(slot => slot.EndDate),
                Color = group.Count() >= 5 ? "#dc3545" : "#f59e0b"
            })
            .ToList();
    }

    public static IReadOnlyList<MudGanttRangeOverlay> BuildRowHeatmapOverlays(IReadOnlyList<ManufacturingMachineSlotDto> slots)
    {
        return slots
            .Where(slot => slot.HasConflict)
            .Select(slot => new MudGanttRangeOverlay
            {
                Id = $"row-heat-{slot.Id}",
                TaskId = slot.Id,
                StartDate = slot.StartDate,
                EndDate = slot.EndDate,
                Color = slot.IsCritical ? "#dc3545" : "#f59e0b",
                Label = slot.HasConflict ? "Conflict" : null
            })
            .ToList();
    }

    public static IReadOnlyList<MudGanttRangeOverlay> BuildWhatIfOverlays(IReadOnlyList<ManufacturingSimulationOverride> overrides)
    {
        return overrides
            .Select(overrideItem => new MudGanttRangeOverlay
            {
                Id = $"whatif-{overrideItem.JobId}",
                TaskId = overrideItem.RowTaskId,
                StartDate = overrideItem.StartDate,
                EndDate = overrideItem.EndDate,
                Color = "#0ea5e9",
                Label = "What-if"
            })
            .ToList();
    }

    public static IReadOnlyList<ManufacturingConflictSuggestion> BuildConflictSuggestions(ManufacturingJobDto job, IReadOnlyList<ManufacturingMachineSlotDto> slots, IReadOnlyList<ManufacturingResourceDto> resources)
    {
        var family = GetMachineFamily(job.Machine);
        var compatibleMachines = resources
            .Where(resource => GetMachineFamily(resource.Machine) == family)
            .OrderBy(resource => resource.IsOverloaded)
            .ThenBy(resource => resource.ConflictCount)
            .ThenBy(resource => resource.UtilizationPercent)
            .Take(3)
            .ToList();

        var suggestions = new List<ManufacturingConflictSuggestion>();
        foreach (var resource in compatibleMachines)
        {
            var latestEnd = slots
                .Where(slot => slot.Machine == resource.Machine && slot.JobId != job.Id)
                .Select(slot => (DateTimeOffset?)slot.EndDate)
                .DefaultIfEmpty(job.StartDate)
                .Max() ?? job.StartDate;

            var candidateStart = latestEnd > job.StartDate ? latestEnd.AddHours(1) : job.StartDate;
            var candidateEnd = candidateStart.Add(job.EndDate - job.StartDate);
            var delta = Math.Round((candidateEnd - job.EndDate).TotalHours, 1);

            suggestions.Add(new ManufacturingConflictSuggestion
            {
                Title = resource.Machine == job.Machine ? "Resequence on current machine" : $"Move to {resource.Machine}",
                Detail = $"Worker pool: {resource.Worker}. Earliest slot: {candidateStart:dd MMM HH:mm} - {candidateEnd:dd MMM HH:mm}.",
                Impact = delta <= 0 ? $"Removes current conflict and can recover {Math.Abs(delta):0.#}h." : $"Removes conflict but adds {delta:0.#}h to finish.",
                Machine = resource.Machine,
                Worker = resource.Worker,
                StartDate = candidateStart,
                EndDate = candidateEnd,
                DelayHours = Math.Max(0, delta)
            });
        }

        if (suggestions.Count == 0)
        {
            suggestions.Add(new ManufacturingConflictSuggestion
            {
                Title = "No alternative found",
                Detail = "Keep on current machine and prioritize predecessor completion.",
                Impact = "Escalate in daily production review.",
                Machine = job.Machine,
                Worker = job.Worker,
                StartDate = job.StartDate,
                EndDate = job.EndDate,
                DelayHours = job.DelayHours
            });
        }

        return suggestions;
    }

    private static string GetMachineFamily(string machine)
    {
        var index = machine.IndexOf('-');
        return index > 0 ? machine[..index] : machine;
    }
}

public sealed class ManufacturingSimulationOverride
{
    public required string JobId { get; set; }
    public required string RowTaskId { get; set; }
    public required string Machine { get; set; }
    public required string Worker { get; set; }
    public required DateTimeOffset StartDate { get; set; }
    public required DateTimeOffset EndDate { get; set; }
    public required double DelayHours { get; set; }
}
