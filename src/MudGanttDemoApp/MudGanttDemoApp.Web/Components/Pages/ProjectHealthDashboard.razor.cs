using MudBlazor;
using MudGanttDemoApp.Web.ManufacturingApi;
using MudGanttDemoApp.Web.SchedulerApi;

namespace MudGanttDemoApp.Web.Components.Pages;

public partial class ProjectHealthDashboard : IAsyncDisposable
{
    private readonly List<ProjectRiskItem> _riskItems = [];
    private List<ChartSeries<double>> _burndownSeries = [];
    private string[] _burndownLabels = [];
    private SchedulerProjectDto? _project;
    private ManufacturingDatasetDto? _dataset;
    private List<string> _clients = [];
    private List<string> _phases = [];
    private string _selectedClient = ProjectHealthDashboardSupport.AllClients;
    private string _selectedPhase = ProjectHealthDashboardSupport.AllPhases;
    private DateTime? _throughDate = DateTime.Today;
    private DateTime? _lastRefreshedUtc;
    private bool _autoRefreshEnabled = true;
    private PeriodicTimer? _timer;
    private CancellationTokenSource? _refreshTokenSource;

    private decimal _plannedHours;
    private decimal _earnedHours;
    private decimal _completionPercent;
    private int _overdueTasks;
    private int _atRiskTasks;
    private decimal _riskScore;

    private Color RiskScoreColor => _riskScore switch
    {
        >= 70 => Color.Error,
        >= 40 => Color.Warning,
        _ => Color.Success
    };

    protected override async Task OnInitializedAsync()
    {
        await RefreshAsync();
        StartAutoRefresh();
    }

    private void StartAutoRefresh()
    {
        _refreshTokenSource = new CancellationTokenSource();
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(30));
        _ = Task.Run(async () =>
        {
            try
            {
                while (await _timer.WaitForNextTickAsync(_refreshTokenSource.Token))
                {
                    if (_autoRefreshEnabled)
                    {
                        await InvokeAsync(RefreshAsync);
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        });
    }

    private async Task RefreshAsync()
    {
        try
        {
            _project = await SchedulerProjectStore.GetProjectAsync(CancellationToken.None);
            _dataset = await ManufacturingDemoStore.GetDatasetAsync(500, CancellationToken.None);
            _clients = _dataset.Clients.OrderBy(client => client).ToList();
            _phases = _project.Tasks.Select(task => task.Phase ?? "Unspecified").Distinct().OrderBy(phase => phase).ToList();
            BuildDashboard();
            _lastRefreshedUtc = DateTime.UtcNow;
        }
        catch
        {
            Snackbar.Add("Unable to refresh project health right now.", Severity.Error);
        }
    }

    private void BuildDashboard()
    {
        if (_project is null || _dataset is null)
        {
            return;
        }

        var throughDate = (_throughDate ?? DateTime.Today).Date;
        var scopedTasks = _project.Tasks
            .Where(task => _selectedPhase == ProjectHealthDashboardSupport.AllPhases || (task.Phase ?? "Unspecified") == _selectedPhase)
            .ToList();
        var scopedJobs = _dataset.Jobs
            .Where(job => _selectedClient == ProjectHealthDashboardSupport.AllClients || job.Client == _selectedClient)
            .ToList();

        var metrics = ProjectHealthDashboardSupport.BuildMetrics(scopedTasks, scopedJobs, throughDate);
        _plannedHours = metrics.PlannedHours;
        _earnedHours = metrics.EarnedHours;
        _completionPercent = metrics.CompletionPercent;
        _overdueTasks = metrics.OverdueTasks;
        _atRiskTasks = metrics.AtRiskTasks;
        _riskScore = metrics.RiskScore;

        var burndown = ProjectHealthDashboardSupport.BuildBurndown(scopedTasks);
        _burndownLabels = burndown.Labels;
        _burndownSeries = burndown.Series;

        _riskItems.Clear();
        _riskItems.AddRange(ProjectHealthDashboardSupport.BuildRiskItems(scopedTasks, scopedJobs, throughDate));
    }

    public async ValueTask DisposeAsync()
    {
        _refreshTokenSource?.Cancel();
        _timer?.Dispose();
        _refreshTokenSource?.Dispose();
        await Task.CompletedTask;
    }
}
