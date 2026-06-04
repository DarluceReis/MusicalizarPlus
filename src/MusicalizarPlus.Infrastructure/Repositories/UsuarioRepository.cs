using MusicalizarPlus.Application.Abstractions.Repositories;
using MusicalizarPlus.Domain.Entities;
using MusicalizarPlus.Infrastructure.Database;
using Npgsql;

namespace MusicalizarPlus.Infrastructure.Repositories;

public sealed class UsuarioRepository(NpgsqlDataSource dataSource) : IUsuarioRepository
{
    public async Task<Usuario?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            select id_usuario, nome, email, senha_hash, tipo, data_cadastro
            from usuarios
            where id_usuario = @id
            """;
        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? Read(reader) : null;
    }

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            select id_usuario, nome, email, senha_hash, tipo, data_cadastro
            from usuarios
            where email = @email
            """;
        command.Parameters.AddWithValue("email", email);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? Read(reader) : null;
    }

    public async Task<Usuario> CreateAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            insert into usuarios (nome, email, senha_hash, tipo)
            values (@nome, @email, @senha_hash, @tipo)
            returning id_usuario, nome, email, senha_hash, tipo, data_cadastro
            """;
        command.Parameters.AddWithValue("nome", usuario.Nome);
        command.Parameters.AddWithValue("email", usuario.Email);
        command.Parameters.AddWithValue("senha_hash", usuario.SenhaHash);
        command.Parameters.AddWithValue("tipo", PostgresTypeMapper.ToDatabaseValue(usuario.Tipo));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        await reader.ReadAsync(cancellationToken);
        return Read(reader);
    }

    public async Task<Usuario?> UpdateProfileAsync(int id, string nome, string email, CancellationToken cancellationToken = default)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            update usuarios
            set nome = @nome,
                email = @email
            where id_usuario = @id
            returning id_usuario, nome, email, senha_hash, tipo, data_cadastro
            """;
        command.Parameters.AddWithValue("id", id);
        command.Parameters.AddWithValue("nome", nome);
        command.Parameters.AddWithValue("email", email);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? Read(reader) : null;
    }

    public async Task<bool> UpdatePasswordAsync(int id, string senhaHash, CancellationToken cancellationToken = default)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            update usuarios
            set senha_hash = @senha_hash
            where id_usuario = @id
            """;
        command.Parameters.AddWithValue("id", id);
        command.Parameters.AddWithValue("senha_hash", senhaHash);

        var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
        return affectedRows == 1;
    }

    private static Usuario Read(NpgsqlDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        Nome = reader.GetString(1),
        Email = reader.GetString(2),
        SenhaHash = reader.GetString(3),
        Tipo = PostgresTypeMapper.ToTipoUsuario(reader.GetString(4)),
        DataCadastro = reader.GetDateTime(5)
    };
}
