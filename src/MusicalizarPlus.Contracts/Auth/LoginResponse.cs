namespace MusicalizarPlus.Contracts.Auth;

public sealed record LoginResponse(int IdUsuario, string Nome, string Email, string Tipo);
