using System.Threading.Tasks;
using System.Collections.Generic;
using PlatformService.Models;

namespace PlatformService.Data
{
  public interface IPlatformRepository
  {
    Task<bool> SaveChanges();

    Task<IEnumerable<Platform>> GetAllPlatforms();

    Task<Platform> GetPlatformById(int id);

    void CreatePlatform(Platform platform);
  }
}