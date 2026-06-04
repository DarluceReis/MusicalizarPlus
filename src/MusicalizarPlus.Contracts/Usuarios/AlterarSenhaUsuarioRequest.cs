namespace MusicalizarPlus.Contracts.Usuarios;

public sealed record AlterarSenhaUsuarioRequest(string SenhaAtual, string NovaSenha, string ConfirmarSenha);
