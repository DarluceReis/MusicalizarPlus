namespace MusicalizarPlus.Domain.Entities;

public sealed class MaterialAula
{
    public int Id { get; init; }
    public int IdAula { get; init; }
    public required string Tipo { get; init; }
    public required string UrlArquivo { get; init; }
    public string? Descricao { get; init; }
}
