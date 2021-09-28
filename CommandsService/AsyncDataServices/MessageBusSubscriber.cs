using System.Threading.Tasks;
using Contract.Dtos;
using CommandsService.EventProcessing;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CommandsService.AsyncDataServices
{
  public class MessageBusSubscriber : IConsumer<PlatformPublishedDto>
  {
    private readonly ILogger<MessageBusSubscriber> _logger;
    private readonly IEventProcessor _eventProcessor;

    public MessageBusSubscriber(ILogger<MessageBusSubscriber> logger, IEventProcessor eventProcessor)
    {
      _logger = logger;
      _eventProcessor = eventProcessor;
    }

    public async Task Consume(ConsumeContext<PlatformPublishedDto> context)
    {
      await _eventProcessor.ProcessEvent(context.Message);
      _logger.LogInformation($"Got new message {context.Message.Name}");
    }
  }
}