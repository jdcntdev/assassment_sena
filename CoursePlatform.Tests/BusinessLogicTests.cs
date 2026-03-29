using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CoursePlatform.Application.DTOs;
using CoursePlatform.Application.Services;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using CoursePlatform.Infrastructure.Persistence;
using Xunit;

namespace CoursePlatform.Tests;

public class BusinessLogicTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    // ── 1. ActivateProject con tareas → debe tener éxito ────────────
    [Fact]
    public async Task ActivateProject_WithTasks_ShouldSucceed()
    {
        // Arrange
        var context = GetDbContext();
        var service = new ProjectService(context);

        var project = new Project { Name = "Proyecto Test", Description = "Desc" };
        context.Projects.Add(project);
        context.Tasks.Add(new TaskItem { ProjectId = project.Id, Title = "Tarea 1", Order = 1 });
        await context.SaveChangesAsync();

        // Act
        var result = await service.ActivateProject(project.Id);

        // Assert
        Assert.Equal(ProjectStatus.Active, result.Status);
    }

    // ── 2. ActivateProject sin tareas → debe fallar ─────────────────
    [Fact]
    public async Task ActivateProject_WithoutTasks_ShouldFail()
    {
        // Arrange
        var context = GetDbContext();
        var service = new ProjectService(context);

        var project = new Project { Name = "Proyecto Vacío", Description = "Sin tareas" };
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ActivateProject(project.Id));
    }

    // ── 3. CompleteProject con tareas pendientes → debe fallar ──────
    [Fact]
    public async Task CompleteProject_WithPendingTasks_ShouldFail()
    {
        // Arrange
        var context = GetDbContext();
        var service = new ProjectService(context);

        var project = new Project { Name = "Proyecto Activo", Description = "Desc" };
        context.Projects.Add(project);
        context.Tasks.Add(new TaskItem
        {
            ProjectId = project.Id,
            Title = "Tarea Pendiente",
            Order = 1,
            Status = CoursePlatform.Domain.Enums.TaskProgressStatus.Todo  // pending!
        });
        await context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CompleteProject(project.Id));
    }

    // ── 4. CreateTask con Order duplicado → debe fallar ─────────────
    [Fact]
    public async Task CreateTask_WithDuplicateOrder_ShouldFail()
    {
        // Arrange
        var context = GetDbContext();
        var projectService = new ProjectService(context);
        var taskService = new TaskService(context);

        var project = new Project { Name = "Proyecto Test", Description = "Desc" };
        context.Projects.Add(project);
        context.Tasks.Add(new TaskItem { ProjectId = project.Id, Title = "Tarea 1", Order = 1 });
        await context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => taskService.CreateTask(new CreateTaskRequest(project.Id, "Tarea Duplicada", Priority.Low, 1)));
    }

    // ── 5. DeleteProject → debe ser SoftDelete ───────────────────────
    [Fact]
    public async Task DeleteProject_ShouldBeDelete()
    {
        // Arrange
        var context = GetDbContext();
        var service = new ProjectService(context);

        var project = new Project { Name = "Proyecto a Eliminar", Description = "Desc" };
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        // Act
        await service.DeleteProject(project.Id);

        // Assert: con filtro global (soft delete) no debe encontrarse
        var found = await context.Projects.FirstOrDefaultAsync(p => p.Id == project.Id);
        Assert.Null(found);

        // Assert: sin filtro debe estar con IsDeleted = true
        var deleted = await context.Projects
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == project.Id);
        Assert.NotNull(deleted);
        Assert.True(deleted.IsDeleted);
    }
}
