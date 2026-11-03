using GodLeaks.Application.Interfaces;
using GodLeaks.Domain.Entities;
using GodLeaks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using NetCord.Services.Commands;

namespace GodLeaks.Commands;

public class AdminCommand : CommandModule<CommandContext>
{
    private readonly GodLeaksDbContext _dbContext;
    private readonly IAdmin _admin;

    public AdminCommand(GodLeaksDbContext dbContext, IAdmin admin)
    {
        _dbContext = dbContext;
        _admin = admin;
    }

    private async Task<bool> CheckPermissionAsync(
        CancellationToken cancellationToken = default)
    {
        if (await _admin.HasAccessAsync(Context.User.Id, cancellationToken))
            return true;

        await ReplyAsync("❌ Você não possui permissão para usar este comando!");
        return false;
    }

    [Command("update_user")]
    public async Task UpdateUser(
        string userId,
        bool isActive,
        int days)
    {
        var cancellationToken = CancellationToken.None;
        if (!await CheckPermissionAsync(cancellationToken))
            return;

        if (days <= 0 || !TryParseDiscordUserId(userId, out var discordUserId))
        {
            await ReplyAsync("❌ Informe um ID do Discord válido e uma duração maior que zero.");
            return;
        }

        var user = await _dbContext.UserSubscriptions
            .FirstOrDefaultAsync(
                u => u.DiscordUserId == discordUserId.ToString(),
                cancellationToken);

        if (user is null)
        {
            await ReplyAsync("❌ Usuário não encontrado!");
            return;
        }

        user.IsActive = isActive;
        user.StartedAt = DateTime.UtcNow;
        user.ExpiresAt = user.StartedAt.AddDays(days);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await ReplyAsync(
            $"✅ Usuário `{discordUserId}` atualizado. " +
            $"Status: {(isActive ? "ativo" : "inativo")} por {days} dias.");
    }

    [Command("add_user")]
    public async Task AddUser(
        string userId,
        int days)
    {
        var cancellationToken = CancellationToken.None;
        if (!await CheckPermissionAsync(cancellationToken))
            return;

        if (days <= 0 || !TryParseDiscordUserId(userId, out var discordUserId))
        {
            await ReplyAsync("❌ Informe um ID do Discord válido e uma duração maior que zero.");
            return;
        }

        var now = DateTime.UtcNow;
        var discordId = discordUserId.ToString();
        var user = await _dbContext.UserSubscriptions
            .FirstOrDefaultAsync(
                subscription => subscription.DiscordUserId == discordId,
                cancellationToken);

        if (user is null)
        {
            user = new UserSubscription
            {
                DiscordUserId = discordId,
                IsActive = true,
                StartedAt = now,
                ExpiresAt = now.AddDays(days)
            };

            _dbContext.UserSubscriptions.Add(user);
        }
        else
        {
            var start = user.IsActive && user.ExpiresAt > now
                ? user.ExpiresAt
                : now;

            user.StartedAt = now;
            user.ExpiresAt = start.AddDays(days);
            user.IsActive = true;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        await ReplyAsync($"✅ Usuário `{discordId}` ativado/adicionado por mais {days} dias.");
    }

    [Command("list_users")]
    public async Task ListUsers()
    {
        var cancellationToken = CancellationToken.None;
        if (!await CheckPermissionAsync(cancellationToken))
            return;

        var users = await _dbContext.UserSubscriptions
            .AsNoTracking()
            .OrderByDescending(user => user.IsActive)
            .ThenBy(user => user.ExpiresAt)
            .ToListAsync(cancellationToken);

        if (users.Count == 0)
        {
            await ReplyAsync("❌ Nenhum usuário cadastrado.");
            return;
        }

        var tasks = users.Select(async user =>
        {
            var nome = await GetUsernameAsync(user.DiscordUserId);
            var status = user.IsActive && user.ExpiresAt > DateTime.UtcNow
                ? "ativo"
                : "inativo";

            return $"`{user.DiscordUserId}` — {nome} — {status} até {user.ExpiresAt:dd/MM/yyyy}";
        });

        var formattedUsers = await Task.WhenAll(tasks);
        await ReplyInChunksAsync("**Usuários cadastrados:**\n" + string.Join("\n", formattedUsers));
    }

    [Command("list_admins")]
    public async Task ListAdmins()
    {
        var cancellationToken = CancellationToken.None;
        if (!await CheckPermissionAsync(cancellationToken))
            return;

        var admins = await _dbContext.Admins
            .AsNoTracking()
            .OrderByDescending(admin => admin.IsActive)
            .ToListAsync(cancellationToken);

        if (admins.Count == 0)
        {
            await ReplyAsync("❌ Nenhum admin cadastrado.");
            return;
        }

        var tasks = admins.Select(async admin =>
        {
            var nome = await GetUsernameAsync(admin.DiscordUserId);
            var status = admin.IsActive ? "ativo" : "inativo";

            return $"`{admin.DiscordUserId}` — {nome} — {status}";
        });

        var formattedAdmins = await Task.WhenAll(tasks);
        await ReplyInChunksAsync("**Administradores:**\n" + string.Join("\n", formattedAdmins));
    }

    [Command("remove_user")]
    public async Task RemoveUser(
        string userId)
    {
        var cancellationToken = CancellationToken.None;
        if (!await CheckPermissionAsync(cancellationToken))
            return;

        if (!TryParseDiscordUserId(userId, out var discordUserId))
        {
            await ReplyAsync("❌ Informe um ID do Discord válido.");
            return;
        }

        var user = await _dbContext.UserSubscriptions
            .FirstOrDefaultAsync(
                subscription => subscription.DiscordUserId == discordUserId.ToString(),
                cancellationToken);

        if (user is null)
        {
            await ReplyAsync("❌ Usuário não encontrado!");
            return;
        }

        _dbContext.UserSubscriptions.Remove(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await ReplyAsync($"✅ Usuário `{discordUserId}` removido!");
    }

    [Command("ban_user")]
    public async Task BanUser(
        string userId)
    {
        var cancellationToken = CancellationToken.None;
        if (!await CheckPermissionAsync(cancellationToken))
            return;

        var user = await FindUserAsync(userId, cancellationToken);
        if (user is null)
            return;

        user.IsActive = false;
        await _dbContext.SaveChangesAsync(cancellationToken);
        await ReplyAsync($"✅ Usuário `{user.DiscordUserId}` bloqueado.");
    }

    [Command("unban_user")]
    public async Task UnbanUser(
        string userId)
    {
        var cancellationToken = CancellationToken.None;
        if (!await CheckPermissionAsync(cancellationToken))
            return;

        var user = await FindUserAsync(userId, cancellationToken);
        if (user is null)
            return;

        if (user.ExpiresAt <= DateTime.UtcNow)
        {
            await ReplyAsync("❌ A assinatura está expirada. Use `!add_user` para renovar.");
            return;
        }

        user.IsActive = true;
        await _dbContext.SaveChangesAsync(cancellationToken);
        await ReplyAsync($"✅ Usuário `{user.DiscordUserId}` desbloqueado.");
    }

    private async Task<UserSubscription?> FindUserAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        if (!TryParseDiscordUserId(userId, out var discordUserId))
        {
            await ReplyAsync("❌ Informe um ID do Discord válido.");
            return null;
        }

        var user = await _dbContext.UserSubscriptions
            .FirstOrDefaultAsync(
                subscription => subscription.DiscordUserId == discordUserId.ToString(),
                cancellationToken);

        if (user is null)
            await ReplyAsync("❌ Usuário não encontrado.");

        return user;
    }

    private async Task<string> GetUsernameAsync(string userId)
    {
        if (!ulong.TryParse(userId, out var discordUserId))
            return "ID inválido";

        try
        {
            var user = await Context.Client.Rest.GetUserAsync(discordUserId);
            return user.Username;
        }
        catch
        {
            return "Desconhecido";
        }
    }

    private async Task ReplyInChunksAsync(string message)
    {
        const int maxLength = 1900;

        for (var start = 0; start < message.Length; start += maxLength)
        {
            var length = Math.Min(maxLength, message.Length - start);
            await ReplyAsync(message.Substring(start, length));
        }
    }

    private static bool TryParseDiscordUserId(string value, out ulong userId) =>
        ulong.TryParse(value.Trim('<', '@', '!', '>'), out userId);
}
