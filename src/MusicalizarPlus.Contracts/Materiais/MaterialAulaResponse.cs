namespace MusicalizarPlus.Contracts.Materiais;

public sealed record MaterialAulaResponse(int Id, int IdAula, string Tipo, string UrlArquivo, string? Descricao);
