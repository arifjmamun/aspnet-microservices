using MassTransit.Topology;

namespace Contract.Dtos
{
  public class PlatformPublishedDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Event { get; set; }
  }
}