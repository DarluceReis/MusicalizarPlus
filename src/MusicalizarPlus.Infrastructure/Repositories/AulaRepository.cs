using MusicalizarPlus.Application.Abstractions.Repositories;
using MusicalizarPlus.Domain.Entities;
using Npgsql;

namespace MusicalizarPlus.Infrastructure.Repositories;

public sealed class AulaRepository(NpgsqlDataSource dataSource) : IAulaRepository
{
    public async Task<Aula?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            select id_aula, id_professor, titulo, descricao, nivel, data_criacao
            from aulas
            where id_aula = @id
            """;
        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? Read(reader) : null;
    }

    public async Task<IReadOnlyList<Aula>> ListAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            select id_aula, id_professor, titulo, descricao, nivel, data_criacao
            from aulas
            order by data_criacao desc
            """;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var result = new List<Aula>();
        while (await reader.ReadAsync(cancellationToken))
        {
            result.Add(Read(reader));
        }

        return result;
    }

    public async Task<Aula> CreateAsync(Aula aula, CancellationToken cancellationToken = default)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            insert into aulas (id_professor, titulo, descricao, nivel)
            values (@id_professor, @titulo, @descricao, @nivel)
            returning id_aula, id_professor, titulo, descricao, nivel, data_criacao
            """;
        command.Parameters.AddWithValue("id_professor", aula.IdProfessor);
        command.Parameters.AddWithValue("titulo", aula.Titulo);
        command.Parameters.AddWithValue("descricao", (object?)aula.Descricao ?? DBNull.Value);
        command.Parameters.AddWithValue("nivel", (object?)aula.Nivel ?? DBNull.Value);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        await reader.ReadAsync(cancellationToken);
        return Read(reader);
    }

    private static Aula Read(NpgsqlDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        IdProfessor = reader.GetInt32(1),
        Titulo = reader.GetString(2),
        Descricao = reader.IsDBNull(3) ? null : reader.GetString(3),
        Nivel = reader.IsDBNull(4) ? null : reader.GetString(4),
        DataCriacao = reader.GetDateTime(5)
    };
}
