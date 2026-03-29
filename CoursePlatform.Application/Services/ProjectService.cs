using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CoursePlatform.Application.DTOs;
using CoursePlatform.Application.Interfaces;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Services;

public class ProjectService(IAppDbContext context) : IProjectService
{
    // ── CRUD ────────────────────────────────────────────────────────

    public async Task<ProjectDto> CreateProject(CreateProjectRequest request)
    {
        var project = new Project
        {
            Name = request.Name,
            Description = request.Description
        };
        context.Projects.Add(project);
        await context.SaveChangesAsync();
        return ToDto(project, 0, 0);
    }

    public async Task<ProjectDto> UpdateProject(Guid id, UpdateProjectRequest request)
    {
        var project = await context.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new Exception("Proyecto no encontrado.");

        project.Name = request.Name;
        project.Description = request.Description;
        await context.SaveChangesAsync();

        var activeTasks = project.Tasks.Where(t => !t.IsDeleted).ToList();
        return ToDto(project, activeTasks.Count, activeTasks.Count(t => t.Status == TaskProgressStatus.Completed));
    }

    public async Task DeleteProject(Guid id)
    {
        var project = await context.Projects.FindAsync(id)
            ?? throw new Exception("Proyecto no encontrado.");
        context.Projects.Remove(project);
        await context.SaveChangesAsync();
    }

    // ── Business Rules ───────────────────────────────────────────────

    /// <summary>
    /// REGLA: Un proyecto solo puede activarse si tiene al menos una tarea.
    /// </summary>
    public async Task<ProjectDto> ActivateProject(Guid id)
    {
        var project = await context.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new Exception("Proyecto no encontrado.");

        var activeTasks = project.Tasks.Where(t => !t.IsDeleted).ToList();
        if (activeTasks.Count == 0)
            throw new InvalidOperationException("Un proyecto solo puede activarse si tiene al menos una tarea.");

        project.Status = ProjectStatus.Active;
        await context.SaveChangesAsync();
        return ToDto(project, activeTasks.Count, activeTasks.Count(t => t.Status == TaskProgressStatus.Completed));
    }

    /// <summary>
    /// REGLA: Un proyecto solo puede completarse si todas sus tareas están completadas.
    /// </summary>
    public async Task<ProjectDto> CompleteProject(Guid id)
    {
        var project = await context.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new Exception("Proyecto no encontrado.");

        var activeTasks = project.Tasks.Where(t => !t.IsDeleted).ToList();
        var pendingCount = activeTasks.Count(t => t.Status != TaskProgressStatus.Completed);
        if (pendingCount > 0)
            throw new InvalidOperationException(
                $"Un proyecto solo puede completarse si todas sus tareas están completadas. Quedan {pendingCount} tareas pendientes.");

        project.Status = ProjectStatus.Completed;
        await context.SaveChangesAsync();
        return ToDto(project, activeTasks.Count, activeTasks.Count);
    }

    // ── Queries ──────────────────────────────────────────────────────

    public async Task<(IEnumerable<ProjectDto> Items, int TotalCount)> SearchProjects(
        ProjectStatus? status, int page, int pageSize, Guid? currentUserId = null)
    {
        var query = context.Projects
            .Include(p => p.Tasks)
            .AsNoTracking();

        if (status.HasValue)
            query = query.Where(p => p.Status == status);

        var totalCount = await query.CountAsync();
        var entities = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var entityIds = entities.Select(p => p.Id).ToList();
        var enrolledProjectIds = new HashSet<Guid>();
        if (currentUserId.HasValue)
        {
            enrolledProjectIds = context.ProjectEnrollments
                .Where(e => entityIds.Contains(e.ProjectId) && e.UserId == currentUserId.Value)
                .Select(e => e.ProjectId)
                .ToHashSet();
        }

        var items = entities.Select(p =>
        {
            var active = p.Tasks.Where(t => !t.IsDeleted).ToList();
            bool isEnrolled = enrolledProjectIds.Contains(p.Id);
            return ToDto(p, active.Count, active.Count(t => t.Status == TaskProgressStatus.Completed), isEnrolled);
        }).ToList();

        return (items, totalCount);
    }

    public async Task EnrollUserAsync(Guid projectId, Guid userId)
    {
        var project = await context.Projects.FindAsync(projectId) ?? throw new Exception("Proyecto no encontrado.");
        if (!await context.ProjectEnrollments.AnyAsync(e => e.ProjectId == projectId && e.UserId == userId))
        {
            context.ProjectEnrollments.Add(new ProjectEnrollment { ProjectId = projectId, UserId = userId });
            await context.SaveChangesAsync();
        }
    }

    public async Task<ProjectSummaryDto> GetProjectSummary(Guid id)
    {
        var project = await context.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new Exception("Proyecto no encontrado.");

        var activeTasks = project.Tasks.Where(t => !t.IsDeleted).ToList();
        return new ProjectSummaryDto(
            project.Id,
            project.Name,
            project.Description,
            project.Status,
            activeTasks.Count,
            activeTasks.Count(t => t.Status == TaskProgressStatus.Completed),
            project.UpdatedAt);
    }

    public async Task<IEnumerable<TaskItemDto>> GetProjectTasks(Guid projectId)
    {
        var tasks = await context.Tasks
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.Order)
            .AsNoTracking()
            .ToListAsync();

        return tasks.Select(ToTaskDto);
    }

    // ── Mapping ──────────────────────────────────────────────────────

    private static ProjectDto ToDto(Project p, int taskCount, int completedCount, bool isEnrolled = false) =>
        new(p.Id, p.Name, p.Description, p.Status, taskCount, completedCount, p.CreatedAt, p.UpdatedAt, isEnrolled);

    private static TaskItemDto ToTaskDto(TaskItem t) =>
        new(t.Id, t.ProjectId, t.Title, t.Priority, t.Order, t.Status, t.CreatedAt, t.UpdatedAt);
}
