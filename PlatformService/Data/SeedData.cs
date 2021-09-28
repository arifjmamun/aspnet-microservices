using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;

namespace PlatformService.Data
{
  public static class SeedData
  {
    private static void Seed(AppDbContext dbContext, bool isProd)
    {
      if (isProd)
      {
        Console.WriteLine("--> Attempting to apply migrations...");
        try
        {
          dbContext.Database.Migrate();
        }
        catch (Exception ex)
        {
          System.Console.WriteLine($"--> Could not run migrations: {ex.Message}");
        }
      }

      if (!dbContext.Platforms.Any())
      {
        Console.WriteLine("--> Seeding data...");

        dbContext.Platforms.AddRange(
            new Platform { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
            new Platform { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
            new Platform { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
        );
        dbContext.SaveChanges();
      }
      else
      {
        System.Console.WriteLine("--> We already have data");
      }
    }

    public static void PopulateDatabase(this IApplicationBuilder app, bool isProd)
    {
      using (var serviceScope = app.ApplicationServices.CreateScope())
      {
        Seed(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProd);
      }
    }
  }
}