using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoursePlatform.Application.DTOs;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Interfaces;

public interface IProjectService
{
    Task<ProjectDto> CreateProject(CreateProjectRequest request);
    Task<ProjectDto> UpdateProject(Guid id, UpdateProjectRequest request);
    Task DeleteProject(Guid id);
    Task<ProjectDto> ActivateProject(Guid id);
    Task<ProjectDto> CompleteProject(Guid id);
    Task<(IEnumerable<ProjectDto> Items, int TotalCount)> SearchProjects(ProjectStatus? status, int page, int pageSize, Guid? currentUserId = null);
    Task EnrollUserAsync(Guid projectId, Guid userId);
    Task<ProjectSummaryDto> GetProjectSummary(Guid id);
    Task<IEnumerable<TaskItemDto>> GetProjectTasks(Guid projectId);
}
