using Microsoft.JSInterop;

namespace MudGantt
{
    internal class GanttInterop : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        public GanttInterop(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/MudGantt/gantt.js").AsTask());
        }


        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated)
            {
                try
                {
                    var module = await moduleTask.Value;
                    await module.DisposeAsync();
                }
                catch (JSDisconnectedException)
                {
                }
            }
        }

        internal async Task DestroyAsync(string id)
        {
            try
            {
                var module = await moduleTask.Value;
                await module.InvokeVoidAsync("destroyGantt", id);
            }
            catch (JSDisconnectedException)
            {
            }
        }

        internal async Task CreateAsync(string id, object callback)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("initGantt", id, $"#{id}", callback);
        }

        internal async Task UpdateAsync(string id, GanttData data)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("updateGantt", id, data);
        }
        internal async Task ResetZoomAsync(string id)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("resetZoomGantt", id);
        }
        internal async Task ZoomInAsync(string id, double amount = 1000.0)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("zoomInGantt", id, amount);
        }
        internal async Task ZoomOutAsync(string id, double amount = 1000.0)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("zoomOutGantt", id, amount);
        }

        internal async Task SetHorizontalScrollRatioAsync(string id, double ratio)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("setGanttScrollRatio", id, ratio);
        }
    }
}
