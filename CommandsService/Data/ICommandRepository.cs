using System.Collections.Generic;
using System.Threading.Tasks;
using CommandsService.Models;

namespace CommandsService.Data
{
  public interface ICommandRepository
  {
    Task<bool> SaveChanges();

    Task<IEnumerable<Platform>> GetAllPlatforms();
    void CreatePlatform(Platform platform);
    Task<bool> PlatformExists(int platformId);
    Task<bool> ExternalPlatformExists(int externalPlatformId);

    Task<IEnumerable<Command>> GetAllCommandsForPlaform(int platformId);
    Task<Command> GetCommand(int platformId, int commandId);
    void CreateCommand(int platformId, Command command);
  }
}