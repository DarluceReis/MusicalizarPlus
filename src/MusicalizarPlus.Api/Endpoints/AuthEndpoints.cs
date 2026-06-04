using MusicalizarPlus.Application.Services;
using MusicalizarPlus.Contracts.Auth;

namespace MusicalizarPlus.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/login", async (LoginRequest request, UsuarioService service, CancellationToken cancellationToken) =>
        {
            var result = await service.LoginAsync(request, cancellationToken);
            return EndpointResults.From(result);
        });

        return app;
    }
}
