using Microsoft.EntityFrameworkCore;
using CoursePlatform.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace CoursePlatform.Application.Interfaces;

public interface IAppDbContext
{
    DbSet<Project> Projects { get; }
    DbSet<TaskItem> Tasks { get; }
    DbSet<ProjectEnrollment> ProjectEnrollments { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
