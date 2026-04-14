using MudGantt;

namespace MudGanttDemoApp.Web.Components.Pages;

internal static class ProjectGanttSampleData
{
    private static DateTimeOffset Anchor => new DateTimeOffset(DateTime.Today).AddDays(-14);
    private static DateTimeOffset At(int dayOffset, int hour = 8) =>
        Anchor.AddDays(dayOffset).Date.AddHours(hour);

    public static List<MudGanttTask> CreateTasks()
    {
        return
        [
            // ── Phase 1: Discovery ───────────────────────────────────────────
            new MudGanttTask
            {
                Id = "ph1", Name = "Phase 1 — Discovery",
                WbsNumber = "1", IndentLevel = 0, IsPhase = true,
                StartDate = At(0), EndDate = At(7, 17),
                Progress = 1.0, Status = "Complete", Color = "#4f46e5",
                BaselineStartDate = At(0), BaselineEndDate = At(7, 17),
                EstimatedHours = 80, ActualHours = 78
            },
            new MudGanttTask
            {
                Id = "t1_1", Name = "Requirements Gathering",
                WbsNumber = "1.1", IndentLevel = 1, ParentId = "ph1",
                StartDate = At(0), EndDate = At(3, 17),
                Progress = 1.0, Status = "Complete", Color = "#6366f1",
                AssigneeName = "Alice Park", AssigneeInitials = "AP", AssigneeAvatarColor = "#4caf50",
                BaselineStartDate = At(0), BaselineEndDate = At(3, 17),
                EstimatedHours = 24, ActualHours = 22
            },
            new MudGanttTask
            {
                Id = "t1_2", Name = "Stakeholder Interviews",
                WbsNumber = "1.2", IndentLevel = 1, ParentId = "ph1",
                StartDate = At(2), EndDate = At(5, 17),
                Progress = 1.0, Status = "Complete", Color = "#6366f1",
                AssigneeName = "Bob Jensen", AssigneeInitials = "BJ", AssigneeAvatarColor = "#2196f3",
                Links = [new Link("t1_1", LinkType.FinishToStart)],
                BaselineStartDate = At(2), BaselineEndDate = At(5, 17),
                EstimatedHours = 32, ActualHours = 35
            },
            new MudGanttTask
            {
                Id = "t1_3", Name = "Technical Assessment",
                WbsNumber = "1.3", IndentLevel = 1, ParentId = "ph1",
                StartDate = At(4), EndDate = At(7, 17),
                Progress = 1.0, Status = "Complete", Color = "#6366f1",
                AssigneeName = "Carol Liu", AssigneeInitials = "CL", AssigneeAvatarColor = "#e91e63",
                Links = [new Link("t1_2", LinkType.FinishToStart)],
                BaselineStartDate = At(4), BaselineEndDate = At(7, 17),
                EstimatedHours = 24, ActualHours = 21
            },

            // ── Phase 2: Design ──────────────────────────────────────────────
            new MudGanttTask
            {
                Id = "ph2", Name = "Phase 2 — Design",
                WbsNumber = "2", IndentLevel = 0, IsPhase = true,
                StartDate = At(7), EndDate = At(21, 17),
                Progress = 0.6, Status = "In Progress", Color = "#0891b2",
                Links = [new Link("ph1", LinkType.FinishToStart)],
                BaselineStartDate = At(7), BaselineEndDate = At(19, 17),
                EstimatedHours = 120, ActualHours = 52
            },
            new MudGanttTask
            {
                Id = "t2_1", Name = "UX Wireframes",
                WbsNumber = "2.1", IndentLevel = 1, ParentId = "ph2",
                StartDate = At(7), EndDate = At(12, 17),
                Progress = 1.0, Status = "Complete", Color = "#06b6d4",
                AssigneeName = "Alice Park", AssigneeInitials = "AP", AssigneeAvatarColor = "#4caf50",
                Links = [new Link("ph1", LinkType.FinishToStart)],
                BaselineStartDate = At(7), BaselineEndDate = At(11, 17),
                EstimatedHours = 40, ActualHours = 44
            },
            new MudGanttTask
            {
                Id = "t2_2", Name = "System Architecture",
                WbsNumber = "2.2", IndentLevel = 1, ParentId = "ph2",
                StartDate = At(10), EndDate = At(17, 17),
                Progress = 0.5, Status = "In Progress", Color = "#06b6d4",
                AssigneeName = "David Mills", AssigneeInitials = "DM", AssigneeAvatarColor = "#ff5722",
                Links = [new Link("t2_1", LinkType.FinishToStart)],
                BaselineStartDate = At(10), BaselineEndDate = At(16, 17),
                IsCritical = true,
                EstimatedHours = 48, ActualHours = 20
            },
            new MudGanttTask
            {
                Id = "t2_3", Name = "Database Schema",
                WbsNumber = "2.3", IndentLevel = 1, ParentId = "ph2",
                StartDate = At(12), EndDate = At(17, 17),
                Progress = 0.4, Status = "In Progress", Color = "#06b6d4",
                AssigneeName = "Eve Patel", AssigneeInitials = "EP", AssigneeAvatarColor = "#9c27b0",
                Links = [new Link("t2_1", LinkType.FinishToStart)],
                BaselineStartDate = At(12), BaselineEndDate = At(16, 17),
                EstimatedHours = 32, ActualHours = 10
            },

            // ── Phase 3: Development ─────────────────────────────────────────
            new MudGanttTask
            {
                Id = "ph3", Name = "Phase 3 — Development",
                WbsNumber = "3", IndentLevel = 0, IsPhase = true,
                StartDate = At(21), EndDate = At(49, 17),
                Progress = 0.0, Status = "Not Started", Color = "#16a34a",
                Links = [new Link("ph2", LinkType.FinishToStart)],
                BaselineStartDate = At(19), BaselineEndDate = At(47, 17),
                IsCritical = true,
                EstimatedHours = 320, ActualHours = 0
            },
            new MudGanttTask
            {
                Id = "t3_1", Name = "Backend API",
                WbsNumber = "3.1", IndentLevel = 1, ParentId = "ph3",
                StartDate = At(21), EndDate = At(35, 17),
                Progress = 0.0, Status = "Not Started", Color = "#22c55e",
                AssigneeName = "David Mills", AssigneeInitials = "DM", AssigneeAvatarColor = "#ff5722",
                Links = [new Link("t2_2", LinkType.FinishToStart), new Link("t2_3", LinkType.FinishToStart)],
                BaselineStartDate = At(19), BaselineEndDate = At(33, 17),
                IsCritical = true,
                EstimatedHours = 120, ActualHours = 0
            },
            new MudGanttTask
            {
                Id = "t3_2", Name = "Frontend UI",
                WbsNumber = "3.2", IndentLevel = 1, ParentId = "ph3",
                StartDate = At(28), EndDate = At(42, 17),
                Progress = 0.0, Status = "Not Started", Color = "#22c55e",
                AssigneeName = "Alice Park", AssigneeInitials = "AP", AssigneeAvatarColor = "#4caf50",
                Links = [new Link("t3_1", LinkType.StartToStart)],
                BaselineStartDate = At(26), BaselineEndDate = At(40, 17),
                IsCritical = true,
                EstimatedHours = 120, ActualHours = 0
            },
            new MudGanttTask
            {
                Id = "t3_3", Name = "Third-Party Integrations",
                WbsNumber = "3.3", IndentLevel = 1, ParentId = "ph3",
                StartDate = At(35), EndDate = At(42, 17),
                Progress = 0.0, Status = "Not Started", Color = "#22c55e",
                AssigneeName = "Frank Ho", AssigneeInitials = "FH", AssigneeAvatarColor = "#00bcd4",
                Links = [new Link("t3_1", LinkType.FinishToStart)],
                BaselineStartDate = At(33), BaselineEndDate = At(40, 17),
                EstimatedHours = 56, ActualHours = 0
            },
            new MudGanttTask
            {
                Id = "t3_4", Name = "Data Migration Scripts",
                WbsNumber = "3.4", IndentLevel = 1, ParentId = "ph3",
                StartDate = At(35), EndDate = At(49, 17),
                Progress = 0.0, Status = "Not Started", Color = "#22c55e",
                AssigneeName = "Eve Patel", AssigneeInitials = "EP", AssigneeAvatarColor = "#9c27b0",
                Links = [new Link("t3_1", LinkType.FinishToStart)],
                BaselineStartDate = At(33), BaselineEndDate = At(47, 17),
                EstimatedHours = 40, ActualHours = 0
            },

            // ── Phase 4: Testing & Launch ────────────────────────────────────
            new MudGanttTask
            {
                Id = "ph4", Name = "Phase 4 — Testing & Launch",
                WbsNumber = "4", IndentLevel = 0, IsPhase = true,
                StartDate = At(49), EndDate = At(63, 17),
                Progress = 0.0, Status = "Not Started", Color = "#dc2626",
                Links = [new Link("ph3", LinkType.FinishToStart)],
                BaselineStartDate = At(47), BaselineEndDate = At(61, 17),
                IsCritical = true,
                EstimatedHours = 120, ActualHours = 0
            },
            new MudGanttTask
            {
                Id = "t4_1", Name = "QA Testing",
                WbsNumber = "4.1", IndentLevel = 1, ParentId = "ph4",
                StartDate = At(49), EndDate = At(56, 17),
                Progress = 0.0, Status = "Not Started", Color = "#ef4444",
                AssigneeName = "Bob Jensen", AssigneeInitials = "BJ", AssigneeAvatarColor = "#2196f3",
                Links = [new Link("t3_2", LinkType.FinishToStart), new Link("t3_3", LinkType.FinishToStart)],
                BaselineStartDate = At(47), BaselineEndDate = At(54, 17),
                EstimatedHours = 48, ActualHours = 0
            },
            new MudGanttTask
            {
                Id = "t4_2", Name = "Performance Testing",
                WbsNumber = "4.2", IndentLevel = 1, ParentId = "ph4",
                StartDate = At(49), EndDate = At(54, 17),
                Progress = 0.0, Status = "Not Started", Color = "#ef4444",
                AssigneeName = "Carol Liu", AssigneeInitials = "CL", AssigneeAvatarColor = "#e91e63",
                Links = [new Link("t3_4", LinkType.FinishToStart)],
                BaselineStartDate = At(47), BaselineEndDate = At(52, 17),
                EstimatedHours = 24, ActualHours = 0
            },
            new MudGanttTask
            {
                Id = "t4_3", Name = "User Acceptance Testing",
                WbsNumber = "4.3", IndentLevel = 1, ParentId = "ph4",
                StartDate = At(56), EndDate = At(61, 17),
                Progress = 0.0, Status = "Not Started", Color = "#ef4444",
                AssigneeName = "Alice Park", AssigneeInitials = "AP", AssigneeAvatarColor = "#4caf50",
                Links = [new Link("t4_1", LinkType.FinishToStart)],
                BaselineStartDate = At(54), BaselineEndDate = At(59, 17),
                IsCritical = true,
                EstimatedHours = 32, ActualHours = 0
            },
            new MudGanttTask
            {
                Id = "t4_4", Name = "Production Deployment",
                WbsNumber = "4.4", IndentLevel = 1, ParentId = "ph4",
                StartDate = At(61), EndDate = At(63, 17),
                Progress = 0.0, Status = "Not Started", Color = "#ef4444",
                AssigneeName = "Frank Ho", AssigneeInitials = "FH", AssigneeAvatarColor = "#00bcd4",
                Links = [new Link("t4_3", LinkType.FinishToStart)],
                BaselineStartDate = At(59), BaselineEndDate = At(61, 17),
                IsCritical = true,
                EstimatedHours = 16, ActualHours = 0,
                RightLabel = "Go-Live 🚀"
            },
        ];
    }

    public static List<MudGanttEvent> CreateEvents()
    {
        return
        [
            new MudGanttEvent { Id = "ev_kickoff",  Name = "Kickoff",          Date = At(0, 9) },
            new MudGanttEvent { Id = "ev_design",   Name = "Design Review",    Date = At(19, 14) },
            new MudGanttEvent { Id = "ev_freeze",   Name = "Code Freeze",      Date = At(49, 9) },
            new MudGanttEvent { Id = "ev_golive",   Name = "Go-Live",          Date = At(63, 12) },
        ];
    }

    public static List<MudGanttRisk> CreateRisks()
    {
        return
        [
            new MudGanttRisk
            {
                Id = "r1",
                Name = "Key developer unavailable during critical path",
                Type = RaidItemType.Risk,
                Priority = RiskPriority.High,
                Impact = RiskLevel.High,
                Likelihood = RiskLevel.Medium,
                Response = RiskResponse.Mitigate,
                Status = RiskStatus.Open,
                AssigneeName = "Alice Park", AssigneeInitials = "AP", AssigneeAvatarColor = "#4caf50",
                Description = "David Mills is the sole backend architect. Illness or resignation during Phase 3 would delay the critical path by 2–3 weeks.",
                DueDate = Anchor.AddDays(21),
                Tags = ["critical-path", "resourcing"],
                CreatedDate = Anchor.AddDays(-7)
            },
            new MudGanttRisk
            {
                Id = "r2",
                Name = "Third-party API rate limits cause integration failures",
                Type = RaidItemType.Risk,
                Priority = RiskPriority.Medium,
                Impact = RiskLevel.Medium,
                Likelihood = RiskLevel.High,
                Response = RiskResponse.Mitigate,
                Status = RiskStatus.InReview,
                AssigneeName = "Frank Ho", AssigneeInitials = "FH", AssigneeAvatarColor = "#00bcd4",
                Description = "Payment and notification APIs impose strict rate limits. High-volume integration testing may breach these limits.",
                Resolution = "Implementing request queuing and exponential back-off. Evaluating enterprise tier upgrade.",
                DueDate = Anchor.AddDays(35),
                Tags = ["integration", "external-dependency"],
                CreatedDate = Anchor.AddDays(-5)
            },
            new MudGanttRisk
            {
                Id = "r3",
                Name = "Legacy data quality issues may delay migration",
                Type = RaidItemType.Issue,
                Priority = RiskPriority.High,
                Impact = RiskLevel.High,
                Likelihood = RiskLevel.High,
                Response = RiskResponse.Mitigate,
                Status = RiskStatus.Open,
                AssigneeName = "Eve Patel", AssigneeInitials = "EP", AssigneeAvatarColor = "#9c27b0",
                Description = "Initial data profiling reveals ~12 % of legacy records have null or malformed date fields that will block migration scripts.",
                DueDate = Anchor.AddDays(28),
                Tags = ["data-migration", "technical-debt"],
                CreatedDate = Anchor.AddDays(-3)
            },
            new MudGanttRisk
            {
                Id = "r4",
                Name = "Assume cloud infrastructure costs remain within budget",
                Type = RaidItemType.Assumption,
                Priority = RiskPriority.Medium,
                Impact = RiskLevel.Medium,
                Likelihood = RiskLevel.Low,
                Response = RiskResponse.Accept,
                Status = RiskStatus.Open,
                AssigneeName = "Bob Jensen", AssigneeInitials = "BJ", AssigneeAvatarColor = "#2196f3",
                Description = "Project budget assumes Azure Standard tier for all services. If workload grows beyond estimates a tier upgrade may be required.",
                DueDate = Anchor.AddDays(14),
                Tags = ["budget", "infrastructure"],
                CreatedDate = Anchor.AddDays(-10)
            },
            new MudGanttRisk
            {
                Id = "r5",
                Name = "Approved: Use React for the client-side UI layer",
                Type = RaidItemType.Decision,
                Priority = RiskPriority.Low,
                Impact = RiskLevel.Low,
                Likelihood = RiskLevel.Low,
                Response = RiskResponse.Accept,
                Status = RiskStatus.Closed,
                AssigneeName = "David Mills", AssigneeInitials = "DM", AssigneeAvatarColor = "#ff5722",
                Description = "Architecture board approved React 18 with TypeScript as the frontend framework after evaluating Angular and Vue alternatives.",
                Resolution = "Decision recorded. Team upskilling plan in place for Q4.",
                DueDate = Anchor.AddDays(7),
                Tags = ["architecture", "frontend"],
                CreatedDate = Anchor.AddDays(-14)
            },
            new MudGanttRisk
            {
                Id = "r6",
                Name = "UAT environment provisioning delays",
                Type = RaidItemType.Issue,
                Priority = RiskPriority.Medium,
                Impact = RiskLevel.Medium,
                Likelihood = RiskLevel.Medium,
                Response = RiskResponse.Mitigate,
                Status = RiskStatus.Open,
                AssigneeName = "Carol Liu", AssigneeInitials = "CL", AssigneeAvatarColor = "#e91e63",
                Description = "IT ops confirmed a 3-week lead time for UAT environment provisioning. This must start by day 35 to avoid delaying UAT testing.",
                DueDate = Anchor.AddDays(35),
                Tags = ["environment", "it-ops"],
                CreatedDate = Anchor.AddDays(-1)
            },
        ];
    }
}
