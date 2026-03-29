namespace MudGanttDemoApp.Web.FeedbackApi;

public interface IBugFeedbackStore
{
    Task<IReadOnlyList<BugFeedbackDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<BugFeedbackDto> CreateAsync(BugFeedbackDto feedback, CancellationToken cancellationToken);
    Task<byte[]> ExportJsonAsync(CancellationToken cancellationToken);
}
