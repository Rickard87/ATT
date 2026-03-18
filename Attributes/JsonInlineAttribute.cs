using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Xunit.Sdk;

public class JsonInlineAttribute : DataAttribute
{
    private readonly string _className;

    public JsonInlineAttribute(string className)
    {
        _className = className;
    }

    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        if (testMethod == null)
            throw new ArgumentNullException(nameof(testMethod));

        var methodName = testMethod.Name;

        var rawData = TheoryTestConfig.GetData(_className, methodName);

        var parameters = testMethod.GetParameters();

        foreach (var item in rawData)
        {
            // Om vi redan har rätt typ, skicka direkt
            if (
                item.Length == parameters.Length
                && item.Zip(parameters, (val, param) => val.GetType() == param.ParameterType)
                    .All(b => b)
            )
            {
                yield return item;
                continue;
            }

            // Annars konvertera varje parameter
            var converted = new object[item.Length];
            for (int i = 0; i < item.Length; i++)
            {
                converted[i] = ConvertJsonElement(item[i], parameters[i].ParameterType);
            }

            yield return converted;
        }
    }

    private static object ConvertJsonElement(object obj, Type targetType)
    {
        if (obj is JsonElement el)
        {
            if (targetType == typeof(int))
                return el.GetInt32();
            if (targetType == typeof(long))
                return el.GetInt64();
            if (targetType == typeof(string))
                return el.GetString()!;
            if (targetType == typeof(bool))
                return el.GetBoolean();
            if (targetType == typeof(double))
                return el.GetDouble();
            if (targetType == typeof(decimal))
                return el.GetDecimal();

            // fallback: deserialisera till targetType
            return JsonSerializer.Deserialize(el.GetRawText(), targetType)!;
        }

        // Om det inte är JsonElement, försök bara convert
        return Convert.ChangeType(obj, targetType);
    }
}
