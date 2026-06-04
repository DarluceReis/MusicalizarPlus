using MusicalizarPlus.Application.Services;

namespace MusicalizarPlus.Api.Endpoints;

internal static class EndpointResults
{
    public static IResult From<T>(ServiceResult<T> result, Func<T, IResult>? onSuccess = null) =>
        result.IsSuccess && result.Value is not null
            ? onSuccess?.Invoke(result.Value) ?? Results.Ok(result.Value)
            : Results.BadRequest(new { message = result.Error });
}
