using MudGantt;
using MudGanttDemoApp.Web.GanttApi;

namespace MudGanttDemoApp.Web.SchedulerApi;

public sealed class InMemorySchedulerProjectStore : ISchedulerProjectStore
{
    private readonly SchedulerProjectDto _project = CreateProject();

    public Task<SchedulerProjectDto> GetProjectAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return global::System.Threading.Tasks.Task.FromResult(Clone(_project));
    }

    private static SchedulerProjectDto CreateProject()
    {
        var project = new SchedulerProjectDto
        {
            Id = "scheduler-pro-starter",
            Name = "Turnkey delivery program",
            Description = "Working scheduler demo with tasks, resources, assignments, dependencies, and a shared working calendar.",
            Resources =
            [
                new SchedulerResourceDto { Id = "res-pm", Name = "Evelyn Hart", Role = "Project Manager", CapacityHoursPerDay = 8 },
                new SchedulerResourceDto { Id = "res-arch", Name = "Marcus Ng", Role = "Architect", CapacityHoursPerDay = 8 },
                new SchedulerResourceDto { Id = "res-api-1", Name = "Priya Shah", Role = "API Engineer", CapacityHoursPerDay = 8 },
                new SchedulerResourceDto { Id = "res-api-2", Name = "Leo Romero", Role = "API Engineer", CapacityHoursPerDay = 8 },
                new SchedulerResourceDto { Id = "res-ui-1", Name = "Sofia Cruz", Role = "Frontend Engineer", CapacityHoursPerDay = 8 },
                new SchedulerResourceDto { Id = "res-ui-2", Name = "Hugo Patel", Role = "Frontend Engineer", CapacityHoursPerDay = 8 },
                new SchedulerResourceDto { Id = "res-qa", Name = "Nina Keller", Role = "QA Lead", CapacityHoursPerDay = 8 },
                new SchedulerResourceDto { Id = "res-devops", Name = "Owen Brooks", Role = "DevOps Engineer", CapacityHoursPerDay = 8 }
            ],
            Calendar =
            [
                new WorkingCalendarDayDto { Day = "Monday", WorkingHours = "08:00 - 17:00" },
                new WorkingCalendarDayDto { Day = "Tuesday", WorkingHours = "08:00 - 17:00" },
                new WorkingCalendarDayDto { Day = "Wednesday", WorkingHours = "08:00 - 17:00" },
                new WorkingCalendarDayDto { Day = "Thursday", WorkingHours = "08:00 - 17:00" },
                new WorkingCalendarDayDto { Day = "Friday", WorkingHours = "08:00 - 16:00" }
            ]
        };

        project.Tasks =
        [
            CreateTask("t-discovery", "Discovery", "Discovery", 2025, 10, 1, 8, 2025, 10, 2, 17, 1.0, 16, "#5b8def"),
            CreateTask("t-architecture", "Architecture", "Design", 2025, 10, 3, 8, 2025, 10, 6, 17, 0.85, 24, "#7c4dff", new SchedulerTaskLinkDto { Id = "t-discovery", LinkType = LinkType.FinishToStart }),
            CreateTask("t-api-core", "Core API", "Build", 2025, 10, 7, 8, 2025, 10, 10, 17, 0.75, 32, "#00a67e", new SchedulerTaskLinkDto { Id = "t-architecture", LinkType = LinkType.FinishToStart }),
            CreateTask("t-api-security", "Security API", "Build", 2025, 10, 7, 8, 2025, 10, 9, 17, 0.55, 24, "#0f9d58", new SchedulerTaskLinkDto { Id = "t-architecture", LinkType = LinkType.FinishToStart }),
            CreateTask("t-ui-shell", "UI Shell", "Build", 2025, 10, 7, 9, 2025, 10, 10, 17, 0.7, 30, "#ff9800", new SchedulerTaskLinkDto { Id = "t-architecture", LinkType = LinkType.FinishToStart }),
            CreateTask("t-ui-workflows", "Workflow UI", "Build", 2025, 10, 10, 8, 2025, 10, 14, 17, 0.35, 40, "#fb8c00", new SchedulerTaskLinkDto { Id = "t-ui-shell", LinkType = LinkType.FinishToStart }, new SchedulerTaskLinkDto { Id = "t-api-core", LinkType = LinkType.StartToStart }),
            CreateTask("t-integration", "Integration", "Validate", 2025, 10, 15, 8, 2025, 10, 16, 17, 0.15, 16, "#ef5350", new SchedulerTaskLinkDto { Id = "t-api-core", LinkType = LinkType.FinishToStart }, new SchedulerTaskLinkDto { Id = "t-ui-workflows", LinkType = LinkType.FinishToStart }),
            CreateTask("t-uat", "UAT", "Validate", 2025, 10, 17, 9, 2025, 10, 20, 15, 0.0, 18, "#d81b60", new SchedulerTaskLinkDto { Id = "t-integration", LinkType = LinkType.FinishToStart }),
            CreateTask("t-release", "Release", "Release", 2025, 10, 21, 10, 2025, 10, 21, 14, 0.0, 4, "#6a1b9a", new SchedulerTaskLinkDto { Id = "t-uat", LinkType = LinkType.FinishToStart })
        ];

        project.Assignments =
        [
            new SchedulerAssignmentDto { TaskId = "t-discovery", ResourceId = "res-pm", AllocationPercent = 100 },
            new SchedulerAssignmentDto { TaskId = "t-discovery", ResourceId = "res-arch", AllocationPercent = 100 },
            new SchedulerAssignmentDto { TaskId = "t-architecture", ResourceId = "res-arch", AllocationPercent = 100 },
            new SchedulerAssignmentDto { TaskId = "t-architecture", ResourceId = "res-pm", AllocationPercent = 50 },
            new SchedulerAssignmentDto { TaskId = "t-api-core", ResourceId = "res-api-1", AllocationPercent = 100 },
            new SchedulerAssignmentDto { TaskId = "t-api-core", ResourceId = "res-api-2", AllocationPercent = 75 },
            new SchedulerAssignmentDto { TaskId = "t-api-security", ResourceId = "res-api-2", AllocationPercent = 100 },
            new SchedulerAssignmentDto { TaskId = "t-ui-shell", ResourceId = "res-ui-1", AllocationPercent = 100 },
            new SchedulerAssignmentDto { TaskId = "t-ui-shell", ResourceId = "res-ui-2", AllocationPercent = 60 },
            new SchedulerAssignmentDto { TaskId = "t-ui-workflows", ResourceId = "res-ui-1", AllocationPercent = 70 },
            new SchedulerAssignmentDto { TaskId = "t-ui-workflows", ResourceId = "res-ui-2", AllocationPercent = 100 },
            new SchedulerAssignmentDto { TaskId = "t-integration", ResourceId = "res-qa", AllocationPercent = 100 },
            new SchedulerAssignmentDto { TaskId = "t-integration", ResourceId = "res-devops", AllocationPercent = 50 },
            new SchedulerAssignmentDto { TaskId = "t-uat", ResourceId = "res-pm", AllocationPercent = 50 },
            new SchedulerAssignmentDto { TaskId = "t-uat", ResourceId = "res-qa", AllocationPercent = 100 },
            new SchedulerAssignmentDto { TaskId = "t-release", ResourceId = "res-devops", AllocationPercent = 100 },
            new SchedulerAssignmentDto { TaskId = "t-release", ResourceId = "res-pm", AllocationPercent = 50 }
        ];

        return project;
    }

    private static SchedulerTaskItemDto CreateTask(string id, string name, string phase, int sy, int sm, int sd, int sh, int ey, int em, int ed, int eh, double progress, double effortHours, string color, params SchedulerTaskLinkDto[] links)
    {
        return new SchedulerTaskItemDto
        {
            Id = id,
            Name = name,
            Phase = phase,
            StartDate = new DateTimeOffset(sy, sm, sd, sh, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(ey, em, ed, eh, 0, 0, TimeSpan.Zero),
            Progress = progress,
            EffortHours = effortHours,
            Color = color,
            Links = links.ToList()
        };
    }

    private static SchedulerProjectDto Clone(SchedulerProjectDto project)
    {
        return new SchedulerProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Tasks = project.Tasks.Select(task => new SchedulerTaskItemDto
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                StartDate = task.StartDate,
                EndDate = task.EndDate,
                Progress = task.Progress,
                Color = task.Color,
                Phase = task.Phase,
                EffortHours = task.EffortHours,
                Links = task.Links.Select(link => new SchedulerTaskLinkDto { Id = link.Id, LinkType = link.LinkType }).ToList()
            }).ToList(),
            Resources = project.Resources.Select(resource => new SchedulerResourceDto
            {
                Id = resource.Id,
                Name = resource.Name,
                Role = resource.Role,
                CapacityHoursPerDay = resource.CapacityHoursPerDay
            }).ToList(),
            Assignments = project.Assignments.Select(assignment => new SchedulerAssignmentDto
            {
                TaskId = assignment.TaskId,
                ResourceId = assignment.ResourceId,
                AllocationPercent = assignment.AllocationPercent
            }).ToList(),
            Calendar = project.Calendar.Select(day => new WorkingCalendarDayDto
            {
                Day = day.Day,
                WorkingHours = day.WorkingHours
            }).ToList()
        };
    }
}
