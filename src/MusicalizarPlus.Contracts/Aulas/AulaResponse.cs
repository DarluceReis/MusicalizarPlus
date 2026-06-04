namespace MusicalizarPlus.Contracts.Aulas;

public sealed record AulaResponse(int Id, int IdProfessor, string Titulo, string? Descricao, string? Nivel, DateTime DataCriacao);
