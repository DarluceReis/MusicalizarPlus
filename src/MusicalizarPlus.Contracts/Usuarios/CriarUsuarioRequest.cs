namespace MusicalizarPlus.Contracts.Usuarios;

public sealed record CriarUsuarioRequest(string Nome, string Email, string Senha, string Tipo);
