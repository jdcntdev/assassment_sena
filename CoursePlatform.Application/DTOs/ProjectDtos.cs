using System;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.DTOs;

// ── Project DTOs ────────────────────────────────────────────────────

public record ProjectDto(
    Guid Id,
    string Name,
    string Description,
    ProjectStatus Status,
    int TaskCount,
    int CompletedTaskCount,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    bool IsEnrolled = false);

public record ProjectSummaryDto(
    Guid Id,
    string Name,
    string Description,
    ProjectStatus Status,
    int TotalTasks,
    int CompletedTasks,
    DateTime UpdatedAt);

public record CreateProjectRequest(string Name, string Description);

public record UpdateProjectRequest(string Name, string Description);

// ── Task DTOs ───────────────────────────────────────────────────────

public record TaskItemDto(
    Guid Id,
    Guid ProjectId,
    string Title,
    Priority Priority,
    int Order,
    TaskProgressStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record CreateTaskRequest(Guid ProjectId, string Title, Priority Priority, int Order);

public record UpdateTaskRequest(string Title, Priority Priority, int Order);

public record ReorderTaskRequest(int NewOrder);
