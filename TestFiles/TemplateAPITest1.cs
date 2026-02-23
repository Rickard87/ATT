namespace ATT;

public class TemplateAPITest1 : IClassFixture<PlaywrightDriver>
{
    private PlaywrightDriver _playwrightDriver;
    private EPUsers _ePUsers;

    public TemplateAPITest1(PlaywrightDriver playwrightDriver)
    {
        _playwrightDriver = playwrightDriver;
        _ePUsers = new(_playwrightDriver);
    }

    [Fact]
    public async Task FirstTest()
    {
        var (userResponse, userData) = await _ePUsers.GetUser(9);
        Assert.True(
            userResponse.Status == 200 || userResponse.Status == 201,
            $"Unexpected status code: {userResponse.Status}"
        );
        Assert.Equal("Rickard", userData.name);
    }
}
