namespace GodLeaks.Application.Interfaces;

public interface IAdmin
{
    Task<bool> HasAccessAsync(
        ulong discordUserId,
        CancellationToken cancellationToken = default);
}