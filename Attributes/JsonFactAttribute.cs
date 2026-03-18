using System;
using System.Runtime.CompilerServices;
using Xunit;

public class JsonFactAttribute : FactAttribute
{
    public JsonFactAttribute(string className, [CallerMemberName] string memberName = "")
    {
        if (!FactTestConfig.ShouldRun(className, memberName))
        {
            Skip = "Skipped via Fact JSON configuration";
        }
    }
}
