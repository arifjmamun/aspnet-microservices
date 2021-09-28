using System.Threading.Tasks;
using Contract.Dtos;

namespace PlatformService.AsyncDataServices
{
  public interface IMessageBusClient
  {
    Task PublishNewPlatform(PlatformPublishedDto platformPublishDto);
  }
}