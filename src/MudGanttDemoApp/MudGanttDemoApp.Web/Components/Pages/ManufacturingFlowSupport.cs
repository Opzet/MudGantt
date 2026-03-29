using MudBlazor;
using MudGantt;
using MudGanttDemoApp.Web.ManufacturingApi;

namespace MudGanttDemoApp.Web.Components.Pages;

internal static class ManufacturingFlowSupport
{
    public const string PresetsStorageKey = "mudgantt-manufacturing-flow-presets";
    public const string CurrentFiltersPreset = "__current";

    public static ManufacturingViewPreset CreatePreset(string name, ManufacturingFilterState filter)
    {
        return new ManufacturingViewPreset
        {
            Name = name,
            Client = filter.Client,
            Machine = filter.Machine,
            Search = filter.Search,
            CriticalOnly = filter.CriticalOnly,
            ConflictsOnly = filter.ConflictsOnly,
            DelayedOnly = filter.DelayedOnly
        };
    }

    public static ManufacturingJobDto ApplyOverride(ManufacturingJobDto job, ManufacturingSimulationOverride? simulation)
    {
        if (simulation is null)
        {
            return job;
        }

        return new ManufacturingJobDto
        {
            Id = job.Id,
            Client = job.Client,
            Name = job.Name,
            Machine = simulation.Machine,
            Worker = simulation.Worker,
            StartDate = simulation.StartDate,
            EndDate = simulation.EndDate,
            Progress = job.Progress,
            IsCritical = job.IsCritical,
            HasConflict = false,
            DelayHours = simulation.DelayHours,
            BaselineStartDate = job.BaselineStartDate,
            BaselineEndDate = job.BaselineEndDate,
            PreviousJobId = job.PreviousJobId
        };
    }

    public static ManufacturingMachineSlotDto ApplyOverride(ManufacturingMachineSlotDto slot, ManufacturingSimulationOverride? simulation)
    {
        if (simulation is null)
        {
            return slot;
        }

        return new ManufacturingMachineSlotDto
        {
            Id = slot.Id,
            Machine = simulation.Machine,
            Worker = simulation.Worker,
            Client = slot.Client,
            JobId = slot.JobId,
            StartDate = simulation.StartDate,
            EndDate = simulation.EndDate,
            Progress = slot.Progress,
            IsCritical = slot.IsCritical,
            HasConflict = false
        };
    }

    public static IReadOnlyList<MudGanttRangeOverlay> BuildJobRangeOverlays(ManufacturingJobDto? selectedJob, ManufacturingSimulationOverride? whatIfOverride)
    {
        return whatIfOverride is null || selectedJob is null
            ? []
            :
            [
                new MudGanttRangeOverlay
                {
                    Id = $"whatif-job-{selectedJob.Id}",
                    TaskId = selectedJob.Id,
                    StartDate = whatIfOverride.StartDate,
                    EndDate = whatIfOverride.EndDate,
                    Color = "#0ea5e9",
                    Label = "What-if"
                }
            ];
    }

    public static Color GetUtilizationColor(double utilization)
    {
        if (utilization >= 85)
        {
            return Color.Error;
        }

        if (utilization >= 65)
        {
            return Color.Warning;
        }

        return Color.Success;
    }
}
