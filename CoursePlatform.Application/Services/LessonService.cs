using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CoursePlatform.Application.DTOs;
using CoursePlatform.Application.Interfaces;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Services;

public class LessonService(ICourseDbContext context) : ILessonService
{
    public async Task<LessonDto> CreateLesson(CreateLessonRequest request)
    {
        var existingOrder = await context.Lessons.AnyAsync(l => l.CourseId == request.CourseId && l.Order == request.Order);
        if (existingOrder)
        {
            throw new InvalidOperationException("El campo Order de las lecciones debe ser único dentro del mismo curso");
        }

        var lesson = new Lesson 
        { 
            CourseId = request.CourseId, 
            Title = request.Title, 
            Order = request.Order 
        };
        context.Lessons.Add(lesson);
        await context.SaveChangesAsync();
        return ToDto(lesson);
    }

    public async Task<LessonDto> UpdateLesson(Guid id, UpdateLessonRequest request)
    {
        var lesson = await context.Lessons.FindAsync(id) ?? throw new Exception("Lesson not found");

        if (lesson.Order != request.Order)
        {
            var existingOrder = await context.Lessons.AnyAsync(l => l.CourseId == lesson.CourseId && l.Order == request.Order);
            if (existingOrder)
            {
                throw new InvalidOperationException("El campo Order de las lecciones debe ser único dentro del mismo curso");
            }
        }

        lesson.Title = request.Title;
        lesson.Order = request.Order;
        await context.SaveChangesAsync();
        return ToDto(lesson);
    }

    public async Task DeleteLesson(Guid id)
    {
        var lesson = await context.Lessons.FindAsync(id) ?? throw new Exception("Lesson not found");
        context.Lessons.Remove(lesson); 
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<LessonDto>> GetLessonsByCourse(Guid courseId)
    {
        var entities = await context.Lessons
            .Where(l => l.CourseId == courseId)
            .OrderBy(l => l.Order)
            .ToListAsync();

        return entities.Select(l => ToDto(l));
    }

    private static LessonDto ToDto(Lesson l) => new LessonDto(l.Id, l.CourseId, l.Title, l.Order, l.CreatedAt, l.UpdatedAt);
}
