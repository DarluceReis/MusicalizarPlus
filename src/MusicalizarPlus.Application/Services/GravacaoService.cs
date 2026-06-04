using MusicalizarPlus.Application.Abstractions.Repositories;
using MusicalizarPlus.Contracts.Gravacoes;
using MusicalizarPlus.Domain.Entities;

namespace MusicalizarPlus.Application.Services;

public sealed class GravacaoService(IGravacaoRepository gravacoes, IMatriculaRepository matriculas)
{
    public async Task<GravacaoResponse?> ObterAsync(int id, CancellationToken cancellationToken = default)
    {
        var gravacao = await gravacoes.GetByIdAsync(id, cancellationToken);
        return gravacao is null ? null : ToResponse(gravacao);
    }

    public async Task<IReadOnlyList<GravacaoResponse>> ListarPorMatriculaAsync(int idMatricula, CancellationToken cancellationToken = default)
    {
        var result = await gravacoes.ListByMatriculaAsync(idMatricula, cancellationToken);
        return result.Select(ToResponse).ToList();
    }

    public async Task<ServiceResult<GravacaoResponse>> CriarAsync(CriarGravacaoRequest request, CancellationToken cancellationToken = default)
    {
        if (await matriculas.GetByIdAsync(request.IdMatricula, cancellationToken) is null)
        {
            return ServiceResult<GravacaoResponse>.Failure("Matricula não encontrada.");
        }

        if (string.IsNullOrWhiteSpace(request.CaminhoArquivo))
        {
            return ServiceResult<GravacaoResponse>.Failure("Caminho do arquivo e obrigatório.");
        }

        var gravacao = await gravacoes.CreateAsync(new Gravacao
        {
            IdMatricula = request.IdMatricula,
            CaminhoArquivo = request.CaminhoArquivo.Trim(),
            ObservacaoAluno = request.ObservacaoAluno?.Trim()
        }, cancellationToken);

        return ServiceResult<GravacaoResponse>.Success(ToResponse(gravacao));
    }

    private static GravacaoResponse ToResponse(Gravacao gravacao) =>
        new(gravacao.Id, gravacao.IdMatricula, gravacao.CaminhoArquivo, gravacao.DataEnvio, gravacao.ObservacaoAluno);
}
