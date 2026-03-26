using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CoursePlatform.Domain.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoursePlatform.Domain.Common;
using CoursePlatform.Application.Interfaces;

namespace CoursePlatform.Infrastructure.Persistence;

public class CourseDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, ICourseDbContext
{
    public CourseDbContext(DbContextOptions<CourseDbContext> options) : base(options) { }

    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Lesson> Lessons => Set<Lesson>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Global query filter for Soft Delete
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(ConvertFilterExpression(entityType.ClrType));
            }
        }
        
        // Configure Order for Lesson to be unique within a Course (Business Rule)
        modelBuilder.Entity<Lesson>()
            .HasIndex(l => new { l.CourseId, l.Order })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false"); // Unique index only for active lessons
    }

    private static dynamic ConvertFilterExpression(Type type)
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(type, "e");
        var property = System.Linq.Expressions.Expression.Property(parameter, "IsDeleted");
        var falseConstant = System.Linq.Expressions.Expression.Constant(false);
        var equalBody = System.Linq.Expressions.Expression.Equal(property, falseConstant);
        var lambda = System.Linq.Expressions.Expression.Lambda(equalBody, parameter);
        return lambda;
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
