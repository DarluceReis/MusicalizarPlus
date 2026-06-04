namespace MusicalizarPlus.Contracts.Matriculas;

public sealed record MatriculaResponse(int Id, int IdAluno, int IdAula, string Status, DateTime DataMatricula);
