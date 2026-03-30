using MudBlazor;

using MudGantt;

using MudGanttDemoApp.Web.SchedulerApi;

namespace MudGanttDemoApp.Web.Components.Pages.MES;

public partial class RosterAdvanced
{
    private readonly HashSet<string> _selectedResourceIds = [];
    private readonly List<RotationDay> _previewDays = [];
    private readonly List<MudGanttTask> _appliedAssignments = [];
    private SchedulerProjectDto? _project;
    private DateTime? _cycleStartDate = DateTime.Today;
    private int _horizonDays = 14;
    private RotationTemplate _rotationTemplate = RotationTemplate.SevenOnSevenOff;
    private string _selectedRole = RosterAdvancedSupport.AllRoles;

    private IReadOnlyList<string> AvailableRoles => _project?.Resources.Select(resource => resource.Role).Distinct().OrderBy(role => role).ToList() ?? [];
    private IReadOnlyList<SchedulerResourceDto> FilteredResources => _project?.Resources.Where(resource => _selectedRole == RosterAdvancedSupport.AllRoles || resource.Role == _selectedRole).OrderBy(resource => resource.Name).ToList() ?? [];
    private IReadOnlyList<PreviewResourceRow> PreviewRows => RosterAdvancedSupport.BuildPreviewRows(FilteredResources, _selectedResourceIds, _previewDays);
    private IReadOnlyList<MudGanttTask> PreviewTasks => RosterAdvancedSupport.BuildPreviewTasks(FilteredResources, _selectedResourceIds, _previewDays);
    private string PreviewHours => (PreviewTasks.Count * 12).ToString();

    protected override async Task OnInitializedAsync()
    {
        _project = await SchedulerProjectStore.GetProjectAsync(CancellationToken.None);
        foreach (var resource in _project.Resources.Take(4))
        {
            _selectedResourceIds.Add(resource.Id);
        }

        RebuildPreview();
    }

    private void OnResourcesChanged(IEnumerable<string> resourceIds)
    {
        _selectedResourceIds.Clear();
        foreach (var resourceId in resourceIds)
        {
            _selectedResourceIds.Add(resourceId);
        }
    }

    private void RebuildPreview()
    {
        _previewDays.Clear();
        var start = (_cycleStartDate ?? DateTime.Today).Date;
        var days = Math.Clamp(_horizonDays, 7, 35);
        for (var offset = 0; offset < days; offset++)
        {
            _previewDays.Add(RosterAdvancedSupport.BuildRotationDay(_rotationTemplate, DateOnly.FromDateTime(start.AddDays(offset)), offset));
        }
    }

    private Task ApplyRotationAsync()
    {
        _appliedAssignments.Clear();
        _appliedAssignments.AddRange(PreviewTasks);
        Snackbar.Add($"Applied {_appliedAssignments.Count} demo roster assignments.", Severity.Success);
        return Task.CompletedTask;
    }

    private static string GetTemplateLabel(RotationTemplate template) => RosterAdvancedSupport.GetTemplateLabel(template);
}
