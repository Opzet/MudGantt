using MudGantt;

namespace MudGantt.Tests;

public class GanttTaskAnalysisTests
{
    [Fact]
    public void BuildDependencyChain_Returns_Predecessors_And_Successors()
    {
        var tasks = new List<MudGanttTask>
        {
            new() { Id = "a", Name = "A", StartDate = DateTimeOffset.UtcNow, EndDate = DateTimeOffset.UtcNow.AddHours(2) },
            new() { Id = "b", Name = "B", StartDate = DateTimeOffset.UtcNow, EndDate = DateTimeOffset.UtcNow.AddHours(2), Links = [new Link("a", LinkType.FinishToStart)] },
            new() { Id = "c", Name = "C", StartDate = DateTimeOffset.UtcNow, EndDate = DateTimeOffset.UtcNow.AddHours(2), Links = [new Link("b", LinkType.FinishToStart)] }
        };

        var chain = GanttTaskAnalysis.BuildDependencyChain(tasks, "b");

        Assert.Contains("a", chain);
        Assert.Contains("b", chain);
        Assert.Contains("c", chain);
    }

    [Fact]
    public void CalculateWeightedProgress_Uses_Duration_As_Weight()
    {
        var start = DateTimeOffset.UtcNow;
        var tasks = new List<MudGanttTask>
        {
            new() { Id = "short", Name = "Short", StartDate = start, EndDate = start.AddHours(1), Progress = 1.0 },
            new() { Id = "long", Name = "Long", StartDate = start, EndDate = start.AddHours(9), Progress = 0.0 }
        };

        var progress = GanttTaskAnalysis.CalculateWeightedProgress(tasks);

        Assert.Equal(0.1, Math.Round(progress, 2));
    }
}
