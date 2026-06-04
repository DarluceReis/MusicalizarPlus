using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using MusicalizarPlus.Components;
using MusicalizarPlus.Contracts.Auth;
using MusicalizarPlus.Contracts.Usuarios;
using MusicalizarPlus.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, ".aspnet-data-protection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "MusicalizarPlus.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/login";
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 500L * 1024 * 1024;
});
builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection("Storage"));
builder.Services.AddSingleton<IFileStorage>(services =>
{
    var options = services.GetRequiredService<Microsoft.Extensions.Options.IOptions<StorageOptions>>().Value;
    return string.Equals(options.Provider, "S3", StringComparison.OrdinalIgnoreCase)
        ? ActivatorUtilities.CreateInstance<S3FileStorage>(services)
        : ActivatorUtilities.CreateInstance<LocalFileStorage>(services);
});
builder.Services.AddSingleton<LearningContentStore>();
builder.Services.AddSingleton<ProfilePreferencesStore>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpClient<MusicalizarPlusApiClient>(client =>
{
    var baseUrl = builder.Configuration["Api:BaseUrl"] ?? "http://localhost:5298";
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(5);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();

app.MapGet("/materiais/aulas/{slug}", async (string slug, LearningContentStore store, IFileStorage storage, CancellationToken cancellationToken) =>
{
    var material = store.GetLesson(slug)?.Material;
    var file = await storage.OpenReadAsync(material, cancellationToken);
    return file is null
        ? Results.NotFound()
        : Results.File(file.Content, file.ContentType, file.FileName);
});

app.MapGet("/videos/aulas/{slug}", async (string slug, LearningContentStore store, IFileStorage storage, CancellationToken cancellationToken) =>
{
    var video = store.GetLesson(slug)?.Video;
    var file = await storage.OpenReadAsync(video, cancellationToken);
    return file is null
        ? Results.NotFound()
        : Results.File(file.Content, file.ContentType, enableRangeProcessing: true);
});

app.MapGet("/videos/praticas/{slug}", async (string slug, LearningContentStore store, IFileStorage storage, CancellationToken cancellationToken) =>
{
    var video = store.GetPractice(slug)?.VideoResponse;
    var file = await storage.OpenReadAsync(video, cancellationToken);
    return file is null
        ? Results.NotFound()
        : Results.File(file.Content, file.ContentType, enableRangeProcessing: true);
});

app.MapPost("/professor/praticas/{slug}/feedback", (
    string slug,
    [FromForm] string feedback,
    LearningContentStore store,
    HttpContext httpContext) =>
{
    var professorName = httpContext.User.Identity?.Name ?? "";
    if (store.GetPracticeForProfessor(slug, professorName) is null)
    {
        return Results.Redirect($"/professor/praticas?mensagem={Uri.EscapeDataString("Esta prática não pertence ao professor logado.")}");
    }

    if (!string.IsNullOrWhiteSpace(feedback))
    {
        store.SaveFeedback(slug, professorName, feedback);
        return Results.Redirect($"/professor/praticas/{slug}?mensagem={Uri.EscapeDataString("Feedback enviado para o aluno.")}");
    }

    return Results.Redirect($"/professor/praticas/{slug}?mensagem={Uri.EscapeDataString("Escreva um feedback antes de salvar.")}");
}).DisableAntiforgery();

app.MapPost("/aluno/praticas/{slug}/responder", (
    string slug,
    [FromForm] string resposta,
    LearningContentStore store) =>
{
    if (!string.IsNullOrWhiteSpace(resposta))
    {
        store.SaveStudentReply(slug, resposta);
        return Results.Redirect($"/aluno/pratica/{slug}?mensagem={Uri.EscapeDataString("Resposta enviada ao professor.")}");
    }

    return Results.Redirect($"/aluno/pratica/{slug}?mensagem={Uri.EscapeDataString("Escreva uma resposta antes de enviar.")}");
}).DisableAntiforgery();

app.MapPost("/aluno/praticas/{slug}/video", async (
    string slug,
    [FromForm] IFormFile video,
    LearningContentStore store,
    HttpContext httpContext) =>
{
    if (video.Length > 0)
    {
        await store.SavePracticeVideoAsync(slug, video, httpContext.User.Identity?.Name ?? "João Alves");
        return Results.Redirect($"/aluno/pratica/{slug}?mensagem={Uri.EscapeDataString("Vídeo resposta enviado ao professor.")}");
    }

    return Results.Redirect($"/aluno/pratica/{slug}?mensagem={Uri.EscapeDataString("Selecione um vídeo antes de enviar.")}");
}).DisableAntiforgery();

app.MapGet("/", (HttpContext httpContext) =>
{
    if (httpContext.User.Identity?.IsAuthenticated != true)
    {
        return Results.Redirect("/login");
    }

    return Results.Redirect(httpContext.User.IsInRole("PROFESSOR") ? "/professor/inicio" : "/aluno/aulas");
});

app.MapPost("/auth/login", async (
    [FromForm] string email,
    [FromForm] string senha,
    MusicalizarPlusApiClient api,
    HttpContext httpContext,
    CancellationToken cancellationToken) =>
{
    var result = await api.LoginAsync(new LoginRequest(email, senha), cancellationToken);
    if (!result.IsSuccess || result.Value is null)
    {
        return Results.Redirect($"/login?erro={Uri.EscapeDataString(result.Error ?? "E-mail ou senha inválidos.")}");
    }

    await SignInAsync(httpContext, result.Value.IdUsuario, result.Value.Nome, result.Value.Email, result.Value.Tipo);
    return Results.Redirect(result.Value.Tipo == "PROFESSOR" ? "/professor/inicio" : "/aluno/aulas");
}).DisableAntiforgery();

app.MapPost("/auth/register", async (
    [FromForm] string tipo,
    [FromForm] string nome,
    [FromForm] string email,
    [FromForm] string senha,
    [FromForm] string confirmarSenha,
    MusicalizarPlusApiClient api,
    HttpContext httpContext,
    CancellationToken cancellationToken) =>
{
    if (senha != confirmarSenha)
    {
        return Results.Redirect("/cadastro?erro=As%20senhas%20n%C3%A3o%20conferem.");
    }

    var result = await api.CriarUsuarioAsync(new CriarUsuarioRequest(nome, email, senha, tipo), cancellationToken);
    if (!result.IsSuccess || result.Value is null)
    {
        return Results.Redirect($"/cadastro?erro={Uri.EscapeDataString(result.Error ?? "Não foi possível criar a conta.")}");
    }

    await SignInAsync(httpContext, result.Value.Id, result.Value.Nome, result.Value.Email, result.Value.Tipo);
    return Results.Redirect(result.Value.Tipo == "PROFESSOR" ? "/professor/inicio" : "/aluno/aulas");
}).DisableAntiforgery();

app.MapGet("/auth/refresh-name", async (
    [FromQuery] string nome,
    [FromQuery] string? email,
    HttpContext httpContext) =>
{
    if (httpContext.User.Identity?.IsAuthenticated != true)
    {
        return Results.Redirect("/login");
    }

    var id = int.TryParse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out var parsedId) ? parsedId : 0;
    var currentEmail = string.IsNullOrWhiteSpace(email) ? httpContext.User.FindFirstValue(ClaimTypes.Email) ?? "" : email;
    var role = httpContext.User.FindFirstValue(ClaimTypes.Role) ?? "ALUNO";
    await SignInAsync(httpContext, id, nome, currentEmail, role);
    return Results.Redirect(role == "PROFESSOR" ? "/professor/dados" : "/aluno/dados");
}).DisableAntiforgery();

app.MapGet("/auth/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

static Task SignInAsync(HttpContext httpContext, int id, string nome, string email, string tipo)
{
    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, id.ToString()),
        new(ClaimTypes.Name, nome),
        new(ClaimTypes.Email, email),
        new(ClaimTypes.Role, tipo)
    };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    return httpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(identity),
        new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
        });
}
