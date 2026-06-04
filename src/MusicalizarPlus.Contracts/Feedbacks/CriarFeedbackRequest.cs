namespace MusicalizarPlus.Contracts.Feedbacks;

public sealed record CriarFeedbackRequest(int IdGravacao, int IdProfessor, string Comentario, string? ComentarioAluno);
