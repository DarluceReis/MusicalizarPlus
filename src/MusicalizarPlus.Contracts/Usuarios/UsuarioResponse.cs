namespace MusicalizarPlus.Contracts.Usuarios;

public sealed record UsuarioResponse(int Id, string Nome, string Email, string Tipo, DateTime DataCadastro);
