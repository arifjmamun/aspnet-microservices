using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using Contract.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;
using Microsoft.Extensions.Logging;

namespace PlatformService.Controllers
{
  [Route("api/[controller]")]
  [Produces("application/json")]
  [ApiController]
  public class PlatformsController : ControllerBase
  {
    private readonly IPlatformRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICommandDataClient _commandDataClient;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<PlatformsController> _loggger;

    public PlatformsController(
      IPlatformRepository platformRepository,
      IMapper mapper,
      ICommandDataClient commandDataClient,
      IPublishEndpoint publishEndpoint,
      ILogger<PlatformsController> loggger
    )
    {
      _repository = platformRepository;
      _mapper = mapper;
      _commandDataClient = commandDataClient;
      _publishEndpoint = publishEndpoint;
      _loggger = loggger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlatformReadDto>>> GetPlatforms()
    {
      var platforms = await _repository.GetAllPlatforms();

      return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
    }

    [HttpGet("{id}", Name = "GetPlatformById")]
    public async Task<ActionResult<PlatformReadDto>> GetPlatformById(int id)
    {
      var platform = await _repository.GetPlatformById(id);

      if (platform == null) return NotFound();

      return Ok(_mapper.Map<PlatformReadDto>(platform));
    }

    [HttpPost]
    public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
    {
      var platformModel = _mapper.Map<Platform>(platformCreateDto);
      _repository.CreatePlatform(platformModel);
      await _repository.SaveChanges();

      var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

      // Send Sync Message
      try
      {
        await _commandDataClient.SendPlatformToCommand(platformReadDto);
      }
      catch (Exception ex)
      {
        System.Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
      }

      // Send Async Message
      try
      {
        var dto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
        dto.Event = "Platform_Published";
        await _publishEndpoint.Publish<PlatformPublishedDto>(dto);
        _loggger.LogInformation($"Published new platform: {dto.Name}");
      }
      catch (Exception ex)
      {
        _loggger.LogError($"--> Could not send asynchronously: {ex.Message}");
      }

      return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id }, platformReadDto);
    }
  }
}