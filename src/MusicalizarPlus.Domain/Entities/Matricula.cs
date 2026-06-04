using MusicalizarPlus.Domain.Enums;

namespace MusicalizarPlus.Domain.Entities;

public sealed class Matricula
{
    public int Id { get; init; }
    public int IdAluno { get; init; }
    public int IdAula { get; init; }
    public DateTime DataMatricula { get; init; }
    public StatusMatricula Status { get; init; }
}
