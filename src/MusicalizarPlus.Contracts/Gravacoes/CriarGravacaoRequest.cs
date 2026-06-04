namespace MusicalizarPlus.Contracts.Gravacoes;

public sealed record CriarGravacaoRequest(int IdMatricula, string CaminhoArquivo, string? ObservacaoAluno);
