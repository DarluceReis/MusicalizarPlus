namespace MusicalizarPlus.Contracts.Gravacoes;

public sealed record GravacaoResponse(int Id, int IdMatricula, string CaminhoArquivo, DateTime DataEnvio, string? ObservacaoAluno);
