using System.Text;
using MudGanttDemoApp.Web.ManufacturingApi;

namespace MudGantt.Tests;

public class ManufacturingReportBuilderTests
{
    [Fact]
    public void BuildJobsCsv_Filters_By_Client()
    {
        var dataset = new ManufacturingDatasetDto
        {
            Name = "Test",
            Jobs =
            [
                new ManufacturingJobDto { Id = "job-1", Client = "A", Name = "Job 1", Machine = "CNC-01", Worker = "Ava", StartDate = DateTimeOffset.UtcNow, EndDate = DateTimeOffset.UtcNow.AddHours(1), Progress = 0.5, IsCritical = false, HasConflict = false, DelayHours = 0, BaselineStartDate = DateTimeOffset.UtcNow, BaselineEndDate = DateTimeOffset.UtcNow.AddHours(1) },
                new ManufacturingJobDto { Id = "job-2", Client = "B", Name = "Job 2", Machine = "CNC-02", Worker = "Ben", StartDate = DateTimeOffset.UtcNow, EndDate = DateTimeOffset.UtcNow.AddHours(1), Progress = 0.5, IsCritical = false, HasConflict = false, DelayHours = 0, BaselineStartDate = DateTimeOffset.UtcNow, BaselineEndDate = DateTimeOffset.UtcNow.AddHours(1) }
            ]
        };

        var bytes = ManufacturingReportBuilder.BuildJobsCsv(dataset, "A");
        var csv = Encoding.UTF8.GetString(bytes);

        Assert.Contains("JobId,Client,Name", csv);
        Assert.Contains("\"A\"", csv);
        Assert.DoesNotContain("\"B\"", csv);
    }
}
