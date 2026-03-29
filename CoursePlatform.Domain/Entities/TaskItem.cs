using System;
using CoursePlatform.Domain.Common;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Domain.Entities;

public class TaskItem : BaseEntity
{
    public Guid ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public Priority Priority { get; set; } = Priority.Medium;
    public int Order { get; set; }
    public TaskProgressStatus Status { get; set; } = TaskProgressStatus.Todo;

    // Relationship
    public virtual Project? Project { get; set; }
}
