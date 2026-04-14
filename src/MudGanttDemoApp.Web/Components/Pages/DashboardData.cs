namespace MudGanttDemoApp.Web.Components.Pages;

public class ProjectSummary
{
    public string Id { get; set; } = "";
    public string ShortName { get; set; } = "";
    public string FullName { get; set; } = "";
    public string TimeHealth { get; set; } = "green";
    public string CostHealth { get; set; } = "green";
    public string WorkloadHealth { get; set; } = "green";
    public int TaskCount { get; set; }
    public double Progress { get; set; }
    public int NotStartedTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int InProgressTasks { get; set; }
    public double AheadPct { get; set; }
    public double BehindPct { get; set; }
    public double OnTimePct { get; set; }
    public double ActualCost { get; set; }
    public double PlannedCost { get; set; }
    public double BudgetCost { get; set; }
    public double CompletedWL { get; set; }
    public double RemainingWL { get; set; }
    public double OverdueWL { get; set; }
    public double PlannedProgress { get; set; }
    public double ActualProgress { get; set; }
}

public class TimesheetEntry
{
    public string Project { get; set; } = "";
    public string Task { get; set; } = "";
    public int[] Hours { get; set; } = new int[5];
    public int Total => Hours.Sum();
}

public class ResourceMember
{
    public string Name { get; set; } = "";
    public string Initials { get; set; } = "";
    public string AvatarColor { get; set; } = "";
    public bool Expanded { get; set; } = true;
    public List<ResourceTask> Tasks { get; set; } = [];
}

public class ResourceTask
{
    public string ProjectName { get; set; } = "";
    public string TaskName { get; set; } = "";
    public Dictionary<int, int> HoursByDayIndex { get; set; } = [];
}

internal static class DashboardData
{
    public static List<ProjectSummary> CreateProjects() =>
    [
        new ProjectSummary
        {
            Id = "5g", ShortName = "5GIGSIT", FullName = "5Gigs IT Infrastructure",
            TimeHealth = "orange", CostHealth = "orange", WorkloadHealth = "orange",
            TaskCount = 7, Progress = 0.80,
            NotStartedTasks = 1, CompletedTasks = 5, InProgressTasks = 1,
            AheadPct = 19, BehindPct = 0, OnTimePct = 81,
            ActualCost = 420, PlannedCost = 380, BudgetCost = 500,
            CompletedWL = 70, RemainingWL = 20, OverdueWL = 10,
            PlannedProgress = 0.75, ActualProgress = 0.80
        },
        new ProjectSummary
        {
            Id = "ca", ShortName = "CANARYS", FullName = "CanaryS Platform",
            TimeHealth = "green", CostHealth = "green", WorkloadHealth = "green",
            TaskCount = 19, Progress = 0.54,
            NotStartedTasks = 8, CompletedTasks = 9, InProgressTasks = 2,
            AheadPct = 54, BehindPct = 0, OnTimePct = 46,
            ActualCost = 680, PlannedCost = 720, BudgetCost = 900,
            CompletedWL = 54, RemainingWL = 40, OverdueWL = 6,
            PlannedProgress = 0.50, ActualProgress = 0.54
        },
        new ProjectSummary
        {
            Id = "dw", ShortName = "DEWALTB", FullName = "DeWalt B Series",
            TimeHealth = "orange", CostHealth = "green", WorkloadHealth = "orange",
            TaskCount = 7, Progress = 0.42,
            NotStartedTasks = 4, CompletedTasks = 2, InProgressTasks = 1,
            AheadPct = 30, BehindPct = 0, OnTimePct = 70,
            ActualCost = 310, PlannedCost = 350, BudgetCost = 450,
            CompletedWL = 42, RemainingWL = 45, OverdueWL = 13,
            PlannedProgress = 0.45, ActualProgress = 0.42
        },
        new ProjectSummary
        {
            Id = "en", ShortName = "ENCLAVE", FullName = "Enclave Security Suite",
            TimeHealth = "green", CostHealth = "green", WorkloadHealth = "green",
            TaskCount = 3, Progress = 0.95,
            NotStartedTasks = 0, CompletedTasks = 3, InProgressTasks = 0,
            AheadPct = 0, BehindPct = 0, OnTimePct = 100,
            ActualCost = 195, PlannedCost = 200, BudgetCost = 220,
            CompletedWL = 95, RemainingWL = 5, OverdueWL = 0,
            PlannedProgress = 0.95, ActualProgress = 0.95
        },
        new ProjectSummary
        {
            Id = "ev", ShortName = "EV1DESI", FullName = "EV1 Design Initiative",
            TimeHealth = "green", CostHealth = "green", WorkloadHealth = "green",
            TaskCount = 11, Progress = 0.71,
            NotStartedTasks = 3, CompletedTasks = 7, InProgressTasks = 1,
            AheadPct = 9, BehindPct = 0, OnTimePct = 91,
            ActualCost = 540, PlannedCost = 580, BudgetCost = 700,
            CompletedWL = 71, RemainingWL = 24, OverdueWL = 5,
            PlannedProgress = 0.68, ActualProgress = 0.71
        },
        new ProjectSummary
        {
            Id = "fo", ShortName = "FORTHEL", FullName = "FortheL Platform",
            TimeHealth = "green", CostHealth = "green", WorkloadHealth = "green",
            TaskCount = 11, Progress = 0.30,
            NotStartedTasks = 7, CompletedTasks = 3, InProgressTasks = 1,
            AheadPct = 30, BehindPct = 0, OnTimePct = 70,
            ActualCost = 220, PlannedCost = 280, BudgetCost = 400,
            CompletedWL = 30, RemainingWL = 65, OverdueWL = 5,
            PlannedProgress = 0.35, ActualProgress = 0.30
        },
        new ProjectSummary
        {
            Id = "nw", ShortName = "NEWPORT", FullName = "Newport Bridge",
            TimeHealth = "green", CostHealth = "green", WorkloadHealth = "green",
            TaskCount = 10, Progress = 0.60,
            NotStartedTasks = 4, CompletedTasks = 5, InProgressTasks = 1,
            AheadPct = 30, BehindPct = 0, OnTimePct = 70,
            ActualCost = 890, PlannedCost = 950, BudgetCost = 1200,
            CompletedWL = 60, RemainingWL = 35, OverdueWL = 5,
            PlannedProgress = 0.55, ActualProgress = 0.60
        },
        new ProjectSummary
        {
            Id = "np", ShortName = "NEWPRIN", FullName = "NewPrin Analytics",
            TimeHealth = "orange", CostHealth = "green", WorkloadHealth = "green",
            TaskCount = 13, Progress = 0.35,
            NotStartedTasks = 8, CompletedTasks = 4, InProgressTasks = 1,
            AheadPct = 35, BehindPct = 0, OnTimePct = 65,
            ActualCost = 410, PlannedCost = 440, BudgetCost = 600,
            CompletedWL = 35, RemainingWL = 60, OverdueWL = 5,
            PlannedProgress = 0.38, ActualProgress = 0.35
        },
        new ProjectSummary
        {
            Id = "wl", ShortName = "WILLOWD", FullName = "Willow Manufacturing",
            TimeHealth = "green", CostHealth = "green", WorkloadHealth = "green",
            TaskCount = 27, Progress = 0.11,
            NotStartedTasks = 22, CompletedTasks = 3, InProgressTasks = 2,
            AheadPct = 0, BehindPct = 5, OnTimePct = 95,
            ActualCost = 180, PlannedCost = 250, BudgetCost = 1100,
            CompletedWL = 11, RemainingWL = 85, OverdueWL = 4,
            PlannedProgress = 0.13, ActualProgress = 0.11
        },
    ];

    public static List<TimesheetEntry> CreateTimesheet() =>
    [
        new TimesheetEntry { Project = "5Gigs IT",            Task = "Train Administrators",  Hours = [3, 2, 0, 0, 0] },
        new TimesheetEntry { Project = "5Gigs IT",            Task = "Train Users",           Hours = [0, 0, 2, 5, 0] },
        new TimesheetEntry { Project = "Park Talks",          Task = "Set goals and objectives", Hours = [3, 0, 0, 1, 0] },
        new TimesheetEntry { Project = "Park Talks",          Task = "Plan itinerary",        Hours = [2, 2, 4, 0, 6] },
        new TimesheetEntry { Project = "Willow Manufacturing", Task = "Market Research",       Hours = [0, 3, 1, 0, 1] },
        new TimesheetEntry { Project = "Willow Manufacturing", Task = "Product Requirements",  Hours = [0, 1, 0, 3, 0] },
    ];

    public static List<ResourceMember> CreateResourceMembers() =>
    [
        new ResourceMember
        {
            Name = "Danny Jones", Initials = "DJ", AvatarColor = "#4caf50",
            Tasks =
            [
                new ResourceTask
                {
                    ProjectName = "5Gigs IT", TaskName = "Implementation",
                    HoursByDayIndex = new() { [1]=8, [2]=8, [3]=6, [8]=6, [9]=6, [10]=2 }
                },
            ]
        },
        new ResourceMember
        {
            Name = "Jennifer Murphey", Initials = "JM", AvatarColor = "#2196f3",
            Tasks =
            [
                new ResourceTask
                {
                    ProjectName = "Creekwood Constr.", TaskName = "Order Equipment",
                    HoursByDayIndex = new() { [3]=8 }
                },
            ]
        },
        new ResourceMember
        {
            Name = "Joe Johnson", Initials = "JJ", AvatarColor = "#ff5722",
            Tasks =
            [
                new ResourceTask
                {
                    ProjectName = "Reserve Housing", TaskName = "Site work",
                    HoursByDayIndex = new() { [6]=2, [7]=2, [8]=2 }
                },
            ]
        },
        new ResourceMember
        {
            Name = "Monica Corrigan", Initials = "MC", AvatarColor = "#9c27b0",
            Tasks =
            [
                new ResourceTask
                {
                    ProjectName = "Willow Manufactur.", TaskName = "Product Requirem.",
                    HoursByDayIndex = new() { [10]=2, [11]=2 }
                },
            ]
        },
        new ResourceMember
        {
            Name = "Natalie Waters", Initials = "NW", AvatarColor = "#e91e63",
            Expanded = true,
            Tasks =
            [
                new ResourceTask
                {
                    ProjectName = "5Gigs IT", TaskName = "Implementation",
                    HoursByDayIndex = new() { [1]=8, [2]=8, [3]=6, [8]=6, [9]=6, [10]=2 }
                },
                new ResourceTask
                {
                    ProjectName = "Creekwood Constr.", TaskName = "Order Equipment",
                    HoursByDayIndex = new() { [3]=8 }
                },
                new ResourceTask
                {
                    ProjectName = "Reserve Housing", TaskName = "Site work",
                    HoursByDayIndex = new() { [6]=2, [7]=2, [8]=2 }
                },
                new ResourceTask
                {
                    ProjectName = "Willow Manufactur.", TaskName = "Product Requirem.",
                    HoursByDayIndex = new() { [10]=2, [11]=2 }
                },
                new ResourceTask
                {
                    ProjectName = "You Got IT!", TaskName = "User Documentation",
                    HoursByDayIndex = new() { [4]=6 }
                },
            ]
        },
    ];
}
