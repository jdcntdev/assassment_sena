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

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<ProjectEnrollment> ProjectEnrollments => Set<ProjectEnrollment>();

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

        // REGLA: El campo Order de las tareas debe ser único dentro del mismo proyecto
        modelBuilder.Entity<TaskItem>()
            .HasIndex(t => new { t.ProjectId, t.Order })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false"); // only for active tasks

        // Project → Tasks relationship
        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // ProjectEnrollment: unique user per project
        modelBuilder.Entity<ProjectEnrollment>()
            .HasIndex(e => new { e.ProjectId, e.UserId })
            .IsUnique();

        modelBuilder.Entity<ProjectEnrollment>()
            .HasOne(e => e.Project)
            .WithMany()
            .HasForeignKey(e => e.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
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
