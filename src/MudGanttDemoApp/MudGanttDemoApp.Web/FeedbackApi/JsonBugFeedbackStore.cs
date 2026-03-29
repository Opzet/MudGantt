using System.Text.Json;

namespace MudGanttDemoApp.Web.FeedbackApi;

public sealed class JsonBugFeedbackStore : IBugFeedbackStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    private readonly string _filePath;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly ILogger<JsonBugFeedbackStore> _logger;

    public JsonBugFeedbackStore(IHostEnvironment environment, ILogger<JsonBugFeedbackStore> logger)
    {
        _logger = logger;
        var directory = Path.Combine(environment.ContentRootPath, "App_Data");
        Directory.CreateDirectory(directory);
        _filePath = Path.Combine(directory, "bug-feedback.json");
        _logger.LogInformation("Bug feedback store initialized at {Path}", _filePath);
    }

    public async Task<IReadOnlyList<BugFeedbackDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            var items = await ReadUnsafeAsync(cancellationToken);
            return items.OrderByDescending(x => x.CreatedUtc).ToList();
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<BugFeedbackDto> CreateAsync(BugFeedbackDto feedback, CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            var items = await ReadUnsafeAsync(cancellationToken);
            items.Add(feedback);
            await WriteUnsafeAsync(items, cancellationToken);
            _logger.LogInformation("Bug feedback captured for area {Area} with severity {Severity}", feedback.Area, feedback.Severity);
            return feedback;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<byte[]> ExportJsonAsync(CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            var items = await ReadUnsafeAsync(cancellationToken);
            return JsonSerializer.SerializeToUtf8Bytes(items.OrderByDescending(x => x.CreatedUtc), SerializerOptions);
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<List<BugFeedbackDto>> ReadUnsafeAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_filePath))
        {
            return [];
        }

        await using var stream = File.OpenRead(_filePath);
        var items = await JsonSerializer.DeserializeAsync<List<BugFeedbackDto>>(stream, SerializerOptions, cancellationToken);
        return items ?? [];
    }

    private async Task WriteUnsafeAsync(List<BugFeedbackDto> items, CancellationToken cancellationToken)
    {
        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, items, SerializerOptions, cancellationToken);
    }
}
