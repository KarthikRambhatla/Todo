using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Dtos;
using TodoApi.Services;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoController(ITodoService service) : ControllerBase
{   
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItemDto>>> GetAll()
    {
        var items = await service.GetAllAsync();
        var result = items.Select(i => TodoItemDto.From(i));
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await service.GetAsync(id);
        return item == null ? NotFound() : Ok(TodoItemDto.From(item));
    }


    [HttpPatch("{id}/done")]
    public async Task<IActionResult> MarkDone(Guid id)
    {
        return await service.MarkDoneAsync(id) ? Ok() : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<TodoItemDto>> Create([FromBody] TodoItemDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            return BadRequest("Title is required");
        }

        var item = await service.AddAsync(dto.Title);
        if (item == null)
        {
            return BadRequest("Could not create todo item");
        }
            
        return CreatedAtAction(nameof(Get), new { id = item?.Id }, TodoItemDto.From(item));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<TodoItemDto>> Delete(Guid id)
    {
        var item =  await service.DeleteAsync(id);
        // may be if we want to give user undo option we can give Ok(item)
        return item == null ? NotFound() : NoContent();
    }
}
