using MusicalizarPlus.Domain.Entities;

namespace MusicalizarPlus.Application.Abstractions.Repositories;

public interface IGravacaoRepository
{
    Task<Gravacao?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Gravacao>> ListByMatriculaAsync(int idMatricula, CancellationToken cancellationToken = default);
    Task<Gravacao> CreateAsync(Gravacao gravacao, CancellationToken cancellationToken = default);
}
