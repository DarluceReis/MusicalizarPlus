using MusicalizarPlus.Application.Abstractions.Repositories;
using MusicalizarPlus.Domain.Entities;
using MusicalizarPlus.Infrastructure.Database;
using Npgsql;

namespace MusicalizarPlus.Infrastructure.Repositories;

public sealed class MatriculaRepository(NpgsqlDataSource dataSource) : IMatriculaRepository
{
    public async Task<Matricula?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            select id_matricula, id_aluno, id_aula, data_matricula, status
            from matriculas
            where id_matricula = @id
            """;
        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? Read(reader) : null;
    }

    public async Task<IReadOnlyList<Matricula>> ListByAlunoAsync(int idAluno, CancellationToken cancellationToken = default)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            select id_matricula, id_aluno, id_aula, data_matricula, status
            from matriculas
            where id_aluno = @id_aluno
            order by data_matricula desc
            """;
        command.Parameters.AddWithValue("id_aluno", idAluno);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var result = new List<Matricula>();
        while (await reader.ReadAsync(cancellationToken))
        {
            result.Add(Read(reader));
        }

        return result;
    }

    public async Task<Matricula> CreateAsync(Matricula matricula, CancellationToken cancellationToken = default)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            insert into matriculas (id_aluno, id_aula, status)
            values (@id_aluno, @id_aula, @status)
            returning id_matricula, id_aluno, id_aula, data_matricula, status
            """;
        command.Parameters.AddWithValue("id_aluno", matricula.IdAluno);
        command.Parameters.AddWithValue("id_aula", matricula.IdAula);
        command.Parameters.AddWithValue("status", PostgresTypeMapper.ToDatabaseValue(matricula.Status));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        await reader.ReadAsync(cancellationToken);
        return Read(reader);
    }

    private static Matricula Read(NpgsqlDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        IdAluno = reader.GetInt32(1),
        IdAula = reader.GetInt32(2),
        DataMatricula = reader.GetDateTime(3),
        Status = PostgresTypeMapper.ToStatusMatricula(reader.GetString(4))
    };
}
