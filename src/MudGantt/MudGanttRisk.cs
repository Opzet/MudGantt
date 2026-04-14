namespace MudGantt;

/// <summary>Represents a single entry in a project RAID log (Risk, Assumption, Issue, or Decision).</summary>
public class MudGanttRisk
{
    /// <summary>Unique identifier.</summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];

    /// <summary>Short title.</summary>
    public string Name { get; set; } = "New Risk";

    /// <summary>RAID category.</summary>
    public RaidItemType Type { get; set; } = RaidItemType.Risk;

    /// <summary>Priority level.</summary>
    public RiskPriority Priority { get; set; } = RiskPriority.Medium;

    /// <summary>Impact if the risk is realized.</summary>
    public RiskLevel Impact { get; set; } = RiskLevel.Medium;

    /// <summary>Likelihood of occurrence.</summary>
    public RiskLevel Likelihood { get; set; } = RiskLevel.Medium;

    /// <summary>Planned response strategy.</summary>
    public RiskResponse Response { get; set; } = RiskResponse.Mitigate;

    /// <summary>Current workflow status.</summary>
    public RiskStatus Status { get; set; } = RiskStatus.Open;

    /// <summary>Detailed description.</summary>
    public string? Description { get; set; }

    /// <summary>Resolution notes (populated when closed).</summary>
    public string? Resolution { get; set; }

    /// <summary>Due date for response action.</summary>
    public DateTimeOffset? DueDate { get; set; }

    /// <summary>Display name of the owner/assignee.</summary>
    public string? AssigneeName { get; set; }

    /// <summary>Initials for the assignee avatar.</summary>
    public string? AssigneeInitials { get; set; }

    /// <summary>Background colour for the assignee avatar.</summary>
    public string? AssigneeAvatarColor { get; set; }

    /// <summary>Optional tags for filtering and reporting.</summary>
    public string[]? Tags { get; set; }

    /// <summary>Date the item was logged.</summary>
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>Computed numeric risk score: (Impact+1) × (Likelihood+1), range 1–9.</summary>
    public int RiskScore => ((int)Impact + 1) * ((int)Likelihood + 1);

    /// <summary>Human-readable risk score band.</summary>
    public string RiskScoreLabel => RiskScore switch
    {
        <= 2 => "Low",
        <= 4 => "Medium",
        <= 6 => "High",
        _ => "Critical"
    };
}

/// <summary>RAID item category.</summary>
public enum RaidItemType { Risk, Assumption, Issue, Decision }

/// <summary>Priority bands.</summary>
public enum RiskPriority { Low, Medium, High, Critical }

/// <summary>Impact or likelihood severity.</summary>
public enum RiskLevel { Low, Medium, High }

/// <summary>Response strategy for a risk.</summary>
public enum RiskResponse { Avoid, Mitigate, Transfer, Accept }

/// <summary>Workflow status of a RAID item.</summary>
public enum RiskStatus { Open, InReview, Closed }
