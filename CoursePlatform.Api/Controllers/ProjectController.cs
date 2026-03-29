using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoursePlatform.Application.DTOs;
using CoursePlatform.Application.Interfaces;
using CoursePlatform.Domain.Enums;
using System.Threading.Tasks;

namespace CoursePlatform.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/projects")]
public class ProjectController(IProjectService projectService) : ControllerBase
{
    // GET /api/projects/search?status=&page=&pageSize=
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] ProjectStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var (items, totalCount) = await projectService.SearchProjects(status, page, pageSize);
        return Ok(new { items, totalCount, page, pageSize });
    }

    // GET /api/projects/{id}/summary
    [HttpGet("{id}/summary")]
    public async Task<IActionResult> GetSummary(Guid id)
    {
        try
        {
            var summary = await projectService.GetProjectSummary(id);
            return Ok(summary);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // GET /api/projects/{id}/tasks
    [HttpGet("{id}/tasks")]
    public async Task<IActionResult> GetTasks(Guid id)
    {
        var tasks = await projectService.GetProjectTasks(id);
        return Ok(tasks);
    }

    // POST /api/projects
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateProjectRequest request)
    {
        var result = await projectService.CreateProject(request);
        return Ok(result);
    }

    // PUT /api/projects/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, UpdateProjectRequest request)
    {
        try
        {
            var result = await projectService.UpdateProject(id, request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // DELETE /api/projects/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await projectService.DeleteProject(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // PATCH /api/projects/{id}/activate
    [HttpPatch("{id}/activate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Activate(Guid id)
    {
        try
        {
            var result = await projectService.ActivateProject(id);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // PATCH /api/projects/{id}/complete
    [HttpPatch("{id}/complete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Complete(Guid id)
    {
        try
        {
            var result = await projectService.CompleteProject(id);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
