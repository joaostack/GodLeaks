using GodLeaks.Application.Interfaces;
using GodLeaks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GodLeaks.Infrastructure.Services;

public class Admin : IAdmin
{
    private readonly GodLeaksDbContext _dbContext;

    public Admin(GodLeaksDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> HasAccessAsync(ulong discordUserId, CancellationToken cancellationToken = default)
    {
        var id = discordUserId.ToString();

        return await _dbContext.Admins.AnyAsync(
            x => x.DiscordUserId == id
                 && x.IsActive,
            cancellationToken);
    }
}