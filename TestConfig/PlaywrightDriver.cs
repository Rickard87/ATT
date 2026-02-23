using Microsoft.Playwright;

namespace ATT;

public class PlaywrightDriver : IDisposable
{
    private readonly Task<IAPIRequestContext?>? _requestContext = null;

    public PlaywrightDriver()
    {
        _requestContext = CreateAPIContext();
    }

    public IAPIRequestContext? ApiRequestContext => _requestContext?.GetAwaiter().GetResult();

    private async Task<IAPIRequestContext?> CreateAPIContext()
    {
        var playwright = await Playwright.CreateAsync();
        var requestContext = await playwright.APIRequest.NewContextAsync(
            new APIRequestNewContextOptions() { BaseURL = AppConfig.BaseURL }
        );
        return requestContext;
    }

    public void Dispose()
    {
        _requestContext?.Dispose();
    }
}
