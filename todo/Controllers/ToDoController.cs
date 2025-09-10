using System.Security.Claims;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using todo.Data;
using todo.Entities;
using todo.ViewModels.Todo;

namespace todo.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ToDoController : ControllerBase
{
    private readonly DataContext _context;
    private readonly UserManager<User> _userManager;
    private readonly HtmlSanitizer _htmlSanitizer = new();

    public ToDoController(DataContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

  
    [HttpGet]
    [EnableRateLimiting("TodoLimiter")]
    public async Task<ActionResult> GetTodos()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            var todos = await _context.Todo
                .Where(t => t.UserId == user.Id)
                .Select(t => new ToDoItemViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    IsDone = t.IsDone
                })
                .ToListAsync();

            return Ok(todos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while fetching todos.", details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [EnableRateLimiting("TodoLimiter")]
    public async Task<ActionResult> GetTodoById(int id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            var todo = await _context.Todo
                .Where(t => t.Id == id && t.UserId == user.Id)
                .Select(t => new ToDoItemViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    IsDone = t.IsDone
                })
                .FirstOrDefaultAsync();

            if (todo is null) return NotFound();

            return Ok(todo);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while fetching the todo.", details = ex.Message });
        }
    }

    [HttpPost]
    [EnableRateLimiting("TodoLimiter")]
  
    public async Task<ActionResult> CreateTodo([FromBody] CreateToDoViewModel model)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            model.Title = _htmlSanitizer.Sanitize(model.Title);

            var user = await _userManager.GetUserAsync(User);

            var todo = new TodoItem
            {
                Title = model.Title,
                IsDone = false,
                UserId = user.Id
            };

            _context.Todo.Add(todo);
            await _context.SaveChangesAsync();

            return Ok(new ToDoItemViewModel { Id = todo.Id, Title = todo.Title, IsDone = todo.IsDone });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating the todo.", details = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [EnableRateLimiting("TodoLimiter")]
   
    public async Task<ActionResult> UpdateTodo(int id, [FromBody] UpdateToDoViewModel model)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.GetUserAsync(User);
            var todo = await _context.Todo.FirstOrDefaultAsync(t => t.Id == id && t.UserId == user.Id);
            if (todo is null) return NotFound();

            todo.Title = _htmlSanitizer.Sanitize(model.Title);
            todo.IsDone = model.IsDone;

            await _context.SaveChangesAsync();

            return Ok(new ToDoItemViewModel { Id = todo.Id, Title = todo.Title, IsDone = todo.IsDone });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating the todo.", details = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [EnableRateLimiting("TodoLimiter")]
   
    public async Task<ActionResult> DeleteTodo(int id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            var todo = await _context.Todo.FirstOrDefaultAsync(t => t.Id == id && t.UserId == user.Id);
            if (todo is null) return NotFound();

            _context.Todo.Remove(todo);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Todo deleted" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred when deleting the todo.", details = ex.Message });
        }
    }
}