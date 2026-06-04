using System.Net.Http.Json;
using MusicalizarPlus.Contracts.Auth;
using MusicalizarPlus.Contracts.Aulas;
using MusicalizarPlus.Contracts.Usuarios;

namespace MusicalizarPlus.Services;

public sealed class MusicalizarPlusApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<AulaResponse>> ListarAulasAsync(CancellationToken cancellationToken = default) =>
        await httpClient.GetFromJsonAsync<List<AulaResponse>>("/api/aulas", cancellationToken) ?? [];

    public async Task<ApiResult<UsuarioResponse>> CriarUsuarioAsync(CriarUsuarioRequest request, CancellationToken cancellationToken = default) =>
        await SendAsync<UsuarioResponse>(HttpMethod.Post, "/api/usuarios", request, cancellationToken);

    public async Task<UsuarioResponse?> ObterUsuarioAsync(int id, CancellationToken cancellationToken = default) =>
        await httpClient.GetFromJsonAsync<UsuarioResponse>($"/api/usuarios/{id}", cancellationToken);

    public async Task<ApiResult<UsuarioResponse>> AtualizarUsuarioAsync(int id, AtualizarUsuarioRequest request, CancellationToken cancellationToken = default) =>
        await SendAsync<UsuarioResponse>(HttpMethod.Put, $"/api/usuarios/{id}", request, cancellationToken);

    public async Task<ApiResult<UsuarioResponse>> AlterarSenhaAsync(int id, AlterarSenhaUsuarioRequest request, CancellationToken cancellationToken = default) =>
        await SendAsync<UsuarioResponse>(HttpMethod.Put, $"/api/usuarios/{id}/senha", request, cancellationToken);

    public async Task<ApiResult<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default) =>
        await SendAsync<LoginResponse>(HttpMethod.Post, "/api/auth/login", request, cancellationToken);

    public async Task<ApiResult<AulaResponse>> CriarAulaAsync(CriarAulaRequest request, CancellationToken cancellationToken = default) =>
        await SendAsync<AulaResponse>(HttpMethod.Post, "/api/aulas", request, cancellationToken);

    private async Task<ApiResult<T>> SendAsync<T>(HttpMethod method, string url, object body, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, url)
        {
            Content = JsonContent.Create(body)
        };

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            var value = await response.Content.ReadFromJsonAsync<T>(cancellationToken);
            return value is null
                ? ApiResult<T>.Failure("Resposta vazia da API.")
                : ApiResult<T>.Success(value);
        }

        var error = await response.Content.ReadFromJsonAsync<ApiError>(cancellationToken);
        return ApiResult<T>.Failure(error?.Message ?? "Não foi possível concluir a operação.");
    }
}

public sealed record ApiResult<T>(T? Value, string? Error)
{
    public bool IsSuccess => Error is null;

    public static ApiResult<T> Success(T value) => new(value, null);
    public static ApiResult<T> Failure(string error) => new(default, error);
}

internal sealed record ApiError(string Message);
