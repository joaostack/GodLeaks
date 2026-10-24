using GodLeaks.Infrastructure.DependencyInjection;
using GodLeaks.Logging;
using GodLeaks.Presentation.Console;
using GodLeaks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.Commands;
using Spectre.Console;

var builder = Host.CreateApplicationBuilder(args);

AnsiConsole.Write(new FigletText("GodLeaks").Color(Color.Red).Smushed());

builder.Logging.ClearProviders();
builder.Logging.AddProvider(new BotLoggerProvider());

// DI
builder.Services.AddServerCtlServices();

var token = builder.Configuration["Discord:Token"]?.Trim();
var prefix = builder.Configuration["Discord:Prefix"];
var apikey = builder.Configuration["APIKEY"];
var consoleUi = new ConsoleUi();

builder
    .Services.AddDiscordGateway(options =>
    {
        options.Intents = GatewayIntents.All;
        options.Token = token;
    })
    .AddCommands();

var host = builder.Build();

// Aplica as migrações pendentes e cria o banco/tabelas na primeira execução.
using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<GodLeaksDbContext>();
    await dbContext.Database.MigrateAsync();
}

if (string.IsNullOrWhiteSpace(token))
{
    consoleUi.WriteError(
        "Token do Discord não configurado. Em Development, defina-o com " +
        "'dotnet user-secrets set \"Discord:Token\" \"SEU_TOKEN\"'.");
    return;
}

if (string.IsNullOrWhiteSpace(prefix))
{
    consoleUi.WriteError("Prefixo do Discord não configurado em Discord:Prefix.");
    return;
}

if (string.IsNullOrWhiteSpace(apikey))
{
    consoleUi.WriteError("APIKEY não definida!");
    return;
}

host.AddModules(typeof(Program).Assembly);

await host.RunAsync();
