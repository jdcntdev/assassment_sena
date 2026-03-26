using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoursePlatform.Application.DTOs;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Interfaces;

public interface ICourseService
{
    Task<CourseDto> CreateCourse(CreateCourseRequest request);
    Task<CourseDto> UpdateCourse(Guid id, UpdateCourseRequest request);
    Task DeleteCourse(Guid id);
    Task<CourseDto> PublishCourse(Guid id);
    Task<CourseDto> UnpublishCourse(Guid id);
    Task<(IEnumerable<CourseDto> Items, int TotalCount)> SearchCourses(string? q, CourseStatus? status, int page, int pageSize);
    Task<CourseSummaryDto> GetCourseSummary(Guid id);
}
