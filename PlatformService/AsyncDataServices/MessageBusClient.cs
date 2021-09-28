using System;
using System.Text.Json;
using System.Threading.Tasks;
using Contract.Dtos;
using MassTransit;

namespace PlatformService.AsyncDataServices
{
  public class MessageBusClient : IMessageBusClient
  {
    private readonly IPublishEndpoint _publishEndpoint;

    public MessageBusClient(IBus bus, IPublishEndpoint publishEndpoint)
    {
      _publishEndpoint = publishEndpoint;
    }

    public async Task PublishNewPlatform(PlatformPublishedDto platformPublishDto)
    {
      await _publishEndpoint.Publish<PlatformPublishedDto>(platformPublishDto);

      var message = JsonSerializer.Serialize(platformPublishDto);
      Console.WriteLine($"--> We have sent {message}");
    }
  }
}