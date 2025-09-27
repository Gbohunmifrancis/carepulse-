using System.Text.Json;

namespace SolveStation.Common.Extensions;

/// <summary>
/// Extension methods for common operations
/// </summary>
public static class CommonExtensions
{
    /// <summary>
    /// Converts object to JSON string
    /// </summary>
    public static string ToJson(this object obj, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Serialize(obj, options ?? new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });
    }

    /// <summary>
    /// Converts JSON string to object
    /// </summary>
    public static T? FromJson<T>(this string json, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize<T>(json, options ?? new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    /// <summary>
    /// Checks if string is null or empty
    /// </summary>
    public static bool IsNullOrEmpty(this string? value)
    {
        return string.IsNullOrEmpty(value);
    }

    /// <summary>
    /// Checks if string is null, empty or whitespace
    /// </summary>
    public static bool IsNullOrWhiteSpace(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// Safe substring operation
    /// </summary>
    public static string SafeSubstring(this string value, int startIndex, int length)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        if (startIndex >= value.Length)
            return string.Empty;

        if (startIndex + length > value.Length)
            length = value.Length - startIndex;

        return value.Substring(startIndex, length);
    }

    /// <summary>
    /// Converts DateTime to UTC if not already
    /// </summary>
    public static DateTime ToUtc(this DateTime dateTime)
    {
        return dateTime.Kind == DateTimeKind.Utc
            ? dateTime
            : DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
    }

    /// <summary>
    /// Chunks an enumerable into batches of specified size
    /// </summary>
    public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int size)
    {
        if (size <= 0)
            throw new ArgumentException("Size must be greater than 0", nameof(size));

        var list = source.ToList();
        for (int i = 0; i < list.Count; i += size)
        {
            yield return list.Skip(i).Take(size);
        }
    }

    /// <summary>
    /// Executes an action if condition is true
    /// </summary>
    public static T If<T>(this T obj, bool condition, Action<T> action)
    {
        if (condition)
            action(obj);
        return obj;
    }

    /// <summary>
    /// Executes an action if object is not null
    /// </summary>
    public static T IfNotNull<T>(this T? obj, Action<T> action) where T : class
    {
        if (obj != null)
            action(obj);
        return obj!;
    }

    /// <summary>
    /// Returns default value if object is null
    /// </summary>
    public static T OrDefault<T>(this T? obj, T defaultValue) where T : class
    {
        return obj ?? defaultValue;
    }

    /// <summary>
    /// Applies a function if object is not null
    /// </summary>
    public static TResult? Map<T, TResult>(this T? obj, Func<T, TResult> func) where T : class
    {
        return obj != null ? func(obj) : default;
    }
}
