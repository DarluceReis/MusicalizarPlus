using MusicalizarPlus.Application.Services;
using MusicalizarPlus.Contracts.Materiais;

namespace MusicalizarPlus.Api.Endpoints;

public static class MaterialAulaEndpoints
{
    public static IEndpointRouteBuilder MapMaterialAulaEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/aulas/{idAula:int}/materiais", async (int idAula, MaterialAulaService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.ListarPorAulaAsync(idAula, cancellationToken)));

        app.MapPost("/api/materiais", async (CriarMaterialAulaRequest request, MaterialAulaService service, CancellationToken cancellationToken) =>
        {
            var result = await service.CriarAsync(request, cancellationToken);
            return EndpointResults.From(result, material => Results.Created($"/api/aulas/{material.IdAula}/materiais", material));
        });

        return app;
    }
}
