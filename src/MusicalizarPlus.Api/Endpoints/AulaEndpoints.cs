using MusicalizarPlus.Application.Services;
using MusicalizarPlus.Contracts.Aulas;

namespace MusicalizarPlus.Api.Endpoints;

public static class AulaEndpoints
{
    public static IEndpointRouteBuilder MapAulaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/aulas").WithTags("Aulas");

        group.MapGet("/", async (AulaService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.ListarAsync(cancellationToken)));

        group.MapGet("/{id:int}", async (int id, AulaService service, CancellationToken cancellationToken) =>
        {
            var aula = await service.ObterAsync(id, cancellationToken);
            return aula is null ? Results.NotFound() : Results.Ok(aula);
        });

        group.MapPost("/", async (CriarAulaRequest request, AulaService service, CancellationToken cancellationToken) =>
        {
            var result = await service.CriarAsync(request, cancellationToken);
            return EndpointResults.From(result, aula => Results.Created($"/api/aulas/{aula.Id}", aula));
        });

        return app;
    }
}
