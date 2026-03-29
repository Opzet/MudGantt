namespace MudGanttDemoApp.Web.Components.Pages;

internal sealed record CapabilityRow(string Area, string Capability, string Status, string Evidence, string Notes);
internal sealed record TestRubricRow(string Capability, string Scenario, string Verification, string Status);
internal sealed record RoadmapRow(string Horizon, string Item, string Outcome);

internal static class ProductReadinessData
{
    public static IReadOnlyList<CapabilityRow> Capabilities =>
    [
        new("Core Gantt", "Task dependencies, selection, zoom, drag/progress editing", "Implemented", "MudGantt core component + demo pages", "Working in demo app"),
        new("Core Gantt", "Critical path, baselines, highlighted dependency chain", "Implemented", "Critical path + baseline demo", "Visualized in chart"),
        new("Manufacturing", "Client/job to machine/worker dual-flow planning", "Implemented", "Manufacturing Flow page", "Linked top/bottom Gantt"),
        new("Manufacturing", "Conflict, delay, overload visibility", "Implemented", "Flow, transparency, utilization pages", "Generated dataset supports it"),
        new("Manufacturing", "Named views / operational presets", "Implemented", "Manufacturing Flow page", "Persisted in localStorage"),
        new("Transparency", "Client portal summary and print-ready reporting", "Implemented", "Client Portal + Reports pages", "CSV + print support"),
        new("Diagnostics", "Bug capture persisted as JSON", "Implemented", "Feedback Center page + JSON API", "Stored under App_Data"),
        new("Supportability", "Feature matrix, rubric, roadmap visibility", "Implemented", "Capability Matrix page", "Tracks implemented scope honestly")
    ];

    public static IReadOnlyList<TestRubricRow> TestRubric =>
    [
        new("Selection and highlight", "Click a task and confirm selected task scrolls into view and linked tasks highlight", "Build verified + demo interaction", "Ready for manual verification"),
        new("Manufacturing flow", "Filter by client/machine and inspect synchronized top/bottom charts", "Build verified + demo page", "Ready for manual verification"),
        new("Conflict reporting", "Inspect conflict chips, delay hours, and overload counts", "Build verified + generated dataset", "Ready for manual verification"),
        new("Exports", "Download jobs/resources CSV reports", "Endpoint implemented", "Ready for manual verification"),
        new("Bug capture", "Submit feedback and confirm persistence in JSON export", "API + page implemented", "Ready for manual verification"),
        new("Portal reporting", "Open client portal and print", "Page implemented", "Ready for manual verification")
    ];

    public static IReadOnlyList<RoadmapRow> Roadmap =>
    [
        new("Now", "Machine row heatmap overlays", "Move from whole-chart overlays to per-resource row overlays for finer capacity diagnosis"),
        new("Now", "Structured logging and diagnostics", "Add logging around presets, exports, and dataset generation"),
        new("Next", "Dedicated mapper/filter services", "Reduce duplication across manufacturing pages and simplify maintenance"),
        new("Next", "Automated tests", "Add test project for analysis, dataset generation, and report builder"),
        new("Later", "What-if reschedule actions", "Allow operator to apply a suggestion and simulate revised finish dates"),
        new("Later", "Portfolio planning", "Multi-project client/program rollup across plants and value streams")
    ];
}
