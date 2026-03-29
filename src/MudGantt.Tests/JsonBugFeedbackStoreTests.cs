using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;
using MudGanttDemoApp.Web.FeedbackApi;

namespace MudGantt.Tests;

public class JsonBugFeedbackStoreTests
{
    [Fact]
    public async Task CreateAsync_Persists_And_Exports_Json()
    {
        var root = Path.Combine(Path.GetTempPath(), "mudgantt-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        var environment = new TestHostEnvironment(root);
        var store = new JsonBugFeedbackStore(environment, NullLogger<JsonBugFeedbackStore>.Instance);

        var item = new BugFeedbackDto
        {
            Id = "bug-1",
            CreatedUtc = DateTimeOffset.UtcNow,
            Area = "Flow",
            Title = "Sample",
            Severity = "High",
            ReproductionSteps = "Open page",
            ExpectedBehavior = "Should work",
            ActualBehavior = "Did not work"
        };

        await store.CreateAsync(item, CancellationToken.None);
        var all = await store.GetAllAsync(CancellationToken.None);
        var export = await store.ExportJsonAsync(CancellationToken.None);
        var json = System.Text.Encoding.UTF8.GetString(export);

        Assert.Single(all);
        Assert.Contains("bug-1", json);
    }

    private sealed class TestHostEnvironment(string rootPath) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Development";
        public string ApplicationName { get; set; } = "MudGantt.Tests";
        public string ContentRootPath { get; set; } = rootPath;
        public IFileProvider ContentRootFileProvider { get; set; } = new PhysicalFileProvider(rootPath);
    }
}
