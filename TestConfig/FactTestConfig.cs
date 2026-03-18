using System.Text.Json;

public static class FactTestConfig
{
    private static readonly Dictionary<string, HashSet<string>> _testsToRun;

    static FactTestConfig()
    {
        var path = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "TestConfig",
            "testrunner_facts.json"
        );
        _testsToRun = new Dictionary<string, HashSet<string>>();

        if (!File.Exists(path))
            return;

        var json = File.ReadAllText(path);
        var doc = JsonSerializer.Deserialize<Dictionary<string, string[]>>(json);

        if (doc != null)
        {
            foreach (var kvp in doc)
            {
                _testsToRun[kvp.Key] = new HashSet<string>(kvp.Value);
            }
        }
    }

    public static bool ShouldRun(string className, string methodName)
    {
        return _testsToRun.ContainsKey(className) && _testsToRun[className].Contains(methodName);
    }
}
