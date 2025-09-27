namespace SolveStation.Common.Models;

/// <summary>
/// Represents the result of an operation that can succeed or fail
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }

    protected Result(bool isSuccess, string error)
    {
        if (isSuccess && !string.IsNullOrEmpty(error))
            throw new InvalidOperationException("Successful result cannot have an error");

        if (!isSuccess && string.IsNullOrEmpty(error))
            throw new InvalidOperationException("Failed result must have an error");

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, string.Empty);
    public static Result Failure(string error) => new(false, error);

    public static Result<T> Success<T>(T value) => new(value, true, string.Empty);
    public static Result<T> Failure<T>(string error) => new(default!, false, error);

    public static implicit operator Result(string error) => Failure(error);
}

/// <summary>
/// Represents the result of an operation that can succeed with a value or fail
/// </summary>
/// <typeparam name="T">The type of the success value</typeparam>
public class Result<T> : Result
{
    public T Value { get; }

    internal Result(T value, bool isSuccess, string error) : base(isSuccess, error)
    {
        Value = value;
    }

    public static implicit operator Result<T>(T value) => Success(value);
    public static implicit operator Result<T>(string error) => Failure<T>(error);
}

/// <summary>
/// Result extensions for better usability
/// </summary>
public static class ResultExtensions
{
    public static Result<TResult> Map<T, TResult>(this Result<T> result, Func<T, TResult> func)
    {
        return result.IsSuccess
            ? Result.Success(func(result.Value))
            : Result.Failure<TResult>(result.Error);
    }

    public static async Task<Result<TResult>> MapAsync<T, TResult>(this Result<T> result, Func<T, Task<TResult>> func)
    {
        return result.IsSuccess
            ? Result.Success(await func(result.Value))
            : Result.Failure<TResult>(result.Error);
    }

    public static Result<TResult> Bind<T, TResult>(this Result<T> result, Func<T, Result<TResult>> func)
    {
        return result.IsSuccess ? func(result.Value) : Result.Failure<TResult>(result.Error);
    }

    public static async Task<Result<TResult>> BindAsync<T, TResult>(this Result<T> result, Func<T, Task<Result<TResult>>> func)
    {
        return result.IsSuccess ? await func(result.Value) : Result.Failure<TResult>(result.Error);
    }

    public static T Match<T>(this Result result, Func<T> onSuccess, Func<string, T> onFailure)
    {
        return result.IsSuccess ? onSuccess() : onFailure(result.Error);
    }

    public static T Match<TResult, T>(this Result<TResult> result, Func<TResult, T> onSuccess, Func<string, T> onFailure)
    {
        return result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);
    }

    public static async Task<T> MatchAsync<T>(this Result result, Func<Task<T>> onSuccess, Func<string, Task<T>> onFailure)
    {
        return result.IsSuccess ? await onSuccess() : await onFailure(result.Error);
    }

    public static void OnSuccess(this Result result, Action action)
    {
        if (result.IsSuccess)
            action();
    }

    public static void OnSuccess<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsSuccess)
            action(result.Value);
    }

    public static void OnFailure(this Result result, Action<string> action)
    {
        if (result.IsFailure)
            action(result.Error);
    }
}
