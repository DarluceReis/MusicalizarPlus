using MusicalizarPlus.Api.Endpoints;
using MusicalizarPlus.Application.Services;
using MusicalizarPlus.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("web", policy =>
        policy.WithOrigins("https://localhost:7279", "http://localhost:5030")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<AulaService>();
builder.Services.AddScoped<MaterialAulaService>();
builder.Services.AddScoped<MatriculaService>();
builder.Services.AddScoped<GravacaoService>();
builder.Services.AddScoped<FeedbackService>();

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' não configurada.");
builder.Services.AddInfrastructure(connectionString);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors("web");

app.MapGet("/", () => Results.Ok(new { Application = "MusicalizarPlus.Api", Status = "OK" }));

app.MapAuthEndpoints();
app.MapUsuarioEndpoints();
app.MapAulaEndpoints();
app.MapMaterialAulaEndpoints();
app.MapMatriculaEndpoints();
app.MapGravacaoEndpoints();
app.MapFeedbackEndpoints();

app.Run();
