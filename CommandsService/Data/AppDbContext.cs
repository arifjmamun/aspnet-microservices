using CommandsService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CommandsService.Data
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Platform> Platforms { get; set; }
    public DbSet<Command> Commands { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      builder
          .Entity<Platform>()
          .HasMany(p => p.Commands)
          .WithOne(p => p.Platform!)
          .HasForeignKey(p => p.PlatformId);

      builder
          .Entity<Command>()
          .HasOne(p => p.Platform)
          .WithMany(p => p.Commands)
          .HasForeignKey(p => p.PlatformId);
    }
  }
}