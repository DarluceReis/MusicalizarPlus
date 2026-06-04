using MusicalizarPlus.Domain.Entities;

namespace MusicalizarPlus.Application.Abstractions.Repositories;

public interface IFeedbackRepository
{
    Task<IReadOnlyList<Feedback>> ListByGravacaoAsync(int idGravacao, CancellationToken cancellationToken = default);
    Task<Feedback> CreateAsync(Feedback feedback, CancellationToken cancellationToken = default);
}
