using MusicalizarPlus.Domain.Entities;

namespace MusicalizarPlus.Application.Abstractions.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Usuario> CreateAsync(Usuario usuario, CancellationToken cancellationToken = default);
    Task<Usuario?> UpdateProfileAsync(int id, string nome, string email, CancellationToken cancellationToken = default);
    Task<bool> UpdatePasswordAsync(int id, string senhaHash, CancellationToken cancellationToken = default);
}
