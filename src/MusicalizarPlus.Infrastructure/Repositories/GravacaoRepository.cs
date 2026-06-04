using MusicalizarPlus.Application.Abstractions.Repositories;
using MusicalizarPlus.Domain.Entities;
using Npgsql;

namespace MusicalizarPlus.Infrastructure.Repositories;

public sealed class GravacaoRepository(NpgsqlDataSource dataSource) : IGravacaoRepository
{
    public async Task<Gravacao?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            select id_gravacao, id_matricula, caminho_arquivo, data_envio, observacao_aluno
            from gravacoes
            where id_gravacao = @id
            """;
        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? Read(reader) : null;
    }

    public async Task<IReadOnlyList<Gravacao>> ListByMatriculaAsync(int idMatricula, CancellationToken cancellationToken = default)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            select id_gravacao, id_matricula, caminho_arquivo, data_envio, observacao_aluno
            from gravacoes
            where id_matricula = @id_matricula
            order by data_envio desc
            """;
        command.Parameters.AddWithValue("id_matricula", idMatricula);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var result = new List<Gravacao>();
        while (await reader.ReadAsync(cancellationToken))
        {
            result.Add(Read(reader));
        }

        return result;
    }

    public async Task<Gravacao> CreateAsync(Gravacao gravacao, CancellationToken cancellationToken = default)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            insert into gravacoes (id_matricula, caminho_arquivo, observacao_aluno)
            values (@id_matricula, @caminho_arquivo, @observacao_aluno)
            returning id_gravacao, id_matricula, caminho_arquivo, data_envio, observacao_aluno
            """;
        command.Parameters.AddWithValue("id_matricula", gravacao.IdMatricula);
        command.Parameters.AddWithValue("caminho_arquivo", gravacao.CaminhoArquivo);
        command.Parameters.AddWithValue("observacao_aluno", (object?)gravacao.ObservacaoAluno ?? DBNull.Value);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        await reader.ReadAsync(cancellationToken);
        return Read(reader);
    }

    private static Gravacao Read(NpgsqlDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        IdMatricula = reader.GetInt32(1),
        CaminhoArquivo = reader.GetString(2),
        DataEnvio = reader.GetDateTime(3),
        ObservacaoAluno = reader.IsDBNull(4) ? null : reader.GetString(4)
    };
}
