namespace _Net6CleanArchitectureQuizzApp.Application.Common.Models;

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }

    public string?[] Errors { get; set; }

    public int? Id { get; } // Optional ID for created entities

    protected Result(bool isSuccess, string error, int? id = -1)
    {
        IsSuccess = isSuccess;
        Error = error;
        Id = id;
    }
    internal Result(bool succeeded, IEnumerable<string> errors)
    {
        IsSuccess = succeeded;
        Errors = errors.ToArray();
    }

    public static Result Success(int? id = null) => new Result(true, null, id);
    public static Result Failure(string error) => new Result(false, error);
    public static Result Failure(string[] errors) => new Result(false, errors);

}

// Generic version if needed
public class Result<T> : Result
{
    public T Value { get; }

    protected Result(bool isSuccess, T value, string error, int? id = -1)
        : base(isSuccess, error, id)
    {
        Value = value;
    }

    public static Result<T> Success(T value, int? id = -1) => new Result<T>(true, value ,null, id);
    public static new Result<T> Failure(string error) => new Result<T>(false, default!, error);
}