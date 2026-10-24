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

    private async Task<bool> CheckPermissionAsync()
    {
        if (await _admin.HasAccessAsync(Context.User.Id))
            return true;

        await ReplyAsync("Você não possui permissão para usar este comando!");
        return false;
    }

    private async Task ReplyAsync(string message)
    {
        await Context.Channel.SendMessageAsync(message);
    }

    private async Task ListAsync<T>(
        IQueryable<T> query,
        Func<T, string> format,
        string emptyMessage)
    {
        var users = await query.ToListAsync();

        if (users.Count == 0)
        {
            await ReplyAsync(emptyMessage);
            return;
        }

        await ReplyAsync(string.Join("\n", users.Select(format)));
    }

    [Command("add_user")]
    public async Task AddUser(string userId, int days)
    {
        if (!await CheckPermissionAsync())
            return;

        if (days <= 0 || !ulong.TryParse(userId, out _))
        {
            await ReplyAsync("Informe um ID válido e uma duração maior que zero.");
            return;
        }

        var now = DateTime.UtcNow;

        var user = new UserSubscription
        {
            DiscordUserId = userId,
            IsActive = true,
            StartedAt = now,
            ExpiresAt = now.AddDays(days)
        };

        await _dbContext.UserSubscriptions.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        await ReplyAsync($"Usuário `{userId}` adicionado por {days} dias.");
    }

    [Command("list_users")]
    public async Task ListUsers()
    {
        if (!await CheckPermissionAsync())
            return;

        var now = DateTime.UtcNow;

        await ListAsync(
            _dbContext.UserSubscriptions.OrderBy(user => user.ExpiresAt),
            user => $"**ID**: `{user.Id}` | **UserID:** `{user.DiscordUserId}` | " +
                    $"**Active:** {(user.IsActive && user.ExpiresAt > now ? "Ativo" : "Inativo")} | " +
                    $"**Expira:** {user.ExpiresAt:dd/MM/yyyy HH:mm} UTC",
            "Nenhuma assinatura cadastrada.");
    }

    [Command("list_admins")]
    public async Task ListAdmins()
    {
        if (!await CheckPermissionAsync())
            return;

        await ListAsync(
            _dbContext.Admins.OrderBy(user => user.IsActive),
            user => $"**UserID:** `{user.DiscordUserId}` | " +
                    $"**Active:** {(user.IsActive ? "Sim" : "Não")}",
            "Nenhum admin cadastrado.");
    }

    [Command("remove_user")]
    public async Task RemoveUser(int userId)
    {
        if (!await CheckPermissionAsync())
            return;

        var user = await _dbContext.UserSubscriptions.FindAsync(userId);

        if (user is null)
        {
            await ReplyAsync("Usuário não encontrado!");
            return;
        }

        _dbContext.UserSubscriptions.Remove(user);
        await _dbContext.SaveChangesAsync();

        await ReplyAsync($"Usuário '{userId}' removido!");
    }
}