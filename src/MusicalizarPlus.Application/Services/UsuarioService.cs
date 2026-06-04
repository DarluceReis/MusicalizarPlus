using MusicalizarPlus.Application.Abstractions.Repositories;
using MusicalizarPlus.Application.Abstractions.Security;
using MusicalizarPlus.Contracts.Auth;
using MusicalizarPlus.Contracts.Usuarios;
using MusicalizarPlus.Domain.Entities;
using MusicalizarPlus.Domain.Enums;

namespace MusicalizarPlus.Application.Services;

public sealed class UsuarioService(IUsuarioRepository usuarios, IPasswordHasher passwordHasher)
{
    public async Task<ServiceResult<UsuarioResponse>> CriarAsync(CriarUsuarioRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
        {
            return ServiceResult<UsuarioResponse>.Failure("Nome e obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return ServiceResult<UsuarioResponse>.Failure("Email e obrigatório.");
        }

        if (request.Senha.Length < 6)
        {
            return ServiceResult<UsuarioResponse>.Failure("A senha deve ter pelo menos 6 caracteres.");
        }

        if (!TryParseTipo(request.Tipo, out var tipo))
        {
            return ServiceResult<UsuarioResponse>.Failure("Tipo deve ser ALUNO ou PROFESSOR.");
        }

        var email = request.Email.Trim().ToLowerInvariant();
        var existente = await usuarios.GetByEmailAsync(email, cancellationToken);
        if (existente is not null)
        {
            return ServiceResult<UsuarioResponse>.Failure("Já existe um usuario com este email.");
        }

        var usuario = await usuarios.CreateAsync(new Usuario
        {
            Nome = request.Nome.Trim(),
            Email = email,
            SenhaHash = passwordHasher.Hash(request.Senha),
            Tipo = tipo
        }, cancellationToken);

        return ServiceResult<UsuarioResponse>.Success(ToResponse(usuario));
    }

    public async Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var usuario = await usuarios.GetByEmailAsync(email, cancellationToken);
        if (usuario is null || !passwordHasher.Verify(request.Senha, usuario.SenhaHash))
        {
            return ServiceResult<LoginResponse>.Failure("Email ou senha inválidos.");
        }

        return ServiceResult<LoginResponse>.Success(new LoginResponse(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            ToDatabaseValue(usuario.Tipo)));
    }

    public async Task<UsuarioResponse?> ObterAsync(int id, CancellationToken cancellationToken = default)
    {
        var usuario = await usuarios.GetByIdAsync(id, cancellationToken);
        return usuario is null ? null : ToResponse(usuario);
    }

    public async Task<ServiceResult<UsuarioResponse>> AtualizarPerfilAsync(int id, AtualizarUsuarioRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
        {
            return ServiceResult<UsuarioResponse>.Failure("Nome e obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return ServiceResult<UsuarioResponse>.Failure("Email e obrigatório.");
        }

        var email = request.Email.Trim().ToLowerInvariant();
        var existente = await usuarios.GetByEmailAsync(email, cancellationToken);
        if (existente is not null && existente.Id != id)
        {
            return ServiceResult<UsuarioResponse>.Failure("Já existe um usuario com este email.");
        }

        var usuario = await usuarios.UpdateProfileAsync(id, request.Nome.Trim(), email, cancellationToken);
        return usuario is null
            ? ServiceResult<UsuarioResponse>.Failure("Usuario não encontrado.")
            : ServiceResult<UsuarioResponse>.Success(ToResponse(usuario));
    }

    public async Task<ServiceResult<UsuarioResponse>> AlterarSenhaAsync(int id, AlterarSenhaUsuarioRequest request, CancellationToken cancellationToken = default)
    {
        if (request.NovaSenha.Length < 6)
        {
            return ServiceResult<UsuarioResponse>.Failure("A nova senha deve ter pelo menos 6 caracteres.");
        }

        if (request.NovaSenha != request.ConfirmarSenha)
        {
            return ServiceResult<UsuarioResponse>.Failure("As senhas não conferem.");
        }

        var usuario = await usuarios.GetByIdAsync(id, cancellationToken);
        if (usuario is null)
        {
            return ServiceResult<UsuarioResponse>.Failure("Usuario não encontrado.");
        }

        if (!passwordHasher.Verify(request.SenhaAtual, usuario.SenhaHash))
        {
            return ServiceResult<UsuarioResponse>.Failure("Senha atual inválida.");
        }

        var updated = await usuarios.UpdatePasswordAsync(id, passwordHasher.Hash(request.NovaSenha), cancellationToken);
        return updated
            ? ServiceResult<UsuarioResponse>.Success(ToResponse(usuario))
            : ServiceResult<UsuarioResponse>.Failure("Não foi possível alterar a senha.");
    }

    private static UsuarioResponse ToResponse(Usuario usuario) =>
        new(usuario.Id, usuario.Nome, usuario.Email, ToDatabaseValue(usuario.Tipo), usuario.DataCadastro);

    private static bool TryParseTipo(string value, out TipoUsuario tipo)
    {
        switch (value.Trim().ToUpperInvariant())
        {
            case "ALUNO":
                tipo = TipoUsuario.Aluno;
                return true;
            case "PROFESSOR":
                tipo = TipoUsuario.Professor;
                return true;
            default:
                tipo = default;
                return false;
        }
    }

    private static string ToDatabaseValue(TipoUsuario tipo) => tipo switch
    {
        TipoUsuario.Aluno => "ALUNO",
        TipoUsuario.Professor => "PROFESSOR",
        _ => throw new ArgumentOutOfRangeException(nameof(tipo))
    };
}
