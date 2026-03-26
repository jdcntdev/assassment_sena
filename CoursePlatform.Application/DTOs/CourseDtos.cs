using System;
using System.Collections.Generic;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.DTOs;

public record CourseDto(Guid Id, string Title, CourseStatus Status, DateTime CreatedAt, DateTime UpdatedAt);

public record CourseSummaryDto(Guid Id, string Title, int TotalLessons, DateTime UpdatedAt);

public record LessonDto(Guid Id, Guid CourseId, string Title, int Order, DateTime CreatedAt, DateTime UpdatedAt);

public record CreateCourseRequest(string Title);

public record UpdateCourseRequest(string Title);

public record CreateLessonRequest(Guid CourseId, string Title, int Order);

public record UpdateLessonRequest(string Title, int Order);
