using Microsoft.JSInterop;

using MudGantt;

using MudGanttDemoApp.Web.ManufacturingApi;

namespace MudGanttDemoApp.Web.Components.Pages.MES;

public partial class ManufacturingFlow
{
    private const string AllClients = ManufacturingQueries.AllClients;
    private const string AllMachines = ManufacturingQueries.AllMachines;
    private MudGanttChart _jobsChart = default!;
    private MudGanttChart _resourcesChart = default!;
    private HttpClient _http = default!;
    private ManufacturingDatasetDto? _dataset;
    private IReadOnlyList<string> _clients = [];
    private IReadOnlyList<string> _machines = [];
    private string _selectedClient = AllClients;
    private string _selectedMachine = AllMachines;
    private string _search = string.Empty;
    private bool _criticalOnly;
    private bool _conflictsOnly;
    private bool _delayedOnly;
    private IReadOnlyList<MudGanttEvent> _holidays = [];
    private IReadOnlyList<ManufacturingViewPreset> _presets = [];
    private string? _selectedJobId;
    private string? _selectedSlotId;
    private readonly Dictionary<string, ManufacturingSimulationOverride> _whatIfOverrides = [];
    private string _presetName = string.Empty;
    private string _selectedPresetName = ManufacturingFlowSupport.CurrentFiltersPreset;

    private ManufacturingFilterState FilterState => new()
    {
        Client = _selectedClient,
        Machine = _selectedMachine,
        Search = _search,
        CriticalOnly = _criticalOnly,
        ConflictsOnly = _conflictsOnly,
        DelayedOnly = _delayedOnly
    };

    private IReadOnlyList<ManufacturingJobDto> FilteredJobs => ManufacturingQueries.FilterJobs(_dataset?.Jobs ?? [], FilterState);
    private IReadOnlyList<ManufacturingMachineSlotDto> FilteredSlots => ManufacturingQueries.FilterSlots(_dataset?.MachineSlots ?? [], FilterState);
    private string AverageProgress => (GanttTaskAnalysis.CalculateWeightedProgress(JobTasks) * 100).ToString("0.0");
    private double VisibleMachineUtilization => FilteredSlots.Count == 0 ? 0 : Math.Round(FilteredSlots.Count(slot => slot.Progress >= 0.7) * 100.0 / FilteredSlots.Count, 1);
    private ManufacturingJobDto? SelectedJob => EffectiveJobs.FirstOrDefault(job => job.Id == _selectedJobId);
    private IReadOnlyList<string> SelectedJobHighlightIds => string.IsNullOrWhiteSpace(_selectedJobId) ? [] : GanttTaskAnalysis.BuildDependencyChain(JobTasks, _selectedJobId);
    private IReadOnlyList<string> SelectedSlotHighlightIds => SelectedJobHighlightIds.Select(id => $"slot-{id.Replace("job-", string.Empty)}").ToList();
    private IReadOnlyList<MudGanttEvent> JobEvents => _holidays.Concat(ManufacturingViewSupport.BuildHeatmapRanges(FilteredSlots)).ToList();
    private IReadOnlyList<MudGanttEvent> MachineEvents => JobEvents;
    private IReadOnlyList<ManufacturingConflictSuggestion> ConflictSuggestions => SelectedJob is null ? [] : ManufacturingViewSupport.BuildConflictSuggestions(SelectedJob, _dataset?.MachineSlots ?? [], _dataset?.Resources ?? []);
    private IReadOnlyList<MudGanttRangeOverlay> MachineRangeOverlays => ManufacturingViewSupport.BuildRowHeatmapOverlays(EffectiveSlots).Concat(ManufacturingViewSupport.BuildWhatIfOverlays(_whatIfOverrides.Values.ToList())).ToList();
    private IReadOnlyList<MudGanttRangeOverlay> JobRangeOverlays => ManufacturingFlowSupport.BuildJobRangeOverlays(SelectedJob, WhatIfOverride);
    private ManufacturingSimulationOverride? WhatIfOverride => _selectedJobId is not null && _whatIfOverrides.TryGetValue(_selectedJobId, out var value) ? value : null;
    private IReadOnlyList<ManufacturingJobDto> DependencyTraceJobs => EffectiveJobs.Where(job => SelectedJobHighlightIds.Contains(job.Id)).OrderBy(job => job.StartDate).ToList();
    private IReadOnlyList<ManufacturingJobDto> EffectiveJobs => FilteredJobs.Select(job => ManufacturingFlowSupport.ApplyOverride(job, _whatIfOverrides.GetValueOrDefault(job.Id))).ToList();
    private IReadOnlyList<ManufacturingMachineSlotDto> EffectiveSlots => FilteredSlots.Select(slot => ManufacturingFlowSupport.ApplyOverride(slot, _whatIfOverrides.GetValueOrDefault(slot.JobId))).ToList();
    private IReadOnlyList<MudGanttTask> JobTasks => EffectiveJobs.Select(ManufacturingTaskMapper.ToClientJobTask).ToList();
    private IReadOnlyList<MudGanttTask> MachineTasks => EffectiveSlots.Select(ManufacturingTaskMapper.ToMachineTask).OrderBy(task => task.Name).ThenBy(task => task.StartDate).ToList();
    private bool DisableDeletePreset => string.IsNullOrWhiteSpace(_selectedPresetName) || string.Equals(_selectedPresetName, ManufacturingFlowSupport.CurrentFiltersPreset, StringComparison.Ordinal);

    protected override async Task OnInitializedAsync()
    {
        _http = HttpClientFactory.CreateClient();
        _http.BaseAddress = new Uri(NavigationManager.BaseUri);
        _dataset = await _http.GetFromJsonAsync<ManufacturingDatasetDto>("api/manufacturing/dataset?jobs=250");
        _holidays = await _http.GetFromJsonAsync<List<MudGanttEvent>>("api/manufacturing/holidays") ?? [];
        _clients = _dataset?.Clients ?? [];
        _machines = _dataset?.MachineSlots.Select(slot => slot.Machine).Distinct().OrderBy(machine => machine).ToList() ?? [];
        _presets = ManufacturingViewSupport.DeserializePresets(await JS.InvokeAsync<string?>("localStorage.getItem", ManufacturingFlowSupport.PresetsStorageKey));
        _selectedJobId = FilteredJobs.FirstOrDefault()?.Id;
        _selectedSlotId = FilteredSlots.FirstOrDefault()?.Id;
    }

    private void OnClientChanged(string client)
    {
        _selectedClient = client;
        _selectedPresetName = ManufacturingFlowSupport.CurrentFiltersPreset;
        EnsureSelectionVisible();
    }

    private void OnMachineChanged(string machine)
    {
        _selectedMachine = machine;
        _selectedPresetName = ManufacturingFlowSupport.CurrentFiltersPreset;
        EnsureSelectionVisible();
    }

    private void OnSearchChanged(string search)
    {
        _search = search;
        _selectedPresetName = ManufacturingFlowSupport.CurrentFiltersPreset;
        EnsureSelectionVisible();
    }

    private void OnCriticalOnlyChanged(bool value)
    {
        _criticalOnly = value;
        _selectedPresetName = ManufacturingFlowSupport.CurrentFiltersPreset;
        EnsureSelectionVisible();
    }

    private void OnConflictsOnlyChanged(bool value)
    {
        _conflictsOnly = value;
        _selectedPresetName = ManufacturingFlowSupport.CurrentFiltersPreset;
        EnsureSelectionVisible();
    }

    private void OnDelayedOnlyChanged(bool value)
    {
        _delayedOnly = value;
        _selectedPresetName = ManufacturingFlowSupport.CurrentFiltersPreset;
        EnsureSelectionVisible();
    }

    private void OnPresetNameChanged(string value) => _presetName = value;

    private void OnPresetChanged(string presetName)
    {
        _selectedPresetName = presetName;
        var preset = _presets.FirstOrDefault(item => item.Name == presetName);
        if (preset is null)
        {
            return;
        }

        _selectedClient = preset.Client;
        _selectedMachine = preset.Machine;
        _search = preset.Search;
        _criticalOnly = preset.CriticalOnly;
        _conflictsOnly = preset.ConflictsOnly;
        _delayedOnly = preset.DelayedOnly;
        EnsureSelectionVisible();
    }

    private Task OnJobTaskClicked(MudGanttTask task)
    {
        _selectedJobId = task.Id;
        _selectedSlotId = $"slot-{task.Id.Replace("job-", string.Empty)}";
        return Task.CompletedTask;
    }

    private Task OnMachineTaskClicked(MudGanttTask task)
    {
        _selectedSlotId = task.Id;
        _selectedJobId = task.RightLabel?.Replace("Job ", "job-");
        return Task.CompletedTask;
    }

    private void SelectJob(ManufacturingJobDto job)
    {
        _selectedJobId = job.Id;
        _selectedSlotId = $"slot-{job.Id.Replace("job-", string.Empty)}";
    }

    private void PreviewSuggestion(ManufacturingConflictSuggestion suggestion)
    {
        if (SelectedJob is null)
        {
            return;
        }

        _whatIfOverrides[SelectedJob.Id] = new ManufacturingSimulationOverride
        {
            JobId = SelectedJob.Id,
            RowTaskId = $"slot-{SelectedJob.Id.Replace("job-", string.Empty)}",
            Machine = suggestion.Machine,
            Worker = suggestion.Worker,
            StartDate = suggestion.StartDate,
            EndDate = suggestion.EndDate,
            DelayHours = suggestion.DelayHours
        };
    }

    private Task ClearWhatIf()
    {
        if (_selectedJobId is not null)
        {
            _whatIfOverrides.Remove(_selectedJobId);
        }

        return Task.CompletedTask;
    }

    private async Task SavePresetAsync()
    {
        var name = string.IsNullOrWhiteSpace(_presetName) ? $"View {_presets.Count + 1}" : _presetName.Trim();
        var updated = _presets.Where(preset => !string.Equals(preset.Name, name, StringComparison.OrdinalIgnoreCase)).ToList();
        updated.Add(ManufacturingFlowSupport.CreatePreset(name, FilterState));
        _presets = updated.OrderBy(preset => preset.Name).ToList();
        _selectedPresetName = name;
        _presetName = string.Empty;
        await JS.InvokeVoidAsync("localStorage.setItem", ManufacturingFlowSupport.PresetsStorageKey, ManufacturingViewSupport.SerializePresets(_presets));
    }

    private async Task DeletePresetAsync()
    {
        if (string.IsNullOrWhiteSpace(_selectedPresetName))
        {
            return;
        }

        _presets = _presets.Where(preset => !string.Equals(preset.Name, _selectedPresetName, StringComparison.OrdinalIgnoreCase)).ToList();
        _selectedPresetName = ManufacturingFlowSupport.CurrentFiltersPreset;
        await JS.InvokeVoidAsync("localStorage.setItem", ManufacturingFlowSupport.PresetsStorageKey, ManufacturingViewSupport.SerializePresets(_presets));
    }

    private void EnsureSelectionVisible()
    {
        if (_selectedJobId is not null && FilteredJobs.All(job => job.Id != _selectedJobId))
        {
            _selectedJobId = FilteredJobs.FirstOrDefault()?.Id;
        }

        if (_selectedSlotId is not null && FilteredSlots.All(slot => slot.Id != _selectedSlotId))
        {
            _selectedSlotId = FilteredSlots.FirstOrDefault()?.Id;
        }
    }

    private async Task ZoomIn()
    {
        await _jobsChart.ZoomInAsync(20);
        await _resourcesChart.ZoomInAsync(20);
    }

    private async Task ZoomOut()
    {
        await _jobsChart.ZoomOutAsync(20);
        await _resourcesChart.ZoomOutAsync(20);
    }

    private async Task ResetZoom()
    {
        await _jobsChart.ResetZoomAsync();
        await _resourcesChart.ResetZoomAsync();
    }
}
