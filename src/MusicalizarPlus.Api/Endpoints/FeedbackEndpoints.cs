using MusicalizarPlus.Application.Services;
using MusicalizarPlus.Contracts.Feedbacks;

namespace MusicalizarPlus.Api.Endpoints;

public static class FeedbackEndpoints
{
    public static IEndpointRouteBuilder MapFeedbackEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/feedbacks").WithTags("Feedbacks");

        group.MapGet("/gravacao/{idGravacao:int}", async (int idGravacao, FeedbackService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.ListarPorGravacaoAsync(idGravacao, cancellationToken)));

        group.MapPost("/", async (CriarFeedbackRequest request, FeedbackService service, CancellationToken cancellationToken) =>
        {
            var result = await service.CriarAsync(request, cancellationToken);
            return EndpointResults.From(result, feedback => Results.Created($"/api/feedbacks/{feedback.Id}", feedback));
        });

        return app;
    }
}
