using Microsoft.Extensions.Logging.Abstractions;
using MudGanttDemoApp.Web.ManufacturingApi;

namespace MudGantt.Tests;

public class ManufacturingDemoStoreTests
{
    [Fact]
    public async Task GetDatasetAsync_Generates_Requested_Job_Count_And_Risk_Metadata()
    {
        var store = new InMemoryManufacturingDemoStore(NullLogger<InMemoryManufacturingDemoStore>.Instance);

        var dataset = await store.GetDatasetAsync(250, CancellationToken.None);

        Assert.Equal(250, dataset.Jobs.Count);
        Assert.Equal(250, dataset.MachineSlots.Count);
        Assert.Contains(dataset.Jobs, job => job.HasConflict || job.DelayHours > 0 || job.IsCritical);
        Assert.Contains(dataset.Resources, resource => resource.ConflictCount >= 0);
    }
}
