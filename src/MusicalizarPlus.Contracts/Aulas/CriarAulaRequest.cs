namespace MusicalizarPlus.Contracts.Aulas;

public sealed record CriarAulaRequest(int IdProfessor, string Titulo, string? Descricao, string? Nivel);
