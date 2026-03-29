namespace MudGantt;

/// <summary>
/// Helper methods for deriving scheduling insight from task collections.
/// </summary>
public static class GanttTaskAnalysis
{
    public static IReadOnlyList<string> GetCriticalPathTaskIds(IReadOnlyList<MudGanttTask> tasks)
    {
        if (tasks.Count == 0)
        {
            return [];
        }

        var byId = tasks.ToDictionary(x => x.Id);
        var predecessors = tasks.ToDictionary(task => task.Id, _ => new List<string>());
        foreach (var task in tasks)
        {
            foreach (var link in task.Links)
            {
                if (predecessors.TryGetValue(task.Id, out var list) && byId.ContainsKey(link.Id))
                {
                    list.Add(link.Id);
                }
            }
        }

        var memo = new Dictionary<string, double>();
        var next = new Dictionary<string, string?>();
        foreach (var task in tasks)
        {
            ComputeLongestDuration(task.Id, byId, predecessors, memo, next, new HashSet<string>());
        }

        var startId = memo.OrderByDescending(x => x.Value).First().Key;
        var path = new List<string>();
        var current = startId;
        while (current is not null)
        {
            path.Add(current);
            current = next.GetValueOrDefault(current);
        }

        return path;
    }

    public static IReadOnlyList<string> BuildDependencyChain(IReadOnlyList<MudGanttTask> tasks, string taskId)
    {
        if (tasks.Count == 0 || string.IsNullOrWhiteSpace(taskId))
        {
            return [];
        }

        var forward = tasks.ToDictionary(x => x.Id, x => x.Links.Select(link => link.Id).ToList());
        var reverse = tasks.ToDictionary(x => x.Id, _ => new List<string>());
        foreach (var task in tasks)
        {
            foreach (var link in task.Links)
            {
                if (reverse.TryGetValue(link.Id, out var list))
                {
                    list.Add(task.Id);
                }
            }
        }

        var visited = new HashSet<string>();
        var queue = new Queue<string>();
        queue.Enqueue(taskId);
        visited.Add(taskId);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            foreach (var next in forward.GetValueOrDefault(current, []))
            {
                if (visited.Add(next))
                {
                    queue.Enqueue(next);
                }
            }
            foreach (var previous in reverse.GetValueOrDefault(current, []))
            {
                if (visited.Add(previous))
                {
                    queue.Enqueue(previous);
                }
            }
        }

        return visited.ToList();
    }

    public static double CalculateWeightedProgress(IReadOnlyList<MudGanttTask> tasks)
    {
        if (tasks.Count == 0)
        {
            return 0;
        }

        var weighted = tasks
            .Select(task => (Progress: task.Progress ?? 0, Weight: Math.Max(1, GetDurationHours(task))))
            .ToList();

        var totalWeight = weighted.Sum(x => x.Weight);
        if (totalWeight <= 0)
        {
            return 0;
        }

        return weighted.Sum(x => x.Progress * x.Weight) / totalWeight;
    }

    private static double ComputeLongestDuration(string taskId, IReadOnlyDictionary<string, MudGanttTask> byId, IReadOnlyDictionary<string, List<string>> predecessors, IDictionary<string, double> memo, IDictionary<string, string?> next, HashSet<string> recursionGuard)
    {
        if (memo.TryGetValue(taskId, out var cached))
        {
            return cached;
        }

        if (!recursionGuard.Add(taskId))
        {
            return GetDurationHours(byId[taskId]);
        }

        var bestNext = default(string);
        var bestTail = 0d;
        foreach (var predecessorId in predecessors[taskId])
        {
            var candidate = ComputeLongestDuration(predecessorId, byId, predecessors, memo, next, recursionGuard);
            if (candidate > bestTail)
            {
                bestTail = candidate;
                bestNext = predecessorId;
            }
        }

        recursionGuard.Remove(taskId);
        var total = GetDurationHours(byId[taskId]) + bestTail;
        memo[taskId] = total;
        next[taskId] = bestNext;
        return total;
    }

    private static double GetDurationHours(MudGanttTask task)
    {
        if (task.StartDate is null || task.EndDate is null)
        {
            return 1;
        }

        return Math.Max(1, (task.EndDate.Value - task.StartDate.Value).TotalHours);
    }
}
