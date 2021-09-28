using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
  public class PlatformRepository : IPlatformRepository
  {
    private readonly AppDbContext _dbContext;

    public PlatformRepository(AppDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public void CreatePlatform(Platform platform)
    {
      if (platform == null) throw new ArgumentException(nameof(platform));
      _dbContext.Platforms.Add(platform);
    }

    public async Task<IEnumerable<Platform>> GetAllPlatforms()
    {
      return await _dbContext.Platforms.ToListAsync();
    }

    public async Task<Platform> GetPlatformById(int id)
    {
      return await _dbContext.Platforms.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> SaveChanges()
    {
      var count = await _dbContext.SaveChangesAsync();
      return count >= 0;
    }
  }
}