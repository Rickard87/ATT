using System;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace ATT;

public class PlaywrightDriver : IAsyncLifetime
{
    private IPlaywright _playwright = null!;
    public IAPIRequestContext ApiRequestContext { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();

        ApiRequestContext = await _playwright.APIRequest.NewContextAsync(
            new APIRequestNewContextOptions { BaseURL = AppConfig.BaseURL }
        );
    }

    public async Task DisposeAsync()
    {
        if (ApiRequestContext != null)
            await ApiRequestContext.DisposeAsync();

        _playwright?.Dispose();
    }
}
