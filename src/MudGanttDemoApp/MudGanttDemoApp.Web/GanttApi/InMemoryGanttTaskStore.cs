namespace MudGanttDemoApp.Web.GanttApi;

public sealed class InMemoryGanttTaskStore : IGanttTaskStore
{
    private readonly Lock _sync = new();
    private readonly List<GanttTaskDto> _items = CreateSeedData();

    public Task<IReadOnlyList<GanttTaskDto>> GetAllAsync(string? search, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        lock (_sync)
        {
            IEnumerable<GanttTaskDto> query = _items;
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => x.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            var list = query
                .OrderBy(x => x.StartDate)
                .ThenBy(x => x.Name)
                .Select(Clone)
                .ToList();

            return Task.FromResult<IReadOnlyList<GanttTaskDto>>(list);
        }
    }

    public Task<GanttTaskDto?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        lock (_sync)
        {
            var found = _items.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(found is null ? null : Clone(found));
        }
    }

    public Task<GanttTaskDto> CreateAsync(GanttTaskDto dto, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        lock (_sync)
        {
            var id = string.IsNullOrWhiteSpace(dto.Id) ? Guid.NewGuid().ToString("N") : dto.Id;
            if (_items.Any(x => x.Id == id))
            {
                id = Guid.NewGuid().ToString("N");
            }

            var created = Clone(dto);
            created.Id = id;
            _items.Add(created);
            return Task.FromResult(Clone(created));
        }
    }

    public Task<GanttTaskDto?> UpdateAsync(string id, GanttTaskDto dto, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        lock (_sync)
        {
            var index = _items.FindIndex(x => x.Id == id);
            if (index < 0)
            {
                return Task.FromResult<GanttTaskDto?>(null);
            }

            var updated = Clone(dto);
            updated.Id = id;
            _items[index] = updated;
            return Task.FromResult<GanttTaskDto?>(Clone(updated));
        }
    }

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        lock (_sync)
        {
            var removed = _items.RemoveAll(x => x.Id == id) > 0;
            if (removed)
            {
                foreach (var item in _items)
                {
                    item.Links = item.Links.Where(x => x.Id != id).ToList();
                }
            }

            return Task.FromResult(removed);
        }
    }

    private static GanttTaskDto Clone(GanttTaskDto source)
    {
        return new GanttTaskDto
        {
            Id = source.Id,
            Name = source.Name,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            Progress = source.Progress,
            Color = source.Color,
            Links = source.Links.Select(x => new GanttTaskLinkDto
            {
                Id = x.Id,
                LinkType = x.LinkType
            }).ToList()
        };
    }

    private static List<GanttTaskDto> CreateSeedData()
    {
        string[] firstNames = ["Armin", "Rodel", "Kristopher", "Maria", "John", "Alyssa", "Daniel", "Fatima", "Noah", "Ella"];
        string[] lastNames = ["Francisco", "Gonzales", "Ralph", "Santos", "Dela Cruz", "Reyes", "Garcia", "Torres", "Mendoza", "Castillo"];
        string[] palette = ["#9ec9d1", "#a5d6c9", "#b8c9f1", "#d1c2f0", "#f3d6a1"];

        var baseDate = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var list = new List<GanttTaskDto>(100);

        for (var i = 0; i < 100; i++)
        {
            var id = (2200 + i).ToString("0000");
            var firstName = firstNames[i % firstNames.Length];
            var lastName = lastNames[(i / firstNames.Length) % lastNames.Length];

            var startHour = (i * 3) % 12;
            var startMinute = (i * 17) % 60;
            var durationHours = 6 + (i % 6);
            var durationMinutes = (i * 11) % 60;

            var start = baseDate.AddHours(startHour).AddMinutes(startMinute);
            var end = start.AddHours(durationHours).AddMinutes(durationMinutes);
            var dayEnd = baseDate.AddHours(23).AddMinutes(59);
            if (end > dayEnd)
            {
                end = dayEnd;
            }

            list.Add(new GanttTaskDto
            {
                Id = id,
                Name = $"{firstName} {lastName}",
                StartDate = start,
                EndDate = end,
                Progress = null,
                Color = palette[i % palette.Length]
            });
        }

        return list;
    }
}
