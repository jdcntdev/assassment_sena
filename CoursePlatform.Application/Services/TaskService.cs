using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CoursePlatform.Application.DTOs;
using CoursePlatform.Application.Interfaces;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Services;

public class TaskService(IAppDbContext context) : ITaskService
{
    public async Task<TaskItemDto> CreateTask(CreateTaskRequest request)
    {
        // REGLA: El campo Order debe ser único dentro del mismo proyecto
        var orderExists = await context.Tasks
            .AnyAsync(t => t.ProjectId == request.ProjectId && t.Order == request.Order);
        if (orderExists)
            throw new InvalidOperationException(
                $"Ya existe una tarea con el orden {request.Order} en este proyecto. El orden debe ser único.");

        var task = new TaskItem
        {
            ProjectId = request.ProjectId,
            Title = request.Title,
            Priority = request.Priority,
            Order = request.Order
        };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();
        return ToDto(task);
    }

    public async Task<TaskItemDto> UpdateTask(Guid id, UpdateTaskRequest request)
    {
        var task = await context.Tasks.FindAsync(id)
            ?? throw new Exception("Tarea no encontrada.");

        // REGLA: Order único al actualizar (si cambia)
        if (task.Order != request.Order)
        {
            var orderExists = await context.Tasks
                .AnyAsync(t => t.ProjectId == task.ProjectId && t.Order == request.Order && t.Id != id);
            if (orderExists)
                throw new InvalidOperationException(
                    $"Ya existe una tarea con el orden {request.Order} en este proyecto. El orden debe ser único.");
        }

        task.Title = request.Title;
        task.Priority = request.Priority;
        task.Order = request.Order;
        await context.SaveChangesAsync();
        return ToDto(task);
    }

    public async Task DeleteTask(Guid id)
    {
        var task = await context.Tasks.FindAsync(id)
            ?? throw new Exception("Tarea no encontrada.");
        context.Tasks.Remove(task);
        await context.SaveChangesAsync();
    }

    public async Task<TaskItemDto> ChangeTaskStatus(Guid id, CoursePlatform.Domain.Enums.TaskProgressStatus status)
    {
        var task = await context.Tasks.FindAsync(id)
            ?? throw new Exception("Tarea no encontrada.");
        task.Status = status;
        await context.SaveChangesAsync();
        return ToDto(task);
    }

    /// <summary>
    /// REGLA: El reordenamiento no debe generar órdenes duplicados.
    /// Intercambia el Order con la tarea que actualmente ocupa NewOrder.
    /// </summary>
    public async Task<TaskItemDto> ReorderTask(Guid id, ReorderTaskRequest request)
    {
        var task = await context.Tasks.FindAsync(id)
            ?? throw new Exception("Tarea no encontrada.");

        if (task.Order == request.NewOrder)
            return ToDto(task);

        // Find the task currently occupying the target order (swap)
        var swapTarget = await context.Tasks
            .FirstOrDefaultAsync(t => t.ProjectId == task.ProjectId && t.Order == request.NewOrder);

        if (swapTarget != null)
        {
            // Swap orders atomically to avoid unique constraint violation
            var tempOrder = task.Order;
            task.Order = request.NewOrder;
            swapTarget.Order = tempOrder;
        }
        else
        {
            task.Order = request.NewOrder;
        }

        await context.SaveChangesAsync();
        return ToDto(task);
    }

    private static TaskItemDto ToDto(TaskItem t) =>
        new(t.Id, t.ProjectId, t.Title, t.Priority, t.Order, t.Status, t.CreatedAt, t.UpdatedAt);
}
