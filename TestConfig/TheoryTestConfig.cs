using System.Text.Json;

public static class TheoryTestConfig
{
    private static readonly Dictionary<string, List<object[]>> _data = new();

    static TheoryTestConfig()
    {
        var path = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "TestConfig",
            "testrunner_theories.json"
        );

        if (!File.Exists(path))
            return;

        var json = File.ReadAllText(path);

        var doc = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object[][]>>>(
            json
        );

        if (doc == null)
            return;

        foreach (var category in doc)
        {
            foreach (var method in category.Value)
            {
                var key = $"{category.Key}.{method.Key}";

                _data[key] = method.Value.Select(x => x.Cast<object>().ToArray()).ToList();
            }
        }
    }

    public static IEnumerable<object[]> GetData(string className, string methodName)
    {
        var key = $"{className}.{methodName}";

        if (_data.ContainsKey(key))
            return _data[key];

        return Enumerable.Empty<object[]>();
    }
}
