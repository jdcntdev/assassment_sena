using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoursePlatform.Application.DTOs;
using CoursePlatform.Application.Interfaces;
using CoursePlatform.Domain.Enums;
using System.Threading.Tasks;

namespace CoursePlatform.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/tasks")]
public class TaskController(ITaskService taskService) : ControllerBase
{
    // POST /api/tasks/{projectId}
    [HttpPost("{projectId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Guid projectId, CreateTaskRequest request)
    {
        try
        {
            // Ensure projectId from route is used
            var req = request with { ProjectId = projectId };
            var result = await taskService.CreateTask(req);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT /api/tasks/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, UpdateTaskRequest request)
    {
        try
        {
            var result = await taskService.UpdateTask(id, request);
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

    // DELETE /api/tasks/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await taskService.DeleteTask(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // PATCH /api/tasks/{id}/status
    [HttpPatch("{id}/status")]
    [Authorize]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] TaskProgressStatus status)
    {
        try
        {
            var result = await taskService.ChangeTaskStatus(id, status);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // PATCH /api/tasks/{id}/reorder
    [HttpPatch("{id}/reorder")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Reorder(Guid id, ReorderTaskRequest request)
    {
        try
        {
            var result = await taskService.ReorderTask(id, request);
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
