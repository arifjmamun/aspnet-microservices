using System;
using System.Threading.Tasks;
using AutoMapper;
using CommandsService.Data;
using Contract.Dtos;
using CommandsService.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CommandsService.EventProcessing
{
  public class EventProcessor : IEventProcessor
  {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;

    public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
    {
      _scopeFactory = scopeFactory;
      _mapper = mapper;
    }

    public async Task ProcessEvent(PlatformPublishedDto platformPublishedDto)
    {
      var eventType = DetermineEvent(platformPublishedDto);
      if (eventType == EventType.PlatformPublished)
      {
        await AddPlatform(platformPublishedDto);
      }
    }

    private EventType DetermineEvent(PlatformPublishedDto platformPublishedDto)
    {
      System.Console.WriteLine("--> Determining Event");

      switch (platformPublishedDto.Event)
      {
        case "Platform_Published":
          System.Console.WriteLine("Platform published event detected");
          return EventType.PlatformPublished;
        default:
          System.Console.WriteLine("Could not detected the event");
          return EventType.Undetermined;
      }
    }

    private async Task AddPlatform(PlatformPublishedDto platformPublishedDto)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        var repository = scope.ServiceProvider.GetRequiredService<ICommandRepository>();

        try
        {
          var platform = _mapper.Map<Platform>(platformPublishedDto);
          if (!await repository.ExternalPlatformExists(platform.ExternalID))
          {
            repository.CreatePlatform(platform);
            await repository.SaveChanges();
          }
          else
          {
            System.Console.WriteLine("--> Platform already exists");
          }
        }
        catch (Exception ex)
        {
          System.Console.WriteLine($"--> Could  not add platform to DB: {ex.Message}");
        }
      }
    }
  }

  enum EventType
  {
    PlatformPublished,
    Undetermined
  }
}