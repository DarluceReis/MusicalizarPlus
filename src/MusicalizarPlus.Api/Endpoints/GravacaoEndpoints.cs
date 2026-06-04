using MusicalizarPlus.Application.Services;
using MusicalizarPlus.Contracts.Gravacoes;

namespace MusicalizarPlus.Api.Endpoints;

public static class GravacaoEndpoints
{
    public static IEndpointRouteBuilder MapGravacaoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/gravacoes").WithTags("Gravacoes");

        group.MapGet("/{id:int}", async (int id, GravacaoService service, CancellationToken cancellationToken) =>
        {
            var gravacao = await service.ObterAsync(id, cancellationToken);
            return gravacao is null ? Results.NotFound() : Results.Ok(gravacao);
        });

        group.MapGet("/matricula/{idMatricula:int}", async (int idMatricula, GravacaoService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.ListarPorMatriculaAsync(idMatricula, cancellationToken)));

        group.MapPost("/", async (CriarGravacaoRequest request, GravacaoService service, CancellationToken cancellationToken) =>
        {
            var result = await service.CriarAsync(request, cancellationToken);
            return EndpointResults.From(result, gravacao => Results.Created($"/api/gravacoes/{gravacao.Id}", gravacao));
        });

        return app;
    }
}
