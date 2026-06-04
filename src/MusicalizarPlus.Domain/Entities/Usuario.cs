using MusicalizarPlus.Domain.Enums;

namespace MusicalizarPlus.Domain.Entities;

public sealed class Usuario
{
    public int Id { get; init; }
    public required string Nome { get; init; }
    public required string Email { get; init; }
    public required string SenhaHash { get; init; }
    public TipoUsuario Tipo { get; init; }
    public DateTime DataCadastro { get; init; }
}
