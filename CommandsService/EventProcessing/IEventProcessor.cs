using System.Threading.Tasks;
using Contract.Dtos;

namespace CommandsService.EventProcessing
{
  public interface IEventProcessor
  {
    Task ProcessEvent(PlatformPublishedDto platformPublishedDto);
  }
}