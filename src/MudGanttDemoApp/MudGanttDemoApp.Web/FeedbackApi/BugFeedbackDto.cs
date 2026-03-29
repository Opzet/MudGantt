namespace MudGanttDemoApp.Web.FeedbackApi;

public sealed class BugFeedbackDto
{
    public required string Id { get; set; }
    public required DateTimeOffset CreatedUtc { get; set; }
    public required string Area { get; set; }
    public required string Title { get; set; }
    public required string Severity { get; set; }
    public required string ReproductionSteps { get; set; }
    public required string ExpectedBehavior { get; set; }
    public required string ActualBehavior { get; set; }
    public string? ClientOrProject { get; set; }
    public string? SuggestedBy { get; set; }
    public string Status { get; set; } = "New";
}
