using MudGanttDemoApp.Web;
using MudGanttDemoApp.Web.Components;
using MudGanttDemoApp.Web.FeedbackApi;
using MudGanttDemoApp.Web.GanttApi;
using MudGanttDemoApp.Web.ManufacturingApi;
using MudGanttDemoApp.Web.SchedulerApi;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOutputCache();
builder.Services.AddMudServices();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IGanttTaskStore, InMemoryGanttTaskStore>();
builder.Services.AddSingleton<ISchedulerProjectStore, InMemorySchedulerProjectStore>();
builder.Services.AddSingleton<IManufacturingDemoStore, InMemoryManufacturingDemoStore>();
builder.Services.AddSingleton<IBugFeedbackStore, JsonBugFeedbackStore>();

var app = builder.Build();

var basePath = builder.Environment.IsProduction() ? "/schedule" : "";
if (!string.IsNullOrWhiteSpace(basePath))
{
    app.UsePathBase(basePath);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseOutputCache();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

var ganttApi = app.MapGroup("/api/gantt/tasks");

ganttApi.MapGet("/", async (string? search, IGanttTaskStore store, CancellationToken cancellationToken) =>
{
    var items = await store.GetAllAsync(search, cancellationToken);
    return Results.Ok(items);
});

ganttApi.MapGet("/{id}", async (string id, IGanttTaskStore store, CancellationToken cancellationToken) =>
{
    var item = await store.GetByIdAsync(id, cancellationToken);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

ganttApi.MapPost("/", async (GanttTaskDto dto, IGanttTaskStore store, CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(dto.Name))
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            [nameof(dto.Name)] = ["Name is required"]
        });
    }

    var created = await store.CreateAsync(dto, cancellationToken);
    return Results.Created($"/api/gantt/tasks/{created.Id}", created);
});

ganttApi.MapPut("/{id}", async (string id, GanttTaskDto dto, IGanttTaskStore store, CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(dto.Name))
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            [nameof(dto.Name)] = ["Name is required"]
        });
    }

    var updated = await store.UpdateAsync(id, dto, cancellationToken);
    return updated is null ? Results.NotFound() : Results.Ok(updated);
});

ganttApi.MapDelete("/{id}", async (string id, IGanttTaskStore store, CancellationToken cancellationToken) =>
{
    var removed = await store.DeleteAsync(id, cancellationToken);
    return removed ? Results.NoContent() : Results.NotFound();
});

var schedulerApi = app.MapGroup("/api/scheduler");

schedulerApi.MapGet("/project", async (ISchedulerProjectStore store, CancellationToken cancellationToken) =>
{
    var project = await store.GetProjectAsync(cancellationToken);
    return Results.Ok(project);
});

var manufacturingApi = app.MapGroup("/api/manufacturing");

manufacturingApi.MapGet("/dataset", async (int? jobs, IManufacturingDemoStore store, CancellationToken cancellationToken) =>
{
    var dataset = await store.GetDatasetAsync(jobs ?? 2500, cancellationToken);
    return Results.Ok(dataset);
});

manufacturingApi.MapGet("/holidays", async (IManufacturingDemoStore store, CancellationToken cancellationToken) =>
{
    var holidays = await store.GetHolidaysAsync(cancellationToken);
    return Results.Ok(holidays);
});

manufacturingApi.MapGet("/reports/jobs.csv", async (string? client, IManufacturingDemoStore store, CancellationToken cancellationToken) =>
{
    var dataset = await store.GetDatasetAsync(2500, cancellationToken);
    var bytes = ManufacturingReportBuilder.BuildJobsCsv(dataset, client);
    return Results.File(bytes, "text/csv", "manufacturing-jobs-report.csv");
});

manufacturingApi.MapGet("/reports/resources.csv", async (string? client, IManufacturingDemoStore store, CancellationToken cancellationToken) =>
{
    var dataset = await store.GetDatasetAsync(2500, cancellationToken);
    var bytes = ManufacturingReportBuilder.BuildResourcesCsv(dataset, client);
    return Results.File(bytes, "text/csv", "manufacturing-resources-report.csv");
});

var feedbackApi = app.MapGroup("/api/feedback/bugs");

feedbackApi.MapGet("/", async (IBugFeedbackStore store, CancellationToken cancellationToken) =>
{
    var items = await store.GetAllAsync(cancellationToken);
    return Results.Ok(items);
});

feedbackApi.MapPost("/", async (BugFeedbackDto feedback, IBugFeedbackStore store, CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(feedback.Area) || string.IsNullOrWhiteSpace(feedback.Title) || string.IsNullOrWhiteSpace(feedback.ReproductionSteps) || string.IsNullOrWhiteSpace(feedback.ExpectedBehavior) || string.IsNullOrWhiteSpace(feedback.ActualBehavior))
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            [nameof(feedback.Title)] = ["Required feedback fields are missing"]
        });
    }

    var created = await store.CreateAsync(feedback, cancellationToken);
    return Results.Ok(created);
});

feedbackApi.MapGet("/export", async (IBugFeedbackStore store, CancellationToken cancellationToken) =>
{
    var content = await store.ExportJsonAsync(cancellationToken);
    return Results.File(content, "application/json", "bug-feedback.json");
});

//app.MapDefaultEndpoints();

app.Run();
