using MusicalizarPlus.Application.Abstractions.Repositories;
using MusicalizarPlus.Contracts.Aulas;
using MusicalizarPlus.Domain.Entities;
using MusicalizarPlus.Domain.Enums;

namespace MusicalizarPlus.Application.Services;

public sealed class AulaService(IAulaRepository aulas, IUsuarioRepository usuarios)
{
    public async Task<IReadOnlyList<AulaResponse>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var result = await aulas.ListAsync(cancellationToken);
        return result.Select(ToResponse).ToList();
    }

    public async Task<AulaResponse?> ObterAsync(int id, CancellationToken cancellationToken = default)
    {
        var aula = await aulas.GetByIdAsync(id, cancellationToken);
        return aula is null ? null : ToResponse(aula);
    }

    public async Task<ServiceResult<AulaResponse>> CriarAsync(CriarAulaRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Titulo))
        {
            return ServiceResult<AulaResponse>.Failure("Titulo e obrigatório.");
        }

        var professor = await usuarios.GetByIdAsync(request.IdProfessor, cancellationToken);
        if (professor is null || professor.Tipo != TipoUsuario.Professor)
        {
            return ServiceResult<AulaResponse>.Failure("Informe um professor válido para criar a aula.");
        }

        var aula = await aulas.CreateAsync(new Aula
        {
            IdProfessor = request.IdProfessor,
            Titulo = request.Titulo.Trim(),
            Descricao = request.Descricao?.Trim(),
            Nivel = request.Nivel?.Trim()
        }, cancellationToken);

        return ServiceResult<AulaResponse>.Success(ToResponse(aula));
    }

    private static AulaResponse ToResponse(Aula aula) =>
        new(aula.Id, aula.IdProfessor, aula.Titulo, aula.Descricao, aula.Nivel, aula.DataCriacao);
}
