namespace MusicalizarPlus.Contracts.Feedbacks;

public sealed record FeedbackResponse(int Id, int IdGravacao, int IdProfessor, string Comentario, string? ComentarioAluno, DateTime DataFeedback);
