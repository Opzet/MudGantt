using MudGantt;

namespace MudGanttDemoApp.Web.ManufacturingApi;

public sealed class ManufacturingDatasetDto
{
    public required string Name { get; set; }
    public List<string> Clients { get; set; } = [];
    public List<ManufacturingJobDto> Jobs { get; set; } = [];
    public List<ManufacturingMachineSlotDto> MachineSlots { get; set; } = [];
    public List<ManufacturingResourceDto> Resources { get; set; } = [];
    public List<MudGanttEvent> Holidays { get; set; } = [];
}

public sealed class ManufacturingJobDto
{
    public required string Id { get; set; }
    public required string Client { get; set; }
    public required string Name { get; set; }
    public required string Machine { get; set; }
    public required string Worker { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public double Progress { get; set; }
    public bool IsCritical { get; set; }
    public bool HasConflict { get; set; }
    public double DelayHours { get; set; }
    public DateTimeOffset BaselineStartDate { get; set; }
    public DateTimeOffset BaselineEndDate { get; set; }
    public string? PreviousJobId { get; set; }
}

public sealed class ManufacturingMachineSlotDto
{
    public required string Id { get; set; }
    public required string Machine { get; set; }
    public required string Worker { get; set; }
    public required string Client { get; set; }
    public required string JobId { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public double Progress { get; set; }
    public bool IsCritical { get; set; }
    public bool HasConflict { get; set; }
}

public sealed class ManufacturingResourceDto
{
    public required string Machine { get; set; }
    public required string Worker { get; set; }
    public int BookedJobs { get; set; }
    public double PlannedHours { get; set; }
    public double UtilizationPercent { get; set; }
    public int ConflictCount { get; set; }
    public bool IsOverloaded { get; set; }
}
