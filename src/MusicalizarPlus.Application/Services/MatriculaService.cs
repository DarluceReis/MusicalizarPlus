using MusicalizarPlus.Application.Abstractions.Repositories;
using MusicalizarPlus.Contracts.Matriculas;
using MusicalizarPlus.Domain.Entities;
using MusicalizarPlus.Domain.Enums;

namespace MusicalizarPlus.Application.Services;

public sealed class MatriculaService(IMatriculaRepository matriculas, IUsuarioRepository usuarios, IAulaRepository aulas)
{
    public async Task<IReadOnlyList<MatriculaResponse>> ListarPorAlunoAsync(int idAluno, CancellationToken cancellationToken = default)
    {
        var result = await matriculas.ListByAlunoAsync(idAluno, cancellationToken);
        return result.Select(ToResponse).ToList();
    }

    public async Task<ServiceResult<MatriculaResponse>> CriarAsync(CriarMatriculaRequest request, CancellationToken cancellationToken = default)
    {
        var aluno = await usuarios.GetByIdAsync(request.IdAluno, cancellationToken);
        if (aluno is null || aluno.Tipo != TipoUsuario.Aluno)
        {
            return ServiceResult<MatriculaResponse>.Failure("Informe um aluno válido para a matricula.");
        }

        if (await aulas.GetByIdAsync(request.IdAula, cancellationToken) is null)
        {
            return ServiceResult<MatriculaResponse>.Failure("Aula não encontrada.");
        }

        var matricula = await matriculas.CreateAsync(new Matricula
        {
            IdAluno = request.IdAluno,
            IdAula = request.IdAula,
            Status = StatusMatricula.Ativa
        }, cancellationToken);

        return ServiceResult<MatriculaResponse>.Success(ToResponse(matricula));
    }

    private static MatriculaResponse ToResponse(Matricula matricula) =>
        new(matricula.Id, matricula.IdAluno, matricula.IdAula, ToDatabaseValue(matricula.Status), matricula.DataMatricula);

    private static string ToDatabaseValue(StatusMatricula status) => status switch
    {
        StatusMatricula.Ativa => "ATIVA",
        StatusMatricula.Cancelada => "CANCELADA",
        StatusMatricula.Concluida => "CONCLUIDA",
        _ => throw new ArgumentOutOfRangeException(nameof(status))
    };
}
