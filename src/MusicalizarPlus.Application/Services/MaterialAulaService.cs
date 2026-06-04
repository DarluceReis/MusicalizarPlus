using MusicalizarPlus.Application.Abstractions.Repositories;
using MusicalizarPlus.Contracts.Materiais;
using MusicalizarPlus.Domain.Entities;

namespace MusicalizarPlus.Application.Services;

public sealed class MaterialAulaService(IMaterialAulaRepository materiais, IAulaRepository aulas)
{
    public async Task<IReadOnlyList<MaterialAulaResponse>> ListarPorAulaAsync(int idAula, CancellationToken cancellationToken = default)
    {
        var result = await materiais.ListByAulaAsync(idAula, cancellationToken);
        return result.Select(ToResponse).ToList();
    }

    public async Task<ServiceResult<MaterialAulaResponse>> CriarAsync(CriarMaterialAulaRequest request, CancellationToken cancellationToken = default)
    {
        if (await aulas.GetByIdAsync(request.IdAula, cancellationToken) is null)
        {
            return ServiceResult<MaterialAulaResponse>.Failure("Aula não encontrada.");
        }

        if (string.IsNullOrWhiteSpace(request.Tipo) || string.IsNullOrWhiteSpace(request.UrlArquivo))
        {
            return ServiceResult<MaterialAulaResponse>.Failure("Tipo e URL do arquivo sao obrigatórios.");
        }

        var material = await materiais.CreateAsync(new MaterialAula
        {
            IdAula = request.IdAula,
            Tipo = request.Tipo.Trim(),
            UrlArquivo = request.UrlArquivo.Trim(),
            Descricao = request.Descricao?.Trim()
        }, cancellationToken);

        return ServiceResult<MaterialAulaResponse>.Success(ToResponse(material));
    }

    private static MaterialAulaResponse ToResponse(MaterialAula material) =>
        new(material.Id, material.IdAula, material.Tipo, material.UrlArquivo, material.Descricao);
}
