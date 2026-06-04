using MusicalizarPlus.Domain.Entities;

namespace MusicalizarPlus.Application.Abstractions.Repositories;

public interface IAulaRepository
{
    Task<Aula?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Aula>> ListAsync(CancellationToken cancellationToken = default);
    Task<Aula> CreateAsync(Aula aula, CancellationToken cancellationToken = default);
}
