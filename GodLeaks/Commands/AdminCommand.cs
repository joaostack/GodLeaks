using GodLeaks.Application.Interfaces;
using GodLeaks.Domain.Entities;
using GodLeaks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using NetCord.Rest;
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

        await ReplyAsync("❌ Você não possui permissão para usar este comando!");
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

    [Command("update_user")]
    public async Task UpdateUser(string userId, bool isActive, int days)
    {
        if (!await CheckPermissionAsync())
            return;

        if (days < 0 || !ulong.TryParse(userId, out var discordUserId))
        {
            await ReplyAsync("❌ Informe um ID válido e uma duração maior que -1.");
            return;
        }

        var user = await _dbContext.UserSubscriptions
            .FirstOrDefaultAsync(u => u.DiscordUserId == discordUserId.ToString());

        if (user is null)
        {
            await ReplyAsync("❌ Usuário não encontrado!");
            return;
        }

        var now = DateTime.UtcNow;

        user.IsActive = isActive;
        user.StartedAt = now;
        user.ExpiresAt = now.AddDays(days);
        await _dbContext.SaveChangesAsync();

        await ReplyAsync("✅ Usuário atualizado com sucesso!");
    }

    [Command("add_user")]
    public async Task AddUser(string userId, int days)
    {
        if (!await CheckPermissionAsync())
            return;

        if (days <= 0 || !ulong.TryParse(userId, out _))
        {
            await ReplyAsync("❌ Informe um ID válido e uma duração maior que zero.");
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

        await ReplyAsync($"✅ Usuário `{userId}` adicionado por {days} dias.");
    }

    [Command("list_users")]
    public async Task ListUsers()
    {
        if (!await CheckPermissionAsync())
            return;

        var now = DateTime.UtcNow;

        var users = await _dbContext.UserSubscriptions.OrderBy(user => user.IsActive).ToListAsync();

        if (users.Count == 0)
        {
            await ReplyAsync("❌ Nenhum usuário cadastrado.");
            return;
        }

        var tasks = users.Select(async user =>
        {
            string nome;
            if (ulong.TryParse(user.DiscordUserId, out var discordId))
            {
                try
                {
                    var discordUser = await Context.Client.Rest.GetUserAsync(discordId);
                    nome = discordUser.Username;
                }
                catch
                {
                    nome = "Desconhecido";
                }
            }
            else
            {
                nome = "ID Inválido";
            }
            
            return
                $"**Nome**: `{nome}` | **UserID:** `{user.DiscordUserId}` | **Active:** {(user.IsActive ? "Sim" : "Não")}";
        });
        
        var formattedUsers = await Task.WhenAll(tasks);
        await ReplyAsync(string.Join("\n", formattedUsers));
    }

    [Command("list_admins")]
    public async Task ListAdmins()
    {
        if (!await CheckPermissionAsync())
            return;

        var admins = await _dbContext.Admins.OrderBy(user => user.IsActive).ToListAsync();

        if (admins.Count == 0)
        {
            await ReplyAsync("❌ Nenhum admin cadastrado.");
            return;
        }

        var tasks = admins.Select(async admin =>
        {
            string nome;
            if (ulong.TryParse(admin.DiscordUserId, out var discordId))
            {
                try
                {
                    var discordUser = await Context.Client.Rest.GetUserAsync(discordId);
                    nome = discordUser.Username;
                }
                catch
                {
                    nome = "Desconhecido";
                }
            }
            else
            {
                nome = "ID Inválido";
            }

            return
                $"**Nome**: `{nome}` | **UserID:** `{admin.DiscordUserId}` | **Active:** {(admin.IsActive ? "Sim" : "Não")}";
        });

        var formattedAdmins = await Task.WhenAll(tasks);
        await ReplyAsync(string.Join("\n", formattedAdmins));
    }

    [Command("remove_user")]
    public async Task RemoveUser(int userId)
    {
        if (!await CheckPermissionAsync())
            return;

        var user = await _dbContext.UserSubscriptions.FindAsync(userId);

        if (user is null)
        {
            await ReplyAsync("❌ Usuário não encontrado!");
            return;
        }

        _dbContext.UserSubscriptions.Remove(user);
        await _dbContext.SaveChangesAsync();

        await ReplyAsync($"✅ Usuário '{userId}' removido!");
    }
}