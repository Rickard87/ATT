namespace ATT;

// Example tests using https://jsonplaceholder.typicode.com
// These will not work unless BaseURL in appsettings.json points to that API.
public class UsersTests : IClassFixture<PlaywrightDriver>
{
    private PlaywrightDriver _playwrightDriver;
    private EPUsers _ePUsers;

    public UsersTests(PlaywrightDriver playwrightDriver)
    {
        _playwrightDriver = playwrightDriver;
        _ePUsers = new(_playwrightDriver);
    }

    // [Fact] always runs regardless of testrunner_facts.json
    [Fact]
    public async Task GetUser_AlwaysRuns()
    {
        var (userResponse, userData) = await _ePUsers.GetUser(1);
        Assert.Equal(200, userResponse.Status);
        Assert.Equal("Leanne Graham", userData.name);
    }

    // [JsonFact] only runs if registered in testrunner_facts.json
    [JsonFact("UsersTests")]
    public async Task GetUser_ResponseStatusIsOK()
    {
        var (userResponse, userData) = await _ePUsers.GetUser(1);
        Assert.Equal(200, userResponse.Status);
        Assert.Equal("Leanne Graham", userData.name);
    }

    // The key doesn't have to match the class name — it's a group/category for the JSON lookup
    [JsonFact("SmokeTests")]
    public async Task GetUser_SmokeTest()
    {
        var (userResponse, userData) = await _ePUsers.GetUser(1);
        Assert.Equal(200, userResponse.Status);
        Assert.Equal("Leanne Graham", userData.name);
    }

    // [Theory] + [JsonInline] reads test data from testrunner_theories.json using category and method name
    // The method must be registered in the JSON file — missing entries will crash, not skip
    [Theory]
    [JsonInline("UsersTests")]
    public async Task GetUserById_ResponseStatusIsOK(int userId)
    {
        var (userResponse, userData) = await _ePUsers.GetUser(userId);
        Assert.Equal(200, userResponse.Status);
        Assert.Equal(userId, userData.id);
    }
}
