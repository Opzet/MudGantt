using MudBlazor;
using MudGantt;
using MudGanttDemoApp.Web.GanttApi;

namespace MudGanttDemoApp.Web.Components.Pages;

public partial class Home
{
    private Color _color = Color.Primary;
    private Variant _variant = Variant.Filled;
    private Size _size = Size.Medium;
    private bool _readOnly;
    private bool _darkMode = true;
    private bool _dense;
    private bool _showSettings;
    private bool _collapseTaskList;
    private bool _attendanceMode;
    private MudGanttSynchronizedView _synchronizedView = default!;
    private MudGanttTask? _selectedTask;
    private HttpClient _apiClient = default!;
    private bool _isBusy;
    private string _search = string.Empty;
    private double _taskListWidthPercent = 15;
    private IReadOnlyList<MudGanttEvent> _events = HomePageSupport.CreateDefaultEvents();
    private IReadOnlyList<MudGanttTask> _tasks = HomePageSupport.CreateDefaultTasks();

    private IEnumerable<MudGanttTask> FilteredTasks => string.IsNullOrWhiteSpace(_search)
        ? _tasks
        : _tasks.Where(task => task.Name.Contains(_search, StringComparison.OrdinalIgnoreCase));

   
    private string TaskListPanelWidth => $"{_taskListWidthPercent:0}%";
    private string SynchronizedViewStyle => _selectedTask is null
        ? "height: clamp(420px, calc(100vh - 380px), 760px); min-height: 420px;"
        : "height: clamp(360px, calc(100vh - 520px), 620px); min-height: 360px;";
    private MudGanttLayoutMetrics ChartLayoutMetrics => _attendanceMode
        ? MudGanttLayoutMetrics.From(_dense, _size) with
        {
            AxisHeight = 44,
            TaskHeight = 40,
            TaskSpacing = 4
        }
        : MudGanttLayoutMetrics.From(_dense, _size);

    protected override async Task OnInitializedAsync()
    {
        _apiClient = HttpClientFactory.CreateClient();
        _apiClient.BaseAddress = new Uri(NavigationManager.BaseUri);
        await ReloadTasksAsync();
    }

    private Task ZoomIn() => _synchronizedView.ZoomInAsync();
    private Task ZoomOut() => _synchronizedView.ZoomOutAsync();
    private Task ResetZoom() => _synchronizedView.ResetZoomAsync();
    private Task ToggleSettings()
    {
        _showSettings = !_showSettings;
        return Task.CompletedTask;
    }

    private void ToggleTaskList() => _collapseTaskList = !_collapseTaskList;

    private void ToggleAttendanceMode()
    {
        _attendanceMode = !_attendanceMode;
        if (_attendanceMode)
        {
            _dense = true;
            _size = Size.Small;
            _collapseTaskList = false;
        }
        else
        {
            _dense = false;
            _size = Size.Medium;
        }
    }

    private async Task MakeTaskBlue(MudGanttTask task)
    {
        task.Color = "#1176D3";
        await SaveTaskAsync(task, $"'{task.Name}' is now blue!");
    }

    private async Task DeleteTaskAsync(MudGanttTask task)
    {
        var response = await _apiClient.DeleteAsync($"api/gantt/tasks/{task.Id}");
        if (!response.IsSuccessStatusCode)
        {
            snackbar.Add($"Could not delete '{task.Name}'", Severity.Error);
            return;
        }

        if (_selectedTask?.Id == task.Id)
        {
            _selectedTask = null;
        }

        _tasks = _tasks.Where(item => item.Id != task.Id).ToList();
        snackbar.Add($"Deleted '{task.Name}'", Severity.Warning);
    }

    private Task DeleteSelectedTaskAsync() => _selectedTask is null ? Task.CompletedTask : DeleteTaskAsync(_selectedTask);

    private async Task AddNewTaskAsync()
    {
        var newTask = new MudGanttTask
        {
            Id = string.Empty,
            Name = $"New Task {_tasks.Count + 1}",
            StartDate = DateTimeOffset.Now,
            EndDate = DateTimeOffset.Now.AddDays(3),
            Progress = 0.0
        };

        var response = await _apiClient.PostAsJsonAsync("api/gantt/tasks", HomePageSupport.ToDto(newTask));
        if (!response.IsSuccessStatusCode)
        {
            snackbar.Add("Could not create task", Severity.Error);
            return;
        }

        var created = await response.Content.ReadFromJsonAsync<GanttTaskDto>();
        if (created is null)
        {
            snackbar.Add("Task was created but response was empty", Severity.Warning);
            return;
        }

        var mapped = HomePageSupport.ToTask(created);
        _tasks = _tasks.Append(mapped).ToList();
        _selectedTask = mapped;
        snackbar.Add($"Added '{mapped.Name}'", Severity.Success);
    }

    private void OnTaskClicked(MudGanttTask task)
    {
        snackbar.Add($"'{task.Name}' clicked", Severity.Info);
    }

    private Task OnTaskDatesChanged(MudGanttTask task) => SaveTaskAsync(task, $"'{task.Name}' dates updated");
    private Task OnTaskProgressChanged(MudGanttTask task) => SaveTaskAsync(task, $"'{task.Name}' progress: {(task.Progress ?? 0):P0}");

    private void EditTaskDialog(MudGanttTask task)
    {
        SetSelectedTask(task);
        snackbar.Add($"Editing '{task.Name}' - Use the details panel below", Severity.Info);
    }

    private async Task DuplicateTaskAsync(MudGanttTask task)
    {
        var duplicatedTask = new MudGanttTask
        {
            Id = string.Empty,
            Name = $"{task.Name} (Copy)",
            StartDate = task.StartDate?.AddDays(1),
            EndDate = task.EndDate?.AddDays(1),
            Progress = task.Progress,
            Color = task.Color
        };

        var response = await _apiClient.PostAsJsonAsync("api/gantt/tasks", HomePageSupport.ToDto(duplicatedTask));
        if (!response.IsSuccessStatusCode)
        {
            snackbar.Add("Could not duplicate task", Severity.Error);
            return;
        }

        var created = await response.Content.ReadFromJsonAsync<GanttTaskDto>();
        if (created is null)
        {
            snackbar.Add("Duplicate response was empty", Severity.Warning);
            return;
        }

        var mapped = HomePageSupport.ToTask(created);
        _tasks = _tasks.Append(mapped).ToList();
        snackbar.Add($"Duplicated '{task.Name}'", Severity.Success);
    }

    private Task SaveSelectedTaskAsync() => _selectedTask is null ? Task.CompletedTask : SaveTaskAsync(_selectedTask, $"Saved '{_selectedTask.Name}'");

    private async Task ReloadTasksAsync()
    {
        _isBusy = true;
        try
        {
            var query = string.IsNullOrWhiteSpace(_search) ? string.Empty : $"?search={Uri.EscapeDataString(_search)}";
            var items = await _apiClient.GetFromJsonAsync<List<GanttTaskDto>>($"api/gantt/tasks{query}");
            if (items is not null)
            {
                _tasks = items.Select(HomePageSupport.ToTask).ToList();
                if (_selectedTask is not null)
                {
                    _selectedTask = _tasks.FirstOrDefault(task => task.Id == _selectedTask.Id);
                }
            }
        }
        catch
        {
            snackbar.Add("Failed to load tasks from API", Severity.Error);
        }
        finally
        {
            _isBusy = false;
        }
    }

    private async Task SaveTaskAsync(MudGanttTask task, string successMessage)
    {
        try
        {
            var response = await _apiClient.PutAsJsonAsync($"api/gantt/tasks/{task.Id}", HomePageSupport.ToDto(task));
            if (!response.IsSuccessStatusCode)
            {
                snackbar.Add($"Could not save '{task.Name}'", Severity.Error);
                return;
            }

            snackbar.Add(successMessage, Severity.Info);
        }
        catch
        {
            snackbar.Add($"Could not save '{task.Name}'", Severity.Error);
        }
    }

    private Task CloseTaskDetails()
    {
        SetSelectedTask(null);
        return Task.CompletedTask;
    }

    private void SetSelectedTask(MudGanttTask? task) => _selectedTask = task;

    private void ClearColors()
    {
        foreach (var task in _tasks)
        {
            task.Color = null;
        }

        snackbar.Add("Task colors cleared", Severity.Info);
    }

    private void AssignColors()
    {
        string[] palette = ["#5f0f40", "#9a031e", "#fb8b24", "#e36414", "#0f4c5c"];
        var offset = Random.Shared.Next();
        foreach (var item in _tasks.Index())
        {
            item.Item.Color = palette[(offset + item.Index) % palette.Length];
        }

        snackbar.Add("Random colors assigned", Severity.Success);
    }
}
