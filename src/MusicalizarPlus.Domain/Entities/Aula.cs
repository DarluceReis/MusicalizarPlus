namespace MusicalizarPlus.Domain.Entities;

public sealed class Aula
{
    public int Id { get; init; }
    public int IdProfessor { get; init; }
    public required string Titulo { get; init; }
    public string? Descricao { get; init; }
    public string? Nivel { get; init; }
    public DateTime DataCriacao { get; init; }
}
