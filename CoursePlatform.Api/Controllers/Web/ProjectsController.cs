using CoursePlatform.Application.DTOs;
using CoursePlatform.Application.Interfaces;
using CoursePlatform.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoursePlatform.Api.Controllers.Web;

[Authorize(AuthenticationSchemes = "Identity.Application")]
public class ProjectsController : Controller
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    public async Task<IActionResult> Index(string status = null, int page = 1, int pageSize = 10)
    {
        ProjectStatus? statusEnum = null;
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<ProjectStatus>(status, out var s))
            statusEnum = s;

        Guid? currentUserId = null;
        var userIdStr = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdStr, out var uId)) currentUserId = uId;

        var result = await _projectService.SearchProjects(statusEnum, page, pageSize, currentUserId);
        ViewData["CurrentStatus"] = status;
        ViewData["CurrentPage"] = page;
        ViewData["TotalCount"] = result.TotalCount;
        ViewData["PageSize"] = pageSize;
        return View(result.Items);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Enroll(Guid id)
    {
        var userIdStr = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdStr, out var userId))
        {
            await _projectService.EnrollUserAsync(id, userId);
        }
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View(new ProjectCreateViewModel());

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromForm] ProjectCreateViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        await _projectService.CreateProject(new CreateProjectRequest(vm.Name, vm.Description));
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var summary = await _projectService.GetProjectSummary(id);
        if (summary == null) return NotFound();
        return View(new ProjectUpdateViewModel { Name = summary.Name, Description = summary.Description });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit([FromRoute] Guid id, [FromForm] ProjectUpdateViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        await _projectService.UpdateProject(id, new UpdateProjectRequest(vm.Name, vm.Description));
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _projectService.DeleteProject(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Activate(Guid id)
    {
        try { await _projectService.ActivateProject(id); } catch (Exception ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Complete(Guid id)
    {
        try { await _projectService.CompleteProject(id); } catch (Exception ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }
}

public class ProjectCreateViewModel { public string Name { get; set; } public string Description { get; set; } }
public class ProjectUpdateViewModel { public string Name { get; set; } public string Description { get; set; } }
