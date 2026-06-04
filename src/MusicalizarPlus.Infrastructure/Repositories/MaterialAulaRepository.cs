using MusicalizarPlus.Application.Abstractions.Repositories;
using MusicalizarPlus.Domain.Entities;
using Npgsql;

namespace MusicalizarPlus.Infrastructure.Repositories;

public sealed class MaterialAulaRepository(NpgsqlDataSource dataSource) : IMaterialAulaRepository
{
    public async Task<IReadOnlyList<MaterialAula>> ListByAulaAsync(int idAula, CancellationToken cancellationToken = default)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            select id_material, id_aula, tipo, url_arquivo, descricao
            from materiais_aula
            where id_aula = @id_aula
            order by id_material
            """;
        command.Parameters.AddWithValue("id_aula", idAula);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var result = new List<MaterialAula>();
        while (await reader.ReadAsync(cancellationToken))
        {
            result.Add(Read(reader));
        }

        return result;
    }

    public async Task<MaterialAula> CreateAsync(MaterialAula material, CancellationToken cancellationToken = default)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            insert into materiais_aula (id_aula, tipo, url_arquivo, descricao)
            values (@id_aula, @tipo, @url_arquivo, @descricao)
            returning id_material, id_aula, tipo, url_arquivo, descricao
            """;
        command.Parameters.AddWithValue("id_aula", material.IdAula);
        command.Parameters.AddWithValue("tipo", material.Tipo);
        command.Parameters.AddWithValue("url_arquivo", material.UrlArquivo);
        command.Parameters.AddWithValue("descricao", (object?)material.Descricao ?? DBNull.Value);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        await reader.ReadAsync(cancellationToken);
        return Read(reader);
    }

    private static MaterialAula Read(NpgsqlDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        IdAula = reader.GetInt32(1),
        Tipo = reader.GetString(2),
        UrlArquivo = reader.GetString(3),
        Descricao = reader.IsDBNull(4) ? null : reader.GetString(4)
    };
}
