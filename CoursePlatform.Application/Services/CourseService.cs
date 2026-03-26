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

public class CourseService(ICourseDbContext context) : ICourseService
{
    public async Task<CourseDto> CreateCourse(CreateCourseRequest request)
    {
        var course = new Course { Title = request.Title };
        context.Courses.Add(course);
        await context.SaveChangesAsync();
        return ToDto(course);
    }

    public async Task<CourseDto> UpdateCourse(Guid id, UpdateCourseRequest request)
    {
        var course = await context.Courses.FindAsync(id) ?? throw new Exception("Course not found");
        course.Title = request.Title;
        await context.SaveChangesAsync();
        return ToDto(course);
    }

    public async Task DeleteCourse(Guid id)
    {
        var course = await context.Courses.FindAsync(id) ?? throw new Exception("Course not found");
        context.Courses.Remove(course); 
        await context.SaveChangesAsync();
    }

    public async Task<CourseDto> PublishCourse(Guid id)
    {
        var course = await context.Courses
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == id) ?? throw new Exception("Course not found");

        if (!course.Lessons.Any(l => !l.IsDeleted))
        {
            throw new InvalidOperationException("Un curso solo puede publicarse si tiene al menos una lección activa.");
        }

        course.Status = CourseStatus.Published;
        await context.SaveChangesAsync();
        return ToDto(course);
    }

    public async Task<CourseDto> UnpublishCourse(Guid id)
    {
        var course = await context.Courses.FindAsync(id) ?? throw new Exception("Course not found");
        course.Status = CourseStatus.Draft;
        await context.SaveChangesAsync();
        return ToDto(course);
    }

    public async Task<(IEnumerable<CourseDto> Items, int TotalCount)> SearchCourses(string? q, CourseStatus? status, int page, int pageSize)
    {
        var query = context.Courses.AsNoTracking();

        if (!string.IsNullOrEmpty(q))
        {
            query = query.Where(c => c.Title.ToLower().Contains(q.ToLower()));
        }

        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status);
        }

        var totalCount = await query.CountAsync();
        var items = await query.OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => ToDto(c))
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<CourseSummaryDto> GetCourseSummary(Guid id)
    {
        var course = await context.Courses
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == id) ?? throw new Exception("Course not found");

        return new CourseSummaryDto(
            course.Id,
            course.Title,
            course.Lessons.Count(l => !l.IsDeleted),
            course.UpdatedAt
        );
    }

    private CourseDto ToDto(Course c) => new CourseDto(c.Id, c.Title, c.Status, c.CreatedAt, c.UpdatedAt);
}
