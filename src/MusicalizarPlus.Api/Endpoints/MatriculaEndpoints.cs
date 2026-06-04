using MusicalizarPlus.Application.Services;
using MusicalizarPlus.Contracts.Matriculas;

namespace MusicalizarPlus.Api.Endpoints;

public static class MatriculaEndpoints
{
    public static IEndpointRouteBuilder MapMatriculaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/matriculas").WithTags("Matriculas");

        group.MapGet("/aluno/{idAluno:int}", async (int idAluno, MatriculaService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.ListarPorAlunoAsync(idAluno, cancellationToken)));

        group.MapPost("/", async (CriarMatriculaRequest request, MatriculaService service, CancellationToken cancellationToken) =>
        {
            var result = await service.CriarAsync(request, cancellationToken);
            return EndpointResults.From(result, matricula => Results.Created($"/api/matriculas/{matricula.Id}", matricula));
        });

        return app;
    }
}
