using MudGantt;

namespace MudGanttDemoApp.Web.ManufacturingApi;

public interface IManufacturingDemoStore
{
    Task<ManufacturingDatasetDto> GetDatasetAsync(int jobs, CancellationToken cancellationToken);
    Task<IReadOnlyList<MudGanttEvent>> GetHolidaysAsync(CancellationToken cancellationToken);
}
