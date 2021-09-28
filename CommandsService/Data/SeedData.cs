using System;
using System.Collections.Generic;
using System.Linq;
using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CommandsService.Data
{
  public static class SeedData
  {
    public static void PopulateDatabase(this IApplicationBuilder app)
    {
      using (var serviceScope = app.ApplicationServices.CreateScope())
      {
        var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
        var platforms = grpcClient.ReturnAllPlatforms();

        var repository = serviceScope.ServiceProvider.GetService<ICommandRepository>();

        Seed(repository, platforms);
      }
    }

    private static void Seed(ICommandRepository repository, IEnumerable<Platform> platforms)
    {
      System.Console.WriteLine("--> Seeding new platforms...");

      foreach (var item in platforms)
      {
        if (!repository.ExternalPlatformExists(item.ExternalID).Result)
        {
          repository.CreatePlatform(item);
        }
        repository.SaveChanges().Wait();
      }
    }
  }
}