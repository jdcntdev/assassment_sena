using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoursePlatform.Application.DTOs;
using CoursePlatform.Application.Interfaces;
using CoursePlatform.Domain.Enums;
using System.Threading.Tasks;

namespace CoursePlatform.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/courses")]
public class CourseController(ICourseService courseService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateCourseRequest request)
    {
        var result = await courseService.CreateCourse(request);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateCourseRequest request)
    {
        var result = await courseService.UpdateCourse(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await courseService.DeleteCourse(id);
        return NoContent();
    }

    [HttpPatch("{id}/publish")]
    public async Task<IActionResult> Publish(Guid id)
    {
        try
        {
            var result = await courseService.PublishCourse(id);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/unpublish")]
    public async Task<IActionResult> Unpublish(Guid id)
    {
        var result = await courseService.UnpublishCourse(id);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? q, 
        [FromQuery] CourseStatus? status, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        var (items, totalCount) = await courseService.SearchCourses(q, status, page, pageSize);
        return Ok(new { items, totalCount, page, pageSize });
    }

    [HttpGet("{id}/summary")]
    public async Task<IActionResult> GetSummary(Guid id)
    {
        var result = await courseService.GetCourseSummary(id);
        return Ok(result);
    }
}
