using System.Text;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using PlatformService.Dtos;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace PlatformService.SyncDataServices.Http
{
  public class HttpCommandDataClient : ICommandDataClient
  {
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
    {
      _configuration = configuration;
      _httpClient = httpClient;
    }

    public async Task SendPlatformToCommand(PlatformReadDto platformReadDto)
    {
      var response = await _httpClient.PostAsJsonAsync($"{_configuration["CommandService"]}/api/c/platforms", platformReadDto);

      if (response.IsSuccessStatusCode)
      {
        System.Console.WriteLine("-- > Sync POST to CommandService was OK!");
      }
      else
      {
        System.Console.WriteLine("-- > Sync POST to CommandService was not OK!");
      }
    }
  }
}