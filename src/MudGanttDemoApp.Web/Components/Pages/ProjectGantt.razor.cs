using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudGantt;

namespace MudGanttDemoApp.Web.Components.Pages;

public partial class ProjectGantt
{
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    // ── data ─────────────────────────────────────────────────────────────────
    private List<MudGanttTask> _tasks = ProjectGanttSampleData.CreateTasks();
    private List<MudGanttEvent> _events = ProjectGanttSampleData.CreateEvents();
    private List<MudGanttRisk> _risks = ProjectGanttSampleData.CreateRisks();

    // ── view state ───────────────────────────────────────────────────────────
    private int _activeView = 0;               // 0=Gantt,1=Sheet,2=Board,3=Risk
    private bool _showCriticalPath = false;
    private bool _showBaseline = false;
    private bool _drawerOpen = false;
    private string _search = string.Empty;
    private string _statusFilter = string.Empty;

    // ── gantt sync view ref ───────────────────────────────────────────────────
    private MudGanttSynchronizedView? _syncView;

    // ── layout ───────────────────────────────────────────────────────────────
    public MudGanttLayoutMetrics LayoutMetrics { get; } = new(50, 44, 6);

    // ── selected items ────────────────────────────────────────────────────────
    private MudGanttTask? _selectedTask;
    private MudGanttRisk? _selectedRisk;

    // ── edit buffers ─────────────────────────────────────────────────────────
    private MudGanttTask _editTask = new() { Id = string.Empty, Name = string.Empty };
    private MudGanttRisk _editRisk = new();

    private DateTime? EditStartDate
    {
        get => _editTask.StartDate?.LocalDateTime;
        set => _editTask.StartDate = value.HasValue ? new DateTimeOffset(value.Value) : null;
    }

    private DateTime? EditEndDate
    {
        get => _editTask.EndDate?.LocalDateTime;
        set => _editTask.EndDate = value.HasValue ? new DateTimeOffset(value.Value) : null;
    }

    // ── filtered/sorted task list ─────────────────────────────────────────────
    private IEnumerable<MudGanttTask> DisplayedTasks => _tasks
        .Where(t =>
            (string.IsNullOrEmpty(_search) ||
             t.Name.Contains(_search, StringComparison.OrdinalIgnoreCase) ||
             (t.WbsNumber?.Contains(_search) ?? false)) &&
            (string.IsNullOrEmpty(_statusFilter) ||
             string.Equals(t.Status, _statusFilter, StringComparison.OrdinalIgnoreCase)));

    private IEnumerable<MudGanttTask> GanttTasks =>
        _showCriticalPath
            ? DisplayedTasks.Where(t => t.IsCritical || t.IsPhase)
            : DisplayedTasks;

    private IEnumerable<MudGanttTask> HighlightedTasks =>
        _showCriticalPath ? DisplayedTasks.Where(t => t.IsCritical).Select(t => t) : [];

    private IReadOnlyList<string>? HighlightedTaskIds =>
        _showCriticalPath ? HighlightedTasks.Select(t => t.Id).ToList() : null;

    // ── gantt zoom ───────────────────────────────────────────────────────────
    private async Task ZoomInAsync()
    {
        if (_syncView is not null) await _syncView.ZoomInAsync();
    }
    private async Task ZoomOutAsync()
    {
        if (_syncView is not null) await _syncView.ZoomOutAsync();
    }
    private async Task ResetZoomAsync()
    {
        if (_syncView is not null) await _syncView.ResetZoomAsync();
    }

    // ── task operations ───────────────────────────────────────────────────────
    private void OnTaskSelected(MudGanttTask task)
    {
        _selectedTask = task;
    }

    private void OpenTaskDrawer(MudGanttTask task)
    {
        _editTask = new MudGanttTask
        {
            Id = task.Id,
            Name = task.Name,
            StartDate = task.StartDate,
            EndDate = task.EndDate,
            Progress = task.Progress,
            Status = task.Status,
            WbsNumber = task.WbsNumber,
            AssigneeName = task.AssigneeName,
            AssigneeInitials = task.AssigneeInitials,
            AssigneeAvatarColor = task.AssigneeAvatarColor,
            IndentLevel = task.IndentLevel,
            IsPhase = task.IsPhase,
            IsCritical = task.IsCritical,
            EstimatedHours = task.EstimatedHours,
            ActualHours = task.ActualHours,
            Color = task.Color,
            Links = task.Links
        };
        _selectedTask = task;
        _selectedRisk = null;
        _drawerOpen = true;
    }

    private void SaveTask()
    {
        var idx = _tasks.FindIndex(t => t.Id == _editTask.Id);
        if (idx < 0) return;
        var existing = _tasks[idx];
        existing.Name = _editTask.Name;
        existing.StartDate = _editTask.StartDate;
        existing.EndDate = _editTask.EndDate;
        existing.Progress = _editTask.Progress;
        existing.Status = _editTask.Status;
        existing.AssigneeName = _editTask.AssigneeName;
        existing.EstimatedHours = _editTask.EstimatedHours;
        existing.ActualHours = _editTask.ActualHours;
        _tasks = new List<MudGanttTask>(_tasks);
        _drawerOpen = false;
        Snackbar.Add($"Task '{existing.Name}' updated.", Severity.Success);
    }

    private void AddTask()
    {
        var newTask = new MudGanttTask
        {
            Id = Guid.NewGuid().ToString("N")[..8],
            Name = "New Task",
            WbsNumber = (_tasks.Count + 1).ToString(),
            StartDate = DateTimeOffset.Now,
            EndDate = DateTimeOffset.Now.AddDays(5),
            Progress = 0.0,
            Status = "Not Started",
            Color = "#6366f1",
            IndentLevel = 1
        };
        _tasks.Add(newTask);
        _tasks = new List<MudGanttTask>(_tasks);
        OpenTaskDrawer(newTask);
        Snackbar.Add("New task added.", Severity.Info);
    }

    private void AddPhase()
    {
        var phase = new MudGanttTask
        {
            Id = Guid.NewGuid().ToString("N")[..8],
            Name = $"Phase {_tasks.Count(t => t.IsPhase) + 1}",
            WbsNumber = (_tasks.Count(t => t.IsPhase) + 1).ToString(),
            IsPhase = true,
            IndentLevel = 0,
            StartDate = DateTimeOffset.Now,
            EndDate = DateTimeOffset.Now.AddDays(14),
            Progress = 0.0,
            Status = "Not Started",
            Color = "#0891b2"
        };
        _tasks.Add(phase);
        _tasks = new List<MudGanttTask>(_tasks);
        Snackbar.Add("New phase added.", Severity.Info);
    }

    private void DeleteTask()
    {
        if (_selectedTask is null) return;
        _tasks.Remove(_selectedTask);
        _tasks = new List<MudGanttTask>(_tasks);
        _drawerOpen = false;
        _selectedTask = null;
        Snackbar.Add("Task deleted.", Severity.Warning);
    }

    private void IndentTask()
    {
        if (_selectedTask is null) return;
        _selectedTask.IndentLevel = Math.Min(3, _selectedTask.IndentLevel + 1);
        _tasks = new List<MudGanttTask>(_tasks);
    }

    private void OutdentTask()
    {
        if (_selectedTask is null) return;
        _selectedTask.IndentLevel = Math.Max(0, _selectedTask.IndentLevel - 1);
        _tasks = new List<MudGanttTask>(_tasks);
    }

    // ── risk operations ───────────────────────────────────────────────────────
    private void OpenRiskDrawer(MudGanttRisk risk)
    {
        _editRisk = new MudGanttRisk
        {
            Id = risk.Id,
            Name = risk.Name,
            Type = risk.Type,
            Priority = risk.Priority,
            Impact = risk.Impact,
            Likelihood = risk.Likelihood,
            Response = risk.Response,
            Status = risk.Status,
            Description = risk.Description,
            Resolution = risk.Resolution,
            DueDate = risk.DueDate,
            AssigneeName = risk.AssigneeName,
            AssigneeInitials = risk.AssigneeInitials,
            AssigneeAvatarColor = risk.AssigneeAvatarColor,
            Tags = risk.Tags,
            CreatedDate = risk.CreatedDate
        };
        _selectedRisk = risk;
        _selectedTask = null;
        _drawerOpen = true;
    }

    private void SaveRisk()
    {
        var idx = _risks.FindIndex(r => r.Id == _editRisk.Id);
        if (idx < 0) return;
        _risks[idx] = _editRisk;
        _risks = new List<MudGanttRisk>(_risks);
        _drawerOpen = false;
        Snackbar.Add($"Risk '{_editRisk.Name}' updated.", Severity.Success);
    }

    private void AddRisk()
    {
        var risk = new MudGanttRisk
        {
            Name = "New Risk",
            Type = RaidItemType.Risk,
            Priority = RiskPriority.Medium,
            Impact = RiskLevel.Medium,
            Likelihood = RiskLevel.Medium,
            Response = RiskResponse.Mitigate,
            Status = RiskStatus.Open
        };
        _risks.Add(risk);
        OpenRiskDrawer(risk);
    }

    private void DeleteRisk()
    {
        if (_selectedRisk is null) return;
        _risks.Remove(_selectedRisk);
        _risks = new List<MudGanttRisk>(_risks);
        _drawerOpen = false;
        _selectedRisk = null;
        Snackbar.Add("Risk deleted.", Severity.Warning);
    }

    private void CloseDrawer() => _drawerOpen = false;

    // ── helpers ───────────────────────────────────────────────────────────────
    private static string GetStatusClass(string? status) => status?.Replace(" ", "-").ToLowerInvariant() switch
    {
        "complete" => "pm-s-complete",
        "in-progress" => "pm-s-in-progress",
        "on-hold" => "pm-s-on-hold",
        "at-risk" => "pm-s-at-risk",
        _ => "pm-s-not-started"
    };

    private static int GetDuration(MudGanttTask task)
    {
        if (task.StartDate is null || task.EndDate is null) return 0;
        return (int)Math.Ceiling((task.EndDate.Value - task.StartDate.Value).TotalDays);
    }

    private static string GetMatrixCellClass(RiskLevel impact, RiskLevel likelihood, MudGanttRisk? highlight = null)
    {
        var cls = (impact, likelihood) switch
        {
            (RiskLevel.High, RiskLevel.High) => "pm-cell-critical",
            (RiskLevel.High, RiskLevel.Medium) or (RiskLevel.Medium, RiskLevel.High) => "pm-cell-high",
            (RiskLevel.High, RiskLevel.Low) or (RiskLevel.Low, RiskLevel.High) or (RiskLevel.Medium, RiskLevel.Medium) => "pm-cell-medium",
            _ => "pm-cell-low"
        };
        if (highlight is not null && highlight.Impact == impact && highlight.Likelihood == likelihood)
            cls += " pm-cell-active";
        return cls;
    }

    private static string GetRaidTypeInitial(RaidItemType type) => type switch
    {
        RaidItemType.Assumption => "A",
        RaidItemType.Issue => "I",
        RaidItemType.Decision => "D",
        _ => "R"
    };

    private static string GetRaidTypeClass(RaidItemType type) => type switch
    {
        RaidItemType.Assumption => "pm-raid-assumption",
        RaidItemType.Issue => "pm-raid-issue",
        RaidItemType.Decision => "pm-raid-decision",
        _ => "pm-raid-risk"
    };

    private static string GetScoreClass(int score) => score switch
    {
        <= 2 => "pm-score-low",
        <= 4 => "pm-score-medium",
        <= 6 => "pm-score-high",
        _ => "pm-score-critical"
    };

    private static string GetProgressColor(MudGanttTask task) => task.Progress switch
    {
        >= 1.0 => "#4caf50",
        >= 0.5 => "#2196f3",
        _ => "#ff9800"
    };
}
