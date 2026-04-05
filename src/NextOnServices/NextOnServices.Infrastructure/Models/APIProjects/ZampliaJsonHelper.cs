using System.Globalization;
using System.Text.Json;

namespace NextOnServices.Infrastructure.Models.APIProjects;

public static class ZampliaJsonHelper
{
    public static bool TryGetJsonPropertyIgnoreCase(JsonElement element, out JsonElement value, params string[] propertyNames)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            value = default;
            return false;
        }

        foreach (var property in element.EnumerateObject())
        {
            if (propertyNames.Any(name => string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase)))
            {
                value = property.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    public static string? GetJsonString(JsonElement element, params string[] propertyNames)
    {
        if (!TryGetJsonPropertyIgnoreCase(element, out var value, propertyNames))
        {
            return null;
        }

        return value.ValueKind switch
        {
            JsonValueKind.String => value.GetString(),
            JsonValueKind.Number => value.ToString(),
            JsonValueKind.True => bool.TrueString,
            JsonValueKind.False => bool.FalseString,
            JsonValueKind.Null => null,
            JsonValueKind.Undefined => null,
            _ => value.GetRawText()
        };
    }

    public static int? GetJsonInt(JsonElement element, params string[] propertyNames)
    {
        if (!TryGetJsonPropertyIgnoreCase(element, out var value, propertyNames))
        {
            return null;
        }

        if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var intValue))
        {
            return intValue;
        }

        return value.ValueKind == JsonValueKind.String &&
               int.TryParse(value.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out intValue)
            ? intValue
            : null;
    }

    public static long? GetJsonLong(JsonElement element, params string[] propertyNames)
    {
        if (!TryGetJsonPropertyIgnoreCase(element, out var value, propertyNames))
        {
            return null;
        }

        if (value.ValueKind == JsonValueKind.Number && value.TryGetInt64(out var longValue))
        {
            return longValue;
        }

        return value.ValueKind == JsonValueKind.String &&
               long.TryParse(value.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out longValue)
            ? longValue
            : null;
    }

    public static decimal? GetJsonDecimal(JsonElement element, params string[] propertyNames)
    {
        if (!TryGetJsonPropertyIgnoreCase(element, out var value, propertyNames))
        {
            return null;
        }

        if (value.ValueKind == JsonValueKind.Number && value.TryGetDecimal(out var decimalValue))
        {
            return decimalValue;
        }

        return value.ValueKind == JsonValueKind.String &&
               decimal.TryParse(value.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimalValue)
            ? decimalValue
            : null;
    }

    public static DateTime? GetJsonDateTime(JsonElement element, params string[] propertyNames)
    {
        var raw = GetJsonString(element, propertyNames);
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        return DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AllowWhiteSpaces, out var dateTime)
            ? dateTime
            : null;
    }

    public static bool? GetJsonBoolean(JsonElement element, params string[] propertyNames)
    {
        if (!TryGetJsonPropertyIgnoreCase(element, out var value, propertyNames))
        {
            return null;
        }

        return value.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.String when bool.TryParse(value.GetString(), out var boolValue) => boolValue,
            JsonValueKind.Number when value.TryGetInt32(out var intValue) => intValue != 0,
            _ => null
        };
    }

    public static List<JsonElement> ExtractArrayCandidates(JsonElement root, params string[] propertyNames)
    {
        var items = new List<JsonElement>();
        ExtractArrayCandidatesInternal(root, items, 0, propertyNames);
        return items;
    }

    public static JsonElement ResolveObjectCandidate(JsonElement root, params string[] propertyNames)
    {
        if (root.ValueKind == JsonValueKind.Array)
        {
            return ResolveFirstObjectFromArray(root);
        }

        if (root.ValueKind == JsonValueKind.Object &&
            TryGetJsonPropertyIgnoreCase(root, out var directMatch, propertyNames))
        {
            var directObject = ResolveObjectCandidate(directMatch, propertyNames);
            if (directObject.ValueKind == JsonValueKind.Object)
            {
                return directObject;
            }
        }

        if (root.ValueKind == JsonValueKind.Object)
        {
            foreach (var wrapperName in GetWrapperNames())
            {
                if (!TryGetJsonPropertyIgnoreCase(root, out var wrapperCandidate, wrapperName))
                {
                    continue;
                }

                if (wrapperCandidate.ValueKind == JsonValueKind.Object)
                {
                    var nestedMatch = ResolveObjectCandidate(wrapperCandidate, propertyNames);
                    if (nestedMatch.ValueKind == JsonValueKind.Object)
                    {
                        return nestedMatch;
                    }
                }
            }
        }

        return root.ValueKind == JsonValueKind.Object ? root.Clone() : default;
    }

    private static JsonElement ResolveFirstObjectFromArray(JsonElement arrayElement)
    {
        if (arrayElement.ValueKind != JsonValueKind.Array)
        {
            return default;
        }

        foreach (var item in arrayElement.EnumerateArray())
        {
            if (item.ValueKind == JsonValueKind.Object)
            {
                return item.Clone();
            }

            if (item.ValueKind == JsonValueKind.Array)
            {
                var nestedObject = ResolveFirstObjectFromArray(item);
                if (nestedObject.ValueKind == JsonValueKind.Object)
                {
                    return nestedObject;
                }
            }
        }

        return default;
    }

    private static void ExtractArrayCandidatesInternal(JsonElement root, List<JsonElement> items, int depth, params string[] propertyNames)
    {
        if (depth > 4 || items.Count > 0)
        {
            return;
        }

        if (root.ValueKind == JsonValueKind.Array)
        {
            items.AddRange(root.EnumerateArray().Select(item => item.Clone()));
            return;
        }

        if (root.ValueKind != JsonValueKind.Object)
        {
            return;
        }

        foreach (var propertyName in propertyNames.Concat(new[] { "data", "Data", "results", "Results", "payload", "Payload" }))
        {
            if (!TryGetJsonPropertyIgnoreCase(root, out var candidate, propertyName))
            {
                continue;
            }

            if (candidate.ValueKind == JsonValueKind.Array)
            {
                items.AddRange(candidate.EnumerateArray().Select(item => item.Clone()));
                return;
            }

            if (candidate.ValueKind == JsonValueKind.Object)
            {
                ExtractArrayCandidatesInternal(candidate, items, depth + 1, propertyNames);
                if (items.Count > 0)
                {
                    return;
                }
            }
        }

        foreach (var wrapperName in GetWrapperNames())
        {
            if (TryGetJsonPropertyIgnoreCase(root, out var wrapperCandidate, wrapperName))
            {
                ExtractArrayCandidatesInternal(wrapperCandidate, items, depth + 1, propertyNames);
                if (items.Count > 0)
                {
                    return;
                }
            }
        }
    }

    private static string[] GetWrapperNames() => new[] { "result", "Result", "response", "Response", "payload", "Payload", "data", "Data" };
}
