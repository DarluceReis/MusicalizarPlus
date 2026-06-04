using MusicalizarPlus.Application.Abstractions.Repositories;
using MusicalizarPlus.Contracts.Feedbacks;
using MusicalizarPlus.Domain.Entities;
using MusicalizarPlus.Domain.Enums;

namespace MusicalizarPlus.Application.Services;

public sealed class FeedbackService(IFeedbackRepository feedbacks, IGravacaoRepository gravacoes, IUsuarioRepository usuarios)
{
    public async Task<IReadOnlyList<FeedbackResponse>> ListarPorGravacaoAsync(int idGravacao, CancellationToken cancellationToken = default)
    {
        var result = await feedbacks.ListByGravacaoAsync(idGravacao, cancellationToken);
        return result.Select(ToResponse).ToList();
    }

    public async Task<ServiceResult<FeedbackResponse>> CriarAsync(CriarFeedbackRequest request, CancellationToken cancellationToken = default)
    {
        if (await gravacoes.GetByIdAsync(request.IdGravacao, cancellationToken) is null)
        {
            return ServiceResult<FeedbackResponse>.Failure("Gravacao não encontrada.");
        }

        var professor = await usuarios.GetByIdAsync(request.IdProfessor, cancellationToken);
        if (professor is null || professor.Tipo != TipoUsuario.Professor)
        {
            return ServiceResult<FeedbackResponse>.Failure("Informe um professor válido para o feedback.");
        }

        if (string.IsNullOrWhiteSpace(request.Comentario))
        {
            return ServiceResult<FeedbackResponse>.Failure("Comentario e obrigatório.");
        }

        var feedback = await feedbacks.CreateAsync(new Feedback
        {
            IdGravacao = request.IdGravacao,
            IdProfessor = request.IdProfessor,
            Comentario = request.Comentario.Trim(),
            ComentarioAluno = request.ComentarioAluno?.Trim()
        }, cancellationToken);

        return ServiceResult<FeedbackResponse>.Success(ToResponse(feedback));
    }

    private static FeedbackResponse ToResponse(Feedback feedback) =>
        new(feedback.Id, feedback.IdGravacao, feedback.IdProfessor, feedback.Comentario, feedback.ComentarioAluno, feedback.DataFeedback);
}
