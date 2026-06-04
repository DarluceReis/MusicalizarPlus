using MusicalizarPlus.Domain.Enums;

namespace MusicalizarPlus.Infrastructure.Database;

internal static class PostgresTypeMapper
{
    public static string ToDatabaseValue(TipoUsuario tipo) => tipo switch
    {
        TipoUsuario.Aluno => "ALUNO",
        TipoUsuario.Professor => "PROFESSOR",
        _ => throw new ArgumentOutOfRangeException(nameof(tipo))
    };

    public static TipoUsuario ToTipoUsuario(string tipo) => tipo switch
    {
        "ALUNO" => TipoUsuario.Aluno,
        "PROFESSOR" => TipoUsuario.Professor,
        _ => throw new ArgumentOutOfRangeException(nameof(tipo))
    };

    public static string ToDatabaseValue(StatusMatricula status) => status switch
    {
        StatusMatricula.Ativa => "ATIVA",
        StatusMatricula.Cancelada => "CANCELADA",
        StatusMatricula.Concluida => "CONCLUIDA",
        _ => throw new ArgumentOutOfRangeException(nameof(status))
    };

    public static StatusMatricula ToStatusMatricula(string status) => status switch
    {
        "ATIVA" => StatusMatricula.Ativa,
        "CANCELADA" => StatusMatricula.Cancelada,
        "CONCLUIDA" => StatusMatricula.Concluida,
        _ => throw new ArgumentOutOfRangeException(nameof(status))
    };
}
