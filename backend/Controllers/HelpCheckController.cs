using backend.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/help-check")]
public class HelpCheckController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    public HelpCheckController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost("postgres")]
    public async Task<IActionResult> CheckPostgres()
    {
        try
        {
            await _dbContext.Database.OpenConnectionAsync();
            return Ok("Postgres fine");
        }
        catch (Exception e)
        {
            return Problem("Can't connect to datbase instance",e.Message);
        }
    }
}
