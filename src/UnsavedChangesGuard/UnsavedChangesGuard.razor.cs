using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace UnsavedChangesGuard;

/// <summary>
/// Intercepts in-app navigation and browser tab/window unload to prompt
/// the user before losing unsaved edits.
/// </summary>
public partial class UnsavedChangesGuard : IAsyncDisposable
{
    /// <summary>
    /// Whether unsaved changes are present. When <c>true</c> the guard is active.
    /// </summary>
    [Parameter] public bool When { get; set; }

    /// <summary>
    /// Message shown in the in-app confirmation dialog.
    /// Defaults to a sensible English string.
    /// </summary>
    [Parameter] public string Message { get; set; } = "You have unsaved changes. Are you sure you want to leave?";

    private IDisposable? _locationChangingHandler;
    private bool _jsGuardActive;
    private IJSObjectReference? _module;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _module = await JS.InvokeAsync<IJSObjectReference>(
                "import", "./_content/UnsavedChangesGuard/unsavedchangesguard.js");
        }

        await SyncGuardAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        await SyncGuardAsync();
    }

    private async Task SyncGuardAsync()
    {
        if (When && _locationChangingHandler is null)
        {
            _locationChangingHandler = Navigation.RegisterLocationChangingHandler(OnLocationChanging);
        }
        else if (!When && _locationChangingHandler is not null)
        {
            _locationChangingHandler.Dispose();
            _locationChangingHandler = null;
        }

        if (_module is not null)
        {
            if (When && !_jsGuardActive)
            {
                await _module.InvokeVoidAsync("registerBeforeUnload");
                _jsGuardActive = true;
            }
            else if (!When && _jsGuardActive)
            {
                await _module.InvokeVoidAsync("unregisterBeforeUnload");
                _jsGuardActive = false;
            }
        }
    }

    private async ValueTask OnLocationChanging(LocationChangingContext context)
    {
        if (!When)
        {
            return;
        }

        var confirmed = await JS.InvokeAsync<bool>("confirm", Message);
        if (!confirmed)
        {
            context.PreventNavigation();
        }
    }

    public async ValueTask DisposeAsync()
    {
        _locationChangingHandler?.Dispose();

        if (_module is not null)
        {
            if (_jsGuardActive)
            {
                try
                {
                    await _module.InvokeVoidAsync("unregisterBeforeUnload");
                }
                catch
                {
                }
            }

            await _module.DisposeAsync();
        }
    }
}
