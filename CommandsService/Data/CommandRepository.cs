using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandsService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandsService.Data
{
  public class CommandRepository : ICommandRepository
  {
    private readonly AppDbContext _dbContext;

    public CommandRepository(AppDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public void CreateCommand(int platformId, Command command)
    {
      if (command == null) throw new ArgumentNullException(nameof(command));
      
      command.PlatformId = platformId;
      _dbContext.Commands.Add(command);
    }

    public void CreatePlatform(Platform platform)
    {
      if (platform == null) throw new ArgumentNullException(nameof(platform));

      _dbContext.Platforms.Add(platform);
    }

    public async Task<bool> ExternalPlatformExists(int externalPlatformId)
    {
      return await _dbContext.Platforms.AnyAsync(p => p.ExternalID == externalPlatformId);
    }

    public async Task<IEnumerable<Command>> GetAllCommandsForPlaform(int platformId)
    {
      return await _dbContext.Commands
        .Where(c => c.PlatformId == platformId)
        .OrderBy(c => c.Platform.Name)
        .ToListAsync();
    }

    public async Task<IEnumerable<Platform>> GetAllPlatforms()
    {
      return await _dbContext.Platforms.ToListAsync();
    }

    public async Task<Command> GetCommand(int platformId, int commandId)
    {
      return await _dbContext.Commands.FirstOrDefaultAsync(c => c.PlatformId == platformId && c.Id == commandId);
    }

    public async Task<bool> PlatformExists(int platformId)
    {
      return await _dbContext.Platforms.AnyAsync(p => p.Id == platformId);
    }

    public async Task<bool> SaveChanges()
    {
      return (await _dbContext.SaveChangesAsync()) >= 0;
    }
  }
}