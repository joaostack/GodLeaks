using System.Text.Json;
using GodLeaks.Application.Interfaces;
using GodLeaks.Domain.Entities;
using GodLeaks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using NetCord.Services.ApplicationCommands;

namespace GodLeaks.Commands;

public sealed class SlashCommandModule(
    ILeakSight leakSight,
    ISubscriptionService subscription) : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("comandos", "Exibe a central de comandos")]
    public string Comandos() =>
        "Use `/cpf`, `/email`, `/username`, `/placa`, `/nome`, `/senha`, `/url` ou `/telefone`. " +
        "Os comandos prefixados com `!` continuam disponíveis.";

    [SlashCommand("cpf", "Consulta um CPF")]
    public Task<string> Cpf(string cpf) => SearchAsync(() => leakSight.SearchCPF(cpf));

    [SlashCommand("email", "Consulta um e-mail")]
    public Task<string> Email(string email) => SearchAsync(() => leakSight.SearchEmail(email));

    [SlashCommand("username", "Consulta um username")]
    public Task<string> Username(string username) => SearchAsync(() => leakSight.SearchUsername(username));

    [SlashCommand("placa", "Consulta uma placa de veículo")]
    public Task<string> Placa(string placa) => SearchAsync(() => leakSight.SearchPlaca(placa));

    [SlashCommand("nome", "Consulta um nome completo")]
    public Task<string> Nome(string nome) => SearchAsync(() => leakSight.SearchNome(nome));

    [SlashCommand("senha", "Consulta uma senha")]
    public Task<string> Senha(string senha) => SearchAsync(() => leakSight.SearchPassword(senha));

    [SlashCommand("url", "Consulta logins de uma URL")]
    public Task<string> Url(string url) => SearchAsync(() => leakSight.SearchUrl(url));

    [SlashCommand("telefone", "Consulta um telefone")]
    public Task<string> Telefone(string telefone) => SearchAsync(() => leakSight.SearchPhone(telefone));

    private async Task<string> SearchAsync<T>(Func<Task<T>> search)
    {
        if (!await subscription.HasAccessAsync(Context.User.Id))
            return CommandMessages.InactiveSubscription;

        var result = await search();
        var content = JsonSerializer.Serialize(result);

        return content.Length <= 1900
            ? $"```json\n{content}\n```"
            : $"```json\n{content[..1890]}...\n```";
    }

public sealed class SlashAdminCommandModule(
    GodLeaksDbContext dbContext,
    IAdmin admin) : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("update_user", "Atualiza o status e a duração de um usuário")]
    public async Task<string> UpdateUser(string userId, bool isActive, int days)
    {
        if (!await HasAccessAsync())
            return PermissionDenied;

        if (days <= 0 || !ulong.TryParse(userId, out var discordUserId))
            return "Informe um ID do Discord válido e uma duração maior que zero.";

        var user = await dbContext.UserSubscriptions
            .FirstOrDefaultAsync(item => item.DiscordUserId == discordUserId.ToString());

        if (user is null)
            return "Usuário não encontrado.";

        user.IsActive = isActive;
        user.StartedAt = DateTime.UtcNow;
        user.ExpiresAt = user.StartedAt.AddDays(days);
        await dbContext.SaveChangesAsync();
        return $"Usuário `{discordUserId}` atualizado.";
    }

    [SlashCommand("add_user", "Adiciona ou renova o acesso de um usuário")]
    public async Task<string> AddUser(string userId, int days)
    {
        if (!await HasAccessAsync())
            return PermissionDenied;

        if (days <= 0 || !ulong.TryParse(userId, out var discordUserId))
            return "Informe um ID do Discord válido e uma duração maior que zero.";

        var now = DateTime.UtcNow;
        var id = discordUserId.ToString();
        var user = await dbContext.UserSubscriptions
            .FirstOrDefaultAsync(item => item.DiscordUserId == id);

        if (user is null)
        {
            user = new UserSubscription
            {
                DiscordUserId = id,
                IsActive = true,
                StartedAt = now,
                ExpiresAt = now.AddDays(days)
            };
            dbContext.UserSubscriptions.Add(user);
        }
        else
        {
            var start = user.IsActive && user.ExpiresAt > now ? user.ExpiresAt : now;
            user.StartedAt = now;
            user.ExpiresAt = start.AddDays(days);
            user.IsActive = true;
        }

        await dbContext.SaveChangesAsync();
        return $"Usuário `{id}` ativado por mais {days} dias.";
    }

    [SlashCommand("list_users", "Lista as assinaturas cadastradas")]
    public Task<string> ListUsers() => ListAsync(dbContext.UserSubscriptions);

    [SlashCommand("list_admins", "Lista os administradores cadastrados")]
    public Task<string> ListAdmins() => ListAsync(dbContext.Admins);

    [SlashCommand("remove_user", "Remove um usuário")]
    public Task<string> RemoveUser(string userId) => ChangeUserAsync(userId, user => dbContext.UserSubscriptions.Remove(user), "removido");

    [SlashCommand("ban_user", "Bloqueia um usuário")]
    public Task<string> BanUser(string userId) => ChangeUserAsync(userId, user => user.IsActive = false, "bloqueado");

    [SlashCommand("unban_user", "Desbloqueia um usuário")]
    public async Task<string> UnbanUser(string userId)
    {
        if (!await HasAccessAsync())
            return PermissionDenied;

        var user = await FindUserAsync(userId);
        if (user is null)
            return "Usuário não encontrado.";
        if (user.ExpiresAt <= DateTime.UtcNow)
            return "A assinatura está expirada.";

        user.IsActive = true;
        await dbContext.SaveChangesAsync();
        return $"Usuário `{user.DiscordUserId}` desbloqueado.";
    }

    private async Task<string> ChangeUserAsync(string userId, Action<UserSubscription> change, string action)
    {
        if (!await HasAccessAsync())
            return PermissionDenied;

        var user = await FindUserAsync(userId);
        if (user is null)
            return "Usuário não encontrado.";

        change(user);
        await dbContext.SaveChangesAsync();
        return $"Usuário `{user.DiscordUserId}` {action}.";
    }

    private async Task<UserSubscription?> FindUserAsync(string userId) =>
        ulong.TryParse(userId, out var id)
            ? await dbContext.UserSubscriptions.FirstOrDefaultAsync(item => item.DiscordUserId == id.ToString())
            : null;

    private async Task<string> ListAsync<T>(IQueryable<T> query) where T : class
    {
        if (!await HasAccessAsync())
            return PermissionDenied;

        var items = await query.AsNoTracking().ToListAsync();
        return JsonSerializer.Serialize(items);
    }

    private Task<bool> HasAccessAsync() => admin.HasAccessAsync(Context.User.Id);

    private const string PermissionDenied = "Você não possui permissão para usar este comando.";
}
}