using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
  [Route("api/c/platforms/{platformId}/[controller]")]
  [ApiController]
  public class CommandsController : ControllerBase
  {
    private readonly ICommandRepository _repository;
    private readonly IMapper _mapper;

    public CommandsController(ICommandRepository repository, IMapper mapper)
    {
      _repository = repository;
      _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommandReadDto>>> GetCommandsForPlatform(int platformId)
    {
      if (!(await _repository.PlatformExists(platformId))) return NotFound();

      var commands = await _repository.GetAllCommandsForPlaform(platformId);
      return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
    }

    [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
    public async Task<ActionResult<CommandReadDto>> GetCommandForPlatform(int platformId, int commandId)
    {
      if (!(await _repository.PlatformExists(platformId))) return NotFound();

      var command = await _repository.GetCommand(platformId, commandId);
      if (command == null) return NotFound();

      return Ok(_mapper.Map<CommandReadDto>(command));
    }

    [HttpPost]
    public async Task<ActionResult<CommandReadDto>> CreateCommandForPlatform(int platformId, CommandCreateDto commandDto)
    {
      if (!(await _repository.PlatformExists(platformId))) return NotFound();

      var command = _mapper.Map<Command>(commandDto);

      _repository.CreateCommand(platformId, command);
      await _repository.SaveChanges();

      var commandReadDto = _mapper.Map<CommandReadDto>(command);

      return CreatedAtRoute(
        nameof(GetCommandForPlatform),
        new { platformId = platformId, commandId = commandReadDto.Id },
        commandReadDto
      );
    }
  }
}