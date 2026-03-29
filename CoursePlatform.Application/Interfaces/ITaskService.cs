using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoursePlatform.Application.DTOs;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Interfaces;

public interface ITaskService
{
    Task<TaskItemDto> CreateTask(CreateTaskRequest request);
    Task<TaskItemDto> UpdateTask(Guid id, UpdateTaskRequest request);
    Task DeleteTask(Guid id);
    Task<TaskItemDto> ChangeTaskStatus(Guid id, TaskProgressStatus status);
    Task<TaskItemDto> ReorderTask(Guid id, ReorderTaskRequest request);
}
