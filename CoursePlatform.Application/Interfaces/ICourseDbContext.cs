using Microsoft.EntityFrameworkCore;
using CoursePlatform.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace CoursePlatform.Application.Interfaces;

public interface ICourseDbContext
{
    DbSet<Course> Courses { get; }
    DbSet<Lesson> Lessons { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
