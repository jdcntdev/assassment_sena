using CoursePlatform.Application.DTOs;
using CoursePlatform.Application.Interfaces;
using CoursePlatform.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoursePlatform.Api.Controllers.Web;

[Authorize(AuthenticationSchemes = "Identity.Application")]
[Route("Tasks")]
public class TasksController : Controller
{
    private readonly ITaskService _taskService;
    private readonly IProjectService _projectService;

    public TasksController(ITaskService taskService, IProjectService projectService)
    {
        _taskService = taskService;
        _projectService = projectService;
    }

    [HttpGet("Index/{projectId}")]
    public async Task<IActionResult> Index(Guid projectId)
    {
        var tasks = await _projectService.GetProjectTasks(projectId);
        var summary = await _projectService.GetProjectSummary(projectId);
        
        Guid? currentUserId = null;
        var userIdStr = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdStr, out var uId)) currentUserId = uId;

        ViewData["ProjectId"] = projectId;
        ViewData["ProjectName"] = summary?.Name;
        ViewData["ProjectStatus"] = summary?.Status;
        return View(tasks);
    }

    [HttpGet("Create/{projectId}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Create(Guid projectId)
    {
        ViewData["ProjectId"] = projectId;
        return View(new TaskCreateViewModel { Priority = Priority.Medium, Order = 1 });
    }

    [HttpPost("Create/{projectId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromRoute] Guid projectId, [FromForm] TaskCreateViewModel vm)
    {
        if (!ModelState.IsValid) { ViewData["ProjectId"] = projectId; return View(vm); }
        try {
            await _taskService.CreateTask(new CreateTaskRequest(projectId, vm.Title, vm.Priority, vm.Order));
        } catch (Exception ex) { ModelState.AddModelError("", ex.Message); ViewData["ProjectId"] = projectId; return View(vm); }
        return RedirectToAction(nameof(Index), new { projectId });
    }

    [HttpGet("Edit/{projectId}/{taskId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(Guid projectId, Guid taskId)
    {
        var tasks = await _projectService.GetProjectTasks(projectId);
        var task = tasks.FirstOrDefault(t => t.Id == taskId);
        if (task == null) return NotFound();

        ViewData["ProjectId"] = projectId;
        ViewData["TaskId"] = taskId;
        return View(new TaskUpdateViewModel { Title = task.Title, Priority = task.Priority, Order = task.Order });
    }

    [HttpPost("Edit/{projectId}/{taskId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit([FromRoute] Guid projectId, [FromRoute] Guid taskId, [FromForm] TaskUpdateViewModel vm)
    {
        if (!ModelState.IsValid) { ViewData["ProjectId"] = projectId; ViewData["TaskId"] = taskId; return View(vm); }
        try {
            await _taskService.UpdateTask(taskId, new UpdateTaskRequest(vm.Title, vm.Priority, vm.Order));
        } catch (Exception ex) { ModelState.AddModelError("", ex.Message); ViewData["ProjectId"] = projectId; ViewData["TaskId"] = taskId; return View(vm); }
        return RedirectToAction(nameof(Index), new { projectId });
    }

    [HttpPost("Delete/{projectId}/{taskId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid projectId, Guid taskId)
    {
        await _taskService.DeleteTask(taskId);
        return RedirectToAction(nameof(Index), new { projectId });
    }

    [HttpPost("ChangeStatus/{projectId}/{taskId}")]
    [Authorize]
    public async Task<IActionResult> ChangeStatus(Guid projectId, Guid taskId, [FromForm] TaskProgressStatus status)
    {
        await _taskService.ChangeTaskStatus(taskId, status);
        return RedirectToAction(nameof(Index), new { projectId });
    }

    [HttpPost("Reorder/{projectId}/{taskId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Reorder(Guid projectId, Guid taskId, int newOrder)
    {
        try {
            await _taskService.ReorderTask(taskId, new ReorderTaskRequest(newOrder));
        } catch (Exception ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Index), new { projectId });
    }
}

public class TaskCreateViewModel { public string Title { get; set; } public Priority Priority { get; set; } public int Order { get; set; } }
public class TaskUpdateViewModel { public string Title { get; set; } public Priority Priority { get; set; } public int Order { get; set; } }
