using backend.Pipes.Filter;
using backend.Services.Internals;
using backend.ViewModels.Careers.Requests;
using backend.ViewModels.Careers.Responses;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>Endpoints for CV/Careers section — work experiences and skills.</summary>
[ApiController]
[Route("api/careers")]
public class CareerController : ControllerBase
{
    private readonly ILogger<CareerController> _logger;
    private readonly CareerServices _careerServices;

    /// <summary>Initialises the controller with required dependencies.</summary>
    public CareerController(ILogger<CareerController> logger, CareerServices careerServices)
    {
        _logger = logger;
        _careerServices = careerServices;
    }

    // ── Work Experience ───────────────────────────────────────────────────────

    /// <summary>Returns all published work experience entries for the public about page.</summary>
    [HttpGet("~/public-api/careers/work-experiences")]
    public async Task<ActionResult<List<WorkExperienceResponse>>> GetPublishedWorkExperiences()
    {
        var result = await _careerServices.GetPublishedWorkExperiencesAsync();
        return Ok(result);
    }

    /// <summary>Returns all work experience entries for the admin panel.</summary>
    [HttpGet("work-experiences")]
    public async Task<ActionResult<List<WorkExperienceResponse>>> GetAllWorkExperiences()
    {
        var result = await _careerServices.GetAllWorkExperiencesAsync();
        return Ok(result);
    }

    /// <summary>Creates a new work experience entry.</summary>
    [HttpPost("work-experiences/create")]
    [ValidationFilter(typeof(CreateWorkExperienceRequest))]
    public async Task<ActionResult<WorkExperienceResponse>> CreateWorkExperience([FromBody] CreateWorkExperienceRequest request)
    {
        var result = await _careerServices.CreateWorkExperienceAsync(request);
        return Ok(result);
    }

    /// <summary>Updates an existing work experience entry.</summary>
    [HttpPut("work-experiences/update")]
    [ValidationFilter(typeof(UpdateWorkExperienceRequest))]
    public async Task<ActionResult<WorkExperienceResponse>> UpdateWorkExperience([FromBody] UpdateWorkExperienceRequest request)
    {
        var result = await _careerServices.UpdateWorkExperienceAsync(request);
        return Ok(result);
    }

    /// <summary>Deletes a work experience entry by ID.</summary>
    /// <param name="id">ID of the entry to delete.</param>
    [HttpDelete("work-experiences/delete/{id}")]
    public async Task<IActionResult> DeleteWorkExperience(string id)
    {
        await _careerServices.DeleteWorkExperienceAsync(id);
        return NoContent();
    }

    // ── Projects ─────────────────────────────────────────────────────────────

    /// <summary>Returns all published project entries for the public about page.</summary>
    [HttpGet("~/public-api/careers/projects")]
    public async Task<ActionResult<List<ProjectResponse>>> GetPublishedProjects()
    {
        var result = await _careerServices.GetPublishedProjectsAsync();
        return Ok(result);
    }

    /// <summary>Returns all project entries for the admin panel.</summary>
    [HttpGet("projects")]
    public async Task<ActionResult<List<ProjectResponse>>> GetAllProjects()
    {
        var result = await _careerServices.GetAllProjectsAsync();
        return Ok(result);
    }

    /// <summary>Creates a new project entry.</summary>
    [HttpPost("projects/create")]
    [ValidationFilter(typeof(CreateProjectRequest))]
    public async Task<ActionResult<ProjectResponse>> CreateProject([FromBody] CreateProjectRequest request)
    {
        var result = await _careerServices.CreateProjectAsync(request);
        return Ok(result);
    }

    /// <summary>Updates an existing project entry.</summary>
    [HttpPut("projects/update")]
    [ValidationFilter(typeof(UpdateProjectRequest))]
    public async Task<ActionResult<ProjectResponse>> UpdateProject([FromBody] UpdateProjectRequest request)
    {
        var result = await _careerServices.UpdateProjectAsync(request);
        return Ok(result);
    }

    /// <summary>Deletes a project entry by ID.</summary>
    /// <param name="id">ID of the entry to delete.</param>
    [HttpDelete("projects/delete/{id}")]
    public async Task<IActionResult> DeleteProject(string id)
    {
        await _careerServices.DeleteProjectAsync(id);
        return NoContent();
    }

    // ── Skills ────────────────────────────────────────────────────────────────

    /// <summary>Returns all published skills for the public about page.</summary>
    [HttpGet("~/public-api/careers/skills")]
    public async Task<ActionResult<List<SkillResponse>>> GetPublishedSkills()
    {
        var result = await _careerServices.GetPublishedSkillsAsync();
        return Ok(result);
    }

    /// <summary>Returns all skills for the admin panel.</summary>
    [HttpGet("skills")]
    public async Task<ActionResult<List<SkillResponse>>> GetAllSkills()
    {
        var result = await _careerServices.GetAllSkillsAsync();
        return Ok(result);
    }

    /// <summary>Creates a new skill tag.</summary>
    [HttpPost("skills/create")]
    [ValidationFilter(typeof(CreateSkillRequest))]
    public async Task<ActionResult<SkillResponse>> CreateSkill([FromBody] CreateSkillRequest request)
    {
        var result = await _careerServices.CreateSkillAsync(request);
        return Ok(result);
    }

    /// <summary>Updates an existing skill tag.</summary>
    [HttpPut("skills/update")]
    [ValidationFilter(typeof(UpdateSkillRequest))]
    public async Task<ActionResult<SkillResponse>> UpdateSkill([FromBody] UpdateSkillRequest request)
    {
        var result = await _careerServices.UpdateSkillAsync(request);
        return Ok(result);
    }

    /// <summary>Deletes a skill tag by ID.</summary>
    /// <param name="id">ID of the skill to delete.</param>
    [HttpDelete("skills/delete/{id}")]
    public async Task<IActionResult> DeleteSkill(string id)
    {
        await _careerServices.DeleteSkillAsync(id);
        return NoContent();
    }
}
