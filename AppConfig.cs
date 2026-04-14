using System;
using Microsoft.Extensions.Configuration;

public static class AppConfig
{
    private static readonly IConfigurationRoot _config;

    static AppConfig()
    {
        _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json") // läser filen automatiskt vid bygg
            .Build();
    }

    public static string BaseURL =>
        _config["TestSettings:BaseURL"]
        ?? throw new InvalidOperationException(
            "TestSettings:BaseURL is not configured in appsettings.json"
        );
}
