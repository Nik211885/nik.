using backend.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

/// <summary>Diagnostic endpoints for verifying infrastructure connectivity.</summary>
[ApiController]
[Route("api/help-check")]
public class HelpCheckController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    /// <summary>Initialises the controller with required dependencies.</summary>
    public HelpCheckController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>Opens a PostgreSQL connection and returns a health status message.</summary>
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
            return Problem("Can't connect to datbase instance", e.Message);
        }
    }
}
