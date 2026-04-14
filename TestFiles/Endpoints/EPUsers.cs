using Microsoft.Playwright;

namespace ATT;

public class EPUsers
{
    private PlaywrightDriver _playwrightDriver;

    public record UserRecord(int id, string name, string username, string email);

    public EPUsers(PlaywrightDriver playwrightDriver)
    {
        _playwrightDriver = playwrightDriver;
    }

    public async Task<(IAPIResponse, UserRecord)> GetUser(int userId)
    {
        var response = await _playwrightDriver.ApiRequestContext.GetAsync($"/users/{userId}");
        var data =
            await response.JsonAsync<UserRecord>()
            ?? throw new InvalidOperationException("Data was null here!");
        return (response, data);
    }
}
