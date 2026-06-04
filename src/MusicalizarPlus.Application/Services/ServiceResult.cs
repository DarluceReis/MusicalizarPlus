namespace MusicalizarPlus.Application.Services;

public sealed record ServiceResult<T>(T? Value, string? Error)
{
    public bool IsSuccess => Error is null;

    public static ServiceResult<T> Success(T value) => new(value, null);
    public static ServiceResult<T> Failure(string error) => new(default, error);
}
