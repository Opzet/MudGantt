using MudGanttDemoApp.Web;
using MudGanttDemoApp.Web.Components;
using MudGanttDemoApp.Web.GanttApi;
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

var app = builder.Build();

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

app.MapDefaultEndpoints();

app.Run();
