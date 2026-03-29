using MudGantt;

namespace MudGanttDemoApp.Web.ManufacturingApi;

public sealed class InMemoryManufacturingDemoStore : IManufacturingDemoStore
{
    private static readonly string[] Clients = ["Acme Mining", "Blue Steel", "Contoso Energy", "Delta Food", "Evergreen Water", "Fabrikam Rail", "Globex Pumps", "HMLV Works"];
    private static readonly string[] Machines = ["CNC-01", "CNC-02", "CNC-03", "Laser-01", "Laser-02", "Brake-01", "Paint Booth", "Assembly-01"];
    private static readonly string[] Workers = ["Ava", "Ben", "Cara", "Dylan", "Ethan", "Faith", "Grace", "Hugo", "Ivan", "Jade", "Kai", "Lena"];
    private readonly ILogger<InMemoryManufacturingDemoStore> _logger;

    public InMemoryManufacturingDemoStore(ILogger<InMemoryManufacturingDemoStore> logger)
    {
        _logger = logger;
    }

    public Task<ManufacturingDatasetDto> GetDatasetAsync(int jobs, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        jobs = Math.Clamp(jobs, 50, 2500);
        _logger.LogInformation("Generating manufacturing demo dataset with {JobCount} jobs", jobs);
        return Task.FromResult(CreateDataset(jobs));
    }

    public Task<IReadOnlyList<MudGanttEvent>> GetHolidaysAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<MudGanttEvent> holidays =
        [
            new MudGanttEvent { Id = "holiday-1", Name = "Maintenance Shutdown", Date = new DateTimeOffset(2025, 12, 25, 0, 0, 0, TimeSpan.Zero), EndDate = new DateTimeOffset(2025, 12, 26, 23, 59, 0, TimeSpan.Zero), Color = "#dc3545" },
            new MudGanttEvent { Id = "holiday-2", Name = "Tooling Audit", Date = new DateTimeOffset(2025, 12, 15, 6, 0, 0, TimeSpan.Zero), EndDate = new DateTimeOffset(2025, 12, 15, 18, 0, 0, TimeSpan.Zero), Color = "#f59e0b" }
        ];
        return Task.FromResult(holidays);
    }

    private static ManufacturingDatasetDto CreateDataset(int jobs)
    {
        var baseDate = new DateTimeOffset(2025, 12, 1, 6, 0, 0, TimeSpan.Zero);
        var dataset = new ManufacturingDatasetDto
        {
            Name = $"Manufacturing dataset ({jobs} jobs)",
            Clients = Clients.ToList()
        };

        var machineStats = Machines.ToDictionary(
            m => m,
            _ => new ResourceAccumulator());
        var machineLastEnd = Machines.ToDictionary(m => m, _ => (DateTimeOffset?)null);

        ManufacturingJobDto? previousPerClient = null;
        for (var i = 0; i < jobs; i++)
        {
            var client = Clients[i % Clients.Length];
            var machine = Machines[i % Machines.Length];
            var worker = Workers[(i * 3) % Workers.Length];
            var start = baseDate.AddHours(i * 1.4).AddDays(i / 40);
            start = MoveToWorkingDay(start);
            var durationHours = 4 + (i % 8);
            var end = MoveToWorkingDay(start.AddHours(durationHours));
            var baselineStart = start.AddHours(-(i % 3));
            var baselineEnd = baselineStart.AddHours(durationHours - (i % 2 == 0 ? 1 : 0));
            var jobId = (2500 + i).ToString();
            var progress = Math.Round(((i % 10) + 1) / 10.0, 2);
            var critical = i % 5 == 0 || i % 7 == 0;
            var hasConflict = machineLastEnd[machine] is { } previousMachineEnd && start < previousMachineEnd;
            var delayHours = Math.Max(0, Math.Round((end - baselineEnd).TotalHours, 1));

            var job = new ManufacturingJobDto
            {
                Id = $"job-{jobId}",
                Client = client,
                Name = $"Job {jobId}",
                Machine = machine,
                Worker = worker,
                StartDate = start,
                EndDate = end,
                Progress = progress,
                IsCritical = critical,
                HasConflict = hasConflict,
                DelayHours = delayHours,
                BaselineStartDate = baselineStart,
                BaselineEndDate = baselineEnd,
                PreviousJobId = previousPerClient?.Client == client ? previousPerClient.Id : null
            };
            dataset.Jobs.Add(job);
            previousPerClient = job;

            dataset.MachineSlots.Add(new ManufacturingMachineSlotDto
            {
                Id = $"slot-{jobId}",
                Machine = machine,
                Worker = worker,
                Client = client,
                JobId = job.Id,
                StartDate = start,
                EndDate = end,
                Progress = progress,
                IsCritical = critical,
                HasConflict = hasConflict
            });

            var stat = machineStats[machine];
            stat.BookedJobs++;
            stat.PlannedHours += (end - start).TotalHours;
            stat.Workers.Add(worker);
            if (hasConflict)
            {
                stat.ConflictCount++;
            }

            machineLastEnd[machine] = machineLastEnd[machine] is { } lastEnd && lastEnd > end ? lastEnd : end;
        }

        dataset.Resources = machineStats.Select(kvp => new ManufacturingResourceDto
        {
            Machine = kvp.Key,
            Worker = string.Join(", ", kvp.Value.Workers.OrderBy(x => x)),
            BookedJobs = kvp.Value.BookedJobs,
            PlannedHours = Math.Round(kvp.Value.PlannedHours, 1),
            UtilizationPercent = Math.Min(100, Math.Round(kvp.Value.PlannedHours / Math.Max(8, jobs / 10.0), 1)),
            ConflictCount = kvp.Value.ConflictCount,
            IsOverloaded = kvp.Value.PlannedHours > Math.Max(8, jobs / 10.0) || kvp.Value.ConflictCount > 0
        }).OrderByDescending(x => x.UtilizationPercent).ToList();

        dataset.Holidays =
        [
            new MudGanttEvent { Id = "window-1", Name = "Public Holiday", Date = new DateTimeOffset(2025, 12, 25, 0, 0, 0, TimeSpan.Zero), EndDate = new DateTimeOffset(2025, 12, 26, 23, 59, 0, TimeSpan.Zero), Color = "#dc3545" },
            new MudGanttEvent { Id = "window-2", Name = "Stocktake", Date = new DateTimeOffset(2025, 12, 12, 6, 0, 0, TimeSpan.Zero), EndDate = new DateTimeOffset(2025, 12, 12, 14, 0, 0, TimeSpan.Zero), Color = "#f59e0b" }
        ];

        return dataset;
    }

    private static DateTimeOffset MoveToWorkingDay(DateTimeOffset value)
    {
        while (value.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            value = value.AddDays(1);
        }
        return value;
    }

    private sealed class ResourceAccumulator
    {
        public int BookedJobs { get; set; }
        public double PlannedHours { get; set; }
        public int ConflictCount { get; set; }
        public HashSet<string> Workers { get; } = [];
    }
}
