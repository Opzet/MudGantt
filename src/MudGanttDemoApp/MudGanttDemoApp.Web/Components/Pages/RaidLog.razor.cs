using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudGantt;

namespace MudGanttDemoApp.Web.Components.Pages;

public partial class RaidLog
{
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    private List<MudGanttRisk> _risks = ProjectGanttSampleData.CreateRisks();

    private RaidItemType? _typeFilter = null;
    private RiskStatus? _statusFilter = null;
    private bool _drawerOpen = false;
    private MudGanttRisk? _selectedRisk;
    private MudGanttRisk _editRisk = new();

    private string _sortCol = "score";
    private bool _sortDesc = true;

    // ── counts ─────────────────────────────────────────────────────────────
    private int OpenCount => _risks.Count(r => r.Status == RiskStatus.Open);
    private int InReviewCount => _risks.Count(r => r.Status == RiskStatus.InReview);
    private int ClosedCount => _risks.Count(r => r.Status == RiskStatus.Closed);

    // ── filtered + sorted list ──────────────────────────────────────────────
    private IEnumerable<MudGanttRisk> FilteredRisks
    {
        get
        {
            var q = _risks.AsEnumerable();
            if (_typeFilter.HasValue) q = q.Where(r => r.Type == _typeFilter.Value);
            if (_statusFilter.HasValue) q = q.Where(r => r.Status == _statusFilter.Value);
            q = _sortCol switch
            {
                "name" => _sortDesc ? q.OrderByDescending(r => r.Name) : q.OrderBy(r => r.Name),
                "priority" => _sortDesc ? q.OrderByDescending(r => r.Priority) : q.OrderBy(r => r.Priority),
                "impact" => _sortDesc ? q.OrderByDescending(r => r.Impact) : q.OrderBy(r => r.Impact),
                "likelihood" => _sortDesc ? q.OrderByDescending(r => r.Likelihood) : q.OrderBy(r => r.Likelihood),
                "due" => _sortDesc ? q.OrderByDescending(r => r.DueDate) : q.OrderBy(r => r.DueDate),
                _ => _sortDesc ? q.OrderByDescending(r => r.RiskScore) : q.OrderBy(r => r.RiskScore)
            };
            return q;
        }
    }

    private void Sort(string col)
    {
        if (_sortCol == col) _sortDesc = !_sortDesc;
        else { _sortCol = col; _sortDesc = true; }
    }

    private void OpenCard(MudGanttRisk risk)
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
        _drawerOpen = true;
    }

    private void CloseCard() => _drawerOpen = false;

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
        OpenCard(risk);
    }

    private void SaveRisk()
    {
        var idx = _risks.FindIndex(r => r.Id == _editRisk.Id);
        if (idx < 0) return;
        _risks[idx] = _editRisk;
        _risks = new List<MudGanttRisk>(_risks);
        _drawerOpen = false;
        Snackbar.Add($"'{_editRisk.Name}' saved.", Severity.Success);
    }

    private void DeleteRisk()
    {
        if (_selectedRisk is null) return;
        _risks.Remove(_selectedRisk);
        _risks = new List<MudGanttRisk>(_risks);
        _drawerOpen = false;
        _selectedRisk = null;
        Snackbar.Add("Item deleted.", Severity.Warning);
    }

    // ── CSS helpers ─────────────────────────────────────────────────────────
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

    private static string GetPriorityClass(RiskPriority p) => p switch
    {
        RiskPriority.Critical => "pm-priority-critical",
        RiskPriority.High => "pm-priority-high",
        RiskPriority.Medium => "pm-priority-medium",
        _ => "pm-priority-low"
    };

    private string SortIcon(string col) =>
        _sortCol != col ? Icons.Material.Filled.UnfoldMore :
        _sortDesc ? Icons.Material.Filled.ArrowDownward : Icons.Material.Filled.ArrowUpward;
}
