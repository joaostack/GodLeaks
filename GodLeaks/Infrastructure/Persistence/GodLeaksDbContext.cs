using GodLeaks.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GodLeaks.Infrastructure.Persistence;

public class GodLeaksDbContext(
    DbContextOptions<GodLeaksDbContext> options)
    : DbContext(options)
{
    public DbSet<UserSubscription> UserSubscriptions =>
        Set<UserSubscription>();

    public DbSet<Admin> Admins => Set<Admin>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserSubscription>()
            .HasIndex(x => x.DiscordUserId)
            .IsUnique();
        
        modelBuilder.Entity<Admin>()
            .HasIndex(x => x.DiscordUserId)
            .IsUnique();
    }
}