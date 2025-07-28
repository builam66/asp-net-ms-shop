namespace BuildingBlocks.Common;

public class Result
{
    protected Result(bool isSuccess, string error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }

    public static Result Success() => new(true, string.Empty);
    public static Result Failure(string error) => new(false, error);

    public static Result<T> Success<T>(T value) => new(value, true, string.Empty);
    public static Result<T> Failure<T>(string error) => new(default!, false, error);
}

public class Result<T> : Result
{
    private readonly T _value;

    protected internal Result(T value, bool isSuccess, string error) : base(isSuccess, error)
    {
        _value = value;
    }

    public T Value => IsSuccess ? _value : throw new InvalidOperationException("Cannot access value of a failed result");

    public static implicit operator Result<T>(T value) => Success(value);
    public static implicit operator Result<T>(string error) => Failure<T>(error);
}

public static class ResultExtensions
{
    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> mapper)
    {
        return result.IsSuccess 
            ? Result.Success(mapper(result.Value)) 
            : Result.Failure<TOut>(result.Error);
    }

    public static async Task<Result<TOut>> MapAsync<TIn, TOut>(this Result<TIn> result, Func<TIn, Task<TOut>> mapper)
    {
        return result.IsSuccess 
            ? Result.Success(await mapper(result.Value)) 
            : Result.Failure<TOut>(result.Error);
    }

    public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, string error)
    {
        if (result.IsFailure)
            return result;

        return predicate(result.Value) 
            ? result 
            : Result.Failure<T>(error);
    }

    public static async Task<Result<T>> EnsureAsync<T>(this Result<T> result, Func<T, Task<bool>> predicate, string error)
    {
        if (result.IsFailure)
            return result;

        return await predicate(result.Value) 
            ? result 
            : Result.Failure<T>(error);
    }
}