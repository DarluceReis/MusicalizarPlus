using MusicalizarPlus.Domain.Entities;

namespace MusicalizarPlus.Application.Abstractions.Repositories;

public interface IMaterialAulaRepository
{
    Task<IReadOnlyList<MaterialAula>> ListByAulaAsync(int idAula, CancellationToken cancellationToken = default);
    Task<MaterialAula> CreateAsync(MaterialAula material, CancellationToken cancellationToken = default);
}
