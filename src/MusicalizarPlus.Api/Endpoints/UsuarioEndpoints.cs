using MusicalizarPlus.Application.Services;
using MusicalizarPlus.Contracts.Usuarios;

namespace MusicalizarPlus.Api.Endpoints;

public static class UsuarioEndpoints
{
    public static IEndpointRouteBuilder MapUsuarioEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/usuarios").WithTags("Usuarios");

        group.MapPost("/", async (CriarUsuarioRequest request, UsuarioService service, CancellationToken cancellationToken) =>
        {
            var result = await service.CriarAsync(request, cancellationToken);
            return EndpointResults.From(result, usuario => Results.Created($"/api/usuarios/{usuario.Id}", usuario));
        });

        group.MapGet("/{id:int}", async (int id, UsuarioService service, CancellationToken cancellationToken) =>
        {
            var usuario = await service.ObterAsync(id, cancellationToken);
            return usuario is null ? Results.NotFound() : Results.Ok(usuario);
        });

        group.MapPut("/{id:int}", async (int id, AtualizarUsuarioRequest request, UsuarioService service, CancellationToken cancellationToken) =>
        {
            var result = await service.AtualizarPerfilAsync(id, request, cancellationToken);
            return EndpointResults.From(result, Results.Ok);
        });

        group.MapPut("/{id:int}/senha", async (int id, AlterarSenhaUsuarioRequest request, UsuarioService service, CancellationToken cancellationToken) =>
        {
            var result = await service.AlterarSenhaAsync(id, request, cancellationToken);
            return EndpointResults.From(result, Results.Ok);
        });

        return app;
    }
}
