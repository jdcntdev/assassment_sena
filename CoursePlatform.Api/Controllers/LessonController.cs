using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoursePlatform.Application.DTOs;
using CoursePlatform.Application.Interfaces;
using System.Threading.Tasks;

namespace CoursePlatform.Api.Controllers;

[Authorize]
[ApiController]
[Route("api")]
public class LessonController(ILessonService lessonService) : ControllerBase
{
    [HttpPost("lessons")]
    public async Task<IActionResult> Create(CreateLessonRequest request)
    {
        try
        {
            var result = await lessonService.CreateLesson(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("lessons/{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateLessonRequest request)
    {
        try
        {
            var result = await lessonService.UpdateLesson(id, request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("lessons/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await lessonService.DeleteLesson(id);
        return NoContent();
    }

    [HttpGet("courses/{courseId}/lessons")]
    public async Task<IActionResult> GetByCourse(Guid courseId)
    {
        var result = await lessonService.GetLessonsByCourse(courseId);
        return Ok(result);
    }
}
