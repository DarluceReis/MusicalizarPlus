using Microsoft.Extensions.DependencyInjection;
using MusicalizarPlus.Application.Abstractions.Repositories;
using MusicalizarPlus.Application.Abstractions.Security;
using MusicalizarPlus.Infrastructure.Repositories;
using MusicalizarPlus.Infrastructure.Security;
using Npgsql;

namespace MusicalizarPlus.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton(new NpgsqlDataSourceBuilder(connectionString).Build());
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IAulaRepository, AulaRepository>();
        services.AddScoped<IMaterialAulaRepository, MaterialAulaRepository>();
        services.AddScoped<IMatriculaRepository, MatriculaRepository>();
        services.AddScoped<IGravacaoRepository, GravacaoRepository>();
        services.AddScoped<IFeedbackRepository, FeedbackRepository>();

        return services;
    }
}
