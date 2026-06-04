using MusicalizarPlus.Domain.Entities;

namespace MusicalizarPlus.Application.Abstractions.Repositories;

public interface IMatriculaRepository
{
    Task<Matricula?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Matricula>> ListByAlunoAsync(int idAluno, CancellationToken cancellationToken = default);
    Task<Matricula> CreateAsync(Matricula matricula, CancellationToken cancellationToken = default);
}
