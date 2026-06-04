namespace MusicalizarPlus.Contracts.Materiais;

public sealed record CriarMaterialAulaRequest(int IdAula, string Tipo, string UrlArquivo, string? Descricao);
