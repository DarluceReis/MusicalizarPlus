using MusicalizarPlus.Application.Abstractions.Repositories;
using MusicalizarPlus.Domain.Entities;
using Npgsql;

namespace MusicalizarPlus.Infrastructure.Repositories;

public sealed class FeedbackRepository(NpgsqlDataSource dataSource) : IFeedbackRepository
{
    public async Task<IReadOnlyList<Feedback>> ListByGravacaoAsync(int idGravacao, CancellationToken cancellationToken = default)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            select id_feedback, id_gravacao, id_professor, comentario, comentario_aluno, data_feedback
            from feedbacks
            where id_gravacao = @id_gravacao
            order by data_feedback desc
            """;
        command.Parameters.AddWithValue("id_gravacao", idGravacao);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var result = new List<Feedback>();
        while (await reader.ReadAsync(cancellationToken))
        {
            result.Add(Read(reader));
        }

        return result;
    }

    public async Task<Feedback> CreateAsync(Feedback feedback, CancellationToken cancellationToken = default)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            insert into feedbacks (id_gravacao, id_professor, comentario, comentario_aluno)
            values (@id_gravacao, @id_professor, @comentario, @comentario_aluno)
            returning id_feedback, id_gravacao, id_professor, comentario, comentario_aluno, data_feedback
            """;
        command.Parameters.AddWithValue("id_gravacao", feedback.IdGravacao);
        command.Parameters.AddWithValue("id_professor", feedback.IdProfessor);
        command.Parameters.AddWithValue("comentario", feedback.Comentario);
        command.Parameters.AddWithValue("comentario_aluno", (object?)feedback.ComentarioAluno ?? DBNull.Value);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        await reader.ReadAsync(cancellationToken);
        return Read(reader);
    }

    private static Feedback Read(NpgsqlDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        IdGravacao = reader.GetInt32(1),
        IdProfessor = reader.GetInt32(2),
        Comentario = reader.GetString(3),
        ComentarioAluno = reader.IsDBNull(4) ? null : reader.GetString(4),
        DataFeedback = reader.GetDateTime(5)
    };
}
