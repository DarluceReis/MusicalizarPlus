namespace MusicalizarPlus.Domain.Entities;

public sealed class Gravacao
{
    public int Id { get; init; }
    public int IdMatricula { get; init; }
    public required string CaminhoArquivo { get; init; }
    public DateTime DataEnvio { get; init; }
    public string? ObservacaoAluno { get; init; }
}
