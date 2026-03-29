namespace MudGanttDemoApp.Web.SchedulerApi;

public interface ISchedulerProjectStore
{
    Task<SchedulerProjectDto> GetProjectAsync(CancellationToken cancellationToken);
}
