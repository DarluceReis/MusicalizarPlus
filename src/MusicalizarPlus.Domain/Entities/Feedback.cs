namespace MusicalizarPlus.Domain.Entities;

public sealed class Feedback
{
    public int Id { get; init; }
    public int IdGravacao { get; init; }
    public int IdProfessor { get; init; }
    public required string Comentario { get; init; }
    public string? ComentarioAluno { get; init; }
    public DateTime DataFeedback { get; init; }
}
