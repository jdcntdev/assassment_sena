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
    private CourseDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<CourseDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new CourseDbContext(options);
    }

    [Fact]
    public async Task PublishCourse_WithLessons_ShouldSucceed()
    {
        // Arrange
        var context = GetDbContext();
        var service = new CourseService(context);
        var course = new Course { Title = "Test Course" };
        context.Courses.Add(course);
        context.Lessons.Add(new Lesson { CourseId = course.Id, Title = "Lesson 1", Order = 1 });
        await context.SaveChangesAsync();

        // Act
        var result = await service.PublishCourse(course.Id);

        // Assert
        Assert.Equal(CourseStatus.Published, result.Status);
    }

    [Fact]
    public async Task PublishCourse_WithoutLessons_ShouldFail()
    {
        // Arrange
        var context = GetDbContext();
        var service = new CourseService(context);
        var course = new Course { Title = "Empty Course" };
        context.Courses.Add(course);
        await context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.PublishCourse(course.Id));
    }

    [Fact]
    public async Task CreateLesson_WithUniqueOrder_ShouldSucceed()
    {
        // Arrange
        var context = GetDbContext();
        var service = new LessonService(context);
        var course = new Course { Title = "Test Course" };
        context.Courses.Add(course);
        await context.SaveChangesAsync();

        // Act
        var result = await service.CreateLesson(new CreateLessonRequest(course.Id, "Lesson 1", 1));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Order);
    }

    [Fact]
    public async Task CreateLesson_WithDuplicateOrder_ShouldFail()
    {
        // Arrange
        var context = GetDbContext();
        var service = new LessonService(context);
        var course = new Course { Title = "Test Course" };
        context.Courses.Add(course);
        context.Lessons.Add(new Lesson { CourseId = course.Id, Title = "Lesson 1", Order = 1 });
        await context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateLesson(new CreateLessonRequest(course.Id, "Lesson 2", 1)));
    }

    [Fact]
    public async Task DeleteCourse_ShouldBeSoftDelete()
    {
        // Arrange
        var context = GetDbContext();
        var service = new CourseService(context);
        var course = new Course { Title = "Delete Me" };
        context.Courses.Add(course);
        await context.SaveChangesAsync();

        // Act
        await service.DeleteCourse(course.Id);

        // Assert
        // We need a context WITHOUT the global query filter to check if it's actually in the DB with IsDeleted = true
        // But for InMemory, we can check the tracker or a separate query ignore the filters if possible.
        // Or simply query using context with IgnoreQueryFilters()
        var deletedCourse = await context.Courses.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == course.Id);
        
        Assert.NotNull(deletedCourse);
        Assert.True(deletedCourse.IsDeleted);

        // Check it's NOT found with filters
        var countFound = await context.Courses.CountAsync();
        Assert.Equal(0, countFound);
    }
}
